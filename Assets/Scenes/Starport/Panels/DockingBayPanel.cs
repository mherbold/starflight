
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DockingBayPanel : Panel
{
	// the exit button
	public Button m_exitButton;

	// the error messages
	public TextMeshProUGUI m_errorText;

	// the renderer for the astronaut so we can fade him out
	public Renderer m_astronautRenderer;

	// the transporter particle system
	public ParticleSystem m_transporterParticleSystem;

	// the starport controller
	public StarportController m_starportController;

	// set this to true to transport the astronaut to the docking bay
	bool m_isTransporting;

	// our timer (for the transporting effect)
	float m_timer;

	// call this to transport the astronaut to the ship
	public override bool Open()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// let's start the pre-flight check
		bool preflightCheckPassed = true;
		m_errorText.text = "";

		// do we have all of the crew positions assigned?
		for ( CrewAssignment.Role role = CrewAssignment.Role.First; role < CrewAssignment.Role.Length; role++ )
		{
			if ( !playerData.m_crewAssignment.IsAssigned( role ) )
			{
				m_errorText.text += "\u2022 Report to Crew Assignment.\n";
				preflightCheckPassed = false;
				break;
			}

			Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( role );

			if ( personnelFile.m_vitality == 0 )
			{
				m_errorText.text += "\u2022 At least one of your crew members is dead.\n";
				preflightCheckPassed = false;
				break;
			}
		}

		// did we name the ship?
		if ( playerData.m_ship.m_name.Length == 0 )
		{
			m_errorText.text += "\u2022 Report to Ship Configuration to christen ship.\n";
			preflightCheckPassed = false;
		}

		// does the ship have engines?
		if ( playerData.m_ship.m_enginesClass == 0 )
		{
			m_errorText.text += "\u2022 Report to Ship Configuration to purchase engines.\n";
			preflightCheckPassed = false;
		}

		// do we have fuel?
		if ( playerData.m_ship.m_elementStorage.Find( 5 ) == null )
		{
			m_errorText.text += "\u2022 Report to Trade Depot to purchase fuel.\n";
			preflightCheckPassed = false;
		}

		// did we pass the pre-flight checks?
		if ( preflightCheckPassed )
		{
			// yes - ok lets start transporting
			m_isTransporting = true;

			// reset the transporter timer
			m_timer = 0.0f;

			// start the particle system
			m_transporterParticleSystem.Play();

			// play the transporting sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Transport );

			// activate this game object so that update is called
			gameObject.SetActive( true );

			// return false because we did not open the panel
			return false;
		}
		else
		{
			// base panel open
			base.Open();

			// automatically select the "exit" button for the player
			m_exitButton.Select();

			// panel was opened
			return true;
		}
	}

	// panel closed
	public override void Closed()
	{
		// base panel closed
		base.Closed();

		// let the starport controller know
		m_starportController.PanelWasClosed();
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
				m_transporterParticleSystem.Stop();
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
				DataController.m_instance.SaveActiveGame();

				// start fading out and switch to the spaceflight scene
				SceneFadeController.m_instance.FadeOut( "Spaceflight" );
			}
		}
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

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		// close this panel
		PanelController.m_instance.Close();
	}

	// call this to check if the astronaut is transporting
	public bool IsTransporting()
	{
		return m_isTransporting;
	}
}
