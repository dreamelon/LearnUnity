Shader "Custom/Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint", Color) = (1,1,1,1)
		_Magnitude("Distortion Magnitude", Range(0, 0.15)) = 0.1 //控制水流波动幅度
		_Frequency("Distortion Frequency", Range(0, 10)) = 1 //控制波动频率
		_InvWaveLength("Distortion Inverse Wave Length", Range(0, 5 )) = 10 //控制波长
		_Speed("Speed", Float) = 0.5 //控制水流纹理移动速度
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "IgnoreProjector"="True"  "Queue" = "Transparent" "DisableBatching"="True" } 
        LOD 100

        Pass
        {
			Tags {"LightMode" = "ForwardBase"}
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;
			float _Speed;
			float _Frequency;
			float _Magnitude;
			float _InvWaveLength;

            v2f vert (appdata v)
            {
                v2f o;
				float4 offset;
				offset.yzw = float3(0.0, 0.0, 0.0);
				//注意该模型空间，vertex.y无影响；x大家都一样；z轴不同，所以导致波形
				offset.x = sin(_Frequency * _Time.y + v.vertex.x * _InvWaveLength + v.vertex.y * _InvWaveLength
					+ v.vertex.z * _InvWaveLength) * _Magnitude;
                o.vertex = UnityObjectToClipPos(v.vertex + offset);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + float2(0.0, _Time.y * _Speed);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * _Color;
            }
            ENDCG
        }
    }
	Fallback "Transparent/VertexLit"
}
