// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/Posterization" {
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
	float tonesAmount, gammaFactor;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
		float3 result = tex2D(_MainTex, i.uv).rgb;
		result = pow(result, float3(gammaFactor, gammaFactor, gammaFactor));
		result *= tonesAmount;
		result = floor(result);
		result /= tonesAmount;
		result = pow(result, (float3)(1 / gammaFactor));
		return float4(result, 1);
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

