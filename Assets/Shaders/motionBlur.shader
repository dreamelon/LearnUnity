﻿Shader "Custom/motionBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BlurAmount("Blur Amount", Float) = 1.0
	}
		SubShader
		{
			CGINCLUDE
			#include "UnityCG.cginc"
				sampler2D _MainTex;
				fixed _BlurAmount;
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


				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				fixed4 fragRGB(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = fixed4(tex2D(_MainTex, i.uv).rgb, _BlurAmount);

					return col;
				}

				half4 fragA(v2f i) : SV_Target{
					return tex2D(_MainTex, i.uv);
				}

			ENDCG


        Tags { "RenderType"="Opaque" }
        LOD 100

		ZTest Always
		Cull Off
		ZWrite Off

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragRGB
            ENDCG
        }
		Pass{
			Blend One Zero
			ColorMask A

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment fragA

			ENDCG

		}
    }

	Fallback Off
}
