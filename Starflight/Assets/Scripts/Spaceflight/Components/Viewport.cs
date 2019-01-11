
using UnityEngine;
using TMPro;

public class Viewport : MonoBehaviour
{
	// text elements
	public TextMeshProUGUI m_label;

	// the fade mesh renderer
	public MeshRenderer m_fadeMeshRenderer;

	// the ui canvas
	public Canvas m_canvas;

	// the player camera
	public Camera m_playerCamera;

	// the cameras that need to use the render texture
	public Camera[] m_cameras;

	// the fade material
	Material m_fadeMaterial;

	// set to true to run the fade sequence
	bool m_isFading;

	// our fade timer
	float m_fadeTimer;

	// how long to fade
	float m_fadeDuration;

	// the original and target fade
	float m_originalFadeAmount;
	float m_targetFadeAmount;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
		// get the material from the fade mesh renderer
		m_fadeMaterial = m_fadeMeshRenderer.material;

		// turn off the fade
		m_fadeMeshRenderer.gameObject.SetActive( false );
	}

	// unity update
	void Update()
	{
		// get the canvas width in pixels
		var canvasWidth = m_canvas.pixelRect.width;

		// calculate the new rect for a viewport that fits exactly in the hole
		var x = 12.0f / canvasWidth * m_canvas.scaleFactor;
		var y = 12.0f / 1080.0f;
		var w = 1.0f - ( ( 12.0f + 512.0f ) / canvasWidth * m_canvas.scaleFactor );
		var h = 1.0f - ( ( 12.0f + 32.0f + 12.0f + 12.0f ) / 1080.0f );

		var newCameraRect = new Rect( x, y, w, h );

		// update the rect on the player camera
		m_playerCamera.rect = newCameraRect;

		// update all of the encounter cameras
		foreach ( var camera in m_cameras )
		{
			camera.rect = newCameraRect;
		}

		// are we fading the viewport?
		if ( m_isFading )
		{
			// make sure the pop up dialog is not visible
			if ( !PopupController.m_instance.IsActive() )
			{
				// update the timer
				m_fadeTimer += Time.deltaTime;

				// are we done?
				if ( m_fadeTimer >= m_fadeDuration )
				{
					// yes - stop fading
					m_isFading = false;

					// turn off the fade object if necessary
					if ( m_targetFadeAmount == 1.0f )
					{
						m_fadeMeshRenderer.gameObject.SetActive( false );
					}
				}

				var alpha = m_isFading ? Mathf.SmoothStep( m_originalFadeAmount, m_targetFadeAmount, m_fadeTimer / m_fadeDuration ) : m_targetFadeAmount;

				m_fadeMaterial.SetColor( "SF_AlbedoColor", new Color( alpha, alpha, alpha ) );
			}
		}
	}

	// call this to fade in or out the viewport
	public void StartFade( float targetFadeAmount, float fadeDuration )
	{
		// do we want to set it instantly?
		if ( fadeDuration == 0.0f )
		{
			// yes - make it so
			m_fadeMaterial.SetColor( "SF_AlbedoColor", new Color( targetFadeAmount, targetFadeAmount, targetFadeAmount ) );

			// make the fade object active if necessary
			m_fadeMeshRenderer.gameObject.SetActive( targetFadeAmount < 1.0f );

			// if we were previously fading - stop it
			m_isFading = false;
		}
		else
		{
			var currentFadeAmount = GetCurrentFadeAmount();

			if ( ( ( m_isFading == false ) && ( targetFadeAmount != currentFadeAmount ) ) || ( targetFadeAmount != m_targetFadeAmount ) )
			{
				// no - set up a smooth fade transition
				m_isFading = true;
				m_fadeTimer = 0.0f;
				m_fadeDuration = fadeDuration;
				m_originalFadeAmount = currentFadeAmount;
				m_targetFadeAmount = targetFadeAmount;

				// make the fade object active
				m_fadeMeshRenderer.gameObject.SetActive( true );
			}
		}
	}

	// check whether or not the viewport is still fading (in or out)
	public bool IsFading()
	{
		return m_isFading;
	}

	// call this to get the current viewport fade amount
	public float GetCurrentFadeAmount()
	{
		// get the current color
		var color = m_fadeMaterial.GetColor( "SF_AlbedoColor" );
		
		// return the current fade amount
		return color.r;
	}

	// call this to update the label
	public void UpdateLabel( string text )
	{
		m_label.text = text;
	}

	// call this to update the coordinates
	public void UpdateCoordinates()
	{
		var playerData = DataController.m_instance.m_playerData;

		var gameCoordinates = Tools.WorldToGameCoordinates( playerData.m_general.m_lastHyperspaceCoordinates );

		var x = Mathf.RoundToInt( gameCoordinates.x );
		var y = Mathf.RoundToInt( gameCoordinates.z );

		var text = x.ToString() + "   " + y.ToString();

		UpdateLabel( text );
	}
}
