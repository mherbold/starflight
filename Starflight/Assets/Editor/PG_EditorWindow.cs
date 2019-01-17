
using UnityEngine;
using UnityEditor;

using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

public class PG_EditorWindow : EditorWindow
{
	// version number
	const int c_versionNumber = 3;

	// number of planets
	const int c_numPlanets = 811;

	// other settings
	string m_gameDataFileName;
	string m_planetImagesPath;
	string m_resourcesPath;
	bool m_debugMode;
	int m_debugPlanetID;

	// non-gas giant planet settings
	int m_textureMapWidth;
	int m_textureMapHeight;
	int m_numPolePaddingRows;

	// mountain settings
	int m_octaves;
	int m_mountainScale;
	float m_mountainPersistence;
	float m_mountainGain;

	// hydraulic erosion settings
	bool m_doHydraulicErosionPass;
	float m_xyScaleToMeters;
	float m_zScaleToMeters;
	float m_rainWaterAmount;
	float m_gravityConstant;
	float m_frictionConstant;
	float m_evaporationConstant;
	float m_sedimentCapacity;
	float m_depositionConstant;
	float m_dissolvingConstant;
	float m_stepDeltaTime;
	int m_finalBlurRadius;

	// generated parameters
	GameObject m_gameObject;
	MeshRenderer m_meshRenderer;
	int m_textureMapScaleX;
	int m_textureMapScaleY;
	int m_mountainScalePowerOfTwo;
	Color m_topPaddingColor;
	Color m_bottomPaddingColor;

#if DEBUG
	[MenuItem( "Starflight Remake/Planet Generator (Debug Build)" )]
#else
	[MenuItem( "Starflight Remake/Planet Generator (Release Build)" )]
#endif

	public static void ShowWindow()
	{
		GetWindow( typeof( PG_EditorWindow ), true, "Planet Generator" );
	}

	void OnEnable()
	{
		// other settings
		m_gameDataFileName = EditorPrefs.GetString( "PlanetGenerator_GameDataFileName" );
		m_planetImagesPath = EditorPrefs.GetString( "PlanetGenerator_PlanetImagesPath" );
		m_resourcesPath = EditorPrefs.GetString( "PlanetGenerator_ResourcesPath" );
		m_debugMode = EditorPrefs.GetBool( "PlanetGenerator_DebugMode" );
		m_debugPlanetID = EditorPrefs.GetInt( "PlanetGenerator_DebugPlanetID" );

		// non-gas giant planet settings
		m_numPolePaddingRows = EditorPrefs.GetInt( "PlanetGenerator_NumPolePaddingRows", 3 );

		int textureMapHeight = EditorPrefs.GetInt( "PlanetGenerator_TextureMapHeight", 1024 );

		m_textureMapWidth = textureMapHeight * 2;
		m_textureMapHeight = textureMapHeight;

		// mountain settings
		m_octaves = EditorPrefs.GetInt( "PlanetGenerator_Octaves", 10 );
		m_mountainScale = EditorPrefs.GetInt( "PlanetGenerator_MountainScale", 4 );
		m_mountainPersistence = EditorPrefs.GetFloat( "PlanetGenerator_MountainPersistence", 0.5f );
		m_mountainGain = EditorPrefs.GetFloat( "PlanetGenerator_MountainGain", 0.075f );

		// hydraulic erosion settings
		m_doHydraulicErosionPass = EditorPrefs.GetBool( "PlanetGenerator_DoHydraulicErosionPass", true );
		m_xyScaleToMeters = EditorPrefs.GetFloat( "PlanetGenerator_XYScaleToMeters", 10.0f );
		m_zScaleToMeters = EditorPrefs.GetFloat( "PlanetGenerator_ZScaleToMeters", 400.0f );
		m_rainWaterAmount = EditorPrefs.GetFloat( "PlanetGenerator_RainWaterAmount", 1.0f );
		m_sedimentCapacity = EditorPrefs.GetFloat( "PlanetGenerator_SedimentCapacity", 100.0f );
		m_gravityConstant = EditorPrefs.GetFloat( "PlanetGenerator_GravityConstant", -9.8f );
		m_frictionConstant = EditorPrefs.GetFloat( "PlanetGenerator_FrictionConstant", 0.5f );
		m_evaporationConstant = EditorPrefs.GetFloat( "PlanetGenerator_EvaporationConstant", 1.0f );
		m_depositionConstant = EditorPrefs.GetFloat( "PlanetGenerator_DepositionConstant", 5.0f );
		m_dissolvingConstant = EditorPrefs.GetFloat( "PlanetGenerator_DissolvingConstant", 4.0f );
		m_stepDeltaTime = EditorPrefs.GetFloat( "PlanetGenerator_StepDeltaTime", 0.005f );
		m_finalBlurRadius = EditorPrefs.GetInt( "PlanetGenerator_FinalBlurRadius", 3 );
	}

