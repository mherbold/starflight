
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public GameObject m_sceneTransitionGameObject;
	public bool m_fadeIn;
	public float m_fadeInStartTime;
	public float m_fadeInEndTime;
	public bool m_fadeOut;
	public float m_fadeOutStartTime;
	public float m_fadeOutEndTime;
	public bool m_allowSkipToFadeOut;
	public bool m_loadNextScene;
	public string m_nextSceneName;

	// private stuff we don't want the editor to see
	private InputManager m_inputManager;
	private float m_timer;
	private Image m_image;

	// this is called by unity before start
	private void Awake()
	{
		// get access to the input manager
		m_inputManager = GetComponent<InputManager>();

		// get to the image component of this game object
		m_image = m_sceneTransitionGameObject.GetComponent<Image>();

		// reset timer to 0
		m_timer = 0.0f;
	}

	// this is called by unity every frame
	void Update()
	{
		// increment timer by the number of seconds that has passed by so far
		m_timer += Time.deltaTime;

		// if we pressed the fire button then skip ahead
		if ( m_allowSkipToFadeOut )
		{
			if ( m_inputManager.GetSubmitDown( false ) )
			{
				if ( m_timer < m_fadeOutStartTime )
				{
					m_timer = m_fadeOutStartTime;
				}
			}
		}

		// calculate the alpha of the fader image to fade in / out the scene
		float alpha = 0.0f;

		if ( m_fadeIn )
		{
			if ( m_timer < m_fadeInEndTime )
			{
				alpha = 1.0f - Mathf.Clamp( ( m_timer - m_fadeInStartTime ) / ( m_fadeInEndTime - m_fadeInStartTime ), 0.0f, 1.0f );
			}
		}

		if ( m_fadeOut )
		{
			if ( m_timer >= m_fadeOutStartTime )
			{
				alpha = Mathf.Clamp( ( m_timer - m_fadeOutStartTime ) / ( m_fadeOutEndTime - m_fadeOutStartTime ), 0.0f, 1.0f );
			}
		}

		// update the alpha of the image (or just turn off the image if it is 0)
		if ( alpha == 0.0f )
		{
			m_image.enabled = false;
		}
		else
		{
			m_image.enabled = true;

			Color color = m_image.color;
			color.a = alpha;
			m_image.color = color;
		}

		// if we have faded the scene out then load the next scene
		if ( m_timer > m_fadeOutEndTime )
		{
			if ( m_loadNextScene )
			{
				SceneManager.LoadScene( m_nextSceneName );
			}
		}
	}
}
