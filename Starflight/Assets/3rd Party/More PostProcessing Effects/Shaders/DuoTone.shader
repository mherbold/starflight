// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/DuoTone" {
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
	float4 color1, color2;
	float minLimit, maxLimit;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
		float4 result;
	    result = tex2D(_MainTex, i.uv);
		result.rgb = (result.r + result.g + result.b) / 3;

		if (result.r < minLimit || result.r > maxLimit)
			result.r = color1.r;
		else
			result.r = color2.r;

		if (result.g < minLimit || result.g > maxLimit)
			result.g = color1.g;
		else
			result.g = color2.g;

		if (result.b < minLimit || result.b > maxLimit)
			result.b = color1.b;
		else
			result.b = color2.b;

	    return result;
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