	void OnDisable()
	{
		// other settings
		EditorPrefs.SetString( "PlanetGenerator_GameDataFileName", m_gameDataFileName );
		EditorPrefs.SetString( "PlanetGenerator_PlanetImagesPath", m_planetImagesPath );
		EditorPrefs.SetString( "PlanetGenerator_ResourcesPath", m_resourcesPath );
		EditorPrefs.SetBool( "PlanetGenerator_DebugMode", m_debugMode );
		EditorPrefs.SetInt( "PlanetGenerator_DebugPlanetID", m_debugPlanetID );

		// non-gas giant planet settings
		EditorPrefs.SetInt( "PlanetGenerator_TextureMapHeight", m_textureMapHeight );
		EditorPrefs.SetInt( "PlanetGenerator_NumPolePaddingRows", m_numPolePaddingRows );

		// mountain settings
		EditorPrefs.SetInt( "PlanetGenerator_Octaves", m_octaves );
		EditorPrefs.SetInt( "PlanetGenerator_MountainScale", m_mountainScale );
		EditorPrefs.SetFloat( "PlanetGenerator_MountainPersistence", m_mountainPersistence );
		EditorPrefs.SetFloat( "PlanetGenerator_MountainGain", m_mountainGain );

		// hydraulic erosion settings
		EditorPrefs.SetBool( "PlanetGenerator_DoHydraulicErosionPass", m_doHydraulicErosionPass );
		EditorPrefs.SetFloat( "PlanetGenerator_XYScaleToMeters", m_xyScaleToMeters );
		EditorPrefs.SetFloat( "PlanetGenerator_ZScaleToMeters", m_zScaleToMeters );
		EditorPrefs.SetFloat( "PlanetGenerator_RainWaterAmount", m_rainWaterAmount );
		EditorPrefs.SetFloat( "PlanetGenerator_SedimentCapacity", m_sedimentCapacity );
		EditorPrefs.SetFloat( "PlanetGenerator_GravityConstant", m_gravityConstant );
		EditorPrefs.SetFloat( "PlanetGenerator_FrictionConstant", m_frictionConstant );
		EditorPrefs.SetFloat( "PlanetGenerator_EvaporationConstant", m_evaporationConstant );
		EditorPrefs.SetFloat( "PlanetGenerator_DepositionConstant", m_depositionConstant );
		EditorPrefs.SetFloat( "PlanetGenerator_DissolvingConstant", m_dissolvingConstant );
		EditorPrefs.SetFloat( "PlanetGenerator_StepDeltaTime", m_stepDeltaTime );
		EditorPrefs.SetInt( "PlanetGenerator_FinalBlurRadius", m_finalBlurRadius );
	}

