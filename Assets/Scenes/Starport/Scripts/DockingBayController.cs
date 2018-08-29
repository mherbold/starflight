
using UnityEngine;

public class DockingBayController : DoorController
{
	// the renderer for the astronaut so we can fade him out
	public Renderer m_astronautRenderer;

	// set this to true to transport the astronaut to the docking bay
	bool m_isTransporting;

	// our timer (for the transporting effect)
	float m_timer;

	// unity start
	protected override void Start()
	{
		// call the base start function
		base.Start();

		// we're not transporting yet
		m_isTransporting = false;
	}

	// unity update
	void Update()
	{
		// are we transporting the astronaut?
		if ( m_isTransporting )
		{
			// yes - update the transport timer
			m_timer += Time.deltaTime;

			// let the particle system run for only a second and a half
			if ( m_timer >= 1.5f )
			{
				m_starportController.m_astronautController.m_transporterParticleSystem.Stop();
			}

			// TODO - Fix this!  Need to write a 2 pass shader to fade the astronaut out (a-la distance fade)
			// fade out the astronaut over two and a half seconds
			if ( m_timer < 2.5f )
			{
				UpdateOpacity( 1.0f - ( m_timer / 2.5f ) );
			}

			// give the particles time to completely fade out
			if ( m_timer >= 4.0f )
			{
				// force the astronaut to be completely transparent
				UpdateOpacity( 0.0f );

				// we are no longer transporting
				m_isTransporting = false;

				// get to the player data
				PlayerData playerData = DataController.m_instance.m_playerData;

				// update the player location
				playerData.m_starflight.m_location = Starflight.Location.DockingBay;

				// save the player data
				DataController.m_instance.SavePlayerData();

				// start fading out and switch to the spaceflight scene
				SceneFadeController.m_instance.FadeOut( "Spaceflight" );
			}
		}
	}

	// call this to transport the astronaut to the ship
	public override void Show()
	{
		// ok lets start transporting
		m_isTransporting = true;

		// reset the transporter timer
		m_timer = 0.0f;

		// start the particle system
		m_starportController.m_astronautController.m_transporterParticleSystem.Play();

		// play the transporting sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Transport );
	}

	// this updates the opacity of the astronaut
	void UpdateOpacity( float opacity )
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
