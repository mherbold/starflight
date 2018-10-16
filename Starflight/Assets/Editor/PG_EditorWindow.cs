
using UnityEngine;
using UnityEditor;

using System.IO;
using System.IO.Compression;
using System.Diagnostics;

public class PG_EditorWindow : EditorWindow
{
	// version number
	const int c_versionNumber = 1;

	// other settings
	string m_planetDataFileName;
	string m_resourcesPath;
	bool m_debugMode;

	// non-gas giant planet settings
	int m_textureMapWidth;
	int m_textureMapHeight;
	int m_numPolePaddingRows;
	int m_highBlurRadius;
	int m_lowBlurRadius;

	// base mountain and hill settings
	int m_octaves;

	// mountain settings
	int m_mountainScale;
	float m_mountainPersistence;
	float m_mountainGain;

	// hill settings
	int m_hillScale;
	float m_hillPersistence;
	float m_hillGain;

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
	PG_PlanetData m_pgPlanetData;
	GameObject m_gameObject;
	MeshRenderer m_meshRenderer;
	int m_textureMapScaleX;
	int m_textureMapScaleY;
	int m_mountainScalePowerOfTwo;
	int m_hillScalePowerOfTwo;

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
		m_planetDataFileName = EditorPrefs.GetString( "PlanetGenerator_PlanetDataFileName" );
		m_resourcesPath = EditorPrefs.GetString( "PlanetGenerator_ResourcesPath" );
		m_debugMode = EditorPrefs.GetBool( "PlanetGenerator_DebugMode" );

		// non-gas giant planet settings
		m_numPolePaddingRows = EditorPrefs.GetInt( "PlanetGenerator_NumPolePaddingRows", 3 );
		m_highBlurRadius = EditorPrefs.GetInt( "PlanetGenerator_HighBlurRadius", 45 );
		m_lowBlurRadius = EditorPrefs.GetInt( "PlanetGenerator_LowBlurRadius", 11 );

		int textureMapHeight = EditorPrefs.GetInt( "PlanetGenerator_TextureMapHeight", 1024 );

		m_textureMapWidth = textureMapHeight * 2;
		m_textureMapHeight = textureMapHeight;

		// base mountain and hill settings
		m_octaves = EditorPrefs.GetInt( "PlanetGenerator_Octaves", 10 );

		// mountain settings
		m_mountainScale = EditorPrefs.GetInt( "PlanetGenerator_MountainScale", 4 );
		m_mountainPersistence = EditorPrefs.GetFloat( "PlanetGenerator_MountainPersistence", 0.5f );
		m_mountainGain = EditorPrefs.GetFloat( "PlanetGenerator_MountainGain", 0.075f );

		// hill settings
		m_hillScale = EditorPrefs.GetInt( "PlanetGenerator_HillScale", 4 );
		m_hillPersistence = EditorPrefs.GetFloat( "PlanetGenerator_HillPersistence", 0.9f );
		m_hillGain = EditorPrefs.GetFloat( "PlanetGenerator_HillGain", 0.05f );

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
		EditorPrefs.SetString( "PlanetGenerator_PlanetDataFileName", m_planetDataFileName );
		EditorPrefs.SetString( "PlanetGenerator_ResourcesPath", m_resourcesPath );
		EditorPrefs.SetBool( "PlanetGenerator_DebugMode", m_debugMode );

		// non-gas giant planet settings
		EditorPrefs.SetInt( "PlanetGenerator_TextureMapHeight", m_textureMapHeight );
		EditorPrefs.SetInt( "PlanetGenerator_NumPolePaddingRows", m_numPolePaddingRows );
		EditorPrefs.SetInt( "PlanetGenerator_HighBlurRadius", m_highBlurRadius );
		EditorPrefs.SetInt( "PlanetGenerator_LowBlurRadius", m_lowBlurRadius );

		// base mountain and hill settings
		EditorPrefs.SetInt( "PlanetGenerator_Octaves", m_octaves );

