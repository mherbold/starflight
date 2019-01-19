
using UnityEngine;

using System.IO;
using System.IO.Compression;

using System.Diagnostics;

public class PlanetGenerator
{
	// generator constants
	const int c_nonGasGiantTextureMapWidth = 2048;
	const int c_nonGasGiantTextureMapHeight = 1024;

	const int c_gasGiantTextureMapWidth = 256;
	const int c_gasGiantTextureMapHeight = 128;

	const int c_xBlurRadiusGasGiant = 127;
	const int c_yBlurRadiusGasGiant = 1;

	const float c_normalScale = 256.0f;

	// the planet
	GD_Planet m_planet;

	// remember where we were in the maps generation process
	int m_currentStep;

	// resource request handle
	ResourceRequest m_resourceRequest;

	// the texture map size
	public int m_textureMapWidth;
	public int m_textureMapHeight;

	// misc planet map data
	public float m_minimumHeight;
	public float m_waterHeight;
	public float m_snowHeight;

	Color m_waterColor;
	Color m_snowColor;
	Color m_groundColor;

	// the prepared height map and color map
	float[,] m_preparedHeightMap;
	Color[,] m_preparedColorMap;

	// difference buffer parameters (does not exist for gas giants)
	float m_minimumDifference;
	float m_maximumDifference;

	// difference buffer (does not exist for gas giants)
	byte[] m_differenceBuffer;

	// the elevation buffer
	public float[,] m_elevation;

	// the generated maps
	Color[,] m_albedoMap;
	Color[,] m_specularMap;
	Color[,] m_normalMap;
	Color[,] m_waterMaskMap;

	// remember if we are done
	public bool m_mapsGenerated;

	// this becomes true if the data for the current planet is missing
	public bool m_abort;

	// the texture maps
	public Texture2D m_albedoTexture;
	public Texture2D m_specularTexture;
	public Texture2D m_normalTexture;
	public Texture2D m_waterMaskTexture;
	public Texture2D m_legendTexture;

	public void Start( GD_Planet planet )
	{
		m_planet = planet;
		m_currentStep = 0;
		m_mapsGenerated = false;
		m_abort = false;
	}

	public float Process()
	{
		switch ( m_currentStep )
		{
			case 0:
				StartLoadingPlanetData();
				return 0.0f;

			case 1:
				WaitForPlanetData();
				return 0.1f;

			case 2:
				DecompressPlanetData();
				DoBicubicScale();
				return 0.3f;

			case 4:
				ApplyDifferenceBuffer();
				CreateAlbedoBuffer();
				return 0.5f;

			case 6:
				CreateAlbedoTexture();
				return 0.6f;

			case 7:
				CreateSpecularBuffer();
				CreateSpecularTexture();
				CreateWaterMaskBuffer();
				CreateWaterMaskTexture();
				return 1.0f;

			case 11:
				CreateNormalBuffer();
				CreateNormalTexture();
				CleanUp();
				return 1.3f;

			default:
				return 0.0f;
		}
	}

	void StartLoadingPlanetData()
	{
		m_resourceRequest = Resources.LoadAsync( "Planets/" + m_planet.m_id, typeof( TextAsset ) );

		m_currentStep++;
	}

	void WaitForPlanetData()
	{
		if ( m_resourceRequest.isDone )
		{
			m_currentStep++;
		}
	}

	void DecompressPlanetData()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var compressedPlanetData = m_resourceRequest.asset as TextAsset;

