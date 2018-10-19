
using UnityEngine;
using System.Diagnostics;
using System.Threading.Tasks;

public class Contours
{
	readonly float[,] m_buffer;

	readonly int m_width;
	readonly int m_height;

	public Contours( float[,] buffer )
	{
		m_buffer = buffer;

		m_width = buffer.GetLength( 1 );
		m_height = buffer.GetLength( 0 );
	}

	public float[,] Process( int xScale, int yScale, Color[] legend )
	{
		// UnityEngine.Debug.Log( "*** Contours Process ***" );

		// var stopwatch = new Stopwatch();

		// stopwatch.Start();

		var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };

		var halfScaleX = xScale / 2;
		var halfScaleY = yScale / 2;

		var center = new Vector2( halfScaleY, halfScaleY );
		var ratio = (float) halfScaleY / (float) halfScaleX;

		// create square corner
		var squareCorner = new float[ halfScaleY, halfScaleX ];

		for ( var y = 0; y < halfScaleY; y++ )
		{
			for ( var x = 0; x < halfScaleX; x++ )
			{
				squareCorner[ y, x ] = 1.0f;
			}
		}

		// create inside corner
		var insideCorner = new float[ halfScaleY, halfScaleX ];

		Parallel.For( 0, halfScaleY, parallelOptions, y =>
		{
			for ( var x = 0; x < halfScaleX; x++ )
			{
				var subPixelsCovered = 0;

				for ( var subY = 0; subY < 16; subY++ )
				{
					for ( var subX = 0; subX < 16; subX++ )
					{
						var point = new Vector2( ( x + subX / 16.0f ) * ratio, y + subY / 16.0f );

						var distance = Vector2.Distance( center, point );

						if ( distance >= halfScaleY )
						{
							subPixelsCovered++;
						}
					}
				}

				insideCorner[ y, x ] = subPixelsCovered / 256.0f;
			}
		} );

		// create outside corner
		var outsideCorner = new float[ halfScaleY, halfScaleX ];

		Parallel.For( 0, halfScaleY, parallelOptions, y =>
		{
			for ( var x = 0; x < halfScaleX; x++ )
			{
				var subPixelsCovered = 0;

				for ( var subY = 0; subY < 16; subY++ )
				{
					for ( var subX = 0; subX < 16; subX++ )
					{
						var point = new Vector2( ( x + subX / 16.0f ) * ratio, y + subY / 16.0f );

						var distance = Vector2.Distance( center, point );

						if ( distance <= halfScaleY )
						{
							subPixelsCovered++;
						}
					}
				}

				outsideCorner[ y, x ] = subPixelsCovered / 256.0f;
			}
		} );

		// output buffer
		var outputWidth = m_width * xScale;
		var outputHeight = m_height * yScale;

		var outputBuffer = new float[ outputHeight, outputWidth ];

		// initalize water level
		var level = legend[ 0 ].a;

		Parallel.For( 0, m_height, parallelOptions, y =>
		{
			for ( var x = 0; x < outputWidth; x++ )
			{
				outputBuffer[ y, x ] = level;
			}
		} );

