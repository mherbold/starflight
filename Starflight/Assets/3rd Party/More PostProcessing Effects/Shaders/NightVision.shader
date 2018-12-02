// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/NightVision" {
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
	float timer;
	float luminosityThreshold;
	float amplification;
	float noiseStrength, linesStrength;
	float textureOffset;
	int linesAmount, noiseSaturation;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
	    float4 camTex = tex2D(_MainTex, i.uv); 
 
	    float x = i.uv.x * i.uv.y * timer * 1000 + 10; 
	    x = fmod(x, 20) * fmod(x, 150);  
	    float dx = fmod(x, 0.01f); 

	    float3 noise = camTex.rgb + camTex.rgb * saturate(0.2f + dx.xxx * 100); 
	 
	    float2 sclines;
	    sincos(i.uv.y * linesAmount, sclines.x, sclines.y); 
	    noise += camTex.rgb * float3(sclines.x, sclines.y, sclines.x) * linesStrength; 

	    if (noiseSaturation == 1)
	 		noise = lerp(camTex, noise, saturate(noiseStrength)); 
	 	else
	 		noise = lerp(camTex, noise, noiseStrength); 
	 
	    float4 noiseResult = float4(noise, 1); 
	    
	    float3 col = tex2D(_MainTex, i.uv + noiseResult.xy * 0.005f * textureOffset).rgb;	                       

	    float green = dot(float3(0.30f, 0.59f, 0.11f), col);
	    if (green < luminosityThreshold)
	      col *= amplification; 

	    float3 vision = float3(0.1f, 0.9f, 0.2f);
		return float4((col + (noiseResult * 0.2f)) * vision, 1);
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