	void OnGUI()
	{
		GUILayout.Label( "Other Settings", EditorStyles.boldLabel );

		m_gameDataFileName = EditorGUILayout.TextField( "Game Data File Name", m_gameDataFileName );
		m_planetImagesPath = EditorGUILayout.TextField( "Planet Images Path", m_planetImagesPath );
		m_resourcesPath = EditorGUILayout.TextField( "Planet Resources Path", m_resourcesPath );
		m_debugMode = EditorGUILayout.Toggle( "Debug Mode", m_debugMode );
		m_debugPlanetID = EditorGUILayout.IntField( "Debug Planet ID", m_debugPlanetID );

		GUILayout.Label( "Non-Gas Giant Planet Settings", EditorStyles.boldLabel );

		m_textureMapHeight = EditorGUILayout.IntField( "Texture Map Height", m_textureMapHeight );
		m_numPolePaddingRows = EditorGUILayout.IntSlider( "Num Pole Padding Rows", m_numPolePaddingRows, 0, 8 );

		m_textureMapWidth = m_textureMapHeight * 2;

		GUILayout.Label( "Mountain Settings", EditorStyles.boldLabel );

		m_octaves = EditorGUILayout.IntSlider( "Octaves", m_octaves, 1, 12 );
		m_mountainScale = EditorGUILayout.IntSlider( "Frequency", m_mountainScale, 1, 12 );
		m_mountainPersistence = EditorGUILayout.Slider( "Persistence", m_mountainPersistence, 0.0f, 1.0f );
		m_mountainGain = EditorGUILayout.Slider( "Output Gain", m_mountainGain, 0.0f, 10.0f );

		GUILayout.Label( "Hydraulic Erosion Settings", EditorStyles.boldLabel );

		m_doHydraulicErosionPass = EditorGUILayout.Toggle( "Enable Hydraulic Erosion", m_doHydraulicErosionPass );
		m_xyScaleToMeters = EditorGUILayout.Slider( "XY Scale To Meters", m_xyScaleToMeters, 0.001f, 100.0f );
		m_zScaleToMeters = EditorGUILayout.Slider( "Z Scale To Meters", m_zScaleToMeters, 1.0f, 10000.0f );
		m_rainWaterAmount = EditorGUILayout.Slider( "Rain Water Amount", m_rainWaterAmount, 0.001f, 10.0f );
		m_sedimentCapacity = EditorGUILayout.Slider( "Sediment Capacity", m_sedimentCapacity, 0.001f, 500.0f );
		m_gravityConstant = EditorGUILayout.Slider( "Gravity Constant", m_gravityConstant, -20.0f, 0.0f );
		m_frictionConstant = EditorGUILayout.Slider( "Friction Constant", m_frictionConstant, 0.0f, 10.0f );
		m_evaporationConstant = EditorGUILayout.Slider( "Evaporation Constant", m_evaporationConstant, 0.0f, 10.0f );
		m_depositionConstant = EditorGUILayout.Slider( "Deposition Constant", m_depositionConstant, 0.001f, 100.0f );
		m_dissolvingConstant = EditorGUILayout.Slider( "Dissolving Constant", m_dissolvingConstant, 0.001f, 100.0f );
		m_stepDeltaTime = EditorGUILayout.Slider( "Step Delta Time", m_stepDeltaTime, 0.001f, 1.0f );
		m_finalBlurRadius = EditorGUILayout.IntSlider( "Final Blur Radius", m_finalBlurRadius, 0, 256 );

		GUILayout.Space( 15 );

		if ( GUILayout.Button( "Generate Delta Map For All Planets" ) )
		{
			MakeSomeMagic();
		}
	}

	// generate maps and show them on the planet game object
	void MakeSomeMagic()
	{
		// show progress bar
		EditorUtility.DisplayProgressBar( "Planet Generator", "Initializing...", 0.0f );

		// load the game data
		var textAsset = Resources.Load( m_gameDataFileName ) as TextAsset;

		// convert it from the json string to our game data class
		var gameData = JsonUtility.FromJson<GameData>( textAsset.text );

		// initialize the game data
		gameData.Initialize();

		// calculate the texture map scale (and it must be an even number)
		m_textureMapScaleX = Mathf.FloorToInt( (float) m_textureMapWidth / (float) PG_Planet.c_width );
		m_textureMapScaleY = Mathf.FloorToInt( (float) m_textureMapHeight / (float) ( PG_Planet.c_height + m_numPolePaddingRows * 2 ) );

		if ( m_textureMapScaleX < 2 )
		{
			m_textureMapScaleX = 2;
		}
		else if ( ( m_textureMapScaleX & 1 ) == 1 )
		{
			m_textureMapScaleX--;
		}

		if ( m_textureMapScaleY < 2 )
		{
			m_textureMapScaleY = 2;
		}
		else if ( ( m_textureMapScaleY & 1 ) == 1 )
		{
			m_textureMapScaleY--;
		}

		// calculate the mountian scale power of two
		m_mountainScalePowerOfTwo = Mathf.RoundToInt( Mathf.Pow( 2, m_mountainScale - 1 ) );

		// do some magic
		PG_Planet pgPlanet;

		for ( var id = 0; id < c_numPlanets; id++ )
		{
			if ( m_debugMode )
			{
				id = m_debugPlanetID;
			}

			pgPlanet = new PG_Planet( gameData, m_planetImagesPath, id );

			if ( pgPlanet.m_mapIsValid )
			{
				var filename = Application.dataPath + "/" + m_resourcesPath + "/Planets/" + pgPlanet.m_id + ".bytes";

				if ( m_debugMode || !File.Exists( filename ) )
				{
					if ( !GeneratePlanetTextureMaps( pgPlanet, filename ) )
					{
						break;
					}
				}
			}

			if ( m_debugMode )
			{
				break;
			}
		}

		// show progress bar
		EditorUtility.ClearProgressBar();
	}

