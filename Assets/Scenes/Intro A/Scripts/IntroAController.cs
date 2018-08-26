
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAController : MonoBehaviour
{
	// the two pages of the intro
	public CanvasGroup[] m_pages;

	// first page fade in / out times
	public float m_firstPageFadeInStartTime;
	public float m_firstPageFadeInEndTime;

	// second page fade in / out times
	public float m_secondPageTransitionStartTime;
	public float m_secondPageTransitionEndTime;

	// the scene transition controller
	AutomaticSceneTransitionController m_automaticSceneTransitionController;

	// the input controller
	InputManager m_inputManager;

	// our timer
	private float m_timer;

	// the constructor
	private void Awake()
	{
		// check if we loaded the persistent scene
		if ( DataController.m_instance == null )
		{
			// nope - so then do it now and tell it to skip the intro scene
			DataController.m_sceneToLoad = "Intro A";

			SceneManager.LoadScene( "Persistent" );
		}
		else
		{
			// get access to the scene transition controller
			m_automaticSceneTransitionController = GetComponent<AutomaticSceneTransitionController>();

			// get access to the input manager
			m_inputManager = GetComponent<InputManager>();
		}
	}

	// use this for initialization
	void Start()
	{
		// start playing the intro music
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Intro );

		// reset timer to 0
		m_timer = 0.0f;
	}

	// this is called by unity every frame
	private void Update()
	{
		// increment timer by the number of seconds that has passed by so far
		m_timer += Time.deltaTime;

		// if we pressed the fire button then skip ahead
		if ( m_inputManager.GetSubmitDown( false ) )
		{
			if ( m_timer < m_secondPageTransitionStartTime )
			{
				m_timer = m_secondPageTransitionStartTime;
			}
		}

		// if we are past the transition point then allow the player to skip to the fade out
		if ( m_timer >= m_secondPageTransitionEndTime )
		{
			m_automaticSceneTransitionController.m_allowSkipToFadeOut = true;
		}

		// fade the first page in
		float firstPageAlpha = Mathf.Clamp( ( m_timer - m_firstPageFadeInStartTime ) / ( m_firstPageFadeInEndTime - m_firstPageFadeInStartTime ), 0.0f, 1.0f ); // TODO: change to Mathf.Lerp

		// cross fade between our two pages
		float secondPageAlpha = Mathf.Clamp( ( m_timer - m_secondPageTransitionStartTime ) / ( m_secondPageTransitionEndTime - m_secondPageTransitionStartTime ), 0.0f, 1.0f ); // TODO: change to Mathf.Lerp

		firstPageAlpha = Mathf.Clamp( firstPageAlpha - secondPageAlpha, 0.0f, 1.0f );

		// update the alpha of the first page (turn it off if it is 0)
		if ( firstPageAlpha == 0.0f )
		{
			m_pages[ 0 ].gameObject.SetActive( false );
		}
		else
		{
			m_pages[ 0 ].gameObject.SetActive( true );

			m_pages[ 0 ].alpha = firstPageAlpha;
		}

		// update the alpha of the second page (turn it off if it is 0)
		if ( secondPageAlpha == 0.0f )
		{
			m_pages[ 1 ].gameObject.SetActive( false );
		}
		else
		{
			m_pages[ 1 ].gameObject.SetActive( true );

			m_pages[ 1 ].alpha = secondPageAlpha;
		}
	}
}
