
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManualSceneTransitionController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public GameObject m_sceneTransitionGameObject;
	public float m_fadeOutDuration;
	public bool m_loadNextScene;
	public string m_nextSceneName;

	// private stuff we don't want the editor to see
	private bool m_fadeOut;
	private float m_timer;
	private Image m_image;

	// this is called by unity before start
	private void Awake()
	{
		// get to the image component of this game object
		m_image = m_sceneTransitionGameObject.GetComponent<Image>();

		// we don't fade out until a script says to
		m_fadeOut = false;
	}

	// this is called by unity every frame
	private void Update()
	{
		if ( m_fadeOut )
		{
			// increment timer by the number of seconds that has passed by so far
			m_timer += Time.deltaTime;

			// calculate the alpha of the fader image to fade out the scene
			float alpha = 0.0f;

			alpha = Mathf.Clamp( m_timer / m_fadeOutDuration, 0.0f, 1.0f );

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
			if ( m_timer > m_fadeOutDuration )
			{
				// yep - go ahead and deactivate this component to save cpu cycles
				this.enabled = false;

				// load the next scene
				if ( m_loadNextScene )
				{
					SceneManager.LoadScene( m_nextSceneName );
				}
			}
		}
	}

	public void BeginTransition()
	{
		// start the fade out
		m_fadeOut = true;

		// reset timer to 0
		m_timer = 0.0f;
	}
}
