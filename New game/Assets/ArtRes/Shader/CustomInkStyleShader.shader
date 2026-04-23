// ====================================================
// 中国风水墨风格 Shader (Built-in 通用版)
// 所有Unity版本直接用，不用URP
// ====================================================

Shader "Custom/InkStyleShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("主纹理", 2D) = "white" {}
        
        _InkColor ("墨色", Color) = (0.08, 0.08, 0.12, 1.0)
        _InkDensity ("墨色浓度", Range(0.0, 1.0)) = 0.85
        _CenterDarkness ("中心浓墨强度", Range(0.0, 1.0)) = 0.95
        _EdgeLightness ("边缘浅墨强度", Range(0.0, 1.0)) = 0.25
        _BlurRange ("晕染范围", Range(0.01, 1.0)) = 0.6
        _EdgeBlur ("边缘模糊度", Range(0.0, 1.0)) = 0.4
        _BrushSize ("笔触粗细", Range(0.2, 2.0)) = 1.0
        _NoiseScale ("噪声尺度", Range(1.0, 20.0)) = 6.0
        _NoiseStrength ("噪声强度", Range(0.0, 0.5)) = 0.2
        _FlyoutThreshold ("飞白阈值", Range(0.0, 1.0)) = 0.65
        _FlyoutStrength ("飞白强度", Range(0.0, 1.0)) = 0.6
        _FlyoutEdgeOnly ("仅边缘飞白", Range(0.0, 1.0)) = 0.8
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "PreviewType" = "Plane"
            "IgnoreProjector" = "True"
        }
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            
            // 内置管线通用头文件（不会报错！）
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _InkColor;
            float _InkDensity;
            float _CenterDarkness;
            float _EdgeLightness;
            float _BlurRange;
            float _EdgeBlur;
            float _BrushSize;
            float _NoiseScale;
            float _NoiseStrength;
            float _FlyoutThreshold;
            float _FlyoutStrength;
            float _FlyoutEdgeOnly;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            float random (float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
            }
            
            float valueNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a,b,u.x), lerp(c,d,u.x), u.y);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float baseAlpha = tex2D(_MainTex, uv).a;
                if(baseAlpha < 0.01) discard;
                
                float noiseStrength = _NoiseStrength * _BrushSize;
                float2 noiseUV = uv * _NoiseScale;
                float offsetX = (valueNoise(noiseUV) - 0.5) * noiseStrength;
                float offsetY = (valueNoise(noiseUV + float2(10.0, 0.0)) - 0.5) * noiseStrength;
                float2 distortedUV = uv + float2(offsetX, offsetY);
                distortedUV = clamp(distortedUV, 0.001, 0.999);
                
                float2 center = float2(0.5, 0.5);
                float dist = length(distortedUV - center);
                float radius = _BlurRange;
                float t = saturate(dist / radius);
                float power = 1.0 / max(_EdgeBlur, 0.05);
                float gradient = 1.0 - pow(t, power);
                float inkStrength = lerp(_EdgeLightness, _CenterDarkness, gradient);
                
                float2 flyUV = uv * _NoiseScale * 2.0;
                float flyNoise = valueNoise(flyUV);
                float edgeFactor = pow(saturate(dist / radius), _FlyoutEdgeOnly * 2.0);
                float threshold = _FlyoutThreshold * (1.0 - edgeFactor * 0.6);
                float flyEffect = step(threshold, flyNoise);
                float flyIntensity = lerp(1.0, 1.0 - _FlyoutStrength, flyEffect * edgeFactor);
                inkStrength *= flyIntensity;
                
                float detailNoise = valueNoise(uv * _NoiseScale * 4.0);
                inkStrength *= (0.85 + detailNoise * 0.3);
                inkStrength *= _InkDensity;
                
                float finalAlpha = baseAlpha * inkStrength * i.color.a;
                if(finalAlpha < 0.01) discard;
                
                half3 finalColor = _InkColor.rgb;
                finalColor = lerp(0.5, finalColor, inkStrength);
                
                return half4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}