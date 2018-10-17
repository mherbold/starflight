// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Starflight Skybox"
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

		matrix _ModelMatrix;
		matrix _ProjectionMatrix;
		float _BlendFactor;

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

			float3 worldPosition = mul( _ModelMatrix, vd.position );
			float3 viewPosition = mul( UNITY_MATRIX_MV, worldPosition );

			float4x4 projectionMatrix = UNITY_MATRIX_P;

			//float halfFov = atan( 1.0 / projectionMatrix._11 );

			//halfFov *= 1.5;

			//projectionMatrix._11 = 1 / tan( halfFov );
			//projectionMatrix._22 = -projectionMatrix._11;

			o.position = mul( projectionMatrix, viewPosition );
			o.texcoord = vd.texcoord;

			return o;
		}

		half4 skybox_frag( vs_out i, sampler2D smpA, sampler2D smpB )
		{
			half3 texA = tex2D( smpA, i.texcoord );
			half3 texB = tex2D( smpB, i.texcoord );

			half3 tex = lerp( texA, texB, _BlendFactor );

			return half4( tex, 1 );
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
