Shader "Custom/No"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 vert () : SV_POSITION
			{
				return 0;
			}

			fixed4 frag () : SV_Target
			{
				discard;
				return 0;
			}
			ENDCG
		}
	}
}