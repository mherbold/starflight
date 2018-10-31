
public class StarmapButton : ShipButton
{
	public override string GetLabel()
	{
		return "Starmap";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_general.m_location != PD_General.Location.Hyperspace )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			m_spaceflightController.m_messages.ChangeText( "<color=white>We're not in hyperspace, captain.</color>" );

			m_spaceflightController.m_buttonController.UpdateButtonSprites();
		}

		return false;
	}
}
