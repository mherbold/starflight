
using UnityEngine;

public class ManeuverButton : ShipButton
{
	// keep track of the ship's current speed
	float m_currentSpeed;

	// keep track of the ship's current direction
	Vector3 m_currentDirection;

	// keep track of the skybox rotation
	Matrix4x4 m_skyboxRotation = Matrix4x4.identity;

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

		// reset the current speed and direction
		m_currentSpeed = 0.0f;
		m_currentDirection = Vector3.forward;

		// update the skybox rotation on the material
		Material skyboxMaterial = RenderSettings.skybox;
		skyboxMaterial.SetMatrix( "_Rotation", m_skyboxRotation );

		return true;
	}

	public override bool Update()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

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

				m_currentSpeed = Mathf.Lerp( m_currentSpeed, maximumShipSpeed, Time.deltaTime / m_spaceflightController.m_timeToReachMaximumShipSpeed );

				// normalize the move vector to a length of 1.0
				moveVector.Normalize();

				// update the direction
				m_currentDirection = Vector3.Slerp( m_currentDirection, moveVector, Time.deltaTime * 2.0f );
			}
			else
			{
				// slow the ship to a stop
				m_currentSpeed = Mathf.Lerp( m_currentSpeed, 0.0f, Time.deltaTime / m_spaceflightController.m_timeToStop );
			}

			// calculate the new position of the player
			Vector3 newPosition = m_spaceflightController.m_player.transform.position + m_currentDirection * m_currentSpeed;

			// make sure the ship stays on the zero plane
			newPosition.y = 0.0f;

			// update the player position
			m_spaceflightController.UpdatePlayerPosition( newPosition );

			// calculate the new rotation of the player
			Quaternion newRotation = Quaternion.LookRotation( m_currentDirection, Vector3.up );

			// update the player rotation
			m_spaceflightController.m_player.SetRotation( newRotation );

			// check if the ship is moving
			if ( m_currentSpeed >= 0.001f )
			{
				// rotate the skybox accordingly
				Vector3 currentRightVector = newRotation * Vector3.right;
				Quaternion rotation = Quaternion.AngleAxis( m_currentSpeed * Time.deltaTime * 0.5f, currentRightVector );
				m_skyboxRotation = Matrix4x4.Rotate( rotation ) * m_skyboxRotation;

				Material skyboxMaterial = RenderSettings.skybox;
				skyboxMaterial.SetMatrix( "_Rotation", m_skyboxRotation );

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
						playerData.m_starflight.m_hyperspaceCoordinates = star.m_worldCoordinates + newPosition * ( star.GetBreachDistance() + 16.0f );

						// limit the ship speed to hyperspace speed
						m_currentSpeed = Mathf.Min( m_currentSpeed, m_spaceflightController.m_maximumShipSpeedHyperspace );

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
