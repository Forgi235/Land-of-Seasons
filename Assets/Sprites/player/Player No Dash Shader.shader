Shader "Unlit/PlayerNoDashShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OldColor1 ("Old Color 1", Color) = (0.647, 0.188, 0.188, 1)
        _NewColor1 ("New Color 1", Color) = (0.235, 0.369, 0.545, 1)
        _OldColor2 ("Old Color 2", Color) = (0.0, 1.0, 0.0, 1)
        _NewColor2 ("New Color 2", Color) = (0, 0, 1, 1)
        _OldColor3 ("Old Color 3", Color) = (1.0, 1.0, 0.0, 1)
        _NewColor3 ("New Color 3", Color) = (0, 1, 1, 1)
        _Tolerance ("Tolerance", Range(0, 1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Cull off
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _OldColor1;
            float4 _NewColor1;
            float4 _OldColor2;
            float4 _NewColor2;
            float4 _OldColor3;
            float4 _NewColor3;
            float _Tolerance;

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

                float dist1 = distance(col.rgb, _OldColor1.rgb);
                if (dist1 < _Tolerance)
                {
                    col.rgb = _NewColor1.rgb;
                }
                else
                {
                    float dist2 = distance(col.rgb, _OldColor2.rgb);
                    if (dist2 < _Tolerance)
                    {
                        col.rgb = _NewColor2.rgb;
                    }
                    else
                    {
                        float dist3 = distance(col.rgb, _OldColor3.rgb);
                        if (dist3 < _Tolerance)
                        {
                            col.rgb = _NewColor3.rgb;
                        }
                    }
                }

                return fixed4(col.rgb, col.a);
            }
            ENDCG
        }
    }
}
