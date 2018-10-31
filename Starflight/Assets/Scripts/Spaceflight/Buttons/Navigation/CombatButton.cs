
public class CombatButton : ShipButton
{
	public override string GetLabel()
	{
		return "Combat";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		switch ( playerData.m_general.m_location )
		{
			default:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;
		}

		return false;
	}
}
