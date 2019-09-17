﻿Shader "Custom/Shadow"
{
    Properties
    {
		_Diffuse("Diffuse", Color) = (1, 1, 1, 1)
		_Specular("Specular", Color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(8.0, 256)) = 20
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass{
			Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
			#pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
            };

            struct v2f
            {
				float3 worldNormal : TEXCOORD1;
                float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				SHADOW_COORDS(2)
            };

			fixed4 _Diffuse;
			fixed4 _Specular;
			float _Gloss;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = UnityObjectToWorldDir(v.vertex);

				TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {	
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(worldNormal, worldLightDir));

				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

				//fixed atten = 1.0;
				fixed shadow = SHADOW_ATTENUATION(i);
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                
				return fixed4(ambient + (diffuse + specular) * atten, 1.0);
            }
            ENDCG
        }

		Pass {
				// Pass for other pixel lights
				Tags { "LightMode" = "ForwardAdd" }

				Blend One One

				CGPROGRAM

				// Apparently need to add this declaration
				#pragma multi_compile_fwdadd
				// Use the line below to add shadows for point and spot lights
	//			#pragma multi_compile_fwdadd_fullshadows

				#pragma vertex vert
				#pragma fragment frag

				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				fixed4 _Diffuse;
				fixed4 _Specular;
				float _Gloss;

				struct a2v {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct v2f {
					float4 position : SV_POSITION;
					float3 worldNormal : TEXCOORD0;
					float3 worldPos : TEXCOORD1;
				};

				v2f vert(a2v v) {
					v2f o;
					o.position = UnityObjectToClipPos(v.vertex);

					o.worldNormal = UnityObjectToWorldNormal(v.normal);

					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					fixed3 worldNormal = normalize(i.worldNormal);
					#ifdef USING_DIRECTIONAL_LIGHT
						fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
					#else
						fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
					#endif

					fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(worldNormal, worldLightDir));

					fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
					fixed3 halfDir = normalize(worldLightDir + viewDir);
					fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

					#ifdef USING_DIRECTIONAL_LIGHT
						fixed atten = 1.0;
					#else
						float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
						fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
					#endif

					return fixed4((diffuse + specular) * atten, 1.0);
				}

				ENDCG
			}
    }

	FallBack "Specular"
}