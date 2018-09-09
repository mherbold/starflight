
public class LandButton : ShipButton
{
	public override string GetLabel()
	{
		return "Land";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		switch ( playerData.m_starflight.m_location )
		{
			case Starflight.Location.JustLaunched:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_spaceflightUI.ChangeMessageText( "We can't land on Arth." );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;

			default:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_spaceflightUI.ChangeMessageText( "We're not in orbit." );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;
		}

		return false;
	}
}
