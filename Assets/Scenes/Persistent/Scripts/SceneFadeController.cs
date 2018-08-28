
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFadeController : MonoBehaviour
{
	// static reference to this instance
	public static SceneFadeController m_instance;

	// how slowly do we want to fade between scenes?
	public float m_fadeDuration = 1.0f;

	// the fade image in this scene's ui
	Image m_fadeImage;

	// the name of the next scene to load after we have faded the scene out
	string m_nextSceneName;

	// whether or not to be fading in
	bool m_fadeIn;

	// whether or not to be fading out
	bool m_fadeOut;

	// our timer
	float m_timer;

	// unity awake
	void Awake()
	{
		// remember this instance to this
		m_instance = this;
	}

	// unity start
	void Start()
	{
		m_fadeIn = false;
		m_fadeOut = false;
	}

	// unity update
	void Update()
	{
		// are we fading in?
		if ( m_fadeIn )
		{
			// increment timer by the number of seconds that has passed by so far
			m_timer += Time.deltaTime;

			// are we done fading in?
			if ( m_timer >= m_fadeDuration )
			{
				// yes - just turn off the scene fade object
				m_fadeImage.gameObject.SetActive( false );

				// we are done
				m_fadeIn = false;
			}
			else
			{
				// no - calculate the new alpha value
				float alpha = Mathf.Lerp( 1.0f, 0.0f, m_timer / m_fadeDuration );

				// update the fade image color
				m_fadeImage.color = new Color( 0.0f, 0.0f, 0.0f, alpha );
			}
		}

		if ( m_fadeOut )
		{
			// increment timer by the number of seconds that has passed by so far
			m_timer += Time.deltaTime;

			// are we done fading out?
			if ( m_timer >= m_fadeDuration )
			{
				// we are done
				m_fadeOut = false;

				// load the next scene
				SceneManager.LoadScene( m_nextSceneName );
			}
			else
			{
				// no - calculate the new alpha value
				float alpha = Mathf.Lerp( 0.0f, 1.0f, m_timer / m_fadeDuration );

				// update the fade image color
				m_fadeImage.color = new Color( 0.0f, 0.0f, 0.0f, alpha );
			}
		}
	}

	// grab the fade image and turn it on
	void FindSceneFadeGameObjectAndImage()
	{
		// find the scene fade game object in the current scene
		GameObject sceneFadeGameObject = GameObject.FindWithTag( "Scene Fade" ).transform.Find( "Image" ).gameObject;

		// turn on the object
		sceneFadeGameObject.SetActive( true );

		// grab the scene fade image from that game object
		m_fadeImage = sceneFadeGameObject.GetComponent<Image>();
	}

	// call this to start fading the scene in
	public void FadeIn()
	{
		// find the scene fade object and turn it on
		FindSceneFadeGameObjectAndImage();

		// we are now fading in
		m_fadeIn = true;

		// reset our timer to zero
		m_timer = 0.0f;
	}

	// call this to start fading the scene out
	public void FadeOut( string nextSceneName )
	{
		// if we are already fading out don't do anything
		if ( !m_fadeOut )
		{
			// find the scene fade object and turn it on
			FindSceneFadeGameObjectAndImage();

			// we are now fading out
			m_fadeOut = true;

			// reset our timer to zero
			m_timer = 0.0f;

			// remember the name of the next scene
			m_nextSceneName = nextSceneName;
		}
	}
}
