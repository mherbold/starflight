
using UnityEngine;
using System;

[Serializable]

public class PG_Planet
{
	public const int c_mapWidth = 48;
	public const int c_mapHeight = 24;

	public int m_id;
	public int m_surfaceId;

	public int[] m_key;
	public int[] m_map;

	public Color[] m_mapLegend { get; private set; }
	public Color[,] m_mapColor { get; private set; }

	public bool m_mapIsValid { get; private set; }

	public void Initialize()
	{
		m_mapIsValid = true;

		if ( m_map.Length != c_mapWidth * c_mapHeight )
		{
			m_mapIsValid = false;

			return;
		}

		var trueKeyLength = 1;

		for ( var i = 1; i < m_key.Length; i++ )
		{
			if ( m_key[ i ] != m_key[ i - 1 ] )
			{
				trueKeyLength++;
			}
		}

		Debug.Log( "...true key length = " + trueKeyLength );

		m_mapLegend = new Color[ trueKeyLength ];

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

			m_mapLegend[ keyIndex ] = new Color( r, g, b, height );

			lastColor = originalColor;

			keyIndex++;
		}

		m_mapColor = new Color[ c_mapHeight, c_mapWidth ];

		for ( var y = 0; y < c_mapHeight; y++ )
		{
			var flippedY = c_mapHeight - y - 1;

			for ( var x = 0; x < c_mapWidth; x++ )
			{
				var offset = y * c_mapWidth + x;

				var originalColor = m_map[ offset ];

				var r = ( ( originalColor >> 16 ) & 0xFF ) / 255.0f;
				var g = ( ( originalColor >> 8 ) & 0xFF ) / 255.0f;
				var b = ( ( originalColor >> 0 ) & 0xFF ) / 255.0f;

				var colorFound = false;

				for ( var i = 0; i < trueKeyLength; i++ )
				{
					if ( ( r == m_mapLegend[ i ].r ) && ( g == m_mapLegend[ i ].g ) && ( b == m_mapLegend[ i ].b ) )
					{
						m_mapColor[ flippedY, x ] = m_mapLegend[ i ];

						colorFound = true;

						break;
					}
				}

				if ( !colorFound )
				{
					m_mapIsValid = false;
				}
			}
		}
	}

	public Color GetMostUsedColor( int y )
	{
		Color mostUsedColor = Color.black;

		int highestCount = 0;

		var count = new int[ m_mapLegend.Length ];

		for ( var i = 0; i < count.Length; i++ )
		{
			count[ i ] = 0;
		}

		for ( var x = 0; x < c_mapWidth; x++ )
		{
			var color = m_mapColor[ y, x ];

			for ( var i = 0; i < m_mapLegend.Length; i++ )
			{
				if ( color == m_mapLegend[ i ] )
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
