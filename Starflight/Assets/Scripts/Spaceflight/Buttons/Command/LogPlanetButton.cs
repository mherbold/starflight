
public class LogPlanetButton : ShipButton
{
	public override string GetLabel()
	{
		return "Log Planet";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		switch ( playerData.m_starflight.m_location )
		{
			case PD_General.Location.JustLaunched:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_spaceflightUI.ChangeMessageText( "<color=white>That's Arth you fool!</color>" );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;

			case PD_General.Location.Hyperspace:
			case PD_General.Location.StarSystem:
			case PD_General.Location.DockingBay:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_spaceflightUI.ChangeMessageText( "<color=white>We're not in orbit.</color>" );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;

			case PD_General.Location.InOrbit:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_spaceflightUI.ChangeMessageText( "<color=red>Not yet implemented.</color>" );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;
		}

		return false;
	}
}
