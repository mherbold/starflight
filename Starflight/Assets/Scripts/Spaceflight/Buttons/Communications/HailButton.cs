
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

		switch ( playerData.m_general.m_location )
		{
			default:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_messages.ChangeText( "<color=white>There's no one to hail.</color>" );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;
		}

		return false;
	}
}
