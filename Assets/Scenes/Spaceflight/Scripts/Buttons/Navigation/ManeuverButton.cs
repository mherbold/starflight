
using UnityEngine;

public class ManeuverButton : ShipButton
{
	public override string GetLabel()
	{
		return "Maneuver";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// are we still in the docking bay?
		switch ( playerData.m_starflight.m_location )
		{
			case Starflight.Location.DockingBay:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_spaceflightUI.ChangeMessageText( "Standing by to launch." );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				return false;

			case Starflight.Location.JustLaunched:

				// yes - switch to the star system location
				playerData.m_starflight.m_location = Starflight.Location.StarSystem;

				// switch to the correct mode
				m_spaceflightController.SwitchMode();

				// show the system display
				m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_systemDisplay );

				break;

			case Starflight.Location.StarSystem:

				// show the system display
				m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_systemDisplay );

				break;

			case Starflight.Location.Hyperspace:

				break;
		}

		// reset the current speed
		playerData.m_starflight.m_currentSpeed = 0.0f;

		return true;
	}

	public override bool Update()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// check if we want to stop maneuvering
		if ( InputController.m_instance.SubmitWasPressed() )
		{
			// turn off the maneuver function
			m_spaceflightController.m_buttonController.DeactivateButton();

			// turn off the engines
			m_spaceflightController.m_player.TurnOffEngines();
		}
		else
		{
			// get the controller stick position
			float x = InputController.m_instance.m_x;
			float z = InputController.m_instance.m_y;

			// create our 3d move vector from the controller position
			Vector3 moveVector = new Vector3( x, 0.0f, z );

			// check if the move vector will actually move the ship (that the controller is not centered)
			if ( moveVector.magnitude > 0.5f )
			{
				// normalize the move vector to a length of 1.0
				moveVector.Normalize();

				// update the direction
				playerData.m_starflight.m_currentDirection = Vector3.Slerp( playerData.m_starflight.m_currentDirection, moveVector, Time.deltaTime * 2.0f );

				// turn the engines on
				m_spaceflightController.m_player.TurnOnEngines();
			}
			else
			{
				// turn the engines off
				m_spaceflightController.m_player.TurnOffEngines();
			}
		}

		// are we in a star system?
		if ( playerData.m_starflight.m_location == Starflight.Location.StarSystem )
		{
			// yes - did we just leave it?
			if ( Vector3.Magnitude( playerData.m_starflight.m_systemCoordinates ) >= 8192.0f )
			{
				// yes - transition to hyperspace!
				playerData.m_starflight.m_location = Starflight.Location.Hyperspace;

				// get to the current star
				Star star = gameData.m_starList[ playerData.m_starflight.m_currentStarId ];

				// calculate the position of the ship in hyperspace
				Vector3 newPosition = playerData.m_starflight.m_systemCoordinates;

				newPosition.Normalize();

				newPosition *= star.GetBreachDistance() + 16.0f;
				newPosition += star.m_worldCoordinates;

				// update the player position
				m_spaceflightController.m_player.transform.position = newPosition;

				// save the player's position in the player data
				playerData.m_starflight.m_hyperspaceCoordinates = newPosition;

				// limit the ship speed to hyperspace speed
				playerData.m_starflight.m_currentSpeed = Mathf.Min( playerData.m_starflight.m_currentSpeed, m_spaceflightController.m_maximumShipSpeedHyperspace );

				// switch modes now
				m_spaceflightController.SwitchMode();
			}
		}

		// returning true prevents the default spaceflight update from running
		return true;
	}
}
