Shader "Custom/OverDraw"
{
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		ZTest Always // Ignore what is the depth buffer draw pixels anyway
		ZWrite Off // has same effect as above ^, this never write to the depth buffer
		Blend One One // additive blend

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 _OverDrawColor;

			fixed4 frag(v2f i) : SV_Target
			{
				return _OverDrawColor;
			}
			ENDCG
		}
	}
}
