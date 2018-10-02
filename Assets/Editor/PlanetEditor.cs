
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public class PlanetEditor : EditorWindow
{
	enum Mode
	{
		SelectedPlanet,
		AllPlanets
	}

	// user controlled parameters
	string m_gameDataFileName;
	string m_planetDataFileName;
	string m_outputPath;
	int m_textureMapWidth;
	int m_textureMapHeight;
	int m_numPolePaddingRows;
	int m_landBlurRadius;
	int m_waterBlurRadius;
	float m_inputGain;
	int m_octaves;
	int m_mountainScale;
	float m_mountainPersistence;
	float m_mountainPower;
	float m_mountainGain;
	int m_hillScale;
	float m_hillPersistence;
	float m_hillGain;
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
	int m_snapshotInterval;
	int m_finalBlurRadius;
	float m_normalScale;
	string m_gameObjectName;
	int m_planetId;

	// generated parameters
	GameData m_gameData;
	PlanetData m_planetData;
	GameObject m_gameObject;
	MeshRenderer m_meshRenderer;
	int m_textureMapScaleX;
	int m_textureMapScaleY;
	int m_mountainScalePowerOfTwo;
	int m_hillScalePowerOfTwo;

#if DEBUG
	[MenuItem( "Window/Starflight Remake/Planet Editor" )]
#else
	[MenuItem( "Window/Starflight Remake/Planet Editor (Release Build)" )]
#endif

	public static void ShowWindow()
	{
		GetWindow( typeof( PlanetEditor ) );
	}

	void OnEnable()
	{
		m_gameDataFileName = EditorPrefs.GetString( "PlanetEditor_GameDataFileName" );
		m_planetDataFileName = EditorPrefs.GetString( "PlanetEditor_PlanetDataFileName" );
		m_outputPath = EditorPrefs.GetString( "PlanetEditor_OutputPath" );
		m_numPolePaddingRows = EditorPrefs.GetInt( "PlanetEditor_NumPolePaddingRows", 3 );
		m_landBlurRadius = EditorPrefs.GetInt( "PlanetEditor_LandBlurRadius", 45 );
		m_waterBlurRadius = EditorPrefs.GetInt( "PlanetEditor_WaterBlurRadius", 11 );
		m_inputGain = EditorPrefs.GetFloat( "PlanetEditor_InputGain", 0.5f );
		m_octaves = EditorPrefs.GetInt( "PlanetEditor_Octaves", 8 );
		m_mountainScale = EditorPrefs.GetInt( "PlanetEditor_MountainScale", 4 );
		m_mountainPersistence = EditorPrefs.GetFloat( "PlanetEditor_MountainPersistence", 0.5f );
		m_mountainPower = EditorPrefs.GetFloat( "PlanetEditor_MountainPower", 3.0f );
		m_mountainGain = EditorPrefs.GetFloat( "PlanetEditor_MountainGain", 0.075f );
		m_hillScale = EditorPrefs.GetInt( "PlanetEditor_HillScale", 4 );
		m_hillPersistence = EditorPrefs.GetFloat( "PlanetEditor_HillPersistence", 0.9f );
		m_hillGain = EditorPrefs.GetFloat( "PlanetEditor_HillGain", 0.05f );
		m_xyScaleToMeters = EditorPrefs.GetFloat( "PlanetEditor_XYScaleToMeters", 100.0f );
		m_zScaleToMeters = EditorPrefs.GetFloat( "PlanetEditor_ZScaleToMeters", 1500.0f );
		m_rainWaterAmount = EditorPrefs.GetFloat( "PlanetEditor_RainWaterAmount", 50.0f );
		m_gravityConstant = EditorPrefs.GetFloat( "PlanetEditor_GravityConstant", 9.8f );
		m_frictionConstant = EditorPrefs.GetFloat( "PlanetEditor_FrictionConstant", 0.5f );
		m_evaporationConstant = EditorPrefs.GetFloat( "PlanetEditor_EvaporationConstant", 0.02f );
		m_sedimentCapacity = EditorPrefs.GetFloat( "PlanetEditor_SedimentCapacity", 1.0f );
		m_depositionConstant = EditorPrefs.GetFloat( "PlanetEditor_DepositionConstant", 0.05f );
		m_dissolvingConstant = EditorPrefs.GetFloat( "PlanetEditor_DissolvingConstant", 0.04f );
		m_stepDeltaTime = EditorPrefs.GetFloat( "PlanetEditor_StepDeltaTime", 0.005f );
		m_snapshotInterval = EditorPrefs.GetInt( "PlanetEditor_SnapshotInterval", 8192 );
		m_finalBlurRadius = EditorPrefs.GetInt( "PlanetEditor_FinalBlurRadius", 5 );
		m_normalScale = EditorPrefs.GetFloat( "PlanetEditor_NormalScale", 50.0f );
		m_gameObjectName = EditorPrefs.GetString( "PlanetEditor_GameObjectName" );
		m_planetId = EditorPrefs.GetInt( "PlanetEditor_PlanetId" );

		int textureMapHeight = EditorPrefs.GetInt( "PlanetEditor_TextureMapHeight" );

		m_textureMapWidth = textureMapHeight * 2;
		m_textureMapHeight = textureMapHeight;
	}

	void OnDisable()
	{
		EditorPrefs.SetString( "PlanetEditor_GameDataFileName", m_gameDataFileName );
		EditorPrefs.SetString( "PlanetEditor_PlanetDataFileName", m_planetDataFileName );
		EditorPrefs.SetString( "PlanetEditor_OutputPath", m_outputPath );
		EditorPrefs.SetInt( "PlanetEditor_TextureMapHeight", m_textureMapHeight );
		EditorPrefs.SetInt( "PlanetEditor_NumPolePaddingRows", m_numPolePaddingRows );
		EditorPrefs.SetInt( "PlanetEditor_LandBlurRadius", m_landBlurRadius );
		EditorPrefs.SetInt( "PlanetEditor_WaterBlurRadius", m_waterBlurRadius );
		EditorPrefs.SetFloat( "PlanetEditor_InputGain", m_inputGain );
		EditorPrefs.SetInt( "PlanetEditor_Octaves", m_octaves );
		EditorPrefs.SetInt( "PlanetEditor_MountainScale", m_mountainScale );
		EditorPrefs.SetFloat( "PlanetEditor_MountainPersistence", m_mountainPersistence );
		EditorPrefs.SetFloat( "PlanetEditor_MountainPower", m_mountainPower );
		EditorPrefs.SetFloat( "PlanetEditor_MountainGain", m_mountainGain );
		EditorPrefs.SetInt( "PlanetEditor_HillScale", m_hillScale );
		EditorPrefs.SetFloat( "PlanetEditor_HillPersistence", m_hillPersistence );
		EditorPrefs.SetFloat( "PlanetEditor_HillGain", m_hillGain );
		EditorPrefs.SetFloat( "PlanetEditor_XYScaleToMeters", m_xyScaleToMeters );
		EditorPrefs.SetFloat( "PlanetEditor_ZScaleToMeters", m_zScaleToMeters );
		EditorPrefs.SetFloat( "PlanetEditor_RainWaterAmount", m_rainWaterAmount );
		EditorPrefs.SetFloat( "PlanetEditor_GravityConstant", m_gravityConstant );
		EditorPrefs.SetFloat( "PlanetEditor_FrictionConstant", m_frictionConstant );
		EditorPrefs.SetFloat( "PlanetEditor_EvaporationConstant", m_evaporationConstant );
		EditorPrefs.SetFloat( "PlanetEditor_SedimentCapacity", m_sedimentCapacity );
		EditorPrefs.SetFloat( "PlanetEditor_DepositionConstant", m_depositionConstant );
		EditorPrefs.SetFloat( "PlanetEditor_DissolvingConstant", m_dissolvingConstant );
		EditorPrefs.SetFloat( "PlanetEditor_StepDeltaTime", m_stepDeltaTime );
		EditorPrefs.SetInt( "PlanetEditor_SnapshotInterval", m_snapshotInterval );
		EditorPrefs.SetInt( "PlanetEditor_FinalBlurRadius", m_finalBlurRadius );
		EditorPrefs.SetFloat( "PlanetEditor_NormalScale", m_normalScale );
		EditorPrefs.SetString( "PlanetEditor_GameObjectName", m_gameObjectName );
		EditorPrefs.SetInt( "PlanetEditor_PlanetId", m_planetId );
	}

	void OnGUI()
	{
		GUILayout.Label( "Other Settings", EditorStyles.boldLabel );

		m_gameDataFileName = EditorGUILayout.TextField( "Game Data File Name", m_gameDataFileName );
		m_planetDataFileName = EditorGUILayout.TextField( "Planet Data File Name", m_planetDataFileName );
		m_outputPath = EditorGUILayout.TextField( "Output Path", m_outputPath );

		GUILayout.Label( "Non-Gas Giant Planet Settings", EditorStyles.boldLabel );

		m_textureMapHeight = EditorGUILayout.IntField( "Texture Map Height", m_textureMapHeight );
		m_textureMapWidth = m_textureMapHeight * 2;
		m_numPolePaddingRows = EditorGUILayout.IntSlider( "Num Pole Padding Rows", m_numPolePaddingRows, 0, 8 );
		m_landBlurRadius = EditorGUILayout.IntSlider( "Land Blur Radius", m_landBlurRadius, 1, 512 );
		m_waterBlurRadius = EditorGUILayout.IntSlider( "Water Blur Radius", m_waterBlurRadius, 1, 512 );

		GUILayout.Label( "Base Mountain and Hill Settings", EditorStyles.boldLabel );

		m_inputGain = EditorGUILayout.Slider( "Input Gain", m_inputGain, 0.0f, 1.0f );
		m_octaves = EditorGUILayout.IntSlider( "Octaves", m_octaves, 1, 12 );

		GUILayout.Label( "Mountain Settings", EditorStyles.boldLabel );

		m_mountainScale = EditorGUILayout.IntSlider( "Scale", m_mountainScale, 1, 12 );
		m_mountainPersistence = EditorGUILayout.Slider( "Persistence", m_mountainPersistence, 0.0f, 1.0f );
		m_mountainPower = EditorGUILayout.Slider( "Power", m_mountainPower, 1.0f, 10.0f );
		m_mountainGain = EditorGUILayout.Slider( "Output Gain", m_mountainGain, 0.0f, 1.0f );

		GUILayout.Label( "Hill Settings", EditorStyles.boldLabel );

		m_hillScale = EditorGUILayout.IntSlider( "Scale", m_hillScale, 1, 12 );
		m_hillPersistence = EditorGUILayout.Slider( "Persistence", m_hillPersistence, 0.0f, 1.0f );
		m_hillGain = EditorGUILayout.Slider( "Output Gain", m_hillGain, 0.0f, 1.0f );

		GUILayout.Label( "Hydraulic Erosion Settings", EditorStyles.boldLabel );

		m_xyScaleToMeters = EditorGUILayout.FloatField( "XY Scale To Meters", m_xyScaleToMeters );
		m_zScaleToMeters = EditorGUILayout.FloatField( "Z Scale To Meters", m_zScaleToMeters );
		m_rainWaterAmount = EditorGUILayout.FloatField( "Rain Water Amount", m_rainWaterAmount );
		m_sedimentCapacity = EditorGUILayout.FloatField( "Sediment Capacity", m_sedimentCapacity );
		m_gravityConstant = EditorGUILayout.FloatField( "Gravity Constant", m_gravityConstant );
		m_frictionConstant = EditorGUILayout.FloatField( "Friction Constant", m_frictionConstant );
		m_evaporationConstant = EditorGUILayout.FloatField( "Evaporation Constant", m_evaporationConstant );
		m_depositionConstant = EditorGUILayout.FloatField( "Deposition Constant", m_depositionConstant );
		m_dissolvingConstant = EditorGUILayout.FloatField( "Dissolving Constant", m_dissolvingConstant );
		m_stepDeltaTime = EditorGUILayout.FloatField( "Step Delta Time", m_stepDeltaTime );
		m_snapshotInterval = EditorGUILayout.IntField( "Snapshot Interval", m_snapshotInterval );
		m_finalBlurRadius = EditorGUILayout.IntField( "Final Blur Radius", m_finalBlurRadius );

		GUILayout.Label( "Normal Map Settings", EditorStyles.boldLabel );

		m_normalScale = EditorGUILayout.Slider( "Scale", m_normalScale, 1.0f, 200.0f );

		GUILayout.Label( "Preview Settings", EditorStyles.boldLabel );

		m_gameObjectName = EditorGUILayout.TextField( "Game Object Name", m_gameObjectName );
		m_planetId = EditorGUILayout.IntField( "Planet ID", m_planetId );

		if ( GUILayout.Button( "Generate Texture Maps For Selected Planet" ) )
		{
			MakeSomeMagic( Mode.SelectedPlanet );
		}

		if ( GUILayout.Button( "Generate Texture Maps For ALL Planets" ) )
		{
			MakeSomeMagic( Mode.AllPlanets );
		}
	}

	void OnInspectorUpdate()
	{
		Repaint();
	}

	// generate maps and show them on the planet game object
	void MakeSomeMagic( Mode mode )
	{
		// show progress bar
		EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Initializing...", 0.0f );

		// load the game data
		if ( !LoadGameData() )
		{
			return;
		}

		// load the planet data
		if ( !LoadPlanetData() )
		{
			return;
		}

		// find the game object
		m_gameObject = GameObject.Find( m_gameObjectName );

		if ( m_gameObject == null )
		{
			ShowNotification( new GUIContent( "Game object could not be found." ) );

			return;
		}

		// get the mesh renderer from the game object
		m_meshRenderer = m_gameObject.GetComponent<MeshRenderer>();

		if ( m_meshRenderer == null )
		{
			ShowNotification( new GUIContent( "No mesh renderer component found on the game object." ) );

			return;
		}

		// calculate the texture map scale (and it must be an even number)
		m_textureMapScaleX = Mathf.FloorToInt( (float) m_textureMapWidth / (float) PlanetMap.c_width );
		m_textureMapScaleY = Mathf.FloorToInt( (float) m_textureMapHeight / (float) ( PlanetMap.c_height + m_numPolePaddingRows * 2 ) );

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

		// run the selected subroutine
		Planet planet;
		PlanetMap planetMap;

		switch ( mode )
		{
			case Mode.SelectedPlanet:

				planet = m_gameData.m_planetList[ m_planetId ];

				planetMap = m_planetData.m_planetMapList[ m_planetId ];

				GeneratePlanetTextureMaps( planet, planetMap );

				break;

			case Mode.AllPlanets:

				for ( var i = 0; i < 811; i++ )
				{
					var path = m_outputPath + "\\" + i;

					if ( !Directory.Exists( path ) )
					{
						planet = m_gameData.m_planetList[ i ];

						planetMap = m_planetData.m_planetMapList[ i ];

						GeneratePlanetTextureMaps( planet, planetMap );
					}
				}

				break;
		}

		// show progress bar
		EditorUtility.ClearProgressBar();
	}

	// load the game data file
	bool LoadGameData()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		// clear out the game data
		m_gameData = null;

		// load it as a text asset
		var textAsset = Resources.Load( m_gameDataFileName ) as TextAsset;

		if ( textAsset == null )
		{
			ShowNotification( new GUIContent( "Game data file not found." ) );

			return false;
		}
		else
		{
			// convert it from the json string to our planet data class
			m_gameData = JsonUtility.FromJson<GameData>( textAsset.text );

			m_gameData.Initialize();
		}

		UnityEngine.Debug.Log( "Load Game Data - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return true;
	}

	// load the planet data file
	bool LoadPlanetData()
	{
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		// clear out the old data
		m_planetData = null;

		// load it as a text asset
		var textAsset = Resources.Load( m_planetDataFileName ) as TextAsset;

		if ( textAsset == null )
		{
			ShowNotification( new GUIContent( "Planet data file not found." ) );

			return false;
		}
		else
		{
			// convert it from the json string to our planet data class
			m_planetData = JsonUtility.FromJson<PlanetData>( textAsset.text );

			// prep each planet map for generation (speed optimization)
			foreach ( var planetMap in m_planetData.m_planetMapList )
			{
				planetMap.Initialize();
			}
		}

		for ( var i = 0; i < m_planetData.m_planetMapList.Length; i++ )
		{
			if ( m_planetData.m_planetMapList[ i ].m_badData )
			{
				var planet = m_gameData.m_planetList[ i ];

				var star = m_gameData.m_starList[ planet.m_starId ];

				UnityEngine.Debug.Log( "Suspicious planet #" + ( i + 1 ) + " at " + star.m_xCoordinate + ", " + star.m_yCoordinate + " orbit " + planet.m_orbitPosition );
			}
		}

		UnityEngine.Debug.Log( "Load Planet Data - " + stopwatch.ElapsedMilliseconds + " milliseconds" );

		return true;
	}

	// call this to generate the planet texture maps
	void GeneratePlanetTextureMaps( Planet planet, PlanetMap planetMap )
	{
		// vars for the progress bar
		var currentStep = 1;
		var totalSteps = 9;

		// get prepared source
		EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Preparing original map...", (float) currentStep++ / totalSteps );

		var heightBuffer = PrepareMap( planetMap );

		// Tools.SaveAsPNG( heightBuffer, "Prepared" );

		// contours pass
		EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Doing contours pass...", (float) currentStep++ / totalSteps );

		var contours = new Contours( heightBuffer );

		heightBuffer = contours.Process( m_textureMapScaleX, m_textureMapScaleY, planetMap.m_legend );

		// Tools.SaveAsPNG( heightBuffer, "Contours" );

		// scale to power of two
		EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Scaling to power of two...", (float) currentStep++ / totalSteps );

		var scaleToPowerOfTwo = new ScaleToPowerOfTwo( heightBuffer );

		heightBuffer = scaleToPowerOfTwo.Process( m_textureMapScaleX, m_textureMapScaleY );

		// Tools.SaveAsPNG( heightBuffer, "Scale to Power of Two" );

		// gaussian blur pass
		EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Gaussian blur pass...", (float) currentStep++ / totalSteps );

		var gaussianBlur = new GaussianBlur( heightBuffer );

		heightBuffer = gaussianBlur.Process( m_landBlurRadius, m_waterBlurRadius );

		// Tools.SaveAsPNG( heightBuffer, "Gaussian Blur" );

		// reference to the albedo buffer we will generate
		Color[,] albedoBuffer;

		// build the path to the albedo texture map
		var albedoFilename = m_outputPath + "\\" + planet.m_id + "\\Albedo.png";

		// not a gas giant?
		if ( planet.m_surfaceId != 1 )
		{
			// mountains pass
			EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Creating mountains and hills...", (float) currentStep++ / totalSteps );

			var mountainsAndHills = new MountainsAndHills( heightBuffer );

			heightBuffer = mountainsAndHills.Process( m_planetId, m_inputGain, m_octaves, m_mountainScalePowerOfTwo, m_hillScalePowerOfTwo, m_mountainPersistence, m_hillPersistence, m_mountainPower, m_mountainGain, m_hillGain );

			// Tools.SaveAsPNG( heightBuffer, "Mountains and Hills" );

			// hydraulic erosion pass
			EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Hydraulic erosion pass...", (float) currentStep++ / totalSteps );

			var hydraulicErosion = new HydraulicErosion( heightBuffer );

			heightBuffer = hydraulicErosion.Process( m_xyScaleToMeters, m_zScaleToMeters, m_rainWaterAmount, m_sedimentCapacity, m_gravityConstant, m_frictionConstant, m_evaporationConstant, m_depositionConstant, m_dissolvingConstant, m_stepDeltaTime, m_snapshotInterval, m_finalBlurRadius );

			// Tools.SaveAsPNG( heightBuffer, "Hydraulic Erosion" );

			// albedo pass
			EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Albedo pass...", (float) currentStep++ / totalSteps );

			var albedo = new Albedo( heightBuffer );

			albedoBuffer = albedo.Process( m_planetId, m_inputGain, planetMap.m_legend );

			Tools.SaveAsPNG( albedoBuffer, albedoFilename );

			AssetDatabase.ImportAsset( albedoFilename );
		}
		else
		{
			// albedo pass (temporary 1x1 black pixel)
			EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Albedo pass...", (float) currentStep++ / totalSteps );

			albedoBuffer = new Color[ 1, 1 ];

			albedoBuffer[ 0, 0 ] = Color.black;

			Tools.SaveAsPNG( albedoBuffer, albedoFilename );

			AssetDatabase.ImportAsset( albedoFilename );
		}

		// change import settings on albedo map
		var textureImporter = TextureImporter.GetAtPath( albedoFilename ) as TextureImporter;

		textureImporter.filterMode = FilterMode.Trilinear;
		textureImporter.wrapModeU = TextureWrapMode.Repeat;
		textureImporter.wrapModeV = TextureWrapMode.Clamp;

		textureImporter.SaveAndReimport();

		// apply the texture map to the material
		var texture = AssetDatabase.LoadAssetAtPath( albedoFilename, typeof( Texture2D ) ) as Texture2D;

		m_meshRenderer.material.SetTexture( "_Albedo", texture );

		// now we can get the final width and height from the albedo buffer
		var width = albedoBuffer.GetLength( 1 );
		var height = albedoBuffer.GetLength( 0 );

		// effects pass
		EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Effects pass...", (float) currentStep++ / totalSteps );

		var effectsBuffer = new Color[ height, width ];

		Vector3 legendWaterColor = new Vector3( planetMap.m_legend[ 0 ].r, planetMap.m_legend[ 0 ].g, planetMap.m_legend[ 0 ].b );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				// get the albedo color
				var color = albedoBuffer[ y, x ];

				// get the terrain height
				var terrainHeight = heightBuffer[ y, x ];

				// calculate the difference between this color and the legend water color
				var difference = new Vector3( color.r, color.g, color.b ) - legendWaterColor;

				// calculate roughness based on the difference (water = sharp gloss, not water = dull gloss)
				var roughnessFactor = difference.magnitude * 4.0f;

				var roughness = Mathf.SmoothStep( 0.3f, 1.0f, roughnessFactor );

				// calculate where we want to add in water waves (1 = waves, 0 = no waves)
				var water = Mathf.Lerp( 1.0f, 0.0f, roughnessFactor );

				// add in roughness due to snow on mountains (snow = add in sharp gloss)
				roughness = Mathf.Lerp( roughness, 0.2f, ( terrainHeight - 0.25f ) * 3.0f );

				// calculate reflectivity based on roughness (sharp gloss = also reflective, dull gloss = not so reflective)
				var reflectivity = ( 1.0f - roughness ) * 0.5f;

				// put it all together
				effectsBuffer[ y, x ] = new Color( roughness, water, reflectivity );
			}
		}

		// save the generated map to file
		var effectsFilename = m_outputPath + "\\" + planet.m_id + "\\Effects.png";

		Tools.SaveAsPNG( effectsBuffer, effectsFilename );

		AssetDatabase.ImportAsset( effectsFilename );

		// change import settings on effects map
		textureImporter = TextureImporter.GetAtPath( effectsFilename ) as TextureImporter;

		textureImporter.filterMode = FilterMode.Trilinear;
		textureImporter.wrapModeU = TextureWrapMode.Repeat;
		textureImporter.wrapModeV = TextureWrapMode.Clamp;

		textureImporter.SaveAndReimport();

		// apply the texture map to the material
		texture = AssetDatabase.LoadAssetAtPath( effectsFilename, typeof( Texture2D ) ) as Texture2D;

		m_meshRenderer.material.SetTexture( "_Effects", texture );

		// normals pass
		EditorUtility.DisplayProgressBar( "Planet " + ( planet.m_id + 1 ), "Normals pass...", (float) currentStep++ / totalSteps );

		var normals = new Normals( heightBuffer );

		var normalBuffer = normals.Process( m_normalScale );

		// save the generated map to file
		var normalsFilename = m_outputPath + "\\" + planet.m_id + "\\Normals.png";

		Tools.SaveAsPNG( normalBuffer, normalsFilename );

		AssetDatabase.ImportAsset( normalsFilename );

		// change import settings on albedo map
		textureImporter = TextureImporter.GetAtPath( normalsFilename ) as TextureImporter;

		textureImporter.textureType = TextureImporterType.NormalMap;
		textureImporter.filterMode = FilterMode.Trilinear;
		textureImporter.wrapModeU = TextureWrapMode.Repeat;
		textureImporter.wrapModeV = TextureWrapMode.Clamp;

		textureImporter.SaveAndReimport();

		// apply the texture map to the material
		texture = AssetDatabase.LoadAssetAtPath( normalsFilename, typeof( Texture2D ) ) as Texture2D;

		m_meshRenderer.material.SetTexture( "_Normal", texture );
	}

	float[,] PrepareMap( PlanetMap planetMap )
	{
		var height = PlanetMap.c_height + m_numPolePaddingRows * 2;

		var buffer = new float[ height, PlanetMap.c_width ];

		var color = planetMap.GetMostUsedColor( 0 );

		for ( var y = 0; y < m_numPolePaddingRows; y++ )
		{
			for ( var x = 0; x < PlanetMap.c_width; x++ )
			{
				buffer[ y, x ] = color.a;
			}
		}

		color = planetMap.GetMostUsedColor( PlanetMap.c_height - 1 );

		for ( var y = 0; y < m_numPolePaddingRows; y++ )
		{
			for ( var x = 0; x < PlanetMap.c_width; x++ )
			{
				buffer[ y + height - m_numPolePaddingRows, x ] = color.a;
			}
		}

		for ( var y = 0; y < PlanetMap.c_height; y++ )
		{
			for ( var x = 0; x < PlanetMap.c_width; x++ )
			{
				buffer[ y + m_numPolePaddingRows, x ] = planetMap.m_color[ y, x ].a;
			}
		}

		return buffer;
	}
}
