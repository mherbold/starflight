// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/CircularBlur" {
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
	float strength;
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
		float2 offsetCoord = i.uv - float2(0.5f, 0.5f);
		float r = length(offsetCoord);
		float theta = atan2(offsetCoord.y, offsetCoord.x);

		float4 sum = (float4)0;
		float2 tapCoords = (float2)0;

		for (int j = 0; j < samples; j++)
		{
			float tapTheta = theta + j * (strength * 0.1f / samples);
			float tapR = r;

			tapCoords.x = tapR * cos(tapTheta) + 0.5f;
			tapCoords.y = tapR * sin(tapTheta) + 0.5f;

			sum += tex2D(_MainTex, tapCoords);
		}

		sum /= samples;
		return sum;
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

