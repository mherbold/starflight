// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/Sobel" {
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
	float4 edgeColor;
	int showBackground;
	float4 backgroundColor;
	float threshold;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
		float4 lum = float4(0.30f, 0.59f, 0.11f, 1);

		float sob11 = dot(tex2D(_MainTex, i.uv + float2(-1.0f / 1024.0f, -1.0f / 768.0f)), lum);
		float sob12 = dot(tex2D(_MainTex, i.uv + float2(0, -1.0f / 768.0f)), lum);
		float sob13 = dot(tex2D(_MainTex, i.uv + float2(1.0f / 1024.0f, -1.0f / 768.0f)), lum);
		float sob21 = dot(tex2D(_MainTex, i.uv + float2(-1.0f / 1024.0f, 0)), lum);
		float sob23 = dot(tex2D(_MainTex, i.uv + float2(-1.0f / 1024.0f, 0)), lum);	 
		float sob31 = dot(tex2D(_MainTex, i.uv + float2(-1.0f / 1024.0f, 1.0f / 768.0f)), lum);
		float sob32 = dot(tex2D(_MainTex, i.uv + float2(0, 1.0f / 768.0f)), lum);
		float sob33 = dot(tex2D(_MainTex, i.uv + float2(1.0f / 1024.0f, 1.0f / 768.0f)), lum);

		float t1 = sob13 + sob33 + (2 * sob23) - sob11 - (2 * sob21) - sob31;
		float t2 = sob31 + (2 * sob32) + sob33 - sob11 - (2 * sob12) - sob13;
		 
		float4 result;
		 
		if (((t1 * t1) + (t2 * t2)) > threshold * 0.01f)
			result = edgeColor;
		else
		{
			if (showBackground == 1)
		    	result = tex2D(_MainTex, i.uv);
		    else
		    	result = backgroundColor;
		}
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

