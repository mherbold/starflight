
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{
	// static reference to this instance
	public static DataController m_instance;
	public static string m_sceneToLoad = "Intro";

	// public stuff we want to set using the editor
	public string m_gameDataFileName;
	public string m_planetDataFileName;
	public string m_playerDataFileName;

	public GameData m_gameData;
	public PlanetData m_planetData;
	public PlayerData m_playerData;

	public bool m_resetPlayerData;

	// private stuff we don't want the editor to see
	private float m_delayedSaveTimer;

	// the constructor
	public DataController()
	{
		m_playerData = new PlayerData();
	}

	// this is called by unity before start
	private void Awake()
	{
		// remember this instance to this
		m_instance = this;

		// reset the save timer
		m_delayedSaveTimer = 0.0f;
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		// debug info
		Debug.Log( "Loading data..." );

		// tell unity to not delete this game object when the scene unloads
		DontDestroyOnLoad( this );

		// load the game data
		LoadGameData();

		// load the planet data
		LoadPlanetData();

		// load the saved player data
		LoadPlayerData();

		// initalize the game data
		m_gameData.Initialize();

		// set this to true to force a reset of the player data
		if ( m_resetPlayerData )
		{
			m_playerData.Reset();
		}

		// debug info
		Debug.Log( "Loading scene " + m_sceneToLoad );

		// load the next scene
		SceneManager.LoadScene( m_sceneToLoad );
	}

	// this is called by unity every frame
	private void Update()
	{
		// if we have a delayed save active then update it
		if ( m_delayedSaveTimer > 0.0f )
		{
			m_delayedSaveTimer = Mathf.Max( 0.0f, m_delayedSaveTimer - Time.deltaTime );

			if ( m_delayedSaveTimer == 0.0f )
			{
				// the timer has expired - try saving the player data now
				SavePlayerData();
			}
		}
	}

	// load the game data files
	private void LoadGameData()
	{
		// load the game data
		LoadGameDataFile( out m_gameData, m_gameDataFileName );
	}

	// function to load game data from a file into an object
	private void LoadGameDataFile<T>( out T gameData, string fileName ) where T : GameDataFile
	{
		// get the location of this game data file
		string filePath = Path.Combine( "GameData", fileName );

		// load it as a text asset
		TextAsset textAsset = Resources.Load( filePath ) as TextAsset;

		// convert it from the json string to our game data class
		gameData = JsonUtility.FromJson<T>( textAsset.text );
	}

	// load the planet data files
	private void LoadPlanetData()
	{
		// load the planet data
		LoadPlanetDataFile( out m_planetData, m_planetDataFileName );
	}

	// function to load planet data from a file into an object
	private void LoadPlanetDataFile<T>( out T planetData, string fileName ) where T : PlanetDataFile
	{
		// get the location of this planet data file
		string filePath = Path.Combine( "GameData", fileName );

		// load it as a text asset
		TextAsset textAsset = Resources.Load( filePath ) as TextAsset;

		// convert it from the json string to our planet data class
		planetData = JsonUtility.FromJson<T>( textAsset.text );
	}

	// this loads the current player progress from disk
	private void LoadPlayerData()
	{
		// get the path to the player data file
		string filePath = Application.persistentDataPath + "/" + m_playerDataFileName;

		// keep track of whether or not we were able to load the player data from file
		bool loadSucceeded = false;

		// check if the file exists
		if ( File.Exists( filePath ) )
		{
			try
			{
				// try to load the player data now
				FileStream file = File.Open( filePath, FileMode.Open );
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				m_playerData = (PlayerData) binaryFormatter.Deserialize( file );

				// we were able to load the player data from file (version checking is next)
				loadSucceeded = true;
			}
			catch
			{
				// if we failed then we probably have changed the player data structure in a way that we can't load from older player data files
				Debug.Log( "Failed to load player data - player data has been reset." );
			}
		}

		// if the player data is from an old version then we have to start over
		if ( !loadSucceeded || !m_playerData.IsCurrentVersion() )
		{
			m_playerData.Reset();
		}
	}

	// this saves our current player progress to disk
	public void SavePlayerData()
	{
		// if the delayed save timer is not zero then don't save the player data yet
		if ( m_delayedSaveTimer == 0.0f )
		{
			Debug.Log( "Saving the player data now..." );

			// get the path to the player data file
			string filePath = Application.persistentDataPath + "/" + m_playerDataFileName;

			try
			{
				// try to save the player data now
				FileStream file = File.Create( filePath );
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize( file, m_playerData );
			}
			catch ( IOException )
			{
				// if we got an exception it is possibly a sharing violation which means we may be trying to save too quickly after the last save
				Debug.Log( "Got exception - trying again later." );

				// wait some time before trying to save again
				m_delayedSaveTimer = 2.0f;
			}
		}
	}
}
