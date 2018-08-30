
public class HailButton : ShipButton
{
	public override string GetLabel()
	{
		return "Hail";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_starflight.m_location == Starflight.Location.DockingBay )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			m_spaceflightController.m_spaceflightUI.ChangeMessageText( "There's no one to hail." );

			m_spaceflightController.m_buttonController.UpdateButtonSprites();

			return false;
		}

		return false;
	}
}
