
using UnityEngine;

public class DockingBayController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public ParticleSystem m_transporterParticleSystem;
	public Renderer m_astronautRenderer;

	// private stuff we don't want the editor to see
	private bool m_transporting;
	private float m_transportTimer;

	// this is called by unity before start
	private void Awake()
	{
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		m_transporting = false;
	}

	// this is called by unity every frame
	private void Update()
	{
		if ( m_transporting )
		{
			// update the transport timer
			m_transportTimer += Time.deltaTime;

			// let the particle system run for only a second and a half
			if ( m_transportTimer >= 1.5f )
			{
				m_transporterParticleSystem.Stop();
			}

			// fade out the astronaut over two and a half seconds
			if ( m_transportTimer < 2.5f )
			{
				UpdateOpacity( 1.0f - ( m_transportTimer / 2.5f ) );
			}

			// give the particles time to completely fade out
			if ( m_transportTimer >= 4.0f )
			{
				UpdateOpacity( 0.0f );

				m_transporting = false;
			}
		}
	}

	// call this to try and transport to the docking bay
	public void Transport()
	{
		// ok lets start transporting
		m_transporting = true;

		// reset the transporter timer
		m_transportTimer = 0.0f;

		// start the particle system
		m_transporterParticleSystem.Play();

		// play a ui sound
		GetComponent<UISoundController>().Play( UISoundController.UISound.Transporter );
	}

	// this updates the opacity of the astronaut
	private void UpdateOpacity( float opacity )
	{
		// go through all the materials on the astronaut
		for ( int i = 0; i < m_astronautRenderer.materials.Length; i++ )
		{
			// get the material
			Material material = m_astronautRenderer.materials[ i ];

			// get the current color
			Color color = material.color;

			// update the material color with the new opacity
			material.color = new Color( color.r, color.g, color.b, opacity );
		}
	}
}
