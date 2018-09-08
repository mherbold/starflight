
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataController : MonoBehaviour
{
	// the number of save game slots
	public const int c_numSaveGameSlots = 5;

	// static reference to this instance
	public static DataController m_instance;
	public static string m_sceneToLoad = "Intro";

	// the name of the game data file
	public string m_gameDataFileName;
	
	// the name of the planet data file
	public string m_planetDataFileName;

	// the name of the save game file
	public string m_playerDataFileName;

	// the loaded game data
	public GameData m_gameData;

	// the loaded planet data
	public PlanetData m_planetData;

	// the save game slots
	public PlayerData[] m_playerDataList;

	// the active save game slot number
	public int m_activeSaveGameSlotNumber;

	// the player data from the current save game slot
	public PlayerData m_playerData;

	// unity awake
	void Awake()
	{
		// remember this instance to this
		m_instance = this;
	}

	// unity start
	void Start()
	{
		// load the game data
		LoadGameData();

		// load the planet data
		LoadPlanetData();

		// load the save game slots
		LoadPlayerDataList();

		// debug info
		Debug.Log( "Loading scene " + m_sceneToLoad );

		// load the next scene
		SceneManager.LoadScene( m_sceneToLoad );
	}
	
	// load the game data files
	void LoadGameData()
	{
		// debug info
		Debug.Log( "Loading game data" );

		// get the location of this game data file
		string filePath = Path.Combine( "GameData", m_gameDataFileName );

		// load it as a text asset
		TextAsset textAsset = Resources.Load( filePath ) as TextAsset;

		// convert it from the json string to our game data class
		m_gameData = JsonUtility.FromJson<GameData>( textAsset.text );

		// initalize the game data
		m_gameData.Initialize();
	}

	// load the planet data files
	void LoadPlanetData()
	{
		// debug info
		Debug.Log( "Loading planet data" );

		// get the location of this planet data file
		string filePath = Path.Combine( "GameData", m_planetDataFileName );

		// load it as a text asset
		TextAsset textAsset = Resources.Load( filePath ) as TextAsset;

		// convert it from the json string to our planet data class
		m_planetData = JsonUtility.FromJson<PlanetData>( textAsset.text );
	}

	// this loads the save game slots from disk
	void LoadPlayerDataList()
	{
		// whether or not we have found the current game
		bool currentGameFound = false;

		// create the player data list
		m_playerDataList = new PlayerData[ c_numSaveGameSlots ];

		// go through each save game slot
		for ( int i = 0; i < c_numSaveGameSlots; i++ )
		{
			// get the path to the player data file
			string filePath = Application.persistentDataPath + "/" + m_playerDataFileName + i + ".bin";

			// keep track of whether or not we were able to load the player data file
			bool loadSucceeded = false;

			// check if the file exists
			if ( File.Exists( filePath ) )
			{
				try
				{
					// debug info
					Debug.Log( "Loading player data " + i );

					// try to load the save game file now
					FileStream file = File.Open( filePath, FileMode.Open );
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					m_playerDataList[ i ] = (PlayerData) binaryFormatter.Deserialize( file );

					// we were able to load the save game slots from file (version checking is next)
					loadSucceeded = true;
				}
				catch
				{
					Debug.Log( "...failed" );
				}
			}

			// if the player data is from an old version then we have to start over
			if ( !loadSucceeded || !m_playerDataList[ i ].IsCurrentVersion() )
			{
				// debug info
				Debug.Log( "Creating and resetting player data " + i );

				m_playerDataList[ i ] = new PlayerData();

				m_playerDataList[ i ].Reset();
			}

			// check if this is the active save game slot
			if ( m_playerDataList[ i ].m_isCurrentGame )
			{
				// yes - remember the slot number
				m_activeSaveGameSlotNumber = i;

				// point the current player data to this slot
				m_playerData = m_playerDataList[ m_activeSaveGameSlotNumber ];

				// we have found the current game
				currentGameFound = true;
			}
		}

		// did we not find the current game?
		if ( !currentGameFound )
		{
			// nope - use the first slot
			m_activeSaveGameSlotNumber = 0;

			// point the current player data to this slot
			m_playerData = m_playerDataList[ m_activeSaveGameSlotNumber ];
		}

		// debug info
		Debug.Log( "Active save game slot number is " + m_activeSaveGameSlotNumber );
	}

	// this saves our current save game slot to disk
	public void SaveActiveGame()
	{
		SavePlayerData( m_activeSaveGameSlotNumber );
	}

	// this saves a save game slot to disk
	public void SavePlayerData( int saveGameSlotNumber )
	{
		Debug.Log( "Saving player data in game slot number " + saveGameSlotNumber );

		// get the path to the player data file
		string filePath = Application.persistentDataPath + "/" + m_playerDataFileName + saveGameSlotNumber + ".bin";

		try
		{
			// try to save the player data file
			using ( FileStream file = File.Create( filePath ) )
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize( file, m_playerData );
			}
		}
		catch ( IOException exception )
		{
			// report if we got an exception
			Debug.Log( "...failed - " + exception.Message );
		}
	}

	// call this to get the name of the current scene for the active save game slot
	public string GetCurrentSceneName()
	{
		// figure out what the current scene is
		switch ( m_playerData.m_starflight.m_location )
		{
			case Starflight.Location.DockingBay:
			case Starflight.Location.Hyperspace:
			case Starflight.Location.InOrbit:
			case Starflight.Location.JustLaunched:
			case Starflight.Location.OnPlanet:
			case Starflight.Location.StarSystem:
				return "Spaceflight";

			default:
				return "Starport";
		}
	}

	// call this to change the active save game slot number
	public void SetActiveSaveGameSlotNumber( int newActiveSaveGameSlotNumber )
	{
		// make the current slot not the current game
		m_playerData.m_isCurrentGame = false;

		// save the active game in the old save game slot number
		SaveActiveGame();

		// update the active save game slot number
		m_activeSaveGameSlotNumber = newActiveSaveGameSlotNumber;

		// point the current player data to the new slot
		m_playerData = m_playerDataList[ m_activeSaveGameSlotNumber ];

		// make the current slot the active game
		m_playerData.m_isCurrentGame = true;

		// save the active game
		SaveActiveGame();

		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;

		// figure out which scene to load (based on the player location in the save data)
		string nextSceneName = GetCurrentSceneName();

		// load the next scene
		SceneManager.LoadScene( nextSceneName );
	}

	// call this top copy the active save game slot to another slot
	public void CopyActiveSaveGameSlot( int targetSaveGameSlotNumber )
	{
		// save the active game in the current slot
		SaveActiveGame();

		// clone the player data
		PlayerData clonedPlayerData = Tools.CloneObject( m_playerData );

		// the cloned copy is not the current game
		clonedPlayerData.m_isCurrentGame = false;

		// set the cloned player data to the target slot
		m_playerDataList[ targetSaveGameSlotNumber ] = clonedPlayerData;

		// save the game in the target save game slot
		SavePlayerData( targetSaveGameSlotNumber );
	}

	// call this to reset the active game
	public void ResetGame()
	{
		// reset the player data for the current slot
		m_playerData.Reset();

		// save the active game (with freshly reset data)
		SaveActiveGame();

		// figure out which scene to load (based on the player location in the save data)
		string nextSceneName = GetCurrentSceneName();

		// start fading out the intro scene
		SceneFadeController.m_instance.FadeOut( nextSceneName );

		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;
	}
}
