Shader "Custom/Composite"
{
    Properties
    {
		_MainTex("MainTex", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
			//不知道为什么不能用混合的方式，，，
			//Blend One One
			Cull Off
			//ZWrite Off
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			sampler2D _OutlineTex;

            v2f vert (appdata_img v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                fixed4 col = tex2D(_OutlineTex, i.uv);
				fixed4 main = tex2D(_MainTex, i.uv);
				return main + col;
            }
            ENDCG
        }
    }
}
