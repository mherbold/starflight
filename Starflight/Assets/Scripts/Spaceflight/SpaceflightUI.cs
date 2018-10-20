
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceflightUI : MonoBehaviour
{
	// text elements
	public TextMeshProUGUI m_messages;
	public TextMeshProUGUI m_currentOfficer;
	public TextMeshProUGUI m_coordinates;

	// the player camera
	public Camera m_playerCamera;

	// the map object
	public RawImage m_map;

	// the countdown text
	public TextMeshProUGUI m_countdown;

	// our countdown text animation timer
	float m_countdownTimer;

	// set to true to run the fade sequence
	bool m_isFading;

	// our map fade timer
	float m_fadeTimer;

	// how long to fade the map
	float m_fadeDuration;

	// the original and target fade
	float m_originalFadeAmount;
	float m_targetFadeAmount;

	// whether or not we are currently animating the countdown text
	bool m_animatingCountdownText;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
		// we are not currently animating the countdown text
		m_animatingCountdownText = false;

		// force the canvas to update (so rectTransform is updated and correct)
		Canvas.ForceUpdateCanvases();

		// get the current map size (in pixels)
		var scaleFactor = m_map.canvas.scaleFactor;

		var mapSize = new Vector2( m_map.rectTransform.rect.width, m_map.rectTransform.rect.height ) * m_map.canvas.scaleFactor;

		// create a new render texture
		var renderTexture = new RenderTexture( Mathf.CeilToInt( mapSize.x ), Mathf.CeilToInt( mapSize.y ), 24, RenderTextureFormat.ARGB32 )
		{
			antiAliasing = 1,
			useMipMap = false,
			autoGenerateMips = false,
			useDynamicScale = false,
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point,
			anisoLevel = 0
		};

		// update the map to use the new render texture
		m_map.texture = renderTexture;

		// update the camera to use the new render texture
		m_playerCamera.targetTexture = renderTexture;
	}

	// unity update
	void Update()
	{
		// are we animating the countdown text?
		if ( m_animatingCountdownText )
		{
			// update the timer
			m_countdownTimer += Time.deltaTime;

			// animate the opacity and the size of the numbers
			if ( m_countdownTimer < 0.1f )
			{
				// fade in
				m_countdown.alpha = m_countdownTimer * 10.0f;
				m_countdown.fontSize = 240.0f;
			}
			else if ( m_countdownTimer < 1.0f )
			{
				// fade out and shrink
				m_countdown.alpha = 1.0f - ( m_countdownTimer - 0.1f ) / 0.9f;
				m_countdown.fontSize = 140.0f + m_countdown.alpha * 100.0f;
			}
			else
			{
				// stop animating the countdown text
				m_animatingCountdownText = false;

				// disable the countdown text object
				m_countdown.gameObject.SetActive( false );
			}
		}

		// are we fading the map?
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
				}

				var alpha = Mathf.SmoothStep( m_originalFadeAmount, m_targetFadeAmount, m_fadeTimer / m_fadeDuration );

				m_map.color = new Color( alpha, alpha, alpha );
			}
		}
	}

	// call this to fade in or out the map
	public void FadeMap( float targetFadeAmount, float fadeDuration )
	{
		// do we want to set it instantly?
		if ( fadeDuration == 0.0f )
		{
			// yes - make it so
			m_map.color = new Color( targetFadeAmount, targetFadeAmount, targetFadeAmount );
		}
		else if ( ( ( m_isFading == false ) && ( targetFadeAmount != m_map.color.r ) ) || ( targetFadeAmount != m_targetFadeAmount ) )
		{
			// no - set up a smooth fade transition
			m_isFading = true;
			m_fadeTimer = 0.0f;
			m_fadeDuration = fadeDuration;
			m_originalFadeAmount = m_map.color.r;
			m_targetFadeAmount = targetFadeAmount;
		}
	}

	// call this to get the current map fade amount
	public float GetCurrentMapFadeAmount()
	{
		return m_map.color.r;
	}

	// call this to change the current officer text
	public void ChangeOfficerText( string newOfficer )
	{
		m_currentOfficer.text = newOfficer;
	}

	// call this to change the message text
	public void ChangeMessageText( string newMessage )
	{
		m_messages.text = newMessage;
	}

	// call this to display the countdown text and animate it
	public void SetCountdownText( string newText )
	{
		// change the countdown text
		m_countdown.text = newText;

		// make it active
		m_countdown.gameObject.SetActive( true );

		// reset the timer
		m_countdownTimer = 0.0f;

		// we are now animating the countdown text
		m_animatingCountdownText = true;
	}

	// call this to update the coordinates above the map display
	public void UpdateCoordinates()
	{
		var playerData = DataController.m_instance.m_playerData;

		var gameCoordinates = Tools.WorldToGameCoordinates( playerData.m_starflight.m_hyperspaceCoordinates );

		var x = Mathf.RoundToInt( gameCoordinates.x );
		var y = Mathf.RoundToInt( gameCoordinates.z );

		m_coordinates.text = x.ToString() + "   " + y.ToString();
	}
}
