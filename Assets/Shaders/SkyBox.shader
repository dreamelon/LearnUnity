Shader "Custom/Skybox"
{
    Properties
    {
        _Skybox ("Skybox Texture", Cube) = "white" {}
    }
    SubShader
    {
        Pass
        {
			Cull Off
			ZWrite Off
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
                float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD0;
            };

            float4 _Corner[4];
			TextureCube _Skybox;
			SamplerState sampler_Skybox;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
				o.worldPos = _Corner[v.uv.x + v.uv.y * 2].xyz;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
				float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
				return _Skybox.Sample(sampler_Skybox, viewDir);
            }
            ENDCG
        }
    }
}
