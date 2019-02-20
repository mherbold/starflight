
using UnityEngine;

using System.IO;
using System.IO.Compression;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class PlanetGenerator
{
	// version number
	const int c_versionNumber = 4;

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

	// resource request handle
	ResourceRequest m_resourceRequest;

	// the texture map size
	public int m_textureMapWidth;
	public int m_textureMapHeight;

	// misc planet map data
	public float m_minimumElevation;
	public float m_waterElevation;
	public float m_snowElevation;
	public float m_maximumElevation;

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

	// the pixels
	Color[] m_albedoPixels;
	Color[] m_specularPixels;
	Color[] m_normalPixels;
	Color[] m_waterMaskPixels;

	// the current step
	int m_step;

	// the async task
	Task m_asyncTask;

	// generation progress from 0.0 to 1.0
	public float m_progress;

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
	public Texture2D m_elevationTexture;

	public void Start( GD_Planet planet )
	{
		m_planet = planet;
		m_step = 0;
		m_progress = 0.0f;
		m_mapsGenerated = false;
		m_abort = false;
	}

	public GD_Planet GetPlanet()
	{
		return m_planet;
	}

	public float Process()
	{
		switch ( m_step )
		{
			case 0:
				m_step = 1;
				m_progress = 0.0f;
				m_resourceRequest = Resources.LoadAsync( "Planets/" + m_planet.m_id, typeof( TextAsset ) );
				break;

			case 1:

				if ( m_resourceRequest.isDone )
				{
					var compressedPlanetData = m_resourceRequest.asset as TextAsset;

					if ( compressedPlanetData == null )
					{
						// abort - we have no planet data for this planet!
						m_step = -1;
						m_abort = true;
					}
					else
					{
						m_step = 2;

						// can't get bytes asynchronously - stupid unity
						var bytes = compressedPlanetData.bytes;

						// the secret sauce
						m_asyncTask = Task.Run( () => AsyncProcess( bytes ) );
					}
				}

				break;

			case 2:
				m_asyncTask.Wait();
				break;

			// the stuff below cannot be done asynchronously - stupid unity.

			case 20:
				m_step = 30;
				m_progress = 0.6f;
				CreateAlbedoTexture();
				break;

			case 30:
				m_step = 40;
				m_progress = 0.7f;
				CreateSpecularTexture();
				break;

			case 40:
				m_step = 50;
				m_progress = 0.8f;
				CreateWaterMaskTexture();
				break;

			case 50:
				m_step = 99;
				m_progress = 0.9f;
				CreateNormalTexture();
				break;

			// all done

			case 99:

				m_step = -1;
				m_progress = 1.0f;
				m_mapsGenerated = true;

				// free up memory (but keep the elevation buffer)
				m_preparedHeightMap = null;
				m_preparedColorMap = null;

				m_differenceBuffer = null;

				m_albedoMap = null;
				m_specularMap = null;
				m_normalMap = null;
				m_waterMaskMap = null;

				m_albedoPixels = null;
				m_specularPixels = null;
				m_normalPixels = null;
				m_waterMaskPixels = null;

				break;
		}

		return m_progress;
	}

	public void AsyncProcess( byte[] bytes )
	{
		var progressStepSize = 0.5f / 12.0f;

		m_progress += progressStepSize;

		// decompress the planet data
		using ( var memoryStream = new MemoryStream( bytes ) )
		{
			using ( var gZipStream = new GZipStream( memoryStream, CompressionMode.Decompress, false ) )
			{
				var binaryReader = new BinaryReader( gZipStream );

				var version = binaryReader.ReadInt32();

				if ( version != c_versionNumber )
				{
					m_abort = true;
				}
				else
				{
					m_minimumElevation = binaryReader.ReadSingle();
					m_waterElevation = binaryReader.ReadSingle();
					m_snowElevation = binaryReader.ReadSingle();

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

		// gas giant or not?
		if ( m_planet.IsGasGiant() )
		{
			// yes - do color bicubic scale
			m_progress += progressStepSize;

			var bicubicScaleColor = new PG_BicubicScaleColor();

			m_albedoMap = bicubicScaleColor.Process( m_preparedColorMap, m_textureMapWidth, m_textureMapHeight );

			// do color gaussian blur (this becomes our albedo map)
			m_progress += progressStepSize;

			var gaussianBlurColor = new PG_GaussianBlurColor();

			m_albedoMap = gaussianBlurColor.Process( m_albedoMap, c_xBlurRadiusGasGiant, c_yBlurRadiusGasGiant );

			// do elevation bicubic scale
			m_progress += progressStepSize;

			var bicubicScaleElevation = new PG_BicubicScaleElevation();

			m_elevation = bicubicScaleElevation.Process( m_preparedHeightMap, m_textureMapWidth, m_textureMapHeight );

			// do elevation gaussian blur
			m_progress += progressStepSize;

			var gaussianBlurElevation = new PG_GaussianBlurElevation();

			m_elevation = gaussianBlurElevation.Process( m_elevation, c_xBlurRadiusGasGiant, c_yBlurRadiusGasGiant );

			// build specular map
			m_progress += progressStepSize;

			m_specularMap = new Color[ m_textureMapHeight, m_textureMapWidth ];

			for ( var y = 0; y < m_textureMapHeight; y++ )
			{
				for ( var x = 0; x < m_textureMapWidth; x++ )
				{
					var elevation = m_elevation[ y, x ];

					m_specularMap[ y, x ] = new Color( elevation, elevation, elevation, 0.25f );
				}
			}

			// build water mask map
			m_progress += progressStepSize;

			m_waterMaskMap = new Color[ 4, 4 ];

			for ( var y = 0; y < 4; y++ )
			{
				for ( var x = 0; x < 4; x++ )
				{
					m_waterMaskMap[ y, x ] = Color.black;
				}
			}

			// build normal map
			m_progress += progressStepSize;

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
			// do elevation bicubic scale
			m_progress += progressStepSize;

			var bicubicScaleElevation = new PG_BicubicScaleElevation();

			m_elevation = bicubicScaleElevation.Process( m_preparedHeightMap, m_textureMapWidth, m_textureMapHeight );

			// do craters
			m_progress += progressStepSize;

			if ( m_planet.m_atmosphereDensityId == 0 )
			{
				var craters = new PG_Craters();

				m_elevation = craters.Process( m_elevation, m_planet.m_id, 0.2f, m_waterElevation );
			}

			// factor in elevation difference map
			m_progress += progressStepSize;

			m_maximumElevation = 0.0f;

			var elevationScale = ( m_maximumDifference - m_minimumDifference ) / 255.0f;

			for ( var y = 0; y < m_textureMapHeight; y++ )
			{
				for ( var x = 0; x < m_textureMapWidth; x++ )
				{
					var difference = m_differenceBuffer[ y * m_textureMapWidth + x ];

					m_elevation[ y, x ] += ( difference * elevationScale ) + m_minimumDifference;

					m_maximumElevation = Mathf.Max( m_maximumElevation, m_elevation[ y, x ] );
				}
			}

			// build albedo map
			m_progress += progressStepSize;

			var albedoMap = new PG_AlbedoMap();

			m_albedoMap = albedoMap.Process( m_elevation, m_preparedColorMap, m_waterElevation, m_waterColor, m_groundColor );

			// build specular map
			m_progress += progressStepSize;

			var waterSpecularColor = m_planet.IsMolten() ? new Color( 0.75f, 0.125f, 0.125f ) : new Color( 1.0f, 1.0f, 1.0f );

			var waterSpecularPower = m_planet.IsMolten() ? 0.4f : 0.75f;

			var specularMap = new PG_SpecularMap();

			m_specularMap = specularMap.Process( m_elevation, m_albedoMap, m_waterElevation, waterSpecularColor, waterSpecularPower, 4 );

			// build water mask map
			m_progress += progressStepSize;

			var waterMaskMap = new PG_WaterMaskMap();

			m_waterMaskMap = waterMaskMap.Process( m_elevation, m_waterElevation, 4 );

			// build normal map
			m_progress += progressStepSize;

			var normalMap = new PG_NormalMap();

			m_normalMap = normalMap.Process( m_elevation, c_normalScale, m_waterElevation, 2 );
		}

		// get albedo pixels
		m_progress += progressStepSize;

		m_albedoPixels = new Color[ m_textureMapWidth * m_textureMapHeight ];

		var index = 0;

		for ( var y = 0; y < m_textureMapHeight; y++ )
		{
			for ( var x = 0; x < m_textureMapWidth; x++ )
			{
				m_albedoPixels[ index++ ] = m_albedoMap[ y, x ];
			}
		}

		// get specular pixels
		m_progress += progressStepSize;

		var textureMapWidth = m_specularMap.GetLength( 1 );
		var textureMapHeight = m_specularMap.GetLength( 0 );

		m_specularPixels = new Color[ textureMapWidth * textureMapHeight ];

		index = 0;

		for ( var y = 0; y < textureMapHeight; y++ )
		{
			for ( var x = 0; x < textureMapWidth; x++ )
			{
				m_specularPixels[ index++ ] = m_specularMap[ y, x ];
			}
		}

		// get water mask pixels
		m_progress += progressStepSize;

		textureMapWidth = m_waterMaskMap.GetLength( 1 );
		textureMapHeight = m_waterMaskMap.GetLength( 0 );

		m_waterMaskPixels = new Color[ textureMapWidth * textureMapHeight ];

		index = 0;

		for ( var y = 0; y < textureMapHeight; y++ )
		{
			for ( var x = 0; x < textureMapWidth; x++ )
			{
				m_waterMaskPixels[ index++ ] = m_waterMaskMap[ y, x ];
			}
		}

		// get normal pixels
		m_progress += progressStepSize;

		textureMapWidth = m_normalMap.GetLength( 1 );
		textureMapHeight = m_normalMap.GetLength( 0 );

		m_normalPixels = new Color[ textureMapWidth * textureMapHeight ];

		index = 0;

		for ( var y = 0; y < textureMapHeight; y++ )
		{
			for ( var x = 0; x < textureMapWidth; x++ )
			{
				m_normalPixels[ index++ ] = new Color( 0.0f, m_normalMap[ y, x ].g, 0.0f, m_normalMap[ y, x ].r );
			}
		}

		m_step = 20;
	}

	void CreateAlbedoTexture()
	{
		m_albedoTexture = new Texture2D( m_textureMapWidth, m_textureMapHeight, TextureFormat.RGB24, true );

		m_albedoTexture.SetPixels( m_albedoPixels );

		m_albedoTexture.filterMode = FilterMode.Trilinear;
		m_albedoTexture.wrapModeU = TextureWrapMode.Repeat;
		m_albedoTexture.wrapModeV = TextureWrapMode.Clamp;

		m_albedoTexture.Apply();

		if ( !m_planet.IsGasGiant() )
		{
			m_albedoTexture.Compress( true );
		}
	}

	void CreateSpecularTexture()
	{
		var textureMapWidth = m_specularMap.GetLength( 1 );
		var textureMapHeight = m_specularMap.GetLength( 0 );

		m_specularTexture = new Texture2D( textureMapWidth, textureMapHeight, TextureFormat.RGBA32, true, true );

		m_specularTexture.SetPixels( m_specularPixels );

		m_specularTexture.filterMode = FilterMode.Trilinear;
		m_specularTexture.wrapModeU = TextureWrapMode.Repeat;
		m_specularTexture.wrapModeV = TextureWrapMode.Clamp;

		m_specularTexture.Apply();

		if ( !m_planet.IsGasGiant() )
		{
			m_specularTexture.Compress( true );
		}
	}

	void CreateWaterMaskTexture()
	{
		var textureMapWidth = m_waterMaskMap.GetLength( 1 );
		var textureMapHeight = m_waterMaskMap.GetLength( 0 );

		m_waterMaskTexture = new Texture2D( textureMapWidth, textureMapHeight, TextureFormat.RGB24, true, true );

		m_waterMaskTexture.SetPixels( m_waterMaskPixels );

		m_waterMaskTexture.filterMode = FilterMode.Trilinear;
		m_waterMaskTexture.wrapModeU = TextureWrapMode.Repeat;
		m_waterMaskTexture.wrapModeV = TextureWrapMode.Clamp;

		m_waterMaskTexture.Apply();

		m_waterMaskTexture.Compress( true );
	}

	void CreateNormalTexture()
	{
		var textureMapWidth = m_normalMap.GetLength( 1 );
		var textureMapHeight = m_normalMap.GetLength( 0 );

		m_normalTexture = new Texture2D( textureMapWidth, textureMapHeight, TextureFormat.RGBA32, true, true );

		m_normalTexture.SetPixels( m_normalPixels );

		m_normalTexture.filterMode = FilterMode.Trilinear;
		m_normalTexture.wrapModeU = TextureWrapMode.Repeat;
		m_normalTexture.wrapModeV = TextureWrapMode.Clamp;

		m_normalTexture.Apply();

		m_normalTexture.Compress( true );
	}

	public Texture2D CreateElevationTexture()
	{
		if ( m_elevationTexture == null )
		{
			var textureMapWidth = m_elevation.GetLength( 1 );
			var textureMapHeight = m_elevation.GetLength( 0 );

			var pixels = new Color[ textureMapWidth * textureMapHeight ];

			var index = 0;

			for ( var y = 0; y < textureMapHeight; y++ )
			{
				for ( var x = 0; x < textureMapWidth; x++ )
				{
					var elevation = m_elevation[ y, x ];

					if ( elevation < m_waterElevation )
					{
						elevation = m_waterElevation;
					}

					pixels[ index++ ] = new Color( elevation * 0.25f, 0.0f, 0.0f );
				}
			}

			m_elevationTexture = new Texture2D( textureMapWidth, textureMapHeight, TextureFormat.R16, false, true );

			m_elevationTexture.SetPixels( pixels );

			m_elevationTexture.filterMode = FilterMode.Bilinear;
			m_elevationTexture.wrapModeU = TextureWrapMode.Repeat;
			m_elevationTexture.wrapModeV = TextureWrapMode.Clamp;

			m_elevationTexture.Apply();
		}

		return m_elevationTexture;
	}

	float GetElevation( float x, float y )
	{
		var iX = Mathf.FloorToInt( x );
		var iY = Mathf.FloorToInt( y );

		iX = ( iX + m_textureMapWidth ) & ( m_textureMapWidth - 1 );

		if ( iY < 0 )
		{
			iY = 0;
		}
		else if ( iY >= m_textureMapHeight )
		{
			iY = m_textureMapHeight - 1;
		}

		return m_elevation[ iY, iX ];
	}

	public Vector3 GetBilinearSmoothedNormal( float x, float y, float elevationScale )
	{
		var v1 = new Vector3( x, 0.0f, y );

		v1.y = GetBilinearSmoothedElevation( v1.x, v1.z ) * elevationScale;

		var v2 = new Vector3( x - 0.1f, 0.0f, y - 0.1f );

		v2.y = GetBilinearSmoothedElevation( v2.x, v2.z ) * elevationScale;

		var v3 = new Vector3( x + 0.1f, 0.0f, y - 0.1f );

		v3.y = GetBilinearSmoothedElevation( v3.x, v3.z ) * elevationScale;

		var v4 = new Vector3( x - 0.1f, 0.0f, y + 0.1f );

		v4.y = GetBilinearSmoothedElevation( v4.x, v4.z ) * elevationScale;

		var v5 = new Vector3( x + 0.1f, 0.0f, y + 0.1f );

		v5.y = GetBilinearSmoothedElevation( v5.x, v5.z ) * elevationScale;

		var side1 = v3 - v1;
		var side2 = v2 - v1;

		var normal1 = Vector3.Cross( side1, side2 );

		side1 = v4 - v1;
		side2 = v5 - v1;

		var normal2 = Vector3.Cross( side1, side2 );

		return Vector3.Normalize( normal1 + normal2 );
	}

	public Vector3 GetBicubicSmoothedNormal( float x, float y, float elevationScale )
	{
		var v1 = new Vector3( x, 0.0f, y );

		v1.y = GetBicubicSmoothedElevation( v1.x, v1.z ) * elevationScale;

		var v2 = new Vector3( x - 0.1f, 0.0f, y - 0.1f );

		v2.y = GetBicubicSmoothedElevation( v2.x, v2.z ) * elevationScale;

		var v3 = new Vector3( x + 0.1f, 0.0f, y - 0.1f );

		v3.y = GetBicubicSmoothedElevation( v3.x, v3.z ) * elevationScale;

		var v4 = new Vector3( x - 0.1f, 0.0f, y + 0.1f );

		v4.y = GetBicubicSmoothedElevation( v4.x, v4.z ) * elevationScale;

		var v5 = new Vector3( x + 0.1f, 0.0f, y + 0.1f );

		v5.y = GetBicubicSmoothedElevation( v5.x, v5.z ) * elevationScale;

		var side1 = v3 - v1;
		var side2 = v2 - v1;

		var normal1 = Vector3.Cross( side1, side2 );

		side1 = v4 - v1;
		side2 = v5 - v1;

		var normal2 = Vector3.Cross( side1, side2 );

		return Vector3.Normalize( normal1 + normal2 );
	}

	public float GetBilinearSmoothedElevation( float x, float y )
	{
		var iX = Mathf.FloorToInt( x );
		var iY = Mathf.FloorToInt( y );

		var x0 = iX;
		var x1 = x0 + 1;

		var y0 = iY;
		var y1 = y0 + 1;

		var h00 = GetElevation( x0, y0 );
		var h01 = GetElevation( x1, y0 );

		var h0 = Mathf.Lerp( h00, h01, x - iX );

		var h10 = GetElevation( x0, y1 );
		var h11 = GetElevation( x1, y1 );

		var h1 = Mathf.Lerp( h10, h11, x - iX );

		var elevation = Mathf.Lerp( h0, h1, y - iY );

		return elevation;
	}

	public float GetBicubicSmoothedElevation( float x, float y )
	{
		var iX = Mathf.FloorToInt( x );
		var iY = Mathf.FloorToInt( y );

		var x0 = iX - 1;
		var x1 = x0 + 1;
		var x2 = x1 + 1;
		var x3 = x2 + 1;

		var y0 = iY - 1;
		var y1 = y0 + 1;
		var y2 = y1 + 1;
		var y3 = y2 + 1;

		var h00 = GetElevation( x0, y0 );
		var h01 = GetElevation( x1, y0 );
		var h02 = GetElevation( x2, y0 );
		var h03 = GetElevation( x3, y0 );

		var h0 = InterpolateHermite( h00, h01, h02, h03, x - iX );

		var h10 = GetElevation( x0, y1 );
		var h11 = GetElevation( x1, y1 );
		var h12 = GetElevation( x2, y1 );
		var h13 = GetElevation( x3, y1 );

		var h1 = InterpolateHermite( h10, h11, h12, h13, x - iX );

		var h20 = GetElevation( x0, y2 );
		var h21 = GetElevation( x1, y2 );
		var h22 = GetElevation( x2, y2 );
		var h23 = GetElevation( x3, y2 );

		var h2 = InterpolateHermite( h20, h21, h22, h23, x - iX );

		var h30 = GetElevation( x0, y3 );
		var h31 = GetElevation( x1, y3 );
		var h32 = GetElevation( x2, y3 );
		var h33 = GetElevation( x3, y3 );

		var h3 = InterpolateHermite( h30, h31, h32, h33, x - iX );

		var elevation = InterpolateHermite( h0, h1, h2, h3, y - iY );

		return elevation;
	}

	float InterpolateHermite( float v0, float v1, float v2, float v3, float t )
	{
		var a = 2.0f * v1;
		var b = v2 - v0;
		var c = 2.0f * v0 - 5.0f * v1 + 4.0f * v2 - v3;
		var d = -v0 + 3.0f * v1 - 3.0f * v2 + v3;

		return 0.5f * ( a + ( b * t ) + ( c * t * t ) + ( d * t * t * t ) );
	}
}
