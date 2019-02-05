
public class HostileButton : ShipButton
{
	public override string GetLabel()
	{
		return "Hostile";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// are we in an encounter?
		if ( playerData.m_general.m_location == PD_General.Location.Encounter )
		{
			// yes - are there living alien ships in the encounter?
			if ( SpaceflightController.m_instance.m_encounter.HasLivingAlienShips() )
			{
				// yes - are we responding?
				var isResponding = ( SpaceflightController.m_instance.m_buttonController.GetCurrentButtonSet() == ButtonController.ButtonSet.Respond );

				// try to hail them
				SpaceflightController.m_instance.m_encounter.Hail( GD_Comm.Stance.Hostile, isResponding );

				// are we in video chat?
				if ( SpaceflightController.m_instance.m_encounter.IsConnected() )
				{
					// yes - restore comm buttons
					SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );
				}
				else
				{
					// no - restore the bridge buttons
					SpaceflightController.m_instance.m_buttonController.SetBridgeButtons();
				}

				return false;
			}
		}

		// play the buzzer sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		// let the player know there's no one to hail
		SpaceflightController.m_instance.m_messages.Clear();

		SpaceflightController.m_instance.m_messages.AddText( "<color=white>There's no one to hail.</color>" );

		// deactivate the button
		SpaceflightController.m_instance.m_buttonController.SetBridgeButtons();

		return false;
	}
}
