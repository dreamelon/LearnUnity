Shader "Custom/OutlinePostProces"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BlurTex("BlurTex", 2D) = "white" {}
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

		struct v2f_Cull
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		struct v2f_Blur {
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 uv1 : TEXCOORD1;
			float4 uv2 : TEXCOORD2;
			float4 uv3 : TEXCOORD3;
		};

		struct v2f_Add {
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _BlurTex;
		float4 _BlurTex_ST;
		float4 _offset;

		v2f_Cull vert_cull(appdata v) {
			v2f_Cull o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		fixed4 frag_cull(v2f_Cull i) : SV_Target
		{
			fixed4 colorMain = tex2D(_MainTex, i.uv);
			fixed4 colorBlur = tex2D(_BlurTex, i.uv);
			//模糊纹理减去原纹理
			return colorBlur - colorMain;
		}

		v2f_Blur vert_blur(appdata v)
		{
			v2f_Blur o;
			_offset *= _MainTex_ST.xyxy;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);

			//高斯模糊
			o.uv1 = v.uv.xyxy + _offset.xyxy * float4(1, 1, -1, -1);
			o.uv2 = v.uv.xyxy + _offset.xyxy * float4(1, 1, -1, -1) * 2.0;
			o.uv3 = v.uv.xyxy + _offset.xyxy * float4(1, 1, -1, -1) * 3.0;
			return o;
		}

		fixed4 frag_blur(v2f_Blur i) : SV_Target
		{
			fixed4 color = fixed4(0, 0, 0, 0);
			color += 0.40 * tex2D(_MainTex, i.uv);
			color += 0.15 * tex2D(_MainTex, i.uv1.xy);
			color += 0.15 * tex2D(_MainTex, i.uv1.zw);
			color += 0.10 * tex2D(_MainTex, i.uv2.xy);
			color += 0.10 * tex2D(_MainTex, i.uv2.zw);
			color += 0.05 * tex2D(_MainTex, i.uv3.xy);
			color += 0.05 * tex2D(_MainTex, i.uv3.zw);
			return color;
		}

		v2f_Add vert_add(appdata v) {
			v2f_Add o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			o.uv1 = o.uv;
			return o;
		}

		fixed4 frag_add(v2f_Add i) : SV_Target
		{
			fixed4 colorMain = tex2D(_MainTex, i.uv);
		fixed4 colorOutline = tex2D(_BlurTex, i.uv);
			return colorMain + colorOutline;
		}
		ENDCG

        Pass
        {
			ZTest Off
			Cull Off
			ZWrite Off
            CGPROGRAM
            #pragma vertex vert_blur
            #pragma fragment frag_blur
            ENDCG
        }

		Pass{
			ZTest Off
			Cull Off
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert_cull
			#pragma fragment frag_cull
			ENDCG
		}
		Pass{
			ZTest Off
			Cull Off
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert_add
			#pragma fragment frag_add
			ENDCG
		}

    }
}