		// go through each elevation step
		for ( var i = 1; i < legend.Length; i++ )
		{
			// get the color for this level
			level = legend[ i ].a;

			// go through the source
			Parallel.For( 0, m_height, parallelOptions, y =>
			{
				for ( int x = 0; x < m_width; x++ )
				{
					// build the pattern
					var pattern = 0;

					for ( var y2 = 0; y2 <= 2; y2++ )
					{
						for ( var x2 = 0; x2 <= 2; x2++ )
						{
							var x3 = x + x2 - 1;
							var y3 = y + y2 - 1;

							if ( x3 < 0 )
							{
								x3 += m_width;
							}
							else if ( x3 >= m_width )
							{
								x3 -= m_width;
							}

							if ( y3 < 0 )
							{
								y3 = 0;
							}
							else if ( y3 >= m_height )
							{
								y3 = m_height - 1;
							}

							pattern = ( pattern << 1 ) | ( ( m_buffer[ y3, x3 ] >= level ) ? 1 : 0 );
						}
					}

					// fill in blocks
					var dx = x * xScale;
					var dy = y * yScale;

					if ( ( pattern & 0x010 ) == 0 )
					{
						if ( ( pattern & 0x0A0 ) == 0x0A0 )
						{
							// ?1?
							// 10?
							// ???
							BitBlt( level, insideCorner, outputBuffer, dx, dy, false, false );
						}

						if ( ( pattern & 0x088 ) == 0x088 )
						{
							// ?1?
							// ?01
							// ???
							BitBlt( level, insideCorner, outputBuffer, dx + halfScaleX, dy, true, false );
						}

						if ( ( pattern & 0x022 ) == 0x022 )
						{
							// ???
							// 10?
							// ?1?
							BitBlt( level, insideCorner, outputBuffer, dx, dy + halfScaleY, false, true );
						}

						if ( ( pattern & 0x00A ) == 0x00A )
						{
							// ???
							// ?01
							// ?1?
							BitBlt( level, insideCorner, outputBuffer, dx + halfScaleX, dy + halfScaleY, true, true );
						}
					}
					else
					{
						if ( ( pattern & 0x1B0 ) == 0x010 )
						{
							// 00?
							// 01?
							// ???
							BitBlt( level, outsideCorner, outputBuffer, dx, dy, false, false );
						}

						if ( ( pattern & 0x0D8 ) == 0x010 )
						{
							// ?00
							// ?10
							// ???
							BitBlt( level, outsideCorner, outputBuffer, dx + halfScaleX, dy, true, false );
						}

						if ( ( pattern & 0x036 ) == 0x010 )
						{
							// ???
							// 01?
							// 00?
							BitBlt( level, outsideCorner, outputBuffer, dx, dy + halfScaleY, false, true );
						}

						if ( ( pattern & 0x01B ) == 0x010 )
						{
							// ???
							// ?10
							// ?00
							BitBlt( level, outsideCorner, outputBuffer, dx + halfScaleX, dy + halfScaleY, true, true );
						}

						if ( ( pattern & 0x1B0 ) != 0x010 )
						{
							// 11?
							// 11?
							// ???
							BitBlt( level, squareCorner, outputBuffer, dx, dy, false, false );
						}

						if ( ( pattern & 0x0D8 ) != 0x010 )
						{
							// ?11
							// ?11
							// ???
							BitBlt( level, squareCorner, outputBuffer, dx + halfScaleX, dy, false, false );
						}

						if ( ( pattern & 0x036 ) != 0x010 )
						{
							// ???
							// 11?
							// 11?
							BitBlt( level, squareCorner, outputBuffer, dx, dy + halfScaleY, false, false );
						}

						if ( ( pattern & 0x01B ) != 0x010 )
						{
							// ???
							// ?11
							// ?11
							BitBlt( level, squareCorner, outputBuffer, dx + halfScaleX, dy + halfScaleY, false, false );
						}
					}
				}
			} );
		}

		// UnityEngine.Debug.Log( "Total - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return outputBuffer;
	}

	void BitBlt( float elevation, float[,] block, float[,] outputBuffer, int x, int y, bool flipX, bool flipY )
	{
		var width = block.GetLength( 1 );
		var height = block.GetLength( 0 );

		for ( var yOffset = 0; yOffset < height; yOffset++ )
		{
			for ( var xOffset = 0; xOffset < width; xOffset++ )
			{
				float alpha;

				if ( flipX )
				{
					if ( flipY )
					{
						alpha = block[ height - yOffset - 1, width - xOffset - 1];
					}
					else
					{
						alpha = block[ yOffset, width - xOffset - 1 ];
					}
				}
				else
				{
					if ( flipY )
					{
						alpha = block[ height - yOffset - 1, xOffset ];
					}
					else
					{
						alpha = block[ yOffset, xOffset ];
					}
				}

				outputBuffer[ y + yOffset, x + xOffset ] = Mathf.Lerp( outputBuffer[ y + yOffset, x + xOffset ], elevation, alpha );
			}
		}
	}
}
