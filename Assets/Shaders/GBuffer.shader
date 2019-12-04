Shader "Custom/GBuffer"
{
    SubShader
    {		
        Tags { "RenderType"="Opaque"}
        LOD 100
			
        Pass
        {
			Tags {  "LightMode" = "Deferred" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;  
            };

            struct v2f 
            {
                float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
            };
			struct FragmentOutput {
				float4 gBuffer0 : SV_Target0;
				float4 gBuffer1 : SV_Target1;
				float4 gBuffer2 : SV_Target2;
				float4 gBuffer3 : SV_Target3;
			};

			//texture2D _GBuffer0;
			//texture2D _GBuffer1;
			//texture2D _GBuffer2;
			//texture2D _GBuffer3;
			//SamplerState sampler_GBuffer;
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

			FragmentOutput frag (v2f i) 
			{
				FragmentOutput output;
				output.gBuffer0 = i.worldPos;
				output.gBuffer1 = float4(normalize(i.worldNormal) * 0.5 + 0.5, 1.0);
				output.gBuffer2 = float4(0.8, 0, 0, 1);
				output.gBuffer3 = float4(0, 0, 0, 1);  
				return output;
            }
            ENDCG
        }
    }
}
