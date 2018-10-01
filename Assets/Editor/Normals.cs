
using UnityEngine;
using System.Diagnostics;
using System.Threading.Tasks;

public class Normals
{
	float[,] m_buffer;

	int m_width;
	int m_height;

	readonly int[] c_offsetX = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
	readonly int[] c_offsetY = { -1, -1, -1, 0, 0, 0, 1, 1, 1 };

	public Normals( float[,] buffer )
	{
		m_buffer = buffer;

		m_width = buffer.GetLength( 1 );
		m_height = buffer.GetLength( 0 );
	}

	public Color[,] Process( float scale )
	{
		UnityEngine.Debug.Log( "*** Normals Process ***" );

		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var normalBuffer = new Color[ m_height, m_width ];

		var half = new Vector3( 0.5f, 0.5f, 0.5f );

		// make magic happen

		Parallel.For( 0, m_height, parallelOptions, y =>
		{
			for ( var x = 0; x < m_width; x++ )
			{
				var centerHeight = m_buffer[ y, x ];

				var normal = Vector3.zero;

				for ( int i = 0; i < 9; i++ )
				{
					var srcX = x + c_offsetX[ i ];
					var srcY = y + c_offsetY[ i ];

					if ( srcY < 0 )
					{
						srcY = 0;
					}

					if ( srcY >= m_height )
					{
						srcY = m_height - 1;
					}

					if ( srcX < 0 )
					{
						srcX += m_width;
					}

					if ( srcX >= m_width )
					{
						srcX -= m_width;
					}

					var deltaHeight = ( m_buffer[ srcY, srcX ] - centerHeight ) * scale;

					var vector = new Vector3( c_offsetX[ i ], c_offsetY[ i ], deltaHeight );

					var right = Vector3.Cross( vector, Vector3.forward );

					var up = Vector3.Cross( right, vector );

					normal += Vector3.Normalize( up );
				}

				normal = normal / 9.0f * 0.5f + half;

				normalBuffer[ y, x ] = new Color( normal.x, normal.y, normal.z );
			}
		} );

		UnityEngine.Debug.Log( "Total - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		// return the processed buffer

		return normalBuffer;
	}
}
