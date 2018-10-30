Shader "Test/Diffuse With Shadows"
{
	SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;

				SHADOW_COORDS( 3 )
			};

			v2f vert( appdata_base v )
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);

				TRANSFER_SHADOW( o );

				return o;
			}

			fixed4 frag( v2f i ) : SV_Target
			{
				float shadow = SHADOW_ATTENUATION( i );

				return float4( shadow.xxx, 1 );
			}

			ENDCG
		}

		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
