
using UnityEngine;
using UnityEditor;

using System.Diagnostics;
using System.Threading.Tasks;

public class PG_HydraulicErosion
{
	static float m_minimumElevation;
	static float m_xyScaleToMeters;
	static float m_zScaleToMeters;
	static float m_rainWaterAmount;
	static float m_sedimentCapacity;

	static float m_gravityConstant;
	static float m_frictionConstant;
	static float m_evaporationConstant;
	static float m_depositionConstant;
	static float m_dissolvingConstant;

	static float m_stepDeltaTime;

	static int m_outputElevationWidth;
	static int m_outputElevationHeight;
	static int m_outputElevationWidthMask;

	static float[,] m_outputElevation;

	static int[,] m_twistBuffer;
	static Vector3[] m_directionVector;

	static readonly int[] m_dx = { 0, +1, +1, +1, 0, -1, -1, -1 };
	static readonly int[] m_dy = { +1, +1, 0, -1, -1, -1, 0, +1 };

	public float[,] Process( float[,] sourceElevation, float minimumElevation, float xyScaleToMeters, float zScaleToMeters, float rainWaterAmount, float sedimentCapacity, float gravityConstant, float frictionConstant, float evaporationConstant, float depositionConstant, float dissolvingConstant, float stepDeltaTime, int finalBlurRadius )
	{
		UnityEngine.Debug.Log( "*** Hydraulic Erosion Process ***" );

		UnityEngine.Debug.Log( "Gravity constant = " + gravityConstant + " m/s^2" );
		UnityEngine.Debug.Log( "Rain Water Amount = " + rainWaterAmount + " m^3" );

		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		m_outputElevationWidth = sourceElevation.GetLength( 1 );
		m_outputElevationHeight = sourceElevation.GetLength( 0 );
		m_outputElevationWidthMask = m_outputElevationWidth - 1;

		// remember constants
		m_minimumElevation = minimumElevation * zScaleToMeters;
		m_xyScaleToMeters = xyScaleToMeters;
		m_zScaleToMeters = zScaleToMeters;
		m_rainWaterAmount = rainWaterAmount;
		m_sedimentCapacity = sedimentCapacity;

		m_gravityConstant = gravityConstant;
		m_frictionConstant = 1.0f - ( frictionConstant * stepDeltaTime );
		m_evaporationConstant = 1.0f - ( evaporationConstant * stepDeltaTime );
		m_depositionConstant = depositionConstant * stepDeltaTime;
		m_dissolvingConstant = dissolvingConstant * stepDeltaTime;

		m_stepDeltaTime = stepDeltaTime;

		// allocate terrain level buffer
		m_outputElevation = new float[ m_outputElevationHeight, m_outputElevationWidth ];

		// initialize terrain level buffer
		for ( var y = 0; y < m_outputElevationHeight; y++ )
		{
			for ( var x = 0; x < m_outputElevationWidth; x++ )
			{
				m_outputElevation[ y, x ] = sourceElevation[ y, x ] * zScaleToMeters;
			}
		}

		UnityEngine.Debug.Log( "Initialize Terrain Level - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// perlin noise for generating the twist buffer
		var noise = new SeamlessNoise( 50, 256, 1 );

		// allocate twist buffer
		m_twistBuffer = new int[ m_outputElevationHeight, m_outputElevationWidth ];

		// initialize twist buffer
		Parallel.For( 0, m_outputElevationHeight, parallelOptions, y =>
		{
			for ( var x = 0; x < m_outputElevationWidth; x++ )
			{
				var sample = noise.Perlin( 0, 256, x * 128.0f / m_outputElevationWidth, y * 64.0f / m_outputElevationHeight );

				m_twistBuffer[ y, x ] = Mathf.RoundToInt( sample * 31.0f );
			}
		} );

		UnityEngine.Debug.Log( "Generate Twist Buffer - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// allocate direction vector buffer
		m_directionVector = new Vector3[ 512 ];

		// initialize direction vector buffer
		for ( int i = 0; i < 512; i++ )
		{
			var angle = i * Mathf.PI * 2.0f / 512.0f;

			var x = Mathf.Sin( angle ) * m_xyScaleToMeters;
			var y = Mathf.Cos( angle ) * m_xyScaleToMeters;

			m_directionVector[ i ] = new Vector3( x, 0.0f, y );
		}

		UnityEngine.Debug.Log( "Generate Direction Vectors - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// allocate, initialize, and shuffle random xy field
		var randomXYSize = m_outputElevationHeight / 2;

		var randomXYSizeSquared = randomXYSize * randomXYSize;

		var randomXY = new Vector2Int[ randomXYSizeSquared ];

		for ( var y = 0; y < randomXYSize; y++ )
		{
			for ( var x = 0; x < randomXYSize; x++ )
			{
				randomXY[ y * randomXYSize + x ] = new Vector2Int( x, y );
			}
		}

		Random.InitState( 100 );

		for ( var i = 0; i < randomXYSizeSquared; i++ )
		{
			int x = Random.Range( i, randomXYSizeSquared );

			var tmp = randomXY[ i ];

			randomXY[ i ] = randomXY[ x ];
			randomXY[ x ] = tmp;
		}

		UnityEngine.Debug.Log( "Generate Random XY - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// do erosion steps
		var snapshotInterval = 2048;
		var snapshotSteps = randomXYSizeSquared / snapshotInterval;

		for ( var step = 0; step < snapshotSteps; step++ )
		{
			// show progress bar
			if ( EditorUtility.DisplayCancelableProgressBar( "Planet Generator", "Making it rain...", (float) step / ( snapshotSteps - 1 ) ) )
			{
				return null;
			}

			var offset = step * snapshotInterval;

			Parallel.For( 0, 8, parallelOptions, j =>
			{
				var dx = randomXYSize * ( j % 4 );
				var dy = randomXYSize * ( j / 4 );

				for ( var i = 0; i < snapshotInterval; i++ )
				{
					var x = randomXY[ i + offset ].x + dx;
					var y = randomXY[ i + offset ].y + dy;

					var drop = new Drop( x, y, m_rainWaterAmount, 0.0f, Vector3.zero );

					while ( drop.Update() ) { }
				}
			} );
		}

		UnityEngine.Debug.Log( "Erosion Steps - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// remove original elevation and remove scale (prep for blur)
		var inverseScaleToMeters = 1.0f / m_zScaleToMeters;

		for ( var y = 0; y < m_outputElevationHeight; y++ )
		{
			for ( var x = 0; x < m_outputElevationWidth; x++ )
			{
				var newElevation = Mathf.Max( 0.0f, m_outputElevation[ y, x ] * inverseScaleToMeters );

				var delta = newElevation - sourceElevation[ y, x ];

				m_outputElevation[ y, x ] = Mathf.Min( 0.0f, delta );
			}
		}

		UnityEngine.Debug.Log( "Convert to Height Deltas - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// pole smoothing
		for ( var y = 0; y < m_outputElevationHeight; y++ )
		{
			var p1 = m_outputElevationHeight * 0.1f;
			var p2 = m_outputElevationHeight * 0.2f;
			var p3 = m_outputElevationHeight * 0.8f;
			var p4 = m_outputElevationHeight * 0.9f;

			var poleMultiplier = Mathf.SmoothStep( 0.0f, 1.0f, ( y - p1 ) / ( p2 - p1 ) ) * Mathf.SmoothStep( 1.0f, 0.0f, ( y - p3 ) / ( p4 - p3 ) );

			for ( var x = 0; x < m_outputElevationWidth; x++ )
			{
				m_outputElevation[ y, x ] *= poleMultiplier;
			}
		}

		UnityEngine.Debug.Log( "Pole Smoothing - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// final blur to get rid of the jaggies in the delta heights
		var gaussianBlur = new PG_GaussianBlurElevation();

		m_outputElevation = gaussianBlur.Process( m_outputElevation, finalBlurRadius, finalBlurRadius );

		// put original elevation back in
		for ( var y = 0; y < m_outputElevationHeight; y++ )
		{
			for ( var x = 0; x < m_outputElevationWidth; x++ )
			{
				m_outputElevation[ y, x ] += sourceElevation[ y, x ];
			}
		}

		UnityEngine.Debug.Log( "Final Blur - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// return the processed buffer
		return m_outputElevation;
	}

	class Drop
	{
		float m_x;
		float m_y;

		float m_waterAmount;
		float m_sedimentAmount;

		Vector3 m_velocity;

		public Drop( float x, float y, float waterAmount, float sedimentAmount, Vector3 velocity )
		{
			m_x = x;
			m_y = y;

			m_waterAmount = waterAmount;
			m_sedimentAmount = sedimentAmount;

			m_velocity = velocity;
		}

		public bool Update()
		{
			// cast position to int
			var x = Mathf.FloorToInt( m_x );
			var y = Mathf.FloorToInt( m_y );

			// this drop is done if we go off the map vertically
			if ( ( y < 0 ) || ( y >= m_outputElevationHeight ) )
			{
				return false;
			}

			// wrap position of drop around horizontally
			x = ( x + m_outputElevationWidth ) & m_outputElevationWidthMask;

			// if this drop goes below the minimum elevation then this drop is done
			if ( m_outputElevation[ y, x ] < m_minimumElevation )
			{
				return false;
			}

			// evaporate some of the water in this drop
			m_waterAmount *= m_evaporationConstant;

			// reduce velocity of this drop due to friction
			m_velocity *= m_frictionConstant;

			// calculate the current sediment carrying capacity
			float sedimentCapacity = m_sedimentCapacity * m_waterAmount * m_velocity.magnitude;

			// is this drop holding more sediment than it can carry?
			if ( m_sedimentAmount > sedimentCapacity )
			{
				// yes - calculate how much sediment to return to the terrain
				float depositAmount = ( m_sedimentAmount - sedimentCapacity ) * m_depositionConstant;

				// return it to the terrain
				m_outputElevation[ y, x ] += depositAmount;

				// remove it from the drop
				m_sedimentAmount -= depositAmount;
			}
			else
			{
				// no - calculate how much sediment to pick up from the terrain
				float dissolveAmount = ( sedimentCapacity - m_sedimentAmount ) * m_dissolvingConstant;

				// take it from the terrain
				m_outputElevation[ y, x ] -= dissolveAmount;

				// add it to the drop
				m_sedimentAmount += dissolveAmount;
			}

			// kill this drop if it doesn't have much water left
			if ( m_waterAmount <= 0.001f )
			{
				return false;
			}

			// get the current elevation of the terrain that this drop is on
			var currentElevation = m_outputElevation[ y, x ];

			// choose the direction we want to accelerate towards (pick the direction with the steepest decline)
			var chosenDeltaHeight = 0.0f;
			var chosenDirection = 0;

			for ( var direction = 0; direction < 8; direction++ )
			{
				// compute position of the neighbor we want to check
				var neighborX = x + m_dx[ direction ];
				var neighborY = y + m_dy[ direction ];

				// there are no neighbors off the map to the north or south
				if ( ( neighborY < 0 ) || ( neighborY >= m_outputElevationHeight ) )
				{
					continue;
				}

				// wrap if the neighbor is off the map to the east or west
				neighborX = ( neighborX + m_outputElevationWidth ) & m_outputElevationWidthMask;

				// get the terrain height of neighboring cell
				var neighborHeight = m_outputElevation[ neighborY, neighborX ];

				// calculate the height difference
				var deltaHeight = neighborHeight - currentElevation;

				// is this direction the path of least resistance (so far?)
				if ( deltaHeight < chosenDeltaHeight )
				{
					chosenDeltaHeight = deltaHeight;
					chosenDirection = direction;
				}
			}

			// if there is no decline from this spot, and we have little velocity, then we are done
			if ( ( chosenDeltaHeight == 0.0f ) && ( m_velocity.magnitude <= 0.01f ) )
			{
				return false;
			}

			// modify the chosen direction by the random twist
			chosenDirection = ( chosenDirection * 64 + m_twistBuffer[ y, x ] + 512 ) & 511;

			// get the direction vector for the chosen direction
			var directionVector = m_directionVector[ chosenDirection ];

			// get the vector for the chosen direction (plus twist) and factor in the chosen delta height
			var vector = new Vector3( directionVector.x, chosenDeltaHeight, directionVector.z );

			// calculate the acceleration due to the slope
			var acceleration = chosenDeltaHeight * m_gravityConstant / vector.magnitude * vector;

			// update the velocity of this drop
			m_velocity += acceleration * m_stepDeltaTime;

			// cap velocity magnitude to unit length (so we don't skip cells - think of this as the terminal velocity of water)
			if ( m_velocity.magnitude > 1.0f )
			{
				m_velocity = Vector3.Normalize( m_velocity );
			}

			// update drop position
			m_x += m_velocity.x;
			m_y += m_velocity.z;

			// this drop is still alive
			return true;
		}
	}
}
