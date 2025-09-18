Shader "CelShaders/CelShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness("Brightness", Range(0,2)) = 0.3
        _Strength("Strength", Range(0,1)) = 0.4
        _Color("Colour", Color) = (1,1,1,1)
        _Detail("Detail", Range(0.1,1)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 80

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 worldNormal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;
            float _Strength;
            float4 _Color;
            float _Detail;

            // Toon function with a slight bias and finer control on quantization
            float Toon(float3 normal, float3 lightDir)
            {
                float NdotL = max(0.0, dot(normalize(normal), normalize(lightDir)));
                // Adding a small bias to NdotL to avoid Z-fighting
                NdotL = (floor(NdotL / _Detail) + 0.5) * _Detail;
                return saturate(NdotL); // Ensure the output is within [0, 1]
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the base texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Apply toon shading and tint with color
                float toonShade = Toon(i.worldNormal, _WorldSpaceLightPos0.xyz);
                col.rgb *= toonShade * _Strength * _Color.rgb + (_Brightness * _Color.rgb);

                return col;
            }
            ENDCG
        }

        // Shadow Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            LOD 80

            CGPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma target 2.0
            #pragma multi_compile_shadowcaster

            struct appdata_base
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2fShadow
            {
                float4 pos : SV_POSITION;
            };

            v2fShadow vertShadow(appdata_base v)
            {
                v2fShadow o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 fragShadow(v2fShadow i) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
    }
}
