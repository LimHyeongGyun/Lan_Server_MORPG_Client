Shader "Custom/Monster/Guage"
{
	Properties
	{
		_BGColor("Background", Color) = (0, 0, 0)
		_FGColor("Foreground", Color) = (1, 0, 0)
		_Guage("Guage", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			ZWrite Off // no z-write
			Blend One Zero // no alpha fading
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

			v2f vert (appdata v)
			{
				v2f o;
				float3 scale;
				scale.x = length(float3(UNITY_MATRIX_M._m00, UNITY_MATRIX_M._m10, UNITY_MATRIX_M._m20));
				scale.y = length(float3(UNITY_MATRIX_M._m01, UNITY_MATRIX_M._m11, UNITY_MATRIX_M._m21));
				scale.z = length(float3(UNITY_MATRIX_M._m02, UNITY_MATRIX_M._m12, UNITY_MATRIX_M._m22));
				v.vertex.xyz *= scale;
				v.vertex.xyz = mul((float3x3)unity_CameraToWorld, v.vertex);
				v.vertex.xyz += float3(UNITY_MATRIX_M._m03, UNITY_MATRIX_M._m13, UNITY_MATRIX_M._m23);
				o.vertex = UnityWorldToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float _Guage;
			fixed4 _BGColor, _FGColor;
			fixed4 frag (v2f i) : SV_Target
			{
				return i.uv.x <= _Guage ? _FGColor : _BGColor;
			}
			ENDCG
		}
	}
}