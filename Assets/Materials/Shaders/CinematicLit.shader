Shader "Unlit/CinematicLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
        [HDR]
        _LeftHighlightColor ("Left Highlight Color", Color) = (1,0,.4,1)
        [HDR]
        _RightHighlightColor ("Right Highlight Color", Color) = (.1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float3 worldNormal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _AmbientColor;
            float4 _LeftHighlightColor, _RightHighlightColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                
                float3 normal = normalize(i.worldNormal);
                float leftLightIntensity = dot(float3(-1, 1, 0), normal);
                leftLightIntensity = saturate(leftLightIntensity);
                float rightLightIntensity = dot(float3(1, 1, 0), normal);
                rightLightIntensity = saturate(rightLightIntensity);

                float4 leftLight = _LeftHighlightColor * leftLightIntensity;
                float4 rightLight = _RightHighlightColor * rightLightIntensity;
                
                return col * (leftLight + rightLight + _AmbientColor);
            }
            ENDCG
        }
    }
}
