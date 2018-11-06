
#ifndef SIMPLEX_NOISE
#define SIMPLEX_NOISE

#define NOISE_SIMPLEX_1_DIV_289 0.00346020761245674740484429065744

float mod289( float x )
{
	return x - floor( x * NOISE_SIMPLEX_1_DIV_289 ) * 289;
}

float2 mod289( float2 x )
{
	return x - floor( x * NOISE_SIMPLEX_1_DIV_289 ) * 289;
}

float3 mod289( float3 x )
{
	return x - floor( x * NOISE_SIMPLEX_1_DIV_289 ) * 289;
}

float4 mod289( float4 x )
{
	return x - floor( x * NOISE_SIMPLEX_1_DIV_289 ) * 289;
}

float permute( float x )
{
	return mod289( x * x * 34 + x );
}

float permute( float2 x )
{
	return mod289( x * x * 34 + x );
}

float3 permute( float3 x )
{
	return mod289( x * x * 34 + x );
}

float4 permute( float4 x )
{
	return mod289( x * x * 34 + x );
}

float taylorInvSqrt( float r )
{
	return 1.79284291400159 - 0.85373472095314 * r;
}

float taylorInvSqrt( float2 r )
{
	return 1.79284291400159 - 0.85373472095314 * r;
}

float taylorInvSqrt( float3 r )
{
	return 1.79284291400159 - 0.85373472095314 * r;
}

float4 taylorInvSqrt( float4 r )
{
	return 1.79284291400159 - 0.85373472095314 * r;
}

float4 grad4( float j, float4 ip )
{
	const float4 ones = float4( 1, 1, 1, -1 );

	float4 p;

	p.xyz = floor( frac( j * ip.xyz ) * 7 ) * ip.z - 1;

	p.w = 1.5 - dot( abs( p.xyz ), ones.xyz );

	p.xyz -= sign( p.xyz ) * ( p.w < 0 );

	return p;
}

float simplex_noise( float2 v )
{
	const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );

	float2 i = floor( v + dot( v, C.yy ) );
	float2 x0 = v - i + dot( i, C.xx );
	float4 x12 = x0.xyxy + C.xxzz;

	int2 i1 = ( x0.x > x0.y ) ? float2( 1, 0 ) : float2( 0, 1 );

	x12.xy -= i1;

	i = mod289( i );

	float3 p = permute( permute( i.y + float3( 0, i1.y, 1 ) ) + i.x + float3( 0, i1.x, 1 ) );
	float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0 );

	m = m * m;
	m = m * m;

	float3 x = 2 * frac( p * C.www ) - 1;
	float3 h = abs( x ) - 0.5;
	float3 ox = floor( x + 0.5 );
	float3 a0 = x - ox;

	m *= taylorInvSqrt( a0 * a0 + h * h );

	float3 g;

	g.x = a0.x * x0.x + h.x * x0.y;
	g.yz = a0.yz * x12.xz + h.yz * x12.yw;

	return 130 * dot( m, g );
}

float simplex_noise( float3 v )
{
	const float2 C = float2( 0.166666666666666667, 0.333333333333333333 );
	const float4 D = float4( 0, 0.5, 1, 2 );

	float3 i = floor( v + dot( v, C.yyy ) );
	float3 x0 = v - i + dot( i, C.xxx );
	float3 g = step( x0.yzx, x0.xyz );
	float3 l = 1 - g;
	float3 i1 = min( g.xyz, l.zxy );
	float3 i2 = max( g.xyz, l.zxy );
	float3 x1 = x0 - i1 + C.xxx;
	float3 x2 = x0 - i2 + C.yyy;
	float3 x3 = x0 - D.yyy;

	i = mod289( i );

	float4 p = permute( permute( permute( i.z + float4( 0, i1.z, i2.z, 1 ) ) + i.y + float4( 0, i1.y, i2.y, 1 ) ) + i.x + float4( 0, i1.x, i2.x, 1) );

	float n_ = 0.142857142857;
	float3 ns = n_ * D.wyz - D.xzx;
	float4 j = p - 49 * floor( p * ns.z * ns.z );
	float4 x_ = floor( j * ns.z );
	float4 y_ = floor( j - 7 * x_);
	float4 x = x_ * ns.x + ns.yyyy;
	float4 y = y_ * ns.x + ns.yyyy;
	float4 h = 1 - abs( x ) - abs( y );
	float4 b0 = float4( x.xy, y.xy );
	float4 b1 = float4( x.zw, y.zw );
	float4 s0 = floor( b0 ) * 2 + 1;
	float4 s1 = floor( b1 ) * 2 + 1;
	float4 sh = -step( h, 0 );
	float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
	float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
	float3 p0 = float3( a0.xy, h.x );
	float3 p1 = float3( a0.zw, h.y );
	float3 p2 = float3( a1.xy, h.z );
	float3 p3 = float3( a1.zw, h.w );

	float4 norm = taylorInvSqrt( float4( dot( p0, p0 ), dot( p1, p1 ), dot( p2, p2 ), dot( p3, p3 ) ) );

	p0 *= norm.x;
	p1 *= norm.y;
	p2 *= norm.z;
	p3 *= norm.w;

	float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0 );

	m = m * m;

	return 42 * dot( m * m, float4( dot( p0, x0 ), dot( p1, x1 ), dot( p2, x2 ), dot( p3, x3 ) ) );
}

