
public class AnalysisButton : ShipButton
{
	public override string GetLabel()
	{
		return "Analysis";
	}

	public override bool Execute()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// do we have sensor data to analyze?
		if ( m_spaceflightController.m_displayController.m_sensorsDisplay.m_hasSensorData )
		{
			// yes - where are we at?
			switch ( playerData.m_starflight.m_location )
			{
				case Starflight.Location.InOrbit:

					// get to the planet
					Planet planet = gameData.m_planetList[ playerData.m_starflight.m_currentPlanetId ];

					// start building the new messages text
					string text = "<color=white>Analysis of Last Sensor Reading</color>\n";

					// object and orbit number
					text += "Object: <color=white>Planet</color>   Orbit Number: <color=white>" + ( planet.m_orbitPosition + 1 ) + "</color>\n";

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

					// update the messages text
					m_spaceflightController.m_spaceflightUI.ChangeMessageText( text );

					// turn off the active button dot
					m_spaceflightController.m_buttonController.UpdateButtonSprites();

					break;

				default:

					SoundController.m_instance.PlaySound( SoundController.Sound.Error );

					m_spaceflightController.m_spaceflightUI.ChangeMessageText( "<color=red>Not yet implemented.</color>" );

					m_spaceflightController.m_buttonController.UpdateButtonSprites();

					break;
			}
		}
		else
		{
			// nope - buzz
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			// complain to the player
			m_spaceflightController.m_spaceflightUI.ChangeMessageText( "<color=white>I need a current senor reading.</color>" );

			// turn off the active button
			m_spaceflightController.m_buttonController.UpdateButtonSprites();
		}

		return false;
	}
}
