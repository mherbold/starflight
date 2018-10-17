
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Threading.Tasks;

public class HydraulicErosion
{
	static float[,] m_buffer;

	static int m_width;
	static int m_height;

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

	static float[,] m_terrainElevation;
	static int[,] m_twistBuffer;
	static Vector3[] m_directionVector;

	static readonly int[] m_dx = { 0, +1, +1, +1, 0, -1, -1, -1 };
	static readonly int[] m_dy = { +1, +1, 0, -1, -1, -1, 0, +1 };

	public HydraulicErosion( float[,] buffer )
	{
		m_buffer = buffer;

		m_width = buffer.GetLength( 1 );
		m_height = buffer.GetLength( 0 );
	}

	public float[,] Process( float xyScaleToMeters, float zScaleToMeters, float rainWaterAmount, float sedimentCapacity, float gravityConstant, float frictionConstant, float evaporationConstant, float depositionConstant, float dissolvingConstant, float stepDeltaTime, int finalBlurRadius )
	{
		UnityEngine.Debug.Log( "*** Hydraulic Erosion Process ***" );

		var stopwatch = new Stopwatch();

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		stopwatch.Start();

		// remember constants
		m_xyScaleToMeters = xyScaleToMeters;
		m_zScaleToMeters = zScaleToMeters;
		m_rainWaterAmount = rainWaterAmount;
		m_sedimentCapacity = sedimentCapacity;

		m_gravityConstant = gravityConstant;
		m_frictionConstant = frictionConstant * stepDeltaTime;
		m_evaporationConstant = evaporationConstant * stepDeltaTime;
		m_depositionConstant = depositionConstant * stepDeltaTime;
		m_dissolvingConstant = dissolvingConstant * stepDeltaTime;

		m_stepDeltaTime = stepDeltaTime;

		// allocate terrain level buffer
		m_terrainElevation = new float[ m_height, m_width ];

		// initialize terrain level buffer
		for ( var y = 0; y < m_height; y++ )
		{
			for ( var x = 0; x < m_width; x++ )
			{
				m_terrainElevation[ y, x ] = m_buffer[ y, x ] * zScaleToMeters;
			}
		}

		UnityEngine.Debug.Log( "Initialize Terrain Level - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		stopwatch.Restart();

		// perlin noise for generating the twist buffer
		var noise = new Noise( 50, 256, 1 );

		// allocate twist buffer
		m_twistBuffer = new int[ m_height, m_width ];

		// initialize twist buffer
		Parallel.For( 0, m_height, parallelOptions, y =>
		{
			for ( var x = 0; x < m_width; x++ )
			{
				var sample = noise.Perlin( 0, 256, x * 128.0f / m_width, y * 64.0f / m_height );

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
		var randomXYSize = m_height / 2;

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
			int x = Random.Range( i, randomXYSizeSquared - 1 );

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
			// EditorUtility.DisplayProgressBar( "Planet Generator", "Making it rain...", (float) step / ( snapshotSteps - 1 ) );

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

		for ( var y = 0; y < m_height; y++ )
		{
			for ( var x = 0; x < m_width; x++ )
			{
				var newElevation = Mathf.Max( 0.0f, m_terrainElevation[ y, x ] * inverseScaleToMeters );

				var delta = newElevation - m_buffer[ y, x ];

				m_terrainElevation[ y, x ] = Mathf.Min( 0.0f, delta );
			}
		}

		UnityEngine.Debug.Log( "Convert to Height Deltas - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// final blur to get rid of the jaggies in the delta heights
		var gaussianBlur = new GaussianBlur( m_terrainElevation );

		m_terrainElevation = gaussianBlur.Process( finalBlurRadius, finalBlurRadius );

		// put original elevation back in
		for ( var y = 0; y < m_height; y++ )
		{
			for ( var x = 0; x < m_width; x++ )
			{
				m_terrainElevation[ y, x ] += m_buffer[ y, x ];
			}
		}

		// return the processed buffer
		return m_terrainElevation;
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
			if ( ( y < 0 ) || ( y >= m_height ) )
			{
				return false;
			}

			// wrap position of drop around horizontally
			if ( x < 0 )
			{
				x += m_width;
			}
			else if ( x >= m_width )
			{
				x -= m_width;
			}

			// did we hit sea level?
			if ( m_terrainElevation[ y, x ] <= -1.0f )
			{
				// yep - bail out now - sediment is lost forever (we don't want to build up the sea level)
				return false;
			}

			// evaporate some of the water in this drop
			m_waterAmount -= m_waterAmount * m_evaporationConstant;

			// reduce velocity of this drop due to friction
			m_velocity -= m_velocity * m_frictionConstant;

			// calculate the current sediment carrying capacity
			float sedimentCapacity = m_sedimentCapacity * m_waterAmount * m_velocity.magnitude;

			// is this drop holding more sediment than it can carry?
			if ( m_sedimentAmount > sedimentCapacity )
			{
				// yes - calculate how much sediment to return to the terrain
				float depositAmount = ( m_sedimentAmount - sedimentCapacity ) * m_depositionConstant;

				// return it to the terrain
				m_terrainElevation[ y, x ] += depositAmount;

				// remove it from the drop
				m_sedimentAmount -= depositAmount;
			}
			else
			{
				// no - calculate how much sediment to pick up from the terrain
				float dissolveAmount = ( sedimentCapacity - m_sedimentAmount ) * m_dissolvingConstant;

				// take it from the terrain
				m_terrainElevation[ y, x ] -= dissolveAmount;

				// add it to the drop
				m_sedimentAmount += dissolveAmount;
			}

			// kill this drop if it doesn't have much water left
			if ( m_waterAmount <= 0.001f )
			{
				return false;
			}

			// get the height of the terrain that this drop is on
			var thisHeight = m_terrainElevation[ y, x ];

			// choose the direction we want to accelerate towards (pick the direction with the steepest decline)
			var chosenDeltaHeight = 0.0f;
			var chosenDirection = 0;

			for ( var direction = 0; direction < 8; direction++ )
			{
				// compute position of the neighbor we want to check
				var neighborX = x + m_dx[ direction ];
				var neighborY = y + m_dy[ direction ];

				// there are no neighbors off the map to the north or south
				if ( ( neighborY < 0 ) || ( neighborY >= m_height ) )
				{
					continue;
				}

				// wrap if the neighbor is off the map to the east or west
				if ( neighborX < 0 )
				{
					neighborX += m_width;
				}
				else if ( neighborX >= m_width )
				{
					neighborX -= m_width;
				}

				// get the terrain height of neighboring cell
				var neighborHeight = m_terrainElevation[ neighborY, neighborX ];

				// calculate the height difference
				var deltaHeight = neighborHeight - thisHeight;

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
