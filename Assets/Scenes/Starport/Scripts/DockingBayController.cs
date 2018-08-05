
using UnityEngine;

public class DockingBayController : DoorController
{
	// public stuff we want to set using the editor
	public Renderer m_astronautRenderer;

	// private stuff we don't want the editor to see
	private bool m_isTransporting;
	private float m_transportTimer;

	// this is called by unity once at the start of the level
	protected override void Start()
	{
		base.Start();

		// we're not transporting yet
		m_isTransporting = false;
	}

	// this is called by unity every frame
	private void Update()
	{
		if ( !m_isTransporting )
		{
			return;
		}

		// update the transport timer
		m_transportTimer += Time.deltaTime;

		// let the particle system run for only a second and a half
		if ( m_transportTimer >= 1.5f )
		{
			m_starportController.m_astronautController.m_transporterParticleSystem.Stop();
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

			m_isTransporting = false;
		}
	}

	// call this to transport the astronaut to the ship
	public override void Show()
	{
		// ok lets start transporting
		m_isTransporting = true;

		// reset the transporter timer
		m_transportTimer = 0.0f;

		// start the particle system
		m_starportController.m_astronautController.m_transporterParticleSystem.Play();

		// play the transporting sound
		m_starportController.m_basicSound.PlayOneShot( 2 );
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