		if ( compressedPlanetData == null )
		{
			UnityEngine.Debug.Log( "Missing planet map data for planet " + m_planet.m_id );

			m_abort = true;
		}
		else
		{
			using ( var memoryStream = new MemoryStream( compressedPlanetData.bytes ) )
			{
				using ( var gZipStream = new GZipStream( memoryStream, CompressionMode.Decompress, false ) )
				{
					var binaryReader = new BinaryReader( gZipStream );

					var version = binaryReader.ReadInt32();

					if ( version != 3 )
					{
						UnityEngine.Debug.Log( "Planet map data for planet " + m_planet.m_id + " is the wrong version." );

						m_abort = true;
					}
					else
					{
						m_minimumHeight = binaryReader.ReadSingle();
						m_waterHeight = binaryReader.ReadSingle();
						m_snowHeight = binaryReader.ReadSingle();

						var r = binaryReader.ReadSingle();
						var g = binaryReader.ReadSingle();
						var b = binaryReader.ReadSingle();

						m_waterColor = new Color( r, g, b );

						r = binaryReader.ReadSingle();
						g = binaryReader.ReadSingle();
						b = binaryReader.ReadSingle();

						m_groundColor = new Color( r, g, b );

						r = binaryReader.ReadSingle();
						g = binaryReader.ReadSingle();
						b = binaryReader.ReadSingle();

						m_snowColor = new Color( r, g, b );

						var preparedMapWidth = binaryReader.ReadInt32();
						var preparedMapHeight = binaryReader.ReadInt32();

						m_preparedHeightMap = new float[ preparedMapHeight, preparedMapWidth ];

						for ( var y = 0; y < preparedMapHeight; y++ )
						{
							for ( var x = 0; x < preparedMapWidth; x++ )
							{
								m_preparedHeightMap[ y, x ] = binaryReader.ReadSingle();
							}
						}

						m_preparedColorMap = new Color[ preparedMapHeight, preparedMapWidth ];

						for ( var y = 0; y < preparedMapHeight; y++ )
						{
							for ( var x = 0; x < preparedMapWidth; x++ )
							{
								r = binaryReader.ReadSingle();
								g = binaryReader.ReadSingle();
								b = binaryReader.ReadSingle();

								m_preparedColorMap[ y, x ] = new Color( r, g, b );
							}
						}

						if ( m_planet.IsGasGiant() )
						{
							m_textureMapWidth = c_gasGiantTextureMapWidth;
							m_textureMapHeight = c_gasGiantTextureMapHeight;
						}
						else
						{
							m_textureMapWidth = c_nonGasGiantTextureMapWidth;
							m_textureMapHeight = c_nonGasGiantTextureMapHeight;

							m_minimumDifference = binaryReader.ReadSingle();
							m_maximumDifference = binaryReader.ReadSingle();

							var differenceBufferSize = m_textureMapWidth * m_textureMapHeight;

							m_differenceBuffer = new byte[ differenceBufferSize ];

							gZipStream.Read( m_differenceBuffer, 0, differenceBufferSize );
						}
					}
				}
			}
		}

		m_currentStep++;

		// UnityEngine.Debug.Log( "DecompressPlanetData - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void DoBicubicScale()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		if ( m_planet.IsGasGiant() )
		{
			var bicubicScaleColor = new PG_BicubicScaleColor();

			m_albedoMap = bicubicScaleColor.Process( m_preparedColorMap, m_textureMapWidth, m_textureMapHeight );

			var gaussianBlurColor = new PG_GaussianBlurColor();

			m_albedoMap = gaussianBlurColor.Process( m_albedoMap, c_xBlurRadiusGasGiant, c_yBlurRadiusGasGiant );

			m_currentStep = 6;
		}
		else
		{
			var bicubicScaleElevation = new PG_BicubicScaleElevation();

			m_elevation = bicubicScaleElevation.Process( m_preparedHeightMap, m_textureMapWidth, m_textureMapHeight );

			m_currentStep++;
		}

		// UnityEngine.Debug.Log( "DoBicubicScale - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void ApplyDifferenceBuffer()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var elevationScale = ( m_maximumDifference - m_minimumDifference ) / 255.0f;

		for ( var y = 0; y < m_textureMapHeight; y++ )
		{
			for ( var x = 0; x < m_textureMapWidth; x++ )
			{
				var difference = m_differenceBuffer[ y * m_textureMapWidth + x ];

				m_elevation[ y, x ] += ( difference * elevationScale ) + m_minimumDifference;
			}
		}

		m_currentStep++;

