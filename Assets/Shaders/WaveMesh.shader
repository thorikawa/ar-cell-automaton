Shader "Custom/WaveMesh"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Color Intensity", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

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
                float3 worldPos : TEXCOORD1;
                float worldRefl : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                float3 worldNormal = mul(unity_ObjectToWorld, v.normal);
                o.worldRefl = dot(worldViewDir, worldNormal);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = _Intensity * tex2D(_MainTex, float2(i.worldPos.x / 5.0, i.worldPos.z / 5.0));
                return col;
            }
            ENDCG
        }
    }
}
