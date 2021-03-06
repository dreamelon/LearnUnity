﻿Shader "Custom/EdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		ZWrite Off
		Cull Off
		ZTest Always
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
                half2 uv[9] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			half4 _MainTex_TexelSize;//纹理所对应纹素的大小
			fixed _EdgeOnly;
			fixed4 _EdgeColor;
			fixed4 _BackgroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                half2 uv = v.uv;
				o.uv[0] = uv + _MainTex_TexelSize.xy * half2(-1, -1);
				o.uv[1] = uv + _MainTex_TexelSize.xy * half2(0, -1);
				o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1, -1);
				o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1, 0);
				o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0, 0);
				o.uv[5] = uv + _MainTex_TexelSize.xy * half2(1, 0);
				o.uv[6] = uv + _MainTex_TexelSize.xy * half2(-1, 1);
				o.uv[7] = uv + _MainTex_TexelSize.xy * half2(0, 1);
				o.uv[8] = uv + _MainTex_TexelSize.xy * half2(1, 1);
                return o;
            }

			fixed luminance(fixed4 color) {
				return 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
			}

			half Sobel(v2f i) {
				const half Gx[9] = {
					-1,0,1,
					-2,0,2,
					-1,0,1

				};
				const half Gy[9] = {
					- 1, -2, -1,
						0,  0,  0,
						1,  2,  1 };
				half texColor;
				half edgeX = 0;
				half edgeY = 0;
				for (int it = 0; it < 9; it++) {
					texColor = luminance(tex2D(_MainTex, i.uv[it]));
					edgeX += texColor * Gx[it];
					edgeY += texColor * Gy[it];
				}
				//return 1 - abs(edgeY) - abs(edgeX) 应当调换color,_EdgeColor顺序
				return abs(edgeY) + abs(edgeX);

			}
			fixed4 frag(v2f i) : SV_Target
			{
				half edge = Sobel(i);
				fixed4 color = tex2D(_MainTex, i.uv[4]);
				fixed4 withEdgeColor = lerp(color, _EdgeColor, edge);
				fixed4 onlyEdgeColor = lerp(_BackgroundColor, _EdgeColor, edge);
				return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
            }


            ENDCG
        }
    }
}
