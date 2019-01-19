
Shader "Starflight/Skybox"
{
	Properties
	{
		[NoScaleOffset] _FrontTexA( "Front [+Z]", 2D ) = "grey" {}
		[NoScaleOffset] _BackTexA( "Back [-Z]", 2D ) = "grey" {}
		[NoScaleOffset] _LeftTexA( "Left [+X]", 2D ) = "grey" {}
		[NoScaleOffset] _RightTexA( "Right [-X]", 2D ) = "grey" {}
		[NoScaleOffset] _UpTexA( "Up [+Y]", 2D ) = "grey" {}
		[NoScaleOffset] _DownTexA( "Down [-Y]", 2D ) = "grey" {}

		[NoScaleOffset] _FrontTexB( "Front [+Z]", 2D ) = "grey" {}
		[NoScaleOffset] _BackTexB( "Back [-Z]", 2D ) = "grey" {}
		[NoScaleOffset] _LeftTexB( "Left [+X]", 2D ) = "grey" {}
		[NoScaleOffset] _RightTexB( "Right [-X]", 2D ) = "grey" {}
		[NoScaleOffset] _UpTexB( "Up [+Y]", 2D ) = "grey" {}
		[NoScaleOffset] _DownTexB( "Down [-Y]", 2D ) = "grey" {}

		_BlendFactor( "Blend Factor", Float ) = 0
		
		_ColorTintA( "Color Tint A", Color ) = ( 1, 1, 1, 1 )
		_ColorTintB( "Color Tint B", Color ) = ( 1, 1, 1, 1 )
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Background"
			"RenderType" = "Background"
			"PreviewType" = "Skybox"
		}

		Cull Off
		ZWrite Off

		CGINCLUDE

		#include "UnityCG.cginc"

		matrix SF_ModelMatrix;

		float SF_BlendFactor;

		float3 SF_ColorTintA;
		float3 SF_ColorTintB;

		struct vertex_data
		{
			float4 position : POSITION;
			float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct vs_out
		{
			float4 position : SV_POSITION;
			float2 texcoord : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		vs_out vert( vertex_data vd )
		{
			vs_out o;

			UNITY_SETUP_INSTANCE_ID( vd );
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

			float3 worldPosition = mul( SF_ModelMatrix, vd.position );
			float3 viewPosition = mul( UNITY_MATRIX_MV, worldPosition );

			float4x4 projectionMatrix = UNITY_MATRIX_P;

			o.position = mul( projectionMatrix, viewPosition );
			o.texcoord = vd.texcoord;

			return o;
		}

		half4 skybox_frag( vs_out i, sampler2D smpA, sampler2D smpB )
		{
			float3 texA = tex2D( smpA, i.texcoord ) * SF_ColorTintA;
			float3 texB = tex2D( smpB, i.texcoord ) * SF_ColorTintB;

			float3 tex = lerp( texA, texB, SF_BlendFactor );

			return float4( tex, 1 );
		}

		ENDCG

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _FrontTexA;
			sampler2D _FrontTexB;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _FrontTexA, _FrontTexB );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _BackTexA;
			sampler2D _BackTexB;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _BackTexA, _BackTexB );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _LeftTexA;
			sampler2D _LeftTexB;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _LeftTexA, _LeftTexB );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _RightTexA;
			sampler2D _RightTexB;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _RightTexA, _RightTexB );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _UpTexA;
			sampler2D _UpTexB;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _UpTexA, _UpTexB );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _DownTexA;
			sampler2D _DownTexB;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _DownTexA, _DownTexB );
			}

			ENDCG
		}
	}
}
