Shader "Custom/Inifinite Starfield"
{
	Properties
	{
		_MainTex( "Particle Texture", 2D ) = "white" {}
		_MinimumDistance( "Minimum Particle Distance", Float ) = 1.0
		_MaximumDistance( "Maximum Particle Distance", Float ) = 1.0
		_Alpha( "Alpha", Float ) = 1.0
	}

	Category
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}

		Blend SrcAlpha One
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
					#pragma multi_compile_particles

					#include "UnityCG.cginc"

					sampler2D _MainTex;
					float4 _MainTex_ST;
					float _MinimumDistance;
					float _MaximumDistance;
					float _Alpha;

					struct vs_in
					{
						float4 position : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct vs_out
					{
						float4 position : SV_POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
						UNITY_VERTEX_OUTPUT_STEREO
					};

					vs_out vert( vs_in v )
					{
						vs_out o;

						UNITY_SETUP_INSTANCE_ID( v );
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

						float a = _Alpha * v.color.a * sin( saturate( ( distance( _WorldSpaceCameraPos, v.position ) - _MinimumDistance ) / ( _MaximumDistance - _MinimumDistance ) ) * 3.1415926f );

						o.position = UnityObjectToClipPos( v.position );
						o.color = float4( v.color.rgb, a );
						o.texcoord = TRANSFORM_TEX( v.texcoord, _MainTex );

						return o;
					}

					fixed4 frag( vs_out v ) : SV_Target
					{
						return tex2D( _MainTex, v.texcoord ) * v.color;
					}

				ENDCG
			}
		}
	}
}
