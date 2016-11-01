
Shader "Outlined" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_OutlineColor ("Outline", Color) = (0, 0, 0, 0)
		_Thickness ("Thickness", Float) = 0.1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f {
				float4 vertex : SV_POSITION;
				half lighting : TEXCOORD0;
			};

			fixed4 _Color;

			v2f vert(appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.lighting = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.lighting += ShadeSH9(half4(worldNormal,1));
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				return _Color * i.lighting;
			}
			ENDCG
		}
		Pass {
			Tags {"LightMode" = "Always"}
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f {
				float4 vertex : SV_POSITION;
			};

			fixed4 _OutlineColor;
			float _Thickness;

			v2f vert(appdata_base v) {
				v2f o;
				float3 pos = mul(unity_ObjectToWorld, v.vertex);
				float3 norm = UnityObjectToWorldNormal(v.normal);
				pos += norm * _Thickness;
				o.vertex = UnityWorldToClipPos(pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				return _OutlineColor;
			}
			ENDCG
		}
	}
}
