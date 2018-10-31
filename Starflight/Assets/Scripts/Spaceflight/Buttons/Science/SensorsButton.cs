
public class SensorsButton : ShipButton
{
	public override string GetLabel()
	{
		return "Sensors";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		switch ( playerData.m_general.m_location )
		{
			case PD_General.Location.InOrbit:

				// show the system display
				m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_sensorsDisplay );

				// play the scanning sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Scanning );

				return true;

			case PD_General.Location.DockingBay:
			case PD_General.Location.JustLaunched:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;

			default:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_messages.ChangeText( "<color=red>Not yet implemented.</color>" );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;
		}

		return false;
	}

	public override bool Update()
	{
		return true;
	}
}
