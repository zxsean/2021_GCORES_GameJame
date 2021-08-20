Shader "Custom/Color"
{
    Properties
    {

    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        
        Cull Off
        ZWrite Off
        Blend One Zero

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 vertexWS : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                //v.vertex.y += sin(_Time.w + v.vertex.x) * v.vertex.y;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                o.vertexWS = v.vertex;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                //i.color.rgb *= cos(_Time.y) * 0.5 + 0.5;
                //col.rgb *= col.a;
                float4 color = float4(1.0, 0.0, 0.0, 1.0);
                float cos = dot(i.normal, normalize(float3(0.0, 0.0, 1.0) - i.vertexWS));
                
                return color * cos;
            }
            ENDCG
        }
    }
}
