
public class ObsequiousButton : ShipButton
{
	static public bool m_isResponding;

	public override string GetLabel()
	{
		return "Obsequious";
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
				// yes - try to hail them
				m_spaceflightController.m_encounter.Hail( GD_Comm.Stance.Obsequious, m_isResponding );

				// deactivate the button
				m_spaceflightController.m_buttonController.RestoreBridgeButtons();

				return false;
			}
		}

		// play the buzzer sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		// let the player know there's no one to hail
		m_spaceflightController.m_messages.ChangeText( "<color=white>There's no one to hail.</color>" );

		// deactivate the button
		m_spaceflightController.m_buttonController.RestoreBridgeButtons();

		return false;
	}
}