	// call this to generate the planet texture maps
	bool GeneratePlanetTextureMaps( PG_Planet pgPlanet, string filename )
	{
		// vars for the progress bar
		var currentStep = 1;
		var totalSteps = 7;

		// update the progress bar
		EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Preparing color map...", (float) currentStep++ / totalSteps );

		// prepare the color map
		var preparedColorMap = PrepareColorMap( pgPlanet );

		if ( m_debugMode )
		{
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving prepared color map...", 0.0f );

			PG_Tools.SaveAsPNG( preparedColorMap, Application.dataPath + "/Editor/Debug - Prepared Color Map.png" );
		}

		// update the progress bar
		EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Preparing height map...", (float) currentStep++ / totalSteps );

		// prepare the height map
		var preparedHeightMap = PrepareHeightMap( pgPlanet );

		if ( m_debugMode )
		{
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving prepared height map...", 0.0f );

			PG_Tools.SaveAsPNG( preparedHeightMap, Application.dataPath + "/Editor/Debug - Prepared Height Map.png" );
		}

		float minimumDifference = 0.0f;
		float maximumDifference = 0.0f;
		byte[] differenceBuffer = null;

		if ( pgPlanet.m_surfaceId != 1 )
		{
			// scale to power of two
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Scaling to power of two...", (float) currentStep++ / totalSteps );

			var bicubicScale = new PG_BicubicScaleElevation();

			var elevation = bicubicScale.Process( preparedHeightMap, m_textureMapWidth, m_textureMapHeight );

			if ( m_debugMode )
			{
				EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving bicubic scale map...", 0.0f );

				var contourMap = new PG_ContourMap();

				var tempBuffer = contourMap.Process( elevation, pgPlanet.m_waterHeight );

				PG_Tools.SaveAsEXR( tempBuffer, Application.dataPath + "/Editor/Debug - Bicubic Scale.exr" );
			}

			// at this point we want to save the current elevation buffer to use when calculating the difference map later
			var baseElevationBuffer = elevation;

			// mountains pass
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Creating mountains...", (float) currentStep++ / totalSteps );

			var mountains = new PG_Mountains();

			elevation = mountains.Process( elevation, pgPlanet.m_id, m_octaves, m_mountainScalePowerOfTwo, m_mountainPersistence, m_mountainGain, pgPlanet.m_waterHeight );

			if ( m_debugMode )
			{
				EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving mountains map...", 0.0f );

				var contourMap = new PG_ContourMap();

				var tempBuffer = contourMap.Process( elevation, pgPlanet.m_waterHeight );

				PG_Tools.SaveAsEXR( tempBuffer, Application.dataPath + "/Editor/Debug - Mountains.exr" );
			}

			// hydraulic erosion pass
			if ( m_doHydraulicErosionPass )
			{
				EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Hydraulic erosion pass...", (float) currentStep++ / totalSteps );

				var minimumElevation = pgPlanet.m_waterHeight - ( 1.0f / 16.0f );

				var hydraulicErosion = new PG_HydraulicErosion();

				elevation = hydraulicErosion.Process( elevation, minimumElevation, m_xyScaleToMeters, m_zScaleToMeters, m_rainWaterAmount, m_sedimentCapacity, m_gravityConstant, m_frictionConstant, m_evaporationConstant, m_depositionConstant, m_dissolvingConstant, m_stepDeltaTime, m_finalBlurRadius );

				if ( elevation == null )
				{
					return false;
				}

				if ( m_debugMode )
				{
					EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving hydraulic erosion map...", 0.0f );

					var contourMap = new PG_ContourMap();

					var tempBuffer = contourMap.Process( elevation, pgPlanet.m_waterHeight );

					PG_Tools.SaveAsEXR( tempBuffer, Application.dataPath + "/Editor/Debug - Hydraulic Erosion.exr" );
				}
			}
			else
			{
				currentStep++;
			}

			if ( m_debugMode )
			{
				// generate and save the albedo map
				EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving albedo map...", 0.0f );

				var albedoMap = new PG_AlbedoMap();

				var albedoBuffer = albedoMap.Process( elevation, preparedColorMap, pgPlanet.m_waterHeight, pgPlanet.m_waterColor, pgPlanet.m_groundColor );

				PG_Tools.SaveAsPNG( albedoBuffer, Application.dataPath + "/Editor/Debug - Albedo Map.png" );

				// generate and save the normal map
				EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving normal map...", 0.0f );

				var normalMap = new PG_NormalMap();

				var normalsBuffer = normalMap.Process( elevation, 256.0f, pgPlanet.m_waterHeight, 1 );

				PG_Tools.SaveAsPNG( normalsBuffer, Application.dataPath + "/Editor/Debug - Normal Map.png" );

				// generate and save the specular map
				EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving specular map...", 0.0f );

				var waterSpecularColor = new Color( 1.0f, 1.0f, 1.0f );

				var specularMap = new PG_SpecularMap();

				var specularBuffer = specularMap.Process( elevation, albedoBuffer, pgPlanet.m_waterHeight, waterSpecularColor, 0.75f, 1 );

				PG_Tools.SaveAsPNG( specularBuffer, Application.dataPath + "/Editor/Debug - Specular Map.png", true );

				// generate and save the water mask map
				EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving water mask map...", 0.0f );

				var waterMaskMap = new PG_WaterMaskMap();

				var waterMaskBuffer = waterMaskMap.Process( elevation, pgPlanet.m_waterHeight, 1 );

				PG_Tools.SaveAsPNG( waterMaskBuffer, Application.dataPath + "/Editor/Debug - Water Mask Map.png", true );
			}

			// figure out what our minimum and maximum deltas are
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Computing deltas...", (float) currentStep++ / totalSteps );

			minimumDifference = elevation[ 0, 0 ] - baseElevationBuffer[ 0, 0 ];
			maximumDifference = elevation[ 0, 0 ] - baseElevationBuffer[ 0, 0 ];

			for ( var y = 0; y < m_textureMapHeight; y++ )
			{
				for ( var x = 0; x < m_textureMapWidth; x++ )
				{
					var difference = elevation[ y, x ] - baseElevationBuffer[ y, x ];

					if ( difference < minimumDifference )
					{
						minimumDifference = difference;
					}

					if ( difference > maximumDifference )
					{
						maximumDifference = difference;
					}
				}
			}

			// rescale float deltas to 0 to 255
			var elevationScale = 255.0f / ( maximumDifference - minimumDifference );

			differenceBuffer = new byte[ m_textureMapWidth * m_textureMapHeight ];

			for ( var y = 0; y < m_textureMapHeight; y++ )
			{
				for ( var x = 0; x < m_textureMapWidth; x++ )
				{
					var difference = (byte) Mathf.RoundToInt( ( elevation[ y, x ] - baseElevationBuffer[ y, x ] - minimumDifference ) * elevationScale );

					differenceBuffer[ y * m_textureMapWidth + x ] = difference;

					elevation[ y, x ] = difference / 255.0f;
				}
			}

			if ( m_debugMode )
			{
				EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Saving difference buffer...", 0.0f );

				PG_Tools.SaveAsPNG( elevation, Application.dataPath + "/Editor/" + "Debug - Difference Buffer.png" );
			}
		}

		// save the map!
		EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Compressing and saving the planet data...", (float) currentStep++ / totalSteps );

		SavePlanetMap( filename, pgPlanet, preparedHeightMap, preparedColorMap, minimumDifference, maximumDifference, differenceBuffer );

		return true;
	}

