Shader "Hidden/MobileColorGrading"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 1. Heavy black pedestal removal to ensure floor is pitch dark like Mac
                float3 c = max(0.0, (col.rgb - 0.12) / 0.88);

                // 2. High contrast power curve for dark inky shadows
                c = pow(c, 1.65);

                // 3. Doubled vibrance and color saturation for punchy neon colors and lights
                float luma = dot(c, float3(0.299, 0.587, 0.114));
                c = lerp(float3(luma, luma, luma), c, 1.80);

                return fixed4(saturate(c), col.a);
            }
            ENDCG
        }
    }
}
