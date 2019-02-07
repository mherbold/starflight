
using UnityEditor;
using UnityEngine;
using System;

[Serializable]

public class PG_Planet
{
	public const int c_width = 48;
	public const int c_height = 24;

	public int m_id;
	public int m_surfaceId;

	public float m_minimumElevation;
	public float m_waterElevation;
	public float m_snowElevation;

	public Color m_waterColor = Color.black;
	public Color m_groundColor = Color.black;
	public Color m_snowColor = Color.black;

	public float[,] m_height { get; private set; }
	public Color[,] m_color { get; private set; }

	public bool m_mapIsValid { get; private set; }

	public PG_Planet( GameData gameData, string planetImagesPath, int id )
	{
		// get to the planet data
		var gdPlanet = gameData.m_planetList[ id ];

		// get to the star data
		var gdStar = gameData.m_starList[ gdPlanet.m_starId ];

		// figure out the filename to use to load the original planet maps
		var planetFileName = planetImagesPath + "/planet_" + gdStar.m_xCoordinate + "_" + gdStar.m_yCoordinate + "_" + gdPlanet.m_orbitPosition + "_" + gdPlanet.m_planetTypeId + "_" + gdPlanet.m_surfaceId + ".png";

		// load the original planet maps
		var texture = AssetDatabase.LoadAssetAtPath( planetFileName, typeof( Texture2D ) ) as Texture2D;

		if ( texture == null )
		{
			Debug.Log( "Could not load planet image: " + planetFileName );

			return;
		}

		m_id = gdPlanet.m_id;
		m_surfaceId = gdPlanet.m_surfaceId;

		m_minimumElevation = 1.0f;
		m_waterElevation = 1.0f;
		m_snowElevation = 0.0f;

		m_height = new float[ c_height, c_width ];
		m_color = new Color[ c_height, c_width ];

		for ( var y = 0; y < c_height; y++ )
		{
			var y1 = y + c_height;
			var y2 = y;

			for ( var x = 0; x < c_width; x++ )
			{
				var height = texture.GetPixel( x, y1 ).g;
				var color = texture.GetPixel( x, y2 );

				m_height[ y, x ] = height;
				m_color[ y, x ] = color;

				if ( m_waterColor == color )
				{
					if ( height > m_waterElevation )
					{
						m_waterElevation = height;
					}
				}
				else
				{
					if ( height < m_waterElevation )
					{
						m_waterElevation = height;
						m_waterColor = color;
					}
				}

				if ( height > m_snowElevation )
				{
					m_snowElevation = height;
					m_snowColor = color;
				}

				if ( height < m_minimumElevation )
				{
					m_minimumElevation = height;
				}
			}
		}

		// figure out ground height and color
		var groundHeight = 1.0f;

		for ( var y = 0; y < c_height; y++ )
		{
			for ( var x = 0; x < c_width; x++ )
			{
				if ( m_height[ y, x ] < groundHeight )
				{
					if ( m_height[ y, x ] > m_waterElevation )
					{
						groundHeight = m_height[ y, x ];
						m_groundColor = m_color[ y, x ];
					}
				}
			}
		}

		// special case maps that need adjustments
		switch ( m_id )
		{
			case 5: // earth
				m_waterElevation += 0.005f;

				for ( var y = 0; y < c_height; y++ )
				{
					for ( var x = 0; x < c_width; x++ )
					{
						if ( m_height[ y, x ] < m_waterElevation )
						{
							m_height[ y, x ] = 0;
						}
					}
				}

				break;
		}

		// all good
		m_mapIsValid = true;
	}
}
