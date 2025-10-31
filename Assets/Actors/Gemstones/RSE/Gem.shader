Shader "Custom/URP/Gem"
{
    Properties
    {
        _Color ("Tint Color", Color) = (0.6, 1, 0.8, 1)
        _RefractionStrength ("Refraction Strength", Range(0,0.2)) = 0.05
        _IOR ("Index of Refraction", Range(1,2)) = 1.45
        _Smoothness ("Smoothness", Range(0,1)) = 0.9
        _SpecularStrength ("Specular Strength", Range(0,2)) = 1
        _GlowIntensity ("Bloom Glow Multiplier", Range(1,10)) = 2
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Cube ("Reflection Cubemap", CUBE) = "" {}

        _SparkleMask ("Sparkle Mask", 2D) = "white" {}
        _SparkleIntensity ("Sparkle Intensity", Range(0,5)) = 1
        _SparkleSpeed ("Sparkle Speed", Range(0,5)) = 1
        _SparkleScale ("Sparkle Scale", Range(1,20)) = 5
        _SparkleColor ("Sparkle Color", Color) = (1,1,1,1)

        _CausticTex ("Caustic Texture", 2D) = "gray" {}
        _CausticIntensity ("Caustic Intensity", Range(0,5)) = 1
        _CausticSpeed ("Caustic Speed", Range(0,5)) = 1
        _CausticScale ("Caustic Scale", Range(0.5,10)) = 3
        _CausticDistortion ("Caustic Distortion", Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 300
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURECUBE(_Cube);
            SAMPLER(sampler_Cube);
            TEXTURE2D(_SparkleMask);
            SAMPLER(sampler_SparkleMask);
            TEXTURE2D(_CausticTex);
            SAMPLER(sampler_CausticTex);

            float4 _Color;
            float _RefractionStrength;
            float _IOR;
            float _Smoothness;
            float _SpecularStrength;
            float _GlowIntensity;

            float _SparkleIntensity;
            float _SparkleSpeed;
            float _SparkleScale;
            float4 _SparkleColor;

            float _CausticIntensity;
            float _CausticSpeed;
            float _CausticScale;
            float _CausticDistortion;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float4 screenPos   : TEXCOORD3;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(posWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = normalize(GetWorldSpaceViewDir(posWS));
                OUT.uv = IN.uv;
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 N = normalize(IN.normalWS);
                float3 nTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv));
                N = normalize(N + nTS * 0.6);
                float3 V = normalize(IN.viewDirWS);

                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

                // === Dispersion ===
                float2 baseOffset = N.xy * _RefractionStrength * (1 - 1 / max(_IOR, 1.001));
                float2 offsetR = baseOffset * 0.9;
                float2 offsetG = baseOffset;
                float2 offsetB = baseOffset * 1.1;
                float3 colR = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offsetR).rgb;
                float3 colG = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offsetG).rgb;
                float3 colB = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offsetB).rgb;
                float3 refracted = float3(colR.r, colG.g, colB.b);

                // === Reflection ===
                float3 R = reflect(-V, N);
                float3 reflection = SAMPLE_TEXTURECUBE(_Cube, sampler_Cube, R).rgb;

                // === Fresnel ===
                float fresnel = pow(saturate(1.0 - dot(V, N)), 5.0);
                float3 baseCol = lerp(refracted, reflection, fresnel * _Smoothness);

                // === Specular ===
                Light mainLight = GetMainLight();
                float3 L = normalize(mainLight.direction);
                float3 H = normalize(L + V);
                float NdotL = saturate(dot(N, L));
                float NdotH = saturate(dot(N, H));
                float3 spec = mainLight.color * pow(NdotH, 128.0 * _Smoothness) * NdotL * _SpecularStrength;

                // === Sparkle ===
                float2 sparkleUV = IN.uv * _SparkleScale + float2(_Time.y * _SparkleSpeed, _Time.y * 0.37 * _SparkleSpeed);
                float sparkleBase = SAMPLE_TEXTURE2D(_SparkleMask, sampler_SparkleMask, sparkleUV).r;
                float sparkleAnim = sin(_Time.y * 12.0 + sparkleBase * 6.283) * 0.5 + 0.5;
                float sparkle = pow(sparkleAnim, 8.0) * sparkleBase * _SparkleIntensity;
                float3 sparkleCol = _SparkleColor.rgb * sparkle;

                // === Animated Caustics ===
                float2 causticUV = IN.uv * _CausticScale + float2(_Time.y * _CausticSpeed * 0.2, _Time.y * 0.15 * _CausticSpeed);
                float2 causticUV2 = IN.uv * (_CausticScale * 1.5) - float2(_Time.y * 0.1 * _CausticSpeed, _Time.y * 0.17 * _CausticSpeed);
                float caustic1 = SAMPLE_TEXTURE2D(_CausticTex, sampler_CausticTex, causticUV).r;
                float caustic2 = SAMPLE_TEXTURE2D(_CausticTex, sampler_CausticTex, causticUV2).g;
                float caustic = saturate(caustic1 * 0.7 + caustic2 * 0.3);
                // warp slightly by normals to make it look like refraction waves
                caustic += dot(N.xy, float2(0.5, 0.7)) * _CausticDistortion;
                caustic = pow(saturate(caustic), 2.0) * _CausticIntensity;
                float3 causticCol = caustic * mainLight.color * 1.5;

                // Combine all lighting
                float3 color = baseCol * _Color.rgb + spec + sparkleCol + causticCol;

                // Glow for bloom
                color *= _GlowIntensity;

                float alpha = saturate(0.3 + fresnel * 0.7);
                return float4(color, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
