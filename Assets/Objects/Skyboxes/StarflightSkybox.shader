// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Starflight Skybox"
{
	Properties
	{
		[NoScaleOffset] _FrontTex ("Front [+Z]", 2D) = "grey" {}
		[NoScaleOffset] _BackTex ("Back [-Z]", 2D) = "grey" {}
		[NoScaleOffset] _LeftTex ("Left [+X]", 2D) = "grey" {}
		[NoScaleOffset] _RightTex ("Right [-X]", 2D) = "grey" {}
		[NoScaleOffset] _UpTex ("Up [+Y]", 2D) = "grey" {}
		[NoScaleOffset] _DownTex ("Down [-Y]", 2D) = "grey" {}
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

		matrix _Rotation;

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

			float3 rotatedPosition = mul( _Rotation, vd.position );

			o.position = UnityObjectToClipPos( rotatedPosition );
			o.texcoord = vd.texcoord;

			return o;
		}

		half4 skybox_frag( vs_out i, sampler2D smp )
		{
			half3 tex = tex2D( smp, i.texcoord ); // * unity_ColorSpaceDouble.rgb;

			return half4( tex, 1 );
		}

		ENDCG

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _FrontTex;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _FrontTex );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _BackTex;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _BackTex );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _LeftTex;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _LeftTex );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _RightTex;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _RightTex );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _UpTex;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _UpTex );
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _DownTex;

			half4 frag( vs_out i ) : SV_Target
			{
				return skybox_frag( i, _DownTex );
			}

			ENDCG
		}
	}
}
