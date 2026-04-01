Shader "Custom/RealInkLine"
{
    Properties
    {
        _MainColor ("墨色", Color) = (0.05,0.08,0.1,1)
        _InkSpread ("晕染强度", Range(0.5, 5)) = 2.5
        _EdgeSoft ("边缘柔化", Range(0.05, 0.5)) = 0.3
        _Alpha ("透明度", Range(0,1)) = 0.9
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _MainColor;
            float _InkSpread;
            float _EdgeSoft;
            float _Alpha;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float d = abs(i.uv.y - 0.5);
                // 把 line 改成 lineShape，避开关键字
                float lineShape = 1 - smoothstep(0.5 - _EdgeSoft, 0.5 + _EdgeSoft, d);
                float ink = pow(1 - d, _InkSpread);
                // 用 clamp 替代 saturate，兼容性拉满
                ink = clamp(ink * lineShape, 0.0, 1.0);

                fixed4 col = _MainColor;
                col.a = ink * _Alpha;
                col.rgb *= col.a;
                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}