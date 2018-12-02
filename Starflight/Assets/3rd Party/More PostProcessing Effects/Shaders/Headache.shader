// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Author : Maxime JUMELLE
// Project : More Post-Processing Effects Package
// If you have any suggestion or comment, you can write me at webmaster[at]hardgames3d.com

Shader "MorePPEffects/Headache" {
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
	float strength, speed;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : SV_Target 
	{
		float4 result = tex2D(_MainTex, i.uv);
		result += tex2D(_MainTex, i.uv + (cos(timer * 0.2f * speed) + sin(-timer * 0.2f * speed)) / 200 * strength);
		result += tex2D(_MainTex, i.uv + (sin(timer * 0.2f * speed) + cos(-timer * 0.2f * speed)) / 200 * strength);
		result += tex2D(_MainTex, i.uv+ (cos(timer * 0.2f * speed) + cos(-timer * 0.3f * speed)) / 200 * strength);
		result += tex2D(_MainTex, i.uv + (cos(timer * 0.3f * speed) + cos(-timer * 0.2f * speed)) / 200 * strength);
		result /= 5;

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