		// UnityEngine.Debug.Log( "ApplyDifferenceBuffer - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void CreateAlbedoBuffer()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var albedoMap = new PG_AlbedoMap();

		m_albedoMap = albedoMap.Process( m_elevation, m_preparedColorMap, m_waterHeight, m_waterColor, m_groundColor );

		m_currentStep++;

		// UnityEngine.Debug.Log( "CreateAlbedoBuffer - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void CreateAlbedoTexture()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var pixels = new Color[ m_textureMapWidth * m_textureMapHeight ];

		var index = 0;

		for ( var y = 0; y < m_textureMapHeight; y++ )
		{
			for ( var x = 0; x < m_textureMapWidth; x++ )
			{
				pixels[ index++ ] = m_albedoMap[ y, x ];
			}
		}

		m_albedoTexture = new Texture2D( m_textureMapWidth, m_textureMapHeight, TextureFormat.RGB24, true );

		m_albedoTexture.SetPixels( pixels );

		m_albedoTexture.filterMode = FilterMode.Trilinear;
		m_albedoTexture.wrapModeU = TextureWrapMode.Repeat;
		m_albedoTexture.wrapModeV = TextureWrapMode.Clamp;

		m_albedoTexture.Apply();

		if ( !m_planet.IsGasGiant() )
		{
			m_albedoTexture.Compress( true );
		}

		m_currentStep++;

		// UnityEngine.Debug.Log( "CreateAlbedoTexture - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void CreateSpecularBuffer()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		if ( m_planet.IsGasGiant() )
		{
			var bicubicScaleElevation = new PG_BicubicScaleElevation();

			m_elevation = bicubicScaleElevation.Process( m_preparedHeightMap, m_textureMapWidth, m_textureMapHeight );

			var gaussianBlurElevation = new PG_GaussianBlurElevation();

			m_elevation = gaussianBlurElevation.Process( m_elevation, c_xBlurRadiusGasGiant, c_yBlurRadiusGasGiant );

			m_specularMap = new Color[ m_textureMapHeight, m_textureMapWidth ];

			for ( var y = 0; y < m_textureMapHeight; y++ )
			{
				for ( var x = 0; x < m_textureMapWidth; x++ )
				{
					var elevation = m_elevation[ y, x ];

					m_specularMap[ y, x ] = new Color( elevation, elevation, elevation, 0.25f );
				}
			}
		}
		else
		{
			var waterSpecularColor = m_planet.IsMolten() ? new Color( 0.75f, 0.125f, 0.125f ) : new Color( 1.0f, 1.0f, 1.0f );

			var waterSpecularPower = m_planet.IsMolten() ? 0.4f : 0.75f;

			var specularMap = new PG_SpecularMap();

			m_specularMap = specularMap.Process( m_elevation, m_albedoMap, m_waterHeight, waterSpecularColor, waterSpecularPower, 4 );
		}

		m_currentStep++;

		// UnityEngine.Debug.Log( "CreateSpecularBuffer - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void CreateSpecularTexture()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var textureMapWidth = m_specularMap.GetLength( 1 );
		var textureMapHeight = m_specularMap.GetLength( 0 );

		var pixels = new Color[ textureMapWidth * textureMapHeight ];

		var index = 0;

		for ( var y = 0; y < textureMapHeight; y++ )
		{
			for ( var x = 0; x < textureMapWidth; x++ )
			{
				pixels[ index++ ] = m_specularMap[ y, x ];
			}
		}

		m_specularTexture = new Texture2D( textureMapWidth, textureMapHeight, TextureFormat.RGBA32, true, true );

		m_specularTexture.SetPixels( pixels );

		m_specularTexture.filterMode = FilterMode.Trilinear;
		m_specularTexture.wrapModeU = TextureWrapMode.Repeat;
		m_specularTexture.wrapModeV = TextureWrapMode.Clamp;

		m_specularTexture.Apply();

		if ( !m_planet.IsGasGiant() )
		{
			m_specularTexture.Compress( true );
		}

		m_currentStep++;