float simplex_noise( float4 v )
{
	const float4 C = float4( 0.138196601125011, 0.276393202250021, 0.414589803375032, -0.447213595499958 );

	float4 i = floor( v + dot( v, 0.309016994374947451 ) );
	float4 x0 = v - i + dot( i, C.xxxx );
	float3 isX = step( x0.yzw, x0.xxx );
	float3 isYZ = step( x0.zww, x0.yyz );

	float4 i0;

	i0.x = isX.x + isX.y + isX.z;
	i0.yzw = 1 - isX;
	i0.y += isYZ.x + isYZ.y;
	i0.zw += 1 - isYZ.xy;
	i0.z += isYZ.z;
	i0.w += 1 - isYZ.z;

	float4 i3 = saturate( i0 );
	float4 i2 = saturate( i0 - 1 );
	float4 i1 = saturate( i0 - 2 );
	float4 x1 = x0 - i1 + C.xxxx;
	float4 x2 = x0 - i2 + C.yyyy;
	float4 x3 = x0 - i3 + C.zzzz;
	float4 x4 = x0 + C.wwww;

	i = mod289( i );

	float j0 = permute( permute( permute( permute( i.w ) + i.z ) + i.y ) + i.x );
	float4 j1 = permute( permute( permute( permute( i.w + float4( i1.w, i2.w, i3.w, 1 ) ) + i.z + float4( i1.z, i2.z, i3.z, 1 ) ) + i.y + float4( i1.y, i2.y, i3.y, 1 ) ) + i.x + float4( i1.x, i2.x, i3.x, 1 ) );

	const float4 ip = float4( 0.003401360544217687075, 0.020408163265306122449, 0.142857142857142857143, 0 );

	float4 p0 = grad4( j0, ip );
	float4 p1 = grad4( j1.x, ip );
	float4 p2 = grad4( j1.y, ip );
	float4 p3 = grad4( j1.z, ip );
	float4 p4 = grad4( j1.w, ip );

	float4 norm = taylorInvSqrt( float4( dot( p0, p0 ), dot( p1, p1 ), dot( p2, p2 ), dot( p3, p3 ) ) );

	p0 *= norm.x;
	p1 *= norm.y;
	p2 *= norm.z;
	p3 *= norm.w;
	p4 *= taylorInvSqrt( dot( p4, p4 ) );

	float3 m0 = max( 0.6 - float3( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ) ), 0 );
	float2 m1 = max( 0.6 - float2( dot( x3, x3 ), dot( x4, x4 ) ), 0 );

	m0 = m0 * m0;
	m1 = m1 * m1;

	return 49 * ( dot( m0 * m0, float3( dot( p0, x0 ), dot( p1, x1 ), dot( p2, x2 ) ) ) + dot( m1 * m1, float2( dot( p3, x3 ), dot( p4, x4 ) ) ) );
}

float simplex_turbulence( float4 v, float frequency, float lacunarity, float persistence, int octaves )
{
	float amplitude = 1;
	float sum = 0;

	for ( int i = 0; i < octaves; i++ )
	{
		float h = simplex_noise( float4( v.xyz * frequency, v.w ) );

		sum += amplitude * h;

		frequency *= lacunarity;
		amplitude *= persistence;
	}

	return sum;
}

float ridged_simplex_turbulence( float4 v, float frequency, float lacunarity, float persistence, int octaves )
{
	float amplitude = 1;
	float sum = 0;

	for ( int i = 0; i < octaves; i++ )
	{
		float h = 1 - abs( simplex_noise( float4( v.xyz * frequency, v.w ) ) );

		sum += amplitude * h;

		frequency *= lacunarity;
		amplitude *= persistence;
	}

	return sum;
}

#endif
