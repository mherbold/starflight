Shader "Custom/Shine"
{
	Properties
	{
		_Color( "Color", Color ) = ( 1, 1, 1, 1 )
		_MainTex( "Albedo (RGB)", 2D ) = "white" {}
		_LerpTime( "LerpTime", Float ) = 0.0
		_Size( "Size", Vector ) = ( 5, 10, 0, 0 )
	}

	Category
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off

		SubShader
		{
			Pass
			{
				CGPROGRAM

					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0

					#include "UnityCG.cginc"

					float4 _Color;
					sampler2D _MainTex;
					float4 _MainTex_ST;
					float _LerpTime;
					float4 _Size;

					struct vs_in
					{
						float4 position : POSITION;
						float2 texcoord : TEXCOORD0;
					};

					struct vs_out
					{
						float4 position : SV_POSITION;
						float2 texcoord : TEXCOORD0;
					};

					vs_out vert( vs_in v )
					{
						vs_out o;

						float s = ( _Size.x + sin( v.position.y + _LerpTime ) * _Size.y ) * v.position.z;

						float x = sin( v.position.x ) * s;
						float y = cos( v.position.x ) * s;

						float3 position = float3( x, y, 0.0f );

						o.position = UnityObjectToClipPos( position );
						o.texcoord = TRANSFORM_TEX( v.texcoord, _MainTex );

						return o;
					}

					fixed4 frag( vs_out v ) : SV_Target
					{
						return tex2D( _MainTex, v.texcoord ) * _Color;
					}

				ENDCG
			}
		}
	}
}
