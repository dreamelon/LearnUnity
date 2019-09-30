Shader "Custom/OutlinePostProces"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
		_SrcTex ("SrcTex", 2D) = "white"{}
		_BlurTex ("BlurTex", 2D) = "white" {}
    }


    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		CGINCLUDE
		#include "UnityCG.cginc"
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};
		
		struct v2f_Cull {
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f_Blur {
			float4 vertex : SV_POSITION;
			half2 uv[5] : TEXCOORD0;
		};

		struct v2f_Add {
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
			float2 uv2 : TEXCOORD2;
		};

		sampler2D _MainTex;	
		sampler2D _BlurTex;
		sampler2D _SrcTex;

		half4 _MainTex_TexelSize;

		fixed4 _OutlineColor;
		float _BlurSize;


		v2f_Blur vertBlurVertical(appdata_img v)
		{
			v2f_Blur o;
			o.vertex = UnityObjectToClipPos(v.vertex);

			half2 uv = v.texcoord;

			//高斯模糊
			o.uv[0] = uv;
			o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
			o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
			return o;
		}

		v2f_Blur vertBlurHorizontal(appdata_img v) {
			v2f_Blur o;
			o.vertex = UnityObjectToClipPos(v.vertex);

			half2 uv = v.texcoord;

			o.uv[0] = uv;
			o.uv[1] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[2] = uv - float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[3] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
			o.uv[4] = uv - float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
			return o;
		}


		fixed4 frag_blur(v2f_Blur i) : SV_Target
		{
			float weight[3] = { 0.4026, 0.2442, 0.0545 };

			fixed3 sum = tex2D(_MainTex, i.uv[0]).rgb;
			
			for (int it = 1; it < 3; it++) {
				sum += tex2D(_MainTex, i.uv[it * 2 - 1]).rgb * weight[it];
				sum += tex2D(_MainTex, i.uv[it * 2]).rgb * weight[it];
			}

			return fixed4(sum, 1.0); 
		}



		v2f_Add vert_add(appdata v) {
			v2f_Add o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			o.uv1 = o.uv;
			o.uv2 = o.uv;
			return o;
		}

		fixed4 frag_add(v2f_Add i) : SV_Target
		{
			fixed4 colorMain = tex2D(_MainTex, i.uv);
			fixed4 colorBlur = tex2D(_BlurTex, i.uv);
			fixed4 colorSrc = tex2D(_SrcTex, i.uv);
			return  colorBlur - colorSrc + colorMain;
		}
		ENDCG

		ZTest Always
		Cull Off
		ZWrite Off

        Pass
        {
			ZTest Off
			Cull Off
			ZWrite Off
            CGPROGRAM
            #pragma vertex vertBlurHorizontal
            #pragma fragment frag_blur
            ENDCG
        }

		Pass
		{

			CGPROGRAM
			#pragma vertex vertBlurVertical
			#pragma fragment frag_blur
			ENDCG
		}

		
		Pass{
			CGPROGRAM
			#pragma vertex vert_add
			#pragma fragment frag_add
			ENDCG
		}

    }
}