		// mountain settings
		EditorPrefs.SetInt( "PlanetGenerator_MountainScale", m_mountainScale );
		EditorPrefs.SetFloat( "PlanetGenerator_MountainPersistence", m_mountainPersistence );
		EditorPrefs.SetFloat( "PlanetGenerator_MountainGain", m_mountainGain );

		// hill settings
		EditorPrefs.SetInt( "PlanetGenerator_HillScale", m_hillScale );
		EditorPrefs.SetFloat( "PlanetGenerator_HillPersistence", m_hillPersistence );
		EditorPrefs.SetFloat( "PlanetGenerator_HillGain", m_hillGain );

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

		m_planetDataFileName = EditorGUILayout.TextField( "Planet Data File Name", m_planetDataFileName );
		m_resourcesPath = EditorGUILayout.TextField( "Planet Resources Path", m_resourcesPath );
		m_debugMode = EditorGUILayout.Toggle( "Debug Mode", m_debugMode );

		GUILayout.Label( "Non-Gas Giant Planet Settings", EditorStyles.boldLabel );

		m_textureMapHeight = EditorGUILayout.IntField( "Texture Map Height", m_textureMapHeight );
		m_numPolePaddingRows = EditorGUILayout.IntSlider( "Num Pole Padding Rows", m_numPolePaddingRows, 0, 8 );
		m_highBlurRadius = EditorGUILayout.IntSlider( "High Blur Radius", m_highBlurRadius, 0, 256 );
		m_lowBlurRadius = EditorGUILayout.IntSlider( "Low Blur Radius", m_lowBlurRadius, 0, 256 );

		m_textureMapWidth = m_textureMapHeight * 2;

		GUILayout.Label( "Base Mountain and Hill Settings", EditorStyles.boldLabel );

		m_octaves = EditorGUILayout.IntSlider( "Octaves", m_octaves, 1, 12 );

		GUILayout.Label( "Mountain Settings", EditorStyles.boldLabel );

		m_mountainScale = EditorGUILayout.IntSlider( "Scale", m_mountainScale, 1, 12 );
		m_mountainPersistence = EditorGUILayout.Slider( "Persistence", m_mountainPersistence, 0.0f, 1.0f );
		m_mountainGain = EditorGUILayout.Slider( "Output Gain", m_mountainGain, 0.0f, 1.0f );

		GUILayout.Label( "Hill Settings", EditorStyles.boldLabel );

		m_hillScale = EditorGUILayout.IntSlider( "Scale", m_hillScale, 1, 12 );
		m_hillPersistence = EditorGUILayout.Slider( "Persistence", m_hillPersistence, 0.0f, 1.0f );
		m_hillGain = EditorGUILayout.Slider( "Output Gain", m_hillGain, 0.0f, 1.0f );

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

		// load the planet data
		if ( !LoadPlanetData() )
		{
			return;
		}

		// calculate the texture map scale (and it must be an even number)
		m_textureMapScaleX = Mathf.FloorToInt( (float) m_textureMapWidth / (float) PG_Planet.c_mapWidth );
		m_textureMapScaleY = Mathf.FloorToInt( (float) m_textureMapHeight / (float) ( PG_Planet.c_mapHeight + m_numPolePaddingRows * 2 ) );

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

		// calculate the mountian and hill scale power of two
		m_mountainScalePowerOfTwo = Mathf.RoundToInt( Mathf.Pow( 2, m_mountainScale - 1 ) );
		m_hillScalePowerOfTwo = Mathf.RoundToInt( Mathf.Pow( 2, m_hillScale - 1 ) );

		// do some magic
		PG_Planet pgPlanet;

		for ( var i = 0; i < m_pgPlanetData.m_planetList.Length; i++ )
		{
			pgPlanet = m_pgPlanetData.m_planetList[ i ];

			if ( pgPlanet.m_mapIsValid )
			{
				var filename = Application.dataPath + "/" + m_resourcesPath + "/Planets/" + pgPlanet.m_id + ".bytes";

				if ( !File.Exists( filename ) )
				{
					GeneratePlanetTextureMaps( pgPlanet, filename );
				}
			}
		}

