Shader "Custom/ColorLiquid"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Blur("Blur", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

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
                float4 color  : COLOR;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color  : COLOR;
            };

            sampler2D _MainTex;
            float _Blur;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv1 = i.uv + float2(_Blur / 5, 0);
                float2 uv2 = i.uv + float2(-_Blur / 5, 0);
                float2 uv3 = i.uv + float2(0, _Blur / 5);
                float2 uv4 = i.uv + float2(0, -_Blur / 5);
                float4 col = tex2D(_MainTex, i.uv) * i.color * 0.1;
                col += tex2D(_MainTex, uv1) * i.color * 0.3;
                col += tex2D(_MainTex, uv2) * i.color * 0.3;
                col += tex2D(_MainTex, uv3) * i.color * 0.3;
                col += tex2D(_MainTex, uv4) * i.color * 0.3;

                col.rgb *= col.a;
                return col;
            }
            ENDCG
        }
    }
}
