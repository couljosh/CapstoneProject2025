Shader "Toon/TF2_URP_Illustrative_Stylized"
{
    Properties
    {
        _Color("Main Tint", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}

        _SpecGlossRimMask("Spec(R) Gloss(G) RimMask(B)", 2D) = "white" {}
        _AOMap("AO Map", 2D) = "white" {}
        _Ramp("Ramp (horizontal 1D)", 2D) = "gray" {}
        _NormalMap("Normal Map", 2D) = "bump" {}

        _AmbientIntensity("Ambient Intensity", Float) = 1.0

        _HalfLambertBias("Half-Lambert Bias", Range(0,1)) = 0.5
        _HalfLambertScale("Half-Lambert Scale", Range(0,2)) = 0.5
        _WarpPower("Warp Exponent", Range(0.1,8)) = 1.8
        _WarpWeight("Warp Weight", Range(0,1)) = 0.85

        _SpecularColor("Specular Tint", Color) = (1,1,1,1)
        _SpecularExponent("Specular Exponent", Float) = 64

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimStrength("Rim Strength", Range(0,4)) = 1.0
        _RimStart("Rim start (fresnel edge)", Range(0,1)) = 0.2
        _RimEnd("Rim end (fresnel edge)", Range(0,1)) = 0.6
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);               SAMPLER(sampler_MainTex);
            TEXTURE2D(_SpecGlossRimMask);      SAMPLER(sampler_SpecGlossRimMask);
            TEXTURE2D(_AOMap);                 SAMPLER(sampler_AOMap);
            TEXTURE2D(_Ramp);                  SAMPLER(sampler_Ramp);
            TEXTURE2D(_NormalMap);             SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _AmbientIntensity;

                float _HalfLambertBias;
                float _HalfLambertScale;
                float _WarpPower;
                float _WarpWeight;

                float4 _SpecularColor;
                float _SpecularExponent;

                float4 _RimColor;
                float _RimStrength;
                float _RimStart;
                float _RimEnd;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD5;
                float3 normalWS   : TEXCOORD0;
                float3 tangentWS  : TEXCOORD3;
                float3 bitangentWS: TEXCOORD4;
                float2 uv         : TEXCOORD1;
                float3 viewDirWS  : TEXCOORD2;
                float4 shadowCoord: TEXCOORD6;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                // Stable orthonormal TBN
                float3 Nw = normalize(TransformObjectToWorldNormal(IN.normalOS));
                float3 Tw = normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                Tw = normalize(Tw - Nw * dot(Tw, Nw));
                float3 Bw = normalize(cross(Nw, Tw)) * IN.tangentOS.w;

                OUT.normalWS = Nw;
                OUT.tangentWS = Tw;
                OUT.bitangentWS = Bw;

                OUT.uv = IN.uv;
                OUT.viewDirWS = normalize(GetCameraPositionWS() - OUT.positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                return OUT;
            }

            float HalfLambert(float ndotl)
            {
                float h = saturate(ndotl * _HalfLambertScale + _HalfLambertBias);
                float e = pow(h, _WarpPower);
                return lerp(h, e, _WarpWeight);
            }

            float3 SampleWarp(float halfLambert)
            {
                return SAMPLE_TEXTURE2D(_Ramp, sampler_Ramp, float2(halfLambert, 0.5)).rgb;
            }

            float3 DirectionalAmbient(float3 N)
            {
                return SampleSH(N) * _AmbientIntensity;
            }

            float SpecularTerm(float3 N, float3 V, float3 L, float glossTex, float exponentMax)
            {
                float3 H = normalize(V + L);
                float nh = saturate(dot(N, H));
                float glossCurve = pow(glossTex, 0.5);
                float exp = lerp(1.0, exponentMax, glossCurve);
                return pow(nh, exp) * 2.0;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb * _Color.rgb;
                float ao = SAMPLE_TEXTURE2D(_AOMap, sampler_AOMap, IN.uv).r;

                float3 tNormal = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv));
                float3x3 TBN = float3x3(IN.tangentWS, IN.bitangentWS, IN.normalWS);
                float3 N_map = normalize(mul(TBN, tNormal));
                float3 N_geo = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);

                float3 sglr = SAMPLE_TEXTURE2D(_SpecGlossRimMask, sampler_SpecGlossRimMask, IN.uv).rgb;
                float specMask = sglr.r;
                float glossTex = sglr.g;
                float rimMask = sglr.b;

                float3 lightAccum = 0.0;
                float3 specAccum  = 0.0;

                Light mainLight = GetMainLight();
                float3 L = normalize(mainLight.direction);

                float ndotl = saturate(dot(N_map, L));
                float hl = HalfLambert(ndotl);
                float3 warpedDiffuse = SampleWarp(hl) * mainLight.color.rgb;

                #if defined(_MAIN_LIGHT_SHADOWS)
                    float shadowAtten = MainLightRealtimeShadow(IN.shadowCoord);
                    warpedDiffuse *= shadowAtten;
                #endif

                lightAccum += warpedDiffuse;

                float spec = SpecularTerm(N_map, V, L, glossTex, _SpecularExponent) * specMask;
                specAccum += spec * _SpecularColor.rgb * mainLight.color.rgb * _SpecularColor.a;

                float3 ambientDir = DirectionalAmbient(N_map) * ao;
                float3 diffuseTerm = baseColor * (ambientDir + lightAccum);

                // Stable rim: geometric normal only
                float vdotn = saturate(dot(N_geo, V));
                float rimFresnel = smoothstep(_RimStart, _RimEnd, 1.0 - vdotn);
                float lambertRaw = saturate(dot(N_geo, L));
                float rimGate = saturate(1.0 - lambertRaw);
                float rim = rimFresnel * rimGate * ao * rimMask;
                float3 rimCol = rim * _RimColor.rgb * _RimStrength;

                float3 specular = specAccum * ao;
                float3 finalColor = diffuseTerm + specular + rimCol;
                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
