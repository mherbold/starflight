
using UnityEngine;
using System;

[Serializable]

public class PlanetMap
{
	public const int c_width = 48;
	public const int c_height = 24;

	public int[] m_key;
	public int[] m_map;

	public Color[] m_legend { get; private set; }
	public Color[,] m_color { get; private set; }

	public void Initialize()
	{
		var trueKeyLength = 1;

		for ( var i = 1; i < m_key.Length; i++ )
		{
			if ( m_key[ i ] != m_key[ i - 1 ] )
			{
				trueKeyLength++;
			}
		}

		m_legend = new Color[ trueKeyLength ];

		var keyIndex = 0;
		var lastColor = 0;

		for ( var i = 0; i < m_key.Length; i++ )
		{
			var originalColor = m_key[ m_key.Length - i - 1 ];

			if ( originalColor == lastColor )
			{
				continue;
			}

			var r = ( ( originalColor >> 16 ) & 0xFF ) / 255.0f;
			var g = ( ( originalColor >> 8 ) & 0xFF ) / 255.0f;
			var b = ( ( originalColor >> 0 ) & 0xFF ) / 255.0f;

			var height = Mathf.Lerp( 0.0f, 1.0f, (float) keyIndex / (float) ( trueKeyLength - 1 ) );

			m_legend[ keyIndex ] = new Color( r, g, b, height );

			lastColor = originalColor;

			keyIndex++;
		}

		m_color = new Color[ c_height, c_width ];

		for ( var y = 0; y < c_height; y++ )
		{
			var flippedY = c_height - y - 1;

			for ( var x = 0; x < c_width; x++ )
			{
				var offset = y * c_width + x;

				var originalColor = m_map[ offset ];

				var r = ( ( originalColor >> 16 ) & 0xFF ) / 255.0f;
				var g = ( ( originalColor >> 8 ) & 0xFF ) / 255.0f;
				var b = ( ( originalColor >> 0 ) & 0xFF ) / 255.0f;

				for ( var i = 0; i < trueKeyLength; i++ )
				{
					if ( ( r == m_legend[ i ].r ) && ( g == m_legend[ i ].g ) && ( b == m_legend[ i ].b ) )
					{
						m_color[ flippedY, x ] = m_legend[ i ];
					}
				}
			}
		}
	}

	public Color GetMostUsedColor( int y )
	{
		Color mostUsedColor = Color.black;

		int highestCount = 0;

		var count = new int[ m_legend.Length ];

		for ( var i = 0; i < count.Length; i++ )
		{
			count[ i ] = 0;
		}

		for ( var x = 0; x < c_width; x++ )
		{
			var color = m_color[ y, x ];

			for ( var i = 0; i < m_legend.Length; i++ )
			{
				if ( color == m_legend[ i ] )
				{
					count[ i ]++;

					if ( count[ i ] > highestCount )
					{
						mostUsedColor = color;

						highestCount = count[ i ];
					}

					break;
				}
			}
		}

		return mostUsedColor;
	}
}
