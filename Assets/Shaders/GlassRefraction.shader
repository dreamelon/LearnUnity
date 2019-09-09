Shader "Custom/GlassRefraction"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_Cubemap ("Enviorment Cubemap", Cube) = "_Skybox" {}
		_Distortion ("Distortion", Range(0, 100)) = 10
		_RefractAmount ("Refrac Amount", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  "Queue" = "Transparent"}

		GrabPass {"_RefractionTex"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			samplerCUBE _Cubemap;
			float _Distortion;
			fixed _RefractAmount;
			sampler2D _RefractionTex;
			float4 _RefractionTex_TexelSize;//纹素大小

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 uv : TEXCOORD1;
				float4 scrPos : TEXCOORD0;
				float4 TtoW0 : TEXCOORD2;
				float4 TtoW1 : TEXCOORD3;
				float4 TtoW2 : TEXCOORD4;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				//没除w，线性插值
				o.scrPos = ComputeGrabScreenPos(o.vertex);

				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _BumpMap);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

				o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				//折射，对屏幕坐标进行偏移，在切线空间进行。
				float2 offset = bump.xy * _Distortion * _RefractionTex_TexelSize;
				i.scrPos.xy += offset;
				fixed3 refractCol = tex2D(_RefractionTex, i.scrPos.xy / i.scrPos.w).rgb;

				//反射
				bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
				fixed3 reflectDir = reflect(-worldViewDir, bump);
				fixed4 texColor = tex2D(_MainTex, i.uv.xy);
				fixed3 reflectCol = texCUBE(_Cubemap, reflectDir).rgb * texColor.rgb;

				fixed3 finalColor = reflectCol * (1 - _RefractAmount) + refractCol * _RefractAmount;
			
                return fixed4(finalColor, 1);
            }
            ENDCG
        }
    }
	FallBack "Diffuse"
}
