// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Billboard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color Tint", Color) = (1,1,1,1)
		_VerticalBillboarding ("Vertical Restraints", Range(0,1)) = 1 // 固定法线或者向上的方向(0,1,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "DisableBatching" = "True" }
        LOD 100

        Pass
        {
			Tags{"LightMode" = "ForwardBase"}
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;
			float _VerticalBillboarding;

            v2f vert (appdata v)
            {
                v2f o;
				//物体锚点位置（模型空间）
				float3 center = float3(0, 0, 0);
				float3 viewer = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
				float3 normalDir = viewer - center;
				//_VerticalBillboarding为1时，法线方向为固定视角方向；_VerticalBillboarding为0时，向上固定为(0,1,0)
				normalDir.y *= _VerticalBillboarding;
				normalDir = normalize(normalDir);
				//防止法线和向上方向平行
				//构建旋转矩阵使得多边形看起来始终面对相机
				float3 upDir = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
				float3 rightDir = normalize(cross(upDir, normalDir));
				upDir = normalize(cross(normalDir, rightDir));
				//根据三个正交基以及原始位置相对锚点的偏移量计算得到新的顶点（将其转换到模型空间）
				float3 centerOffs = v.vertex.xyz - center;
				float3 localPos = center +   rightDir * centerOffs.x + upDir * centerOffs.y + normalDir * centerOffs.z;

				o.vertex = UnityObjectToClipPos(float4(localPos, 1));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb *= _Color.rgb;
                return col;
            }
            ENDCG
        }
    }

	Fallback "Transparent/VertexLit "
}
