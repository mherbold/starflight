
using UnityEngine;

public class SpaceWarp : MonoBehaviour
{
	// the warp shader we want to use
	public Shader m_shader;

	// how fast to enter and exit warp
	public float m_enterWarpTransitionTime = 1.0f;
	public float m_exitWarpTransitionTime = 1.0f;

	// how strong the warp effect should be
	public float m_warpStrength;

	// material we will create
	Material m_material;

	// true if we are currently entering the space warp
	bool m_isEnteringWarp;

	// true if we are currently exiting the space warp
	bool m_isExitingWarp;

	// warp effect timer
	float m_timer;

	// unity awke
	void Awake()
	{
		m_material = new Material( m_shader );
	}

	// unity start
	void Start()
	{
		m_timer = 0.0f;
	}

	// unity update
	void Update()
	{
		if ( m_isEnteringWarp )
		{
			m_timer += Time.deltaTime;

			float warpStrength = Mathf.Lerp( 0.0f, m_warpStrength, ( m_timer / m_enterWarpTransitionTime ) );

			if ( m_timer >= m_enterWarpTransitionTime )
			{
				m_isEnteringWarp = false;
			}

			m_material.SetFloat( "_WarpStrength", warpStrength );
		}
		else if ( m_isExitingWarp )
		{
			m_timer += Time.deltaTime;

			float warpStrength = Mathf.Lerp( m_warpStrength, 0.0f, ( m_timer / m_exitWarpTransitionTime ) );

			if ( m_timer >= m_exitWarpTransitionTime )
			{
				m_isExitingWarp = false;
			}

			m_material.SetFloat( "_WarpStrength", warpStrength );
		}
	}

	// unity on render image
	void OnRenderImage( RenderTexture source, RenderTexture destination )
	{
		Graphics.Blit( source, destination, m_material );
	}

	// call this to start adding the warp effect
	public void EnterWarp()
	{
		m_isEnteringWarp = true;
		m_timer = 0.0f;
	}

	// call this to start exiting the warp effect
	public void ExitWarp()
	{
		m_isExitingWarp = true;
		m_timer = 0.0f;
	}
}
