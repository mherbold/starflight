
public class AnalysisButton : ShipButton
{
	public override string GetLabel()
	{
		return "Analysis";
	}

	public override bool Execute()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// new message text
		string text;

		// do we have sensor data to analyze?
		if ( m_spaceflightController.m_displayController.m_sensorsDisplay.m_hasSensorData )
		{
			// yes - start building the new messages text
			text = "<color=white>Analysis of Last Sensor Reading</color>\n";

			// did we just scan a ship?
			if ( m_spaceflightController.m_displayController.m_sensorsDisplay.m_scanType <= SensorsDisplay.ScanType.NoahTransport )
			{
				// yes - display the ship information
				var vessel = gameData.m_vesselList[ (int) m_spaceflightController.m_displayController.m_sensorsDisplay.m_scanType ];

				// object
				text += "Object: <color=white>Vessel</color>\n";

				// type
				text += "Type: <color=white>" + vessel.m_type + "</color>\n";

				// size
				text += "Size: <color=white>" + vessel.m_relativeSize + " times the size of our ship</color>\n";

				// shields
				text += "Shields: <color=yellow>Not certain</color>\n";

				// weapon status
				text += "Weapon Status: <color=yellow>Not certain</color>\n";
			}
			else if ( m_spaceflightController.m_displayController.m_sensorsDisplay.m_scanType == SensorsDisplay.ScanType.Planet )
			{
				// yes - get to the planet
				GD_Planet planet = gameData.m_planetList[ playerData.m_general.m_currentPlanetId ];

				// object and orbit number
				text += "Object: <color=white>Planet</color>   Orbit Number: <color=white>" + planet.m_orbitPosition + "</color>\n";

				// surface
				text += "Predominant Surface: <color=white>" + planet.GetSurfaceText() + "</color>\n";

				// gravity
				text += "Gravity: <color=white>" + planet.GetGravityText() + "</color>\n";

				// atmospheric density
				text += "Atmospheric Density: <color=white>" + planet.GetAtmosphericDensityText() + "</color>\n";

				// temperature
				text += "Temperature: <color=white>" + planet.GetTemperatureText() + "</color>\n";

				// weather
				text += "Global Weather: <color=white>" + planet.GetGlobalWeatherText() + "</color>";
			}
		}
		else
		{
			// nope - buzz
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			// complain to the player
			text = "<color=white>I need a current senor reading.</color>";
		}

		// update the messages text
		m_spaceflightController.m_messages.ChangeText( text );

		// turn off the active button dot
		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
