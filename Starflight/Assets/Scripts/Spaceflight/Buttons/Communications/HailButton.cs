
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

		// are we in an encounter?
		if ( playerData.m_general.m_location == PD_General.Location.Encounter )
		{
			// yes - are there living alien ships in the encounter?
			if ( m_spaceflightController.m_encounter.HasLivingAlienShips() )
			{
				// yes - change the buttons
				m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Hail );

				return true;
			}
		}

		// play the buzzer sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		// let the player know there's no one to hail
		m_spaceflightController.m_messages.ChangeText( "<color=white>There's no one to hail.</color>" );

		// deactivate the button
		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