	void SavePlanetMap( string filename, PG_Planet pgPlanet, float[,] preparedHeightMap, Color[,] preparedColorMap, float minimumDifference, float maximumDifference, byte[] differenceBuffer )
	{
		// save map to compressed bytes file with a header
		using ( var fileStream = new FileStream( filename, FileMode.Create ) )
		{
			using ( var gZipStream = new GZipStream( fileStream, CompressionMode.Compress, false ) )
			{
				var binaryWriter = new BinaryWriter( gZipStream );

				// version number
				binaryWriter.Write( c_versionNumber );

				// misc data
				binaryWriter.Write( pgPlanet.m_minimumHeight );
				binaryWriter.Write( pgPlanet.m_waterHeight );
				binaryWriter.Write( pgPlanet.m_snowHeight );

				binaryWriter.Write( pgPlanet.m_waterColor.r );
				binaryWriter.Write( pgPlanet.m_waterColor.g );
				binaryWriter.Write( pgPlanet.m_waterColor.b );

				binaryWriter.Write( pgPlanet.m_groundColor.r );
				binaryWriter.Write( pgPlanet.m_groundColor.g );
				binaryWriter.Write( pgPlanet.m_groundColor.b );

				binaryWriter.Write( pgPlanet.m_snowColor.r );
				binaryWriter.Write( pgPlanet.m_snowColor.g );
				binaryWriter.Write( pgPlanet.m_snowColor.b );

				// prepared map width and height
				var preparedMapWidth = preparedHeightMap.GetLength( 1 );
				var preparedMapHeight = preparedHeightMap.GetLength( 0 );

				binaryWriter.Write( preparedMapWidth );
				binaryWriter.Write( preparedMapHeight );

				// prepared height map
				for ( var y = 0; y < preparedMapHeight; y++ )
				{
					for ( var x = 0; x < preparedMapWidth; x++ )
					{
						binaryWriter.Write( preparedHeightMap[ y, x ] );
					}
				}

				// prepared color map
				for ( var y = 0; y < preparedMapHeight; y++ )
				{
					for ( var x = 0; x < preparedMapWidth; x++ )
					{
						binaryWriter.Write( preparedColorMap[ y, x ].r );
						binaryWriter.Write( preparedColorMap[ y, x ].g );
						binaryWriter.Write( preparedColorMap[ y, x ].b );
					}
				}

				// difference buffer
				if ( differenceBuffer != null )
				{
					// minimum and maximum difference
					binaryWriter.Write( minimumDifference );
					binaryWriter.Write( maximumDifference );

					// difference buffer
					binaryWriter.Write( differenceBuffer );
				}
			}
		}
	}

