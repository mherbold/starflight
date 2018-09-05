
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
	// the pages of the intro
	public CanvasGroup[] m_pages;

	// first page fade in / out times
	public float m_firstPageFadeInStartTime;
	public float m_firstPageFadeInEndTime;

	// second page fade in / out times
	public float m_secondPageFadeInStartTime;
	public float m_secondPageFadeInEndTime;

	// third page fade in / out times
	public float m_thirdPageFadeInStartTime;
	public float m_thirdPageFadeInEndTime;

	// fourth page fade in / out times
	public float m_fourthPageFadeInStartTime;
	public float m_fourthPageFadeInEndTime;

	// fifth page fade in / out times
	public float m_fifthPageFadeInStartTime;
	public float m_fifthPageFadeInEndTime;

	// our timer
	float m_timer;

	// keep track of whether or not the intro music might still be playing
	bool m_introMusicMightBePlaying;

	// unity awake
	void Awake()
	{
		// check if we loaded the persistent scene
		if ( DataController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			DataController.m_sceneToLoad = "Intro";

			SceneManager.LoadScene( "Persistent" );
		}
	}

	// unity start
	void Start()
	{
		// start playing the intro music
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Intro );

		// the intro music is playing
		m_introMusicMightBePlaying = true;

		// reset timer to 0
		m_timer = 0.0f;

		// fade the scene in
		SceneFadeController.m_instance.FadeIn();
	}

	// unity update
	void Update()
	{
		// increment timer by the number of seconds that has passed by so far
		m_timer += Time.deltaTime;

		// if we pressed the fire button then skip ahead
		if ( InputController.m_instance.SubmitWasPressed() )
		{
			if ( m_timer < m_firstPageFadeInEndTime )
			{
				m_timer = m_firstPageFadeInEndTime;
			}
			else if ( m_timer < m_secondPageFadeInEndTime )
			{
				m_timer = m_secondPageFadeInEndTime;
			}
			else if ( m_timer < m_thirdPageFadeInEndTime )
			{
				m_timer = m_thirdPageFadeInEndTime;
			}
			else if ( m_timer < m_fourthPageFadeInEndTime )
			{
				m_timer = m_fourthPageFadeInEndTime;
			}
			else if ( m_timer < m_fifthPageFadeInEndTime )
			{
				m_timer = m_fifthPageFadeInEndTime;
			}
			else
			{
				// get to the player data
				PlayerData playerData = DataController.m_instance.m_playerData;

				// figure out which scene to load (based on the player location in the save data)
				string nextSceneName;

				switch ( playerData.m_starflight.m_location )
				{
					case Starflight.Location.DockingBay:
					case Starflight.Location.Hyperspace:
					case Starflight.Location.InOrbit:
					case Starflight.Location.JustLaunched:
					case Starflight.Location.OnPlanet:
					case Starflight.Location.StarSystem:
						nextSceneName = "Spaceflight";
						break;

					default:
						nextSceneName = "Starport";
						break;
				}
						
				SceneFadeController.m_instance.FadeOut( nextSceneName );
			}
		}

		// calculate fade for each of the three pages
		float firstPageAlpha = Mathf.Lerp( 0.0f, 1.0f, ( m_timer - m_firstPageFadeInStartTime ) / ( m_firstPageFadeInEndTime - m_firstPageFadeInStartTime ) );
		float secondPageAlpha = Mathf.Lerp( 0.0f, 1.0f, ( m_timer - m_secondPageFadeInStartTime ) / ( m_secondPageFadeInEndTime - m_secondPageFadeInStartTime ) );
		float thirdPageAlpha = Mathf.Lerp( 0.0f, 1.0f, ( m_timer - m_thirdPageFadeInStartTime ) / ( m_thirdPageFadeInEndTime - m_thirdPageFadeInStartTime ) );
		float fourthPageAlpha = Mathf.Lerp( 0.0f, 1.0f, ( m_timer - m_fourthPageFadeInStartTime ) / ( m_fourthPageFadeInEndTime - m_fourthPageFadeInStartTime ) );
		float fifthPageAlpha = Mathf.Lerp( 0.0f, 1.0f, ( m_timer - m_fifthPageFadeInStartTime ) / ( m_fifthPageFadeInEndTime - m_fifthPageFadeInStartTime ) );

		// cross fade between first and second pages
		firstPageAlpha -= secondPageAlpha;

		// cross fade between second and third pages
		secondPageAlpha -= thirdPageAlpha;

		// cross fade between third and fourth pages
		thirdPageAlpha -= fourthPageAlpha;

		// cross fade between fourth and fifth pages
		fourthPageAlpha -= fifthPageAlpha;

		// turn on / off each page depending on whether they have an alpha above zero or not
		m_pages[ 0 ].gameObject.SetActive( firstPageAlpha > 0.0f );
		m_pages[ 1 ].gameObject.SetActive( secondPageAlpha > 0.0f );
		m_pages[ 2 ].gameObject.SetActive( thirdPageAlpha > 0.0f );
		m_pages[ 3 ].gameObject.SetActive( fourthPageAlpha > 0.0f );
		m_pages[ 4 ].gameObject.SetActive( fifthPageAlpha > 0.0f );

		// update the alpha for each page
		m_pages[ 0 ].alpha = firstPageAlpha;
		m_pages[ 1 ].alpha = secondPageAlpha;
		m_pages[ 2 ].alpha = thirdPageAlpha;
		m_pages[ 3 ].alpha = fourthPageAlpha;
		m_pages[ 4 ].alpha = fifthPageAlpha;

		// if the alpha for the last page is not zero then stop playing the music
		if ( m_introMusicMightBePlaying )
		{
			if ( m_pages[ 4 ].alpha > 0.0f )
			{
				MusicController.m_instance.ChangeToTrack( MusicController.Track.None );

				m_introMusicMightBePlaying = false;
			}
		}
	}
}
