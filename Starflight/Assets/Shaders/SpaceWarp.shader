Shader "Hidden/SpaceWarp"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_WarpCenter("Warp Center", Vector) = (0.5, 0.5, 1.0, 1.0)
		_WarpStrength("Warp Strength", Float) = 0.0
	}

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM

			float2 _WarpCenter;
			float _WarpStrength;

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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv;

				float2 d = i.uv - _WarpCenter;
				float t = length(d) * 2;

				t = pow(t, _WarpStrength);

				uv = _WarpCenter + (d * t);

				float4 col = tex2D(_MainTex, uv);

				return col;
			}

			ENDCG
		}
	}
}
