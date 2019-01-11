
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

		switch ( playerData.m_general.m_location )
		{
			case PD_General.Location.JustLaunched:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				SpaceflightController.m_instance.m_messages.ChangeText( "<color=white>That's Arth you fool!</color>" );

				SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

				break;

			case PD_General.Location.Hyperspace:
			case PD_General.Location.StarSystem:
			case PD_General.Location.DockingBay:
			case PD_General.Location.Encounter:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				SpaceflightController.m_instance.m_messages.ChangeText( "<color=white>We're not in orbit.</color>" );

				SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

				break;

			case PD_General.Location.InOrbit:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				SpaceflightController.m_instance.m_messages.ChangeText( "<color=red>Not yet implemented.</color>" );

				SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

				break;
		}

		return false;
	}
}
