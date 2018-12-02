// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/RadialBlur" {
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
	float blurCenterX, blurCenterY;
	float blurStrength;
	int samples;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
		float2 blurCenter = float2(blurCenterX, blurCenterY);
		float4 result = (float4)0;

		for(int n = 0; n < samples; n++) 
		{ 
			float scale = 1.0f - blurStrength * 0.1f * (n / (float)(samples - 1));
			result += tex2D(_MainTex, (i.uv - blurCenter) * scale + blurCenter); 
		} 

		result /= samples; 

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