		// UnityEngine.Debug.Log( "CreateSpecularTexture - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void CreateWaterMaskBuffer()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		if ( m_planet.IsGasGiant() )
		{
			m_waterMaskMap = new Color[ 4, 4 ];

			for ( var y = 0; y < 4; y++ )
			{
				for ( var x = 0; x < 4; x++ )
				{
					m_waterMaskMap[ y, x ] = Color.black;
				}
			}
		}
		else
		{
			var waterMaskMap = new PG_WaterMaskMap();

			m_waterMaskMap = waterMaskMap.Process( m_elevation, m_waterHeight, 4 );
		}

		m_currentStep++;

		// UnityEngine.Debug.Log( "CreateWaterMaskBuffer - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void CreateWaterMaskTexture()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var textureMapWidth = m_waterMaskMap.GetLength( 1 );
		var textureMapHeight = m_waterMaskMap.GetLength( 0 );

		var pixels = new Color[ textureMapWidth * textureMapHeight ];

		var index = 0;

		for ( var y = 0; y < textureMapHeight; y++ )
		{
			for ( var x = 0; x < textureMapWidth; x++ )
			{
				pixels[ index++ ] = m_waterMaskMap[ y, x ];
			}
		}

		m_waterMaskTexture = new Texture2D( textureMapWidth, textureMapHeight, TextureFormat.RGB24, true, true );

		m_waterMaskTexture.SetPixels( pixels );

		m_waterMaskTexture.filterMode = FilterMode.Trilinear;
		m_waterMaskTexture.wrapModeU = TextureWrapMode.Repeat;
		m_waterMaskTexture.wrapModeV = TextureWrapMode.Clamp;

		m_waterMaskTexture.Apply();

		m_waterMaskTexture.Compress( true );

		m_currentStep++;

		// UnityEngine.Debug.Log( "CreateWaterMaskTexture - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void CreateNormalBuffer()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		if ( m_planet.IsGasGiant() )
		{
			m_normalMap = new Color[ 4, 4 ];

			var defaultNormal = new Color( 0.5f, 0.5f, 1.0f );

			for ( var y = 0; y < 4; y++ )
			{
				for ( var x = 0; x < 4; x++ )
				{
					m_normalMap[ y, x ] = defaultNormal;
				}
			}
		}
		else
		{
			var normalMap = new PG_NormalMap();

			m_normalMap = normalMap.Process( m_elevation, c_normalScale, m_waterHeight, 2 );
		}

		m_currentStep++;

		// UnityEngine.Debug.Log( "CreateNormalBuffer - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void CreateNormalTexture()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		var textureMapWidth = m_normalMap.GetLength( 1 );
		var textureMapHeight = m_normalMap.GetLength( 0 );

		var pixels = new Color[ textureMapWidth * textureMapHeight ];

		var index = 0;

		for ( var y = 0; y < textureMapHeight; y++ )
		{
			for ( var x = 0; x < textureMapWidth; x++ )
			{
				pixels[ index++ ] = new Color( 0.0f, m_normalMap[ y, x ].g, 0.0f, m_normalMap[ y, x ].r );
			}
		}

		m_normalTexture = new Texture2D( textureMapWidth, textureMapHeight, TextureFormat.RGBA32, true, true );

		m_normalTexture.SetPixels( pixels );

		m_normalTexture.filterMode = FilterMode.Trilinear;
		m_normalTexture.wrapModeU = TextureWrapMode.Repeat;
		m_normalTexture.wrapModeV = TextureWrapMode.Clamp;

		m_normalTexture.Apply();

		m_normalTexture.Compress( true );

		m_currentStep++;

		// UnityEngine.Debug.Log( "CreateNormalTexture - " + stopwatch.ElapsedMilliseconds + " milliseconds" );
	}

	void CleanUp()
	{
		// all done
		m_mapsGenerated = true;

		// free up memory (but keep the elevation buffer)
		m_preparedHeightMap = null;
		m_preparedColorMap = null;
		m_differenceBuffer = null;
		m_albedoMap = null;
		m_specularMap = null;
		m_normalMap = null;
		m_waterMaskMap = null;
	}
}
