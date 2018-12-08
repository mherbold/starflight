
public class FriendlyButton : ShipButton
{
	public override string GetLabel()
	{
		return "Friendly";
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
				// yes - are we responding?
				var isResponding = ( m_spaceflightController.m_buttonController.GetCurrentButtonSet() == ButtonController.ButtonSet.Respond );

				// try to hail them
				m_spaceflightController.m_encounter.Hail( GD_Comm.Stance.Friendly, isResponding );

				// are we in video chat?
				if ( m_spaceflightController.m_encounter.IsConnected() )
				{
					// yes - restore comm buttons
					m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );
				}
				else
				{
					// no - restore the bridge buttons
					m_spaceflightController.m_buttonController.RestoreBridgeButtons();
				}

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