	// this function generates a copy of the original planet map and adds a number of rows to the top and bottom of the map (to prevent pole pinching)
	Color[,] PrepareColorMap( PG_Planet pgPlanet )
	{
		// compute height of the new map
		var height = PG_Planet.c_height + m_numPolePaddingRows * 2;

		// allocate the new map
		var buffer = new Color[ height, PG_Planet.c_width ];

		// find the most used color on the top row
		var tally = new Dictionary<Color, int>();

		for ( var x = 0; x < PG_Planet.c_width; x++ )
		{
			var color = pgPlanet.m_color[ 0, x ];

			if ( !tally.ContainsKey( color ) )
			{
				tally.Add( color, 0 );
			}

			tally[ color ]++;
		}

		var mostUsedColor = Color.black;
		var mostUsedCount = 0;

		foreach ( var key in tally.Keys )
		{
			if ( tally[ key ] > mostUsedCount )
			{
				mostUsedCount = tally[ key ];
				mostUsedColor = key;
			}
		}

		m_topPaddingColor = mostUsedColor;

		// add north pole color
		for ( var i = 0; i < m_numPolePaddingRows; i++ )
		{
			var y = m_numPolePaddingRows - i - 1;

			for ( var x = 0; x < PG_Planet.c_width; x++ )
			{
				buffer[ y, x ] = m_topPaddingColor;
			}
		}


		// find the most used color on the bottom row
		tally = new Dictionary<Color, int>();

		for ( var x = 0; x < PG_Planet.c_width; x++ )
		{
			var color = pgPlanet.m_color[ PG_Planet.c_height - 1, x ];

			if ( !tally.ContainsKey( color ) )
			{
				tally.Add( color, 0 );
			}

			tally[ color ]++;
		}

		mostUsedColor = Color.black;
		mostUsedCount = 0;

		foreach ( var key in tally.Keys )
		{
			if ( tally[ key ] > mostUsedCount )
			{
				mostUsedCount = tally[ key ];
				mostUsedColor = key;
			}
		}

		m_bottomPaddingColor = mostUsedColor;

		// add south pole color
		for ( var i = 0; i < m_numPolePaddingRows; i++ )
		{
			var y = m_numPolePaddingRows + i + PG_Planet.c_height;

			for ( var x = 0; x < PG_Planet.c_width; x++ )
			{
				buffer[ y, x ] = m_bottomPaddingColor;
			}
		}

		// copy the original map into the are in between the padded rows
		for ( var y = 0; y < PG_Planet.c_height; y++ )
		{
			for ( var x = 0; x < PG_Planet.c_width; x++ )
			{
				buffer[ y + m_numPolePaddingRows, x ] = pgPlanet.m_color[ y, x ];
			}
		}

		// all done
		return buffer;
	}

