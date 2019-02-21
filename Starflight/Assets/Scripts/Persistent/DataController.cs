
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

public class DataController : MonoBehaviour
{
	// the number of save game slots
	public const int c_numSaveGameSlots = 5;

	// static reference to this instance
	public static DataController m_instance;
	public static string m_sceneToLoad = "Intro";

	// the name of the game data file
	public string m_gameDataFileName;
	
	// the name of the save game file
	public string m_playerDataFileName;

	// the loaded game data
	public GameData m_gameData;

	// the save game slots
	public PlayerData[] m_playerDataList;

	// the player data from the current save game slot
	public PlayerData m_playerData;

	// the active save game slot number
	public int m_activeSaveGameSlotNumber;

	// set this to switch to a different save game slot
	int m_targetSaveGameSlotNumber;

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

		// load the save game slots
		LoadPlayerDataList();

		// debug info
		UnityEngine.Debug.Log( "Loading scene " + m_sceneToLoad );

		// load the next scene
		SceneManager.LoadScene( m_sceneToLoad );
	}

	// unity late update
	void LateUpdate()
	{
		// if we want to switch to a different save game do it now and load the next scene
		if ( m_targetSaveGameSlotNumber != m_activeSaveGameSlotNumber )
		{
			// report the change
			UnityEngine.Debug.Log( "Switching to save game slot number " + m_targetSaveGameSlotNumber );

			// make the current slot not the current game
			m_playerData.m_isCurrentGame = false;

			// save the active game in the old save game slot number
			SaveActiveGame();

			// update the active save game slot number
			m_activeSaveGameSlotNumber = m_targetSaveGameSlotNumber;

			// point the current player data to the new slot
			m_playerData = m_playerDataList[ m_activeSaveGameSlotNumber ];

			// make the current slot the active game
			m_playerData.m_isCurrentGame = true;

			// save the active game
			SaveActiveGame();

			// turn off controller navigation of the UI
			EventSystem.current.sendNavigationEvents = false;

			// figure out which scene to load (based on the player location in the save data)
			var nextSceneName = GetCurrentSceneName();

			// debug info
			UnityEngine.Debug.Log( "Loading scene " + nextSceneName );

			// load the next scene
			SceneManager.LoadScene( nextSceneName );
		}
	}

	// load the game data files
	void LoadGameData()
	{
		// load it as an asset
		var textAsset = Resources.Load( m_gameDataFileName ) as TextAsset;

		// convert it from the json string to our game data class
		m_gameData = JsonUtility.FromJson<GameData>( textAsset.text );

		// initalize the game data
		m_gameData.Initialize();
	}

	// this loads the save game slots from disk
	void LoadPlayerDataList()
	{
		// whether or not we have found the current game
		var currentGameFound = false;

		// create the player data list
		m_playerDataList = new PlayerData[ c_numSaveGameSlots ];

		// go through each save game slot
		for ( var i = 0; i < c_numSaveGameSlots; i++ )
		{
			// get the path to the player data file
			var filePath = Application.persistentDataPath + "/" + m_playerDataFileName + i + ".bin";

			// keep track of whether or not we were able to load the player data file
			var loadSucceeded = false;

			// check if the file exists
			if ( File.Exists( filePath ) )
			{
				try
				{
					// try to load the save game file now
					var file = File.Open( filePath, FileMode.Open );

					// create the binary formatter
					var binaryFormatter = new BinaryFormatter();

					// add support for serializing / deserializing Unity.Vector3
					var surrogateSelector = new SurrogateSelector();
					var vector3SerializationSurrogate = new Vector3SerializationSurrogate();
					surrogateSelector.AddSurrogate( typeof( Vector3 ), new StreamingContext( StreamingContextStates.All ), vector3SerializationSurrogate );
					binaryFormatter.SurrogateSelector = surrogateSelector;

					// load and deserialize the player data file
					m_playerDataList[ i ] = (PlayerData) binaryFormatter.Deserialize( file );

					// we were able to load the save game slots from file (version checking is next)
					loadSucceeded = true;
				}
				catch
				{
				}
			}

			// if the player data is from an old version then we have to start over
			if ( !loadSucceeded || !m_playerDataList[ i ].IsCurrentVersion() )
			{
				// debug info
				UnityEngine.Debug.Log( "Creating and resetting player data " + i );

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

		// set the target save game slot number to be the same as the active one
		m_targetSaveGameSlotNumber = m_activeSaveGameSlotNumber;
	}

	// this saves our current save game slot to disk
	public void SaveActiveGame()
	{
		SavePlayerData( m_activeSaveGameSlotNumber );
	}

	// this saves a save game slot to disk
	public void SavePlayerData( int saveGameSlotNumber )
	{
		// measure performance
		var stopwatch = new Stopwatch();

		stopwatch.Start();

		// get the path to the player data file
		var filePath = Application.persistentDataPath + "/" + m_playerDataFileName + saveGameSlotNumber + ".bin";

		try
		{
			// try to save the player data file
			using ( var file = File.Create( filePath ) )
			{
				// create the binary formatter
				var binaryFormatter = new BinaryFormatter();

				// add support for serializing / deserializing Unity.Vector3
				var surrogateSelector = new SurrogateSelector();
				var vector3SerializationSurrogate = new Vector3SerializationSurrogate();
				surrogateSelector.AddSurrogate( typeof( Vector3 ), new StreamingContext( StreamingContextStates.All ), vector3SerializationSurrogate );
				binaryFormatter.SurrogateSelector = surrogateSelector;

				// serialize and save the player data file
				binaryFormatter.Serialize( file, m_playerDataList[ saveGameSlotNumber ] );

				// report how long it took
				UnityEngine.Debug.Log( "Saving the player data took " + stopwatch.ElapsedMilliseconds + " milliseconds." );
			}
		}
		catch ( IOException exception )
		{
			// report if we got an exception
			UnityEngine.Debug.Log( "Saving player data failed - " + exception.Message );
		}
	}

	// call this to get the name of the current scene for the active save game slot
	public string GetCurrentSceneName()
	{
		// figure out what the current scene is
		switch ( m_playerData.m_general.m_location )
		{
			case PD_General.Location.DockingBay:
			case PD_General.Location.Hyperspace:
			case PD_General.Location.InOrbit:
			case PD_General.Location.Planetside:
			case PD_General.Location.JustLaunched:
			case PD_General.Location.StarSystem:
			case PD_General.Location.Encounter:
				return "Spaceflight";

			default:
				return "Starport";
		}
	}

	// call this to change the target save game slot number
	public void SetTargetSaveGameSlotNumber( int targetSaveGameSlotNumber )
	{
		// update the target save game slot number
		m_targetSaveGameSlotNumber = targetSaveGameSlotNumber;
	}

	// call this top copy the active save game slot to another slot
	public void CopyActiveSaveGameSlot( int targetSaveGameSlotNumber )
	{
		// save the active game in the current slot
		SaveActiveGame();

		// clone the player data
		var clonedPlayerData = Tools.CloneObject( m_playerData );

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

		// keep this game the active one
		m_playerData.m_isCurrentGame = true;

		// save the active game (with freshly reset data)
		SaveActiveGame();

		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;

		// figure out which scene to load (based on the player location in the save data)
		var nextSceneName = GetCurrentSceneName();

		// debug info
		UnityEngine.Debug.Log( "Loading scene " + nextSceneName );

		// load the next scene
		SceneManager.LoadScene( nextSceneName );
	}
}
