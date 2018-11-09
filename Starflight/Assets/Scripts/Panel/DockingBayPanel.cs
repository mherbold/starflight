
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

	// the materials to switch to when transporting
	public Material[] m_fadeAstronautMaterials;

	// the transporter particle system
	public ParticleSystem m_transporterParticleSystem;

	// the starport controller
	public AstronautController m_astronautController;

	// the time to run the transporter effect particles and start fading out the astronaut
	public float m_fadeStartTime;

	// how long to run the fade
	public float m_fadeDuration;

	// the time to switch to the spaceflight scene
	public float m_sceneSwitchTime;

	// set this to true to transport the astronaut to the docking bay
	bool m_isTransporting;

	// our timer (for the transporting effect)
	float m_timer;

	// call this to transport the astronaut to the ship
	public override bool Open()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// let's start the pre-flight check
		var preflightCheckPassed = true;
		m_errorText.text = "";

		// do we have all of the crew positions assigned?
		for ( var role = PD_CrewAssignment.Role.First; role < PD_CrewAssignment.Role.Length; role++ )
		{
			if ( !playerData.m_crewAssignment.IsAssigned( role ) )
			{
				m_errorText.text += "\u2022 Report to Crew Assignment.\n";
				preflightCheckPassed = false;
				break;
			}

			var personnelFile = playerData.m_crewAssignment.GetPersonnelFile( role );

			if ( personnelFile.m_vitality == 0 )
			{
				m_errorText.text += "\u2022 At least one of your crew members is dead.\n";
				preflightCheckPassed = false;
				break;
			}
		}

		// did we name the ship?
		if ( playerData.m_playerShip.m_name.Length == 0 )
		{
			m_errorText.text += "\u2022 Report to Ship Configuration to christen ship.\n";
			preflightCheckPassed = false;
		}

		// does the ship have engines?
		if ( playerData.m_playerShip.m_enginesClass == 0 )
		{
			m_errorText.text += "\u2022 Report to Ship Configuration to purchase engines.\n";
			preflightCheckPassed = false;
		}

		// do we have fuel?
		if ( playerData.m_playerShip.m_elementStorage.Find( 5 ) == null )
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

			// play the transporting sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Transport );

			// activate this game object so that update is called
			gameObject.SetActive( true );

			// get a copy of the materials array
			var astronautMaterials = m_astronautRenderer.materials;

			// go through all the materials on the astronaut
			for ( var i = 0; i < astronautMaterials.Length; i++ )
			{
				// switch the shader to the two pass standard shader
				astronautMaterials[ i ] = m_fadeAstronautMaterials[ i ];
			}

			// update the renderer with the new list of materials
			m_astronautRenderer.materials = astronautMaterials;

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
		m_astronautController.PanelWasClosed();
	}

	// unity update
	void Update()
	{
		// are we transporting the astronaut?
		if ( m_isTransporting )
		{
			// yes - update the transport timer
			m_timer += Time.deltaTime;

			// is it time to switch the scene?
			if ( m_timer >= m_sceneSwitchTime )
			{
				// yes - force the astronaut to be completely transparent
				UpdateOpacity( 0.0f );

				// we are no longer transporting
				m_isTransporting = false;

				// get to the player data
				var playerData = DataController.m_instance.m_playerData;

				// update the player location
				playerData.m_general.m_location = PD_General.Location.DockingBay;

				// save the player data
				DataController.m_instance.SaveActiveGame();

				// start fading out and switch to the spaceflight scene
				SceneFadeController.m_instance.FadeOut( "Spaceflight" );
			}
			else if ( m_timer >= ( m_fadeStartTime + m_fadeDuration ) )
			{
				// is the particle system still playing?
				if ( m_transporterParticleSystem.isPlaying )
				{
					// yes - stop it
					m_transporterParticleSystem.Stop();
				}
			}
			else if ( m_timer >= m_fadeStartTime )
			{
				// calculate the astronaut opacity
				var opacity = Mathf.Lerp( 1.0f, 0.0f, ( m_timer - m_fadeStartTime ) / m_fadeDuration );

				// update the astronaut opacity
				UpdateOpacity( opacity );

				// has the particle system been started?
				if ( !m_transporterParticleSystem.isPlaying )
				{
					// no - start the particle system
					m_transporterParticleSystem.Play();
				}
			}
		}
	}

	// this updates the opacity of the astronaut
	void UpdateOpacity( float opacity )
	{
		// go through all the materials on the astronaut
		for ( var i = 0; i < m_astronautRenderer.materials.Length; i++ )
		{
			// get the material
			var material = m_astronautRenderer.materials[ i ];

			// update the material opacity
			Tools.SetOpacity( material, opacity );
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
