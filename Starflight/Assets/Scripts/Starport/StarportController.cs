
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StarportController : MonoBehaviour
{
	// unity awake
	void Awake()
	{
		// check if we loaded the persistent scene
		if ( DataController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			DataController.m_sceneToLoad = "Starport";

			SceneManager.LoadScene( "Persistent" );
		}
	}

	// unity start
	void Start()
	{
		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// make sure we are in the right scene
		switch ( playerData.m_starflight.m_location )
		{
			case Starflight.Location.Starport:

				// start playing the starport music
				MusicController.m_instance.ChangeToTrack( MusicController.Track.Starport );

				// fade the scene in
				SceneFadeController.m_instance.FadeIn();

				break;

			default:
				SceneManager.LoadScene( "Spaceflight" );
				break;
		}
	}
}
