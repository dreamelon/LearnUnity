Shader "Custom/BlurOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Cull Off
			ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            struct v2f
            {
				half4 uv[2]  : TEXCOORD0;
				float4 vertex  : SV_POSITION;
            };

            sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _BlurOffset;

            v2f vert (appdata_img v)
            {
                v2f o;
				float offx = _MainTex_TexelSize * _BlurOffset.x;
				float offy = _MainTex_TexelSize * _BlurOffset.y;

                o.vertex = UnityObjectToClipPos(v.vertex);
				float2 uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord.xy - float2(offx, offy));

				o.uv[0].xy = uv + float2(offx, offy);
				o.uv[0].zw = uv + float2(-offx, offy);
				o.uv[1].xy = uv + float2(offx, -offy);
				o.uv[1].zw = uv + float2(-offx, -offy);
				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

				fixed4 col;
				col = tex2D(_MainTex, i.uv[0].xy);
				col += tex2D(_MainTex, i.uv[0].zw);
				col += tex2D(_MainTex, i.uv[1].xy);
				col += tex2D(_MainTex, i.uv[1].zw);
                return col/2;
            }
            ENDCG
        }
    }
}
