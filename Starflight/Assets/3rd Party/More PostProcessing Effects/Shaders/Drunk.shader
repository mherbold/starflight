// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/Drunk" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	float timer, strength;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
		float2 blurs[12]
		=	{
				-0.326212, -0.405805,
				-0.840144, -0.073580,
				-0.695914,  0.457137,
				-0.203345,  0.620716,
				 0.962340, -0.194983,
				 0.473434, -0.480026,
				 0.519456,  0.767022,
				 0.185461, -0.893124,
				 0.507431,  0.064425,
				 0.896420,  0.412458,
				-0.321940, -0.932615,
				-0.791559, -0.597705
			};
			
		float4 result = tex2D(_MainTex, i.uv);

		for (int j = 0; j < 12; j++)
		{
			result += tex2D(_MainTex, i.uv + (0.001f * strength) * blurs[j] * (1 + 3 * cos(timer + j * 2)));
		}

		return result / 13;
	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
  
}

Fallback off
	
}