		// show progress bar
		EditorUtility.ClearProgressBar();
	}

	// load the planet data file
	bool LoadPlanetData()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		// clear out the old data
		m_pgPlanetData = null;

		// load it as text
		var text =  File.ReadAllText( Application.dataPath + "/Editor/" + m_planetDataFileName + ".json" );

		// convert it from the json string to our planet data class
		m_pgPlanetData = JsonUtility.FromJson<PG_PlanetData>( text );

		// prep each planet map for generation (speed optimization)
		foreach ( var pgPlanet in m_pgPlanetData.m_planetList )
		{
			EditorUtility.DisplayProgressBar( "Initializing Planets", "Planet " + ( pgPlanet.m_id + 1 ) + " of " + m_pgPlanetData.m_planetList.Length, (float) pgPlanet.m_id / m_pgPlanetData.m_planetList.Length );

			pgPlanet.Initialize();
		}

		UnityEngine.Debug.Log( "Load Planet Data - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return true;
	}

	// call this to generate the planet texture maps
	void GeneratePlanetTextureMaps( PG_Planet pgPlanet, string filename )
	{
		// vars for the progress bar
		var currentStep = 1;
		var totalSteps = 9;

		// get prepared source (and save a copy of it for writing out to the header)
		EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Preparing original map...", (float) currentStep++ / totalSteps );

		var preparedMap = PrepareMap( pgPlanet );

		float minimumDifference;
		float maximumDifference;
		byte[] differenceBuffer;

		// gas giant or not?
		if ( pgPlanet.m_surfaceId == 1 )
		{
			// yes - no difference buffer
			minimumDifference = 0.0f;
			maximumDifference = 0.0f;
			differenceBuffer = null;
		}
		else
		{
			// contours pass
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Doing contours pass...", (float) currentStep++ / totalSteps );

			var contours = new Contours( preparedMap );

			var elevationBuffer = contours.Process( m_textureMapScaleX, m_textureMapScaleY, pgPlanet.m_mapLegend );

			// scale to power of two
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Scaling to power of two...", (float) currentStep++ / totalSteps );

			var scaleToPowerOfTwo = new ScaleToPowerOfTwo( elevationBuffer );

			elevationBuffer = scaleToPowerOfTwo.Process( m_textureMapScaleX, m_textureMapScaleY );

			if ( m_debugMode )
			{
				var albedo = new Albedo( elevationBuffer );

				var albedoBuffer = albedo.Process( pgPlanet.m_mapLegend );

				PG_Tools.SaveAsEXR( albedoBuffer, Application.dataPath + "/Editor/" + "Scale to Power of Two.exr" );
			}

			// at this point we want to save the current elevation buffer to use when calculating the difference map later
			var baseElevationBuffer = elevationBuffer;

			// now we know what the final width and height of our map is
			var width = elevationBuffer.GetLength( 1 );
			var height = elevationBuffer.GetLength( 0 );

			// gaussian blur pass (low)
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Gaussian blur pass (low)...", (float) currentStep++ / totalSteps );

			var gaussianBlur = new GaussianBlur( elevationBuffer );

			var lowBlurredElevationBuffer = gaussianBlur.Process( m_lowBlurRadius, m_lowBlurRadius );

			if ( m_debugMode )
			{
				var albedo = new Albedo( lowBlurredElevationBuffer );

				var albedoBuffer = albedo.Process( pgPlanet.m_mapLegend );

				PG_Tools.SaveAsEXR( albedoBuffer, Application.dataPath + "/Editor/" + "Gaussian Blur (Low).exr" );
			}

			// gaussian blur pass (high)
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Gaussian blur pass (high)...", (float) currentStep++ / totalSteps );

			var highBlurredElevationBuffer = gaussianBlur.Process( m_highBlurRadius, m_highBlurRadius );

			if ( m_debugMode )
			{
				var albedo = new Albedo( highBlurredElevationBuffer );

				var albedoBuffer = albedo.Process( pgPlanet.m_mapLegend );

				PG_Tools.SaveAsEXR( albedoBuffer, Application.dataPath + "/Editor/" + "Gaussian Blur (High).exr" );
			}

			// blend the high and low blur buffers
			elevationBuffer = new float[ height, width ];

			for ( var y = 0; y < height; y++ )
			{
				for ( var x = 0; x < width; x++ )
				{
					var lowBlurredElevation = lowBlurredElevationBuffer[ y, x ];
					var highBlurredElevation = highBlurredElevationBuffer[ y, x ];

					var t = highBlurredElevation;

					elevationBuffer[ y, x ] = Mathf.Lerp( lowBlurredElevation, highBlurredElevation, t );
				}
			}

			if ( m_debugMode )
			{
				var albedo = new Albedo( elevationBuffer );

				var albedoBuffer = albedo.Process( pgPlanet.m_mapLegend );

				PG_Tools.SaveAsEXR( albedoBuffer, Application.dataPath + "/Editor/" + "Gaussian Blur (Blended).exr" );
			}

			// mountains pass
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Creating mountains and hills...", (float) currentStep++ / totalSteps );

			var mountainsAndHills = new MountainsAndHills( elevationBuffer );

			elevationBuffer = mountainsAndHills.Process( pgPlanet.m_id, m_octaves, m_mountainScalePowerOfTwo, m_hillScalePowerOfTwo, m_mountainPersistence, m_hillPersistence, m_mountainGain, m_hillGain );

			if ( m_debugMode )
			{
				var albedo = new Albedo( elevationBuffer );

				var albedoBuffer = albedo.Process( pgPlanet.m_mapLegend );

				PG_Tools.SaveAsEXR( albedoBuffer, Application.dataPath + "/Editor/" + "Mountains and Hills.exr" );
			}

			// hydraulic erosion pass
			if ( m_doHydraulicErosionPass )
			{
				EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Hydraulic erosion pass...", (float) currentStep++ / totalSteps );

				var hydraulicErosion = new HydraulicErosion( elevationBuffer );

				elevationBuffer = hydraulicErosion.Process( m_xyScaleToMeters, m_zScaleToMeters, m_rainWaterAmount, m_sedimentCapacity, m_gravityConstant, m_frictionConstant, m_evaporationConstant, m_depositionConstant, m_dissolvingConstant, m_stepDeltaTime, m_finalBlurRadius );

				if ( m_debugMode )
				{
					var albedo = new Albedo( elevationBuffer );

					var albedoBuffer = albedo.Process( pgPlanet.m_mapLegend );

					PG_Tools.SaveAsEXR( albedoBuffer, Application.dataPath + "/Editor/" + "Hydraulic Erosion.exr" );
				}
			}
			else
			{
				currentStep++;
			}

			// figure out what our minimum and maximum deltas are
			EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Computing deltas...", (float) currentStep++ / totalSteps );

			minimumDifference = elevationBuffer[ 0, 0 ] - baseElevationBuffer[ 0, 0 ];
			maximumDifference = elevationBuffer[ 0, 0 ] - baseElevationBuffer[ 0, 0 ];

			for ( var y = 0; y < height; y++ )
			{
				for ( var x = 0; x < width; x++ )
				{
					var difference = elevationBuffer[ y, x ] - baseElevationBuffer[ y, x ];

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

			differenceBuffer = new byte[ width * height ];

			for ( var y = 0; y < height; y++ )
			{
				for ( var x = 0; x < width; x++ )
				{
					var difference = (byte) Mathf.RoundToInt( ( elevationBuffer[ y, x ] - baseElevationBuffer[ y, x ] - minimumDifference ) * elevationScale );

					differenceBuffer[ y * width + x ] = difference;

					elevationBuffer[ y, x ] = difference / 255.0f;
				}
			}

			if ( m_debugMode )
			{
				var albedo = new Albedo( elevationBuffer );

				var albedoBuffer = albedo.Process( pgPlanet.m_mapLegend );

				PG_Tools.SaveAsEXR( albedoBuffer, Application.dataPath + "/Editor/" + "Difference Buffer.exr" );
			}
		}

		// save the map!
		EditorUtility.DisplayProgressBar( "Planet " + ( pgPlanet.m_id + 1 ), "Compressing and saving the map...", (float) currentStep++ / totalSteps );

		SavePlanetMap( filename, pgPlanet, preparedMap, minimumDifference, maximumDifference, differenceBuffer );
	}

	void SavePlanetMap( string filename, PG_Planet pgPlanet, float[,] preparedMap, float minimumDifference, float maximumDifference, byte[] differenceBuffer )
	{
		// save map to compressed bytes file with a header
		using ( var fileStream = new FileStream( filename, FileMode.Create ) )
		{
			using ( var gZipStream = new GZipStream( fileStream, CompressionMode.Compress, false ) )
			{
				var binaryWriter = new BinaryWriter( gZipStream );

				// version number
				binaryWriter.Write( c_versionNumber );

				// legend length
				var legendLength = pgPlanet.m_mapLegend.Length;

				binaryWriter.Write( legendLength );

				// legend
				for ( var i = 0; i < legendLength; i++ )
				{
					binaryWriter.Write( pgPlanet.m_mapLegend[ i ].r );
					binaryWriter.Write( pgPlanet.m_mapLegend[ i ].g );
					binaryWriter.Write( pgPlanet.m_mapLegend[ i ].b );
					binaryWriter.Write( pgPlanet.m_mapLegend[ i ].a );
				}

				// prepared map width and height
				var preparedMapWidth = preparedMap.GetLength( 1 );
				var preparedMapHeight = preparedMap.GetLength( 0 );

				binaryWriter.Write( preparedMapWidth );
				binaryWriter.Write( preparedMapHeight );

				// prepared map
				for ( var y = 0; y < preparedMapHeight; y++ )
				{
					for ( var x = 0; x < preparedMapWidth; x++ )
					{
						binaryWriter.Write( preparedMap[ y, x ] );
					}
				}

				// gas giants have no difference buffer
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
	float[,] PrepareMap( PG_Planet pgPlanet )
	{
		// compute height of the new map
		var height = PG_Planet.c_mapHeight + m_numPolePaddingRows * 2;

		// allocate the new map
		var buffer = new float[ height, PG_Planet.c_mapWidth ];

		// get the most used color of the top row
		var color = pgPlanet.GetMostUsedColor( 0 );

		// pad the top rows with this color
		for ( var y = 0; y < m_numPolePaddingRows; y++ )
		{
			for ( var x = 0; x < PG_Planet.c_mapWidth; x++ )
			{
				buffer[ y, x ] = color.a;
			}
		}

		// get the most used color of the bottom row
		color = pgPlanet.GetMostUsedColor( PG_Planet.c_mapHeight - 1 );

		// pad the bottom rows with this color
		for ( var y = 0; y < m_numPolePaddingRows; y++ )
		{
			for ( var x = 0; x < PG_Planet.c_mapWidth; x++ )
			{
				buffer[ y + height - m_numPolePaddingRows, x ] = color.a;
			}
		}

		// copy the original map into the are in between the padded rows
		for ( var y = 0; y < PG_Planet.c_mapHeight; y++ )
		{
			for ( var x = 0; x < PG_Planet.c_mapWidth; x++ )
			{
				buffer[ y + m_numPolePaddingRows, x ] = pgPlanet.m_mapColor[ y, x ].a;
			}
		}

		// all done
		return buffer;
	}
}
