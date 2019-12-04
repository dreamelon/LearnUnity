Shader "Custom/BrightnessSaturationAndContrast"
{
	Properties{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

    SubShader
    {
		ZTest Always 
		Cull Off 
		ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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
			float _Brightness;
			float _Contrast;
			float _Saturation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				//brightness
				fixed3 finalColor = col.rgb * _Brightness;
				//saturation
				fixed luminance = 0.2125 * col.r + 0.7154 * col.g + 0.0721 * col.b;
				fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
				finalColor = lerp(luminanceColor, finalColor, _Saturation);
				//contrast
				fixed3 avgColor = fixed3(0.5, 0.5, 0.5);
				finalColor = lerp(avgColor, finalColor, _Contrast);
                return fixed4(finalColor.rgb, col.a);
            }
            ENDCG
        }
    }

	Fallback Off
}
