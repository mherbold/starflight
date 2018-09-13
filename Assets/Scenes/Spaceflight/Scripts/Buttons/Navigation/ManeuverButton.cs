
using UnityEngine;

public class ManeuverButton : ShipButton
{
	// the planet controller for the planet we could orbit around
	PlanetController m_orbitPlanetController;

	// if this is true we are transitioning to orbit or the docking bay
	bool m_isTransitioning;

	// what are we transitioning to?
	Starflight.Location m_nextLocation;

	// get the label for this button
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
				m_spaceflightController.SwitchLocation( Starflight.Location.StarSystem );

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

		// are we currently transitioning?
		if ( m_isTransitioning )
		{
			// yes - get the current map fade amount
			float mapFadeAmount = m_spaceflightController.m_spaceflightUI.GetCurrentMapFadeAmount();

			// is it completely black yet?
			if ( mapFadeAmount == 0.0f )
			{
				// yes - turn off the maneuver function
				m_spaceflightController.m_buttonController.DeactivateButton();

				// which location do we want to switch to?
				switch ( m_nextLocation )
				{
					case Starflight.Location.DockingBay:

						// switch to the docking bay location
						m_spaceflightController.SwitchLocation( Starflight.Location.DockingBay );

						// play the docking bay door close animation
						m_spaceflightController.m_dockingBay.CloseDockingBayDoors();

						break;

					case Starflight.Location.InOrbit:

						// switch to the in orbit location
						m_spaceflightController.SwitchLocation( Starflight.Location.InOrbit );

						break;
				}
			}

			return true;
		}

		// check if we want to stop maneuvering
		if ( InputController.m_instance.SubmitWasPressed() )
		{
			// turn off the engines
			m_spaceflightController.m_player.TurnOffEngines();

			// do we have a planet to orbit?
			if ( m_orbitPlanetController != null )
			{
				// fade the map to black
				m_spaceflightController.m_spaceflightUI.FadeMap( 0.0f, 2.0f );

				// we are now transitioning
				m_isTransitioning = true;

				// play the activate sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Activate );

				// is this arth?
				if ( m_orbitPlanetController.m_planet.m_planetTypeId == PlanetController.c_arthPlanetTypeId )
				{
					// yes - transition to the docking bay
					m_nextLocation = Starflight.Location.DockingBay;

					// display message
					m_spaceflightController.m_spaceflightUI.m_messages.text = "Initiating docking procedure...";
				}
				else
				{
					// no - transition to in orbit
					m_nextLocation = Starflight.Location.InOrbit;

					// display message
					m_spaceflightController.m_spaceflightUI.m_messages.text = "Initiating orbital maneuver...";
				}

				// stop here
				return true;
			}
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
			// get to the current star
			Star star = gameData.m_starList[ playerData.m_starflight.m_currentStarId ];

			// yes - did we just leave it?
			if ( Vector3.Magnitude( playerData.m_starflight.m_systemCoordinates ) >= 8192.0f )
			{
				// yes - calculate the position of the ship in hyperspace
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
				m_spaceflightController.SwitchLocation( Starflight.Location.Hyperspace );
			}
			else
			{
				// get the nearest planet controller to the player
				m_orbitPlanetController = m_spaceflightController.m_systemController.GetNearestPlanetController();

				// did we get a planet controller?
				if ( m_orbitPlanetController != null )
				{
					// yes - get the distance of the player is to the planet
					float distanceToPlanet = m_orbitPlanetController.GetDistanceToPlayer();

					// are we close enough to orbit the planet?
					if ( distanceToPlanet <= m_orbitPlanetController.m_scale )
					{
						// yes - let the player know
						m_spaceflightController.m_spaceflightUI.m_messages.text = "Ship is within orbital range.";
					}
					else
					{
						// no - forget this planet
						m_orbitPlanetController = null;

						// display the spectral class and ecosphere
						m_spaceflightController.m_spaceflightUI.m_messages.text = "<u>Stellar Parameters</u>\nSpectral Class: <#4FEDED>" + star.m_class + "</color>\nEcosphere: <#4FEDED>" + star.m_spectralClass.m_ecosphereMin + " - " + star.m_spectralClass.m_ecosphereMax + "</color>";
					}
				}
			}
		}
		else
		{
			// display no messages
			m_spaceflightController.m_spaceflightUI.m_messages.text = "";
		}

		// returning true prevents the default spaceflight update from running
		return true;
	}
}
