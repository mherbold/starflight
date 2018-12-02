// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/BlackAndBlue" {
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
	float smoothness;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
	    float3 inCol, outCol;
        inCol = tex2D(_MainTex, i.uv).rgb;
        inCol = pow(inCol, 0.45f);
        float3 grayscale = dot(inCol.rgb, 1.0f) * 0.33333f;
        float threshold = smoothstep(0.0f, smoothness, inCol.b - grayscale);
        grayscale = pow(grayscale * 1.1f, 2.0f);
        outCol = lerp(grayscale, inCol * float3(0.5f, 0.5f, 1.1f), threshold);
        return pow(float4(outCol, 1.0f), 2.2f);
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

