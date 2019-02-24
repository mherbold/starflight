
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Viewport : MonoBehaviour
{
	// text elements
	public TextMeshProUGUI m_label;

	// the fade image
	public Image m_fadeImage;

	// the ui canvas
	public Canvas m_canvas;

	// the camera
	public Camera m_camera;

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
	float m_currentFadeAmount;
	float m_targetFadeAmount;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
		// get the material from the fade mesh renderer
		m_fadeMaterial = m_fadeImage.material;

		// turn off the fade
		m_fadeImage.enabled = false;
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
		m_camera.rect = newCameraRect;

		// update all of the encounter cameras
		foreach ( var camera in m_cameras )
		{
			camera.rect = newCameraRect;
		}

		// are we fading the viewport?
		if ( m_isFading )
		{
			// is the game paused?
			if ( !SpaceflightController.m_instance.m_gameIsPaused )
			{
				// no - ok update the timer
				m_fadeTimer += Time.deltaTime;

				// are we done?
				if ( m_fadeTimer >= m_fadeDuration )
				{
					// yes - stop fading
					m_isFading = false;

					// turn off the fade object if necessary
					if ( m_targetFadeAmount == 1.0f )
					{
						m_fadeImage.enabled = false;
					}
				}

				// update the current fade amount
				m_currentFadeAmount = m_isFading ? Mathf.SmoothStep( m_originalFadeAmount, m_targetFadeAmount, m_fadeTimer / m_fadeDuration ) : m_targetFadeAmount;

				// update the opacity of the material
				Tools.SetOpacity( m_fadeMaterial, 1.0f - m_currentFadeAmount );
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
			m_currentFadeAmount = targetFadeAmount;

			// update the opacity of the material
			Tools.SetOpacity( m_fadeMaterial, 1.0f - m_currentFadeAmount );

			// make the fade object active if necessary
			m_fadeImage.enabled = targetFadeAmount < 1.0f;

			// if we were previously fading - stop it
			m_isFading = false;
		}
		else
		{
			if ( ( ( m_isFading == false ) && ( targetFadeAmount != m_currentFadeAmount ) ) || ( targetFadeAmount != m_targetFadeAmount ) )
			{
				// no - set up a smooth fade transition
				m_isFading = true;
				m_fadeTimer = 0.0f;
				m_fadeDuration = fadeDuration;
				m_originalFadeAmount = m_currentFadeAmount;
				m_targetFadeAmount = targetFadeAmount;

				// make the fade object active
				m_fadeImage.enabled = true;
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
		// return the current fade amount
		return m_currentFadeAmount;
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

		string text;

		if ( playerData.m_general.m_location == PD_General.Location.Disembarked )
		{
			Tools.WorldToLatLongCoordinates( playerData.m_general.m_lastDisembarkedCoordinates, out var x, out var z );

			var latitude = Mathf.RoundToInt( x );
			var longitude = Mathf.RoundToInt( z );

			if ( latitude < 0 )
			{
				text = ( -latitude ).ToString() + " W";
			}
			else
			{
				text = latitude.ToString() + " E";
			}

			text += "   ";

			if ( longitude < 0 )
			{
				text += ( -longitude ).ToString() + " S";
			}
			else
			{
				text += longitude.ToString() + " N";
			}
		}
		else
		{
			var gameCoordinates = Tools.WorldToGameCoordinates( playerData.m_general.m_lastHyperspaceCoordinates );

			var x = Mathf.RoundToInt( gameCoordinates.x );
			var y = Mathf.RoundToInt( gameCoordinates.z );

			text = x.ToString() + "   " + y.ToString();
		}

		UpdateLabel( text );
	}
}
