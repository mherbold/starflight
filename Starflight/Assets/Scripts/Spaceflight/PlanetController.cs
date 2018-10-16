
using UnityEngine;

using System.IO;
using System.IO.Compression;

public class PlanetController : MonoBehaviour
{
	// various constants that control the planet generator
	const int c_numPolePaddingRows = 3;
	const int c_rockyPlanetTextureMapScaleX = 42;
	const int c_rockyPlanetTextureMapScaleY = 34;
	const int c_gasGiantTextureMapScaleX = 10;
	const int c_gasGiantTextureMapScaleY = 8;
	const int c_xBlurRadius = 255;
	const int c_yBlurRadius = 5;
	const float c_normalScale = 10.0f;

	// the current planet this controller is controlling
	public Planet m_planet;

	// access to the planet model
	public GameObject m_planetModel;

	// access to the starport model
	public GameObject m_starportModel;

	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// access to the mesh renderer (to get to the material)
	MeshRenderer m_meshRenderer;

	// set this to the material for this planet model
	Material m_material;

	// the legend texture
	Texture2D m_legendTexture;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
		// if this planet is off then don't do anything
		if ( m_planet != null )
		{
			// get to the player data
			PlayerData playerData = DataController.m_instance.m_playerData;

			// set the current position of the planet
			transform.localPosition = m_planet.GetPosition();

			// update the rotation angle
			float rotationAngle = ( playerData.m_starflight.m_gameTime + m_planet.m_orbitPosition );

			// update the rotation of the planet
			m_planetModel.transform.localRotation = Quaternion.AngleAxis( -30.0f, Vector3.right ) * Quaternion.AngleAxis( rotationAngle * 360.0f, Vector3.forward );

			// update the rotation of the starport
			if ( m_starportModel != null )
			{
				m_starportModel.transform.localRotation = Quaternion.Euler( -90.0f, 0.0f, rotationAngle * 360.0f * 20.0f );
			}
		}
	}

	// call this before you enable the planet
	public void InitializePlanet( Planet planet )
	{
		// check if we have a planet
		if ( ( planet == null ) || ( planet.m_id == -1 ) )
		{
			// nope - forget this planet
			m_planet = null;

			// don't do anything more here
			return;
		}

		// yep - remember the planet we are controlling
		m_planet = planet;

		// get the mesh renderer component
		m_meshRenderer = m_planetModel.GetComponent<MeshRenderer>();

		// grab the material from the mesh renderer and make a clone of it
		m_material = new Material( m_meshRenderer.material );

		// set the cloned material on the mesh renderer
		m_meshRenderer.material = m_material;

		// generate the texture maps for this planet
		GenerateMaps();
	}

	// disable a planet
	public void DisablePlanet()
	{
		// disable the game object
		gameObject.SetActive( false );
	}

	// change the planet we are controlling
	public void EnablePlanet()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// show this orbit
		gameObject.SetActive( true );

		// show or hide the starport model depending on whether or not this planet is Arth
		if ( m_starportModel != null )
		{
			m_starportModel.SetActive( m_planet.m_id == gameData.m_misc.m_arthPlanetId );
		}

		// scale the planet based on its mass
		m_planetModel.transform.localScale = m_planet.GetScale();

		// move the planet to be just below the zero plane
		m_planetModel.transform.localPosition = new Vector3( 0.0f, -16.0f - m_planetModel.transform.localScale.y, 0.0f );
	}

	// call this to get the distance to the player
	public float GetDistanceToPlayer()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// return the distance from the player to the planet
		return Vector3.Distance( playerData.m_starflight.m_systemCoordinates, transform.localPosition );
	}

	// get the material for this planet
	public Material GetMaterial()
	{
		return m_material;
	}

	// get the legend texture for this planet
	public Texture2D GetLegendTexture()
	{
		return m_legendTexture;
	}

	// generate the texture maps and legend texture for this planet
	void GenerateMaps()
	{
		int textureMapScaleX;
		int textureMapScaleY;

		if ( m_planet.m_surfaceId == 1 )
		{
			textureMapScaleX = c_gasGiantTextureMapScaleX;
			textureMapScaleY = c_gasGiantTextureMapScaleY;
		}
		else
		{
			textureMapScaleX = c_rockyPlanetTextureMapScaleX;
			textureMapScaleY = c_rockyPlanetTextureMapScaleY;
		}

		float[,] elevationBuffer;

		Color[] legend;

		var differenceBufferWidth = 2048;
		var differenceBufferHeight = 1024;
		var differenceBufferSize = differenceBufferWidth * differenceBufferHeight;

		float minimumDifference = 0.0f;
		float maximumDifference = 0.0f;

		byte[] differenceBuffer = null;

		var compressedPlanetData = Resources.Load( "Planets/" + m_planet.m_id ) as TextAsset;

		if ( compressedPlanetData == null )
		{
			elevationBuffer = PrepareMap();

			legend = new Color[ 1 ];

			legend[ 1 ] = new Color( 1.0f, 0.65f, 0.0f );
		}
		else
		{
			using ( var memoryStream = new MemoryStream( compressedPlanetData.bytes ) )
			{
				using ( var gZipStream = new GZipStream( memoryStream, CompressionMode.Decompress, false ) )
				{
					var binaryReader = new BinaryReader( gZipStream );

					var version = binaryReader.ReadInt32();

					if ( version != 1 )
					{
						elevationBuffer = PrepareMap();

						legend = new Color[ 1 ];

						legend[ 1 ] = new Color( 1.0f, 0.65f, 0.0f );
					}
					else
					{
						var legendLength = binaryReader.ReadInt32();

						legend = new Color[ legendLength ];

						for ( var i = 0; i < legendLength; i++ )
						{
							legend[ i ].r = binaryReader.ReadSingle();
							legend[ i ].g = binaryReader.ReadSingle();
							legend[ i ].b = binaryReader.ReadSingle();
							legend[ i ].a = binaryReader.ReadSingle();
						}

						var preparedMapWidth = binaryReader.ReadInt32();
						var preparedMapHeight = binaryReader.ReadInt32();

						elevationBuffer = new float[ preparedMapHeight, preparedMapWidth ];

						for ( var y = 0; y < preparedMapHeight; y++ )
						{
							for ( var x = 0; x < preparedMapWidth; x++ )
							{
								elevationBuffer[ y, x ] = binaryReader.ReadSingle();
							}
						}

						if ( m_planet.m_surfaceId != 1 )
						{
							minimumDifference = binaryReader.ReadSingle();
							maximumDifference = binaryReader.ReadSingle();

							differenceBuffer = new byte[ differenceBufferSize ];

							gZipStream.Read( differenceBuffer, 0, differenceBufferSize );
						}
					}
				}
			}
		}

		var contours = new Contours( elevationBuffer );

		elevationBuffer = contours.Process( textureMapScaleX, textureMapScaleY, legend );

		var scaleToPowerOfTwo = new ScaleToPowerOfTwo( elevationBuffer );

		elevationBuffer = scaleToPowerOfTwo.Process( textureMapScaleX, textureMapScaleY );

		var width = elevationBuffer.GetLength( 1 );
		var height = elevationBuffer.GetLength( 0 );

		if ( m_planet.m_surfaceId == 1 )
		{
			var gaussianBlur = new GaussianBlur( elevationBuffer );

			elevationBuffer = gaussianBlur.Process( c_xBlurRadius, c_yBlurRadius );
		}
		else if ( differenceBuffer != null )
		{
			var elevationScale = ( maximumDifference - minimumDifference ) / 255.0f;

			for ( var y = 0; y < height; y++ )
			{
				for ( var x = 0; x < width; x++ )
				{
					var difference = differenceBuffer[ y * width + x ];

					elevationBuffer[ y, x ] += ( difference * elevationScale ) + minimumDifference;
				}
			}
		}

		// albedo texture
		var albedo = new Albedo( elevationBuffer );

		var albedoBuffer = albedo.Process( legend );

		var textureMap = new Texture2D( width, height, TextureFormat.RGB24, true );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				var color = albedoBuffer[ y, x ];

				textureMap.SetPixel( x, y, new Color( color.r, color.g, color.b, 1.0f ) );
			}
		}

		textureMap.filterMode = FilterMode.Trilinear;
		textureMap.wrapModeU = TextureWrapMode.Repeat;
		textureMap.wrapModeV = TextureWrapMode.Clamp;

		textureMap.Apply();

		textureMap.Compress( true );

		m_material.SetTexture( "_Albedo", textureMap );

		// effects texture
		var effectsBuffer = new Color[ height, width ];

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				// get the albedo color
				var color = albedoBuffer[ y, x ];

				var water = ( ( m_planet.m_surfaceId != 1 ) && ( color.a < 0.5f ) ) ? 1.0f : 0.0f;

				// make it shiny where water is
				var roughness = ( water == 1.0f ) ? 0.3f : 1.0f;

				// add in roughness due to snow on mountains (snow = add in sharp gloss)
				roughness = Mathf.Lerp( roughness, 0.3f, ( color.a - 2.0f ) * 0.5f );

				// calculate reflectivity based on roughness (sharp gloss = also reflective, dull gloss = not so reflective)
				var reflectivity = ( 1.0f - roughness ) * 0.5f;

				// put it all together
				effectsBuffer[ y, x ] = new Color( roughness, water, reflectivity );
			}
		}

		textureMap = new Texture2D( width, height, TextureFormat.RGB24, true );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				var color = effectsBuffer[ y, x ];

				textureMap.SetPixel( x, y, new Color( color.r, color.g, color.b, 1.0f ) );
			}
		}

		textureMap.filterMode = FilterMode.Trilinear;
		textureMap.wrapModeU = TextureWrapMode.Repeat;
		textureMap.wrapModeV = TextureWrapMode.Clamp;

		textureMap.Apply();

		textureMap.Compress( true );

		m_material.SetTexture( "_Effects", textureMap );

		// normals texture
		var normals = new Normals( elevationBuffer );

		var normalBuffer = normals.Process( ( m_planet.m_surfaceId == 1 ) ? 1.0f : c_normalScale );

		textureMap = new Texture2D( width, height, TextureFormat.RGBA32, true );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				var color = normalBuffer[ y, x ];

				textureMap.SetPixel( x, y, new Color( 1.0f, color.g, 1.0f, color.r ) );
			}
		}

		textureMap.filterMode = FilterMode.Trilinear;
		textureMap.wrapModeU = TextureWrapMode.Repeat;
		textureMap.wrapModeV = TextureWrapMode.Clamp;

		textureMap.Apply();

		textureMap.Compress( true );

		m_material.SetTexture( "_Normal", textureMap );

		m_meshRenderer.material = m_material;

		// legend texture
		m_legendTexture = new Texture2D( 1, legend.Length, TextureFormat.RGB24, false );

		for ( var i = 0; i < legend.Length; i++ )
		{
			m_legendTexture.SetPixel( 0, i, legend[ i ] );
		}

		m_legendTexture.filterMode = FilterMode.Bilinear;
		m_legendTexture.wrapMode = TextureWrapMode.Clamp;

		m_legendTexture.Apply();
	}

	// generates the map for a planet we don't have data for
	float[,] PrepareMap()
	{
		var preparedMap = new float[ 1, 1 ];

		preparedMap[ 0, 0 ] = 0.0f;

		return preparedMap;
	}
}
