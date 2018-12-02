// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/BleachBypass" {
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
	float darkness;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
		float3 lum = float3(0.25f, 0.65f, 0.1f);
		float4 base = tex2D(_MainTex, i.uv);
	    
	    float lumFactor = dot(lum,base.rgb);
	    float3 bl = lumFactor.rrr;
	    float minmax = min(1, max(0, 10 * (lumFactor- 0.45f)));
	    float3 result1 = 2.0f * base.rgb * bl;
	    float3 result2 = 1.0f - 2.0f * (1.0f - bl) * (1.0f - base.rgb);
	    float3 newColor = lerp(result1, result2, minmax);
	    float A2 = darkness * base.a;
	    float3 mixColor = A2 * newColor.rgb;
	    mixColor += ((1.0f - A2) * base.rgb);
	    return float4(mixColor, 1);
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

