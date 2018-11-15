
using UnityEngine;
using TMPro;

public class Map : MonoBehaviour
{
	// text elements
	public TextMeshProUGUI m_coordinates;

	// the player camera
	public Camera m_playerCamera;

	// the map object
	Material m_material;

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
		// force the canvas to update (so rectTransform is updated and correct)
		Canvas.ForceUpdateCanvases();

		// get the mesh renderer component
		var meshRenderer = GetComponent<MeshRenderer>();

		// make a copy of the material so it doesnt' get permanently modified
		m_material = new Material( meshRenderer.material );

		// set the material on the mesh renderer
		meshRenderer.material = m_material;

		// get the rect transform component
		var rectTransform = GetComponent<RectTransform>();

		// find the main ui gameobject with the canvas
		var uiGameObject = GameObject.Find( "UI" );

		// get the canvas component
		var canvas = uiGameObject.GetComponent<Canvas>();

		// get the current map size (in pixels)
		var scaleFactor = canvas.scaleFactor;

		var mapSize = new Vector2( rectTransform.rect.width, rectTransform.rect.height ) * canvas.scaleFactor;

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

		// update the material to use the new render texture as the albedo map
		m_material.SetTexture( "SF_AlbedoMap", renderTexture );

		// update the camera to use the new render texture
		m_playerCamera.targetTexture = renderTexture;
	}

	// unity update
	void Update()
	{
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

				m_material.SetColor( "SF_AlbedoColor", new Color( alpha, alpha, alpha ) );
			}
		}
	}

	// call this to fade in or out the map
	public void StartFade( float targetFadeAmount, float fadeDuration )
	{
		// do we want to set it instantly?
		if ( fadeDuration == 0.0f )
		{
			// yes - make it so
			m_material.SetColor( "SF_AlbedoColor", new Color( targetFadeAmount, targetFadeAmount, targetFadeAmount ) );

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
			}
		}
	}

	// call this to get the current map fade amount
	public float GetCurrentFadeAmount()
	{
		// get the current color
		var color = m_material.GetColor( "SF_AlbedoColor" );
		
		// return the current fade amount
		return color.r;
	}

	// call this to update the coordinates
	public void UpdateCoordinates()
	{
		var playerData = DataController.m_instance.m_playerData;

		var gameCoordinates = Tools.WorldToGameCoordinates( playerData.m_general.m_lastHyperspaceCoordinates );

		var x = Mathf.RoundToInt( gameCoordinates.x );
		var y = Mathf.RoundToInt( gameCoordinates.z );

		m_coordinates.text = x.ToString() + "   " + y.ToString();
	}
}