	// this function generates a copy of the original planet map and adds a number of rows to the top and bottom of the map (to prevent pole pinching)
	float[,] PrepareHeightMap( PG_Planet pgPlanet )
	{
		// compute height of the new map
		var height = PG_Planet.c_height + m_numPolePaddingRows * 2;

		// allocate the new map
		var buffer = new float[ height, PG_Planet.c_width ];

		// get the maximum height for the top row
		var maximumHeight = 0.0f;

		for ( var x = 0; x < PG_Planet.c_width; x++ )
		{
			if ( pgPlanet.m_color[ 0, x ] == m_topPaddingColor )
			{
				if ( pgPlanet.m_height[ 0, x ] > maximumHeight )
				{
					maximumHeight = pgPlanet.m_height[ 0, x ];
				}
			}
		}

		if ( m_topPaddingColor == pgPlanet.m_waterColor )
		{
			maximumHeight = 0.0f;
		}

		// set the height of the padded rows
		for ( var i = 0; i < m_numPolePaddingRows; i++ )
		{
			var y = m_numPolePaddingRows - i - 1;

			var t = (float) ( i + 1 ) / (float) m_numPolePaddingRows;

			t *= 2.0f;

			for ( var x = 0; x < PG_Planet.c_width; x++ )
			{
				var originalHeight = pgPlanet.m_height[ 0, x ];

				buffer[ y, x ] = Mathf.Lerp( originalHeight, maximumHeight, t );
			}
		}

		// get the maximum height for the bottom row
		maximumHeight = 0.0f;

		for ( var x = 0; x < PG_Planet.c_width; x++ )
		{
			if ( pgPlanet.m_color[ PG_Planet.c_height - 1, x ] == m_bottomPaddingColor )
			{
				if ( pgPlanet.m_height[ 0, x ] > maximumHeight )
				{
					maximumHeight = pgPlanet.m_height[ 0, x ];
				}
			}
		}

		if ( m_bottomPaddingColor == pgPlanet.m_waterColor )
		{
			maximumHeight = 0.0f;
		}

		// set the height of the padded rows
		for ( var i = 0; i < m_numPolePaddingRows; i++ )
		{
			var y = m_numPolePaddingRows + i + PG_Planet.c_height;

			var t = (float) ( i + 1 ) / (float) m_numPolePaddingRows;

			t *= 2.0f;

			for ( var x = 0; x < PG_Planet.c_width; x++ )
			{
				var originalHeight = pgPlanet.m_height[ PG_Planet.c_height - 1, x ];

				buffer[ y, x ] = Mathf.Lerp( originalHeight, maximumHeight, t );
			}
		}

		// copy the original map into the are in between the padded rows
		for ( var y = 0; y < PG_Planet.c_height; y++ )
		{
			for ( var x = 0; x < PG_Planet.c_width; x++ )
			{
				buffer[ y + m_numPolePaddingRows, x ] = pgPlanet.m_height[ y, x ];
			}
		}

		// all done
		return buffer;
	}
}
