// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/NormalMapDistortion" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex, NormalMap;
	float speedX, speedY;
	float timer;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
		float2 temp;
		temp.x = i.uv.x + 0.1f * speedX * timer;
		temp.y = i.uv.y + 0.1f * speedY * timer;
	    float2 px = tex2D(NormalMap, temp).rg*5;
	    temp.x = (i.uv.x + px.x * 0.005f);
	    temp.y = (i.uv.y + px.y * 0.005f);

	    return float4(tex2D(_MainTex, temp*(1 - px.x * 0.01f)).rgb,1.0f);
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

