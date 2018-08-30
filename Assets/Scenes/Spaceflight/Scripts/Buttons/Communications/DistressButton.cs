
public class DistressButton : ShipButton
{
	public override string GetLabel()
	{
		return "Distress";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_starflight.m_location == Starflight.Location.DockingBay )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			m_spaceflightController.m_spaceflightUI.ChangeMessageText( "Transmit emergency distress call, please confirm." );

			return true;
		}

		return false;
	}
}
