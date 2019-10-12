Shader "Custom/ScrollBackground"
{
    Properties
    {
        _MainTex ("Base Layer(RGB)", 2D) = "white" {} //第一层较远的背景
		_DetailTex ("2nd Layer", 2D) = "white" {}  //第二层较近
		_ScrollX ("Base Layer Scroll Speed", Range(0.0, 1.0)) = 0.5//水平滚动速度
		_Scroll2X ("2nd Layer Scroll Speed", Range(0.0, 1.0)) = 0.6
		_Multiplier ("Layer Multiplier", Float) = 1 //控制纹理亮度
		
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
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _DetailTex;
			float4 _DetailTex_ST;
			float _ScrollX;
			float _Scroll2X;
			float _Multiplier;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex) + frac(float2(_ScrollX, 0.0) * _Time.y);
				o.uv.zw = TRANSFORM_TEX(v.uv, _DetailTex) + frac(float2(_Scroll2X, 0.0) * _Time.y);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 firstLayer = tex2D(_MainTex, i.uv.xy);
				fixed4 secondLayer = tex2D(_DetailTex, i.uv.zw);

				fixed4 col = lerp(firstLayer, secondLayer, secondLayer.a);
				//col.rgb *= _Multiplier;
                return col;
            }
            ENDCG
        }
    }

	Fallback "VertexLit"
}
