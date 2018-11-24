
using UnityEngine;

public class SensorsButton : ShipButton
{
	// is the player selecting something to scan?
	bool m_selectingSomething;

	// the current selection index
	int m_currentSelection;

	// pace the input control
	float m_ignoreControllerTimer;

	// the display label
	public override string GetLabel()
	{
		return "Sensors";
	}

	// called by the spaceflight controller if the sensors button is pressed
	public override bool Execute()
	{
		// reset stuff
		m_selectingSomething = false;
		m_currentSelection = 0;
		m_ignoreControllerTimer = 0.0f;

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// where is the player?
		switch ( playerData.m_general.m_location )
		{
			case PD_General.Location.InOrbit:

				// show the sensor display
				m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_sensorsDisplay );

				// get the planet we are currently orbiting about
				var planet = gameData.m_planetList[ playerData.m_general.m_currentPlanetId ];

				// start the scan
				m_spaceflightController.m_displayController.m_sensorsDisplay.StartScanning( SensorsDisplay.ScanType.Planet, 18, planet.m_mass, planet.m_bioDensity, planet.m_mineralDensity );

				return true;

			case PD_General.Location.DockingBay:
			case PD_General.Location.JustLaunched:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;

			case PD_General.Location.Encounter:

				// let the player select something to scan
				m_selectingSomething = true;

				// show the sensor display
				m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_sensorsDisplay );

				// show the scanner selection ring
				m_spaceflightController.m_scanner.Show();

				return true;

			default:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_messages.ChangeText( "<color=red>Not yet implemented.</color>" );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;
		}

		return false;
	}

	// called by the display controller each frame while this display is active
	public override bool Update()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// is the player selecting something to scan?
		if ( m_selectingSomething )
		{
			// yes - this will be -1 to go to the previous selection, or +1 to go to the next selection
			var selectChangeDirection = 0;

			// update the ignore controller timer
			m_ignoreControllerTimer = Mathf.Max( 0.0f, m_ignoreControllerTimer - Time.deltaTime );

			// let the player change the selection?
			if ( m_ignoreControllerTimer == 0.0f )
			{
				// yes - check the input controller
				if ( InputController.m_instance.m_south || InputController.m_instance.m_east )
				{
					m_ignoreControllerTimer = 0.3f;

					selectChangeDirection = 1;
				}
				else if ( InputController.m_instance.m_north || InputController.m_instance.m_west )
				{
					m_ignoreControllerTimer = 0.3f;

					selectChangeDirection = -1;
				}
			}

			// does the player want to change the selection?
			if ( selectChangeDirection != 0 )
			{
				// yes - play the click sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Click );
			}

			// are we done making a selection? (player hits enter)
			if ( InputController.m_instance.m_submit )
			{
				InputController.m_instance.Debounce();

				// yes - we are no longer selecting something
				m_selectingSomething = false;

				// hide the scanner selection ring
				m_spaceflightController.m_scanner.Hide();
			}

			// where is the player?
			switch ( playerData.m_general.m_location )
			{
				case PD_General.Location.Encounter:

					// update the current selection (if it needs to be updated)
					if ( selectChangeDirection != 0 )
					{
						m_currentSelection = m_spaceflightController.m_encounter.UpdateScannerSelection( m_currentSelection, selectChangeDirection );
					}

					// make the scanner selection ring follow the current selection
					var scannerPosition = m_spaceflightController.m_encounter.GetScannerPosition( m_currentSelection );

					// update the position of the scanner selection ring
					m_spaceflightController.m_scanner.UpdatePosition( scannerPosition );

					// did player make a selection?
					if ( !m_selectingSomething )
					{
						// yes - start the scanning cinematic
						m_spaceflightController.m_encounter.StartScanning( m_currentSelection );
					}

					break;
			}
		}

		return true;
	}
}
