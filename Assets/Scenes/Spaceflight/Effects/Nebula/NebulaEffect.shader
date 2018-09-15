
Shader "Custom/NebulaEffect" 
{
	Properties 
	{
		_Octaves( "Octaves", Float ) = 8.0
		_Frequency( "Frequency", Float ) = 1.0
		_Amplitude( "Amplitude", Float ) = 1.0
		_Lacunarity( "Lacunarity", Float ) = 1.92
		_Persistence( "Persistence", Float ) = 0.8
		_Offset( "Offset", Vector ) = ( 0.0, 0.0, 0.0, 0.0 )
		_AnimSpeed( "Animation Speed", Float ) = 1.0
		_LowColor( "Low Color", Vector ) = ( 0.0, 0.0, 0.0, 1.0 )
		_HighColor( "High Color", Vector ) = ( 1.0, 1.0, 1.0, 1.0 )
		_Scale( "Scale", Float ) = 1.0
	}

	CGINCLUDE

		void FAST32_hash_3D( float3 gridcell, out float4 lowz_hash_0, out float4 lowz_hash_1, out float4 lowz_hash_2, out float4 highz_hash_0, out float4 highz_hash_1, out float4 highz_hash_2	)
		{
			const float2 OFFSET = float2( 50.0, 161.0 );
			const float DOMAIN = 69.0;
			const float3 SOMELARGEFLOATS = float3( 635.298681, 682.357502, 668.926525 );
			const float3 ZINC = float3( 48.500388, 65.294118, 63.934599 );
		
			gridcell.xyz = gridcell.xyz - floor(gridcell.xyz * ( 1.0 / DOMAIN )) * DOMAIN;

			float3 gridcell_inc1 = step( gridcell, float3( DOMAIN - 1.5, DOMAIN - 1.5, DOMAIN - 1.5 ) ) * ( gridcell + 1.0 );
		
			float4 P = float4( gridcell.xy, gridcell_inc1.xy ) + OFFSET.xyxy;

			P *= P;
			P = P.xzxz * P.yyww;

			float3 lowz_mod = float3( 1.0 / ( SOMELARGEFLOATS.xyz + gridcell.zzz * ZINC.xyz ) );
			float3 highz_mod = float3( 1.0 / ( SOMELARGEFLOATS.xyz + gridcell_inc1.zzz * ZINC.xyz ) );

			lowz_hash_0 = frac( P * lowz_mod.xxxx );
			highz_hash_0 = frac( P * highz_mod.xxxx );
			lowz_hash_1 = frac( P * lowz_mod.yyyy );
			highz_hash_1 = frac( P * highz_mod.yyyy );
			lowz_hash_2 = frac( P * lowz_mod.zzzz );
			highz_hash_2 = frac( P * highz_mod.zzzz );
		}

		float3 Interpolation_C2( float3 x )
		{
			return x * x * x * ( x * ( x * 6.0 - 15.0 ) + 10.0 );
		}

		float Perlin3D( float3 P )
		{
			float3 Pi = floor(P);
			float3 Pf = P - Pi;
			float3 Pf_min1 = Pf - 1.0;

			float4 hashx0, hashy0, hashz0, hashx1, hashy1, hashz1;

			FAST32_hash_3D( Pi, hashx0, hashy0, hashz0, hashx1, hashy1, hashz1 );
		
			float4 grad_x0 = hashx0 - 0.49999;
			float4 grad_y0 = hashy0 - 0.49999;
			float4 grad_z0 = hashz0 - 0.49999;
			float4 grad_x1 = hashx1 - 0.49999;
			float4 grad_y1 = hashy1 - 0.49999;
			float4 grad_z1 = hashz1 - 0.49999;
			float4 grad_results_0 = rsqrt( grad_x0 * grad_x0 + grad_y0 * grad_y0 + grad_z0 * grad_z0 ) * ( float2( Pf.x, Pf_min1.x ).xyxy * grad_x0 + float2( Pf.y, Pf_min1.y ).xxyy * grad_y0 + Pf.zzzz * grad_z0 );
			float4 grad_results_1 = rsqrt( grad_x1 * grad_x1 + grad_y1 * grad_y1 + grad_z1 * grad_z1 ) * ( float2( Pf.x, Pf_min1.x ).xyxy * grad_x1 + float2( Pf.y, Pf_min1.y ).xxyy * grad_y1 + Pf_min1.zzzz * grad_z1 );
		
			float3 blend = Interpolation_C2( Pf );
			float4 res0 = lerp( grad_results_0, grad_results_1, blend.z );
			float2 res1 = lerp( res0.xy, res0.zw, blend.y );
			float final = lerp( res1.x, res1.y, blend.x );

			final *= 1.1547005383792515290182975610039;

			return final;
		}

		float PerlinBillowed( float3 p, int octaves, float3 offset, float frequency, float amplitude, float lacunarity, float persistence )
		{
			float sum = 0;

			for ( int i = 0; i < octaves; i++ )
			{
				float h = abs( Perlin3D( ( p + offset ) * frequency ) );

				sum += h * amplitude;

				frequency *= lacunarity;
				amplitude *= persistence;
			}

			return sum;
		}

	ENDCG

	Category
	{
		Tags
		{
			"Queue" = "Transparent+100"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Blend SrcAlpha One
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always

		SubShader
		{
			Pass
			{
				CGPROGRAM

					#pragma vertex vert
					#pragma fragment frag
					#pragma glsl
					#pragma target 3.0
		
					fixed _Octaves;
					float _Frequency;
					float _Amplitude;
					float3 _Offset;
					float _Lacunarity;
					float _Persistence;
					float _AnimSpeed;
					fixed4 _LowColor;
					fixed4 _HighColor;
					float _Scale;

					struct vs_in
					{
						float4 position : POSITION;
						float3 color : COLOR0;
					};

					struct vs_out
					{
						float4 position : SV_POSITION;
						float3 texcoord : TEXCOORD0;
						float3 color : COLOR0;
					};

					vs_out vert( vs_in v )
					{
						vs_out o;

						o.position = UnityObjectToClipPos( v.position );
						o.texcoord = float3( v.position.xy * _Scale, _Time.y * _AnimSpeed );
						o.color = v.color;

						return o;
					}

					fixed4 frag( vs_out v ) : SV_Target
					{
						float t = PerlinBillowed( v.texcoord, _Octaves, _Offset, _Frequency, _Amplitude, _Lacunarity, _Persistence );

						fixed3 color = lerp( _LowColor, _HighColor, t );

						fixed alpha = smoothstep( 0, 1, v.color.r ) * color.g;

						return fixed4( color, alpha );
					}

				ENDCG
			}
		}
	}
}
