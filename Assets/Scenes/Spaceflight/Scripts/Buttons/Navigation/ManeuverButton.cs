
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
		if ( playerData.m_starflight.m_location == Starflight.Location.DockingBay )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			m_spaceflightController.m_spaceflightUI.ChangeMessageText( "Standing by to launch." );

			m_spaceflightController.m_buttonController.UpdateButtonSprites();

			return false;
		}

		// if we aren't in hyperspace then switch to the star system
		if ( playerData.m_starflight.m_location != Starflight.Location.Hyperspace )
		{
			playerData.m_starflight.m_location = Starflight.Location.StarSystem;
		}

		// switch to the correct mode
		m_spaceflightController.SwitchMode();

		// reset the current speed
		playerData.m_starflight.m_currentSpeed = 0.0f;

		return true;
	}

	public override bool Update()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// check if we are currently frozen
		if ( m_spaceflightController.m_player.IsFrozen() )
		{
			// yep - kill the momentum
			playerData.m_starflight.m_currentSpeed = 0.0f;

			// don't do anything more
			return true;
		}

		// check if we want to stop maneuvering
		if ( InputController.m_instance.SubmitWasPressed() )
		{
			// m_spaceflightController.m_buttonController.UpdateButtons();
		}
		else
		{
			// get the controller stick position
			float x = InputController.m_instance.m_x;
			float z = InputController.m_instance.m_y;

			// create our 3d move vector from the controller position
			Vector3 moveVector = new Vector3( x, 0.0f, z );

			// check if the move vector will actually move the ship (controller is not centered)
			if ( moveVector.magnitude > 0.5f )
			{
				// accelerate
				float maximumShipSpeed = ( playerData.m_starflight.m_location == Starflight.Location.Hyperspace ) ? m_spaceflightController.m_maximumShipSpeedHyperspace : m_spaceflightController.m_maximumShipSpeedStarSystem;

				playerData.m_starflight.m_currentSpeed = Mathf.Lerp( playerData.m_starflight.m_currentSpeed, maximumShipSpeed, Time.deltaTime / m_spaceflightController.m_timeToReachMaximumShipSpeed );

				// normalize the move vector to a length of 1.0
				moveVector.Normalize();

				// update the direction
				playerData.m_starflight.m_currentDirection = Vector3.Slerp( playerData.m_starflight.m_currentDirection, moveVector, Time.deltaTime * 2.0f );
			}
			else
			{
				// slow the ship to a stop
				playerData.m_starflight.m_currentSpeed = Mathf.Lerp( playerData.m_starflight.m_currentSpeed, 0.0f, Time.deltaTime / m_spaceflightController.m_timeToStop );
			}

			// check if the ship is moving
			if ( playerData.m_starflight.m_currentSpeed >= 0.001f )
			{
				// calculate the new position of the player
				Vector3 newPosition = m_spaceflightController.m_player.transform.position + (Vector3) playerData.m_starflight.m_currentDirection * playerData.m_starflight.m_currentSpeed;

				// make sure the ship stays on the zero plane
				newPosition.y = 0.0f;

				// update the player position
				m_spaceflightController.UpdatePlayerPosition( newPosition );

				// calculate the new rotation of the player
				Quaternion newRotation = Quaternion.LookRotation( playerData.m_starflight.m_currentDirection, Vector3.up );

				// update the player rotation
				m_spaceflightController.m_player.SetRotation( newRotation );

				// rotate the skybox accordingly
				float multiplier = ( playerData.m_starflight.m_location == Starflight.Location.Hyperspace ) ? 2.0f : 1.0f;
				m_spaceflightController.m_player.RotateSkybox( playerData.m_starflight.m_currentDirection, playerData.m_starflight.m_currentSpeed * Time.deltaTime * multiplier );

				// update the map coordinates
				m_spaceflightController.m_spaceflightUI.UpdateCoordinates();

				// are we in a star system?
				if ( playerData.m_starflight.m_location == Starflight.Location.StarSystem )
				{
					// yes - did we just leave it?
					if ( Vector3.Magnitude( newPosition ) >= 8192.0f )
					{
						// yes - transition to hyperspace!
						playerData.m_starflight.m_location = Starflight.Location.Hyperspace;

						// calculate and set our position in hyperspace
						Star star = gameData.m_starList[ playerData.m_starflight.m_currentStarId ];
						newPosition.Normalize();
						m_spaceflightController.UpdatePlayerPosition( star.m_worldCoordinates + newPosition * ( star.GetBreachDistance() + 16.0f ) );

						// limit the ship speed to hyperspace speed
						playerData.m_starflight.m_currentSpeed = Mathf.Min( playerData.m_starflight.m_currentSpeed, m_spaceflightController.m_maximumShipSpeedHyperspace );

						// switch modes now
						m_spaceflightController.SwitchMode();
					}
				}
			}
		}

		// returning true prevents the default spaceflight update from running
		return true;
	}
}
