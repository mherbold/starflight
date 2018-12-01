﻿
public class HailButton : ShipButton
{
	private readonly ShipButton[] m_buttons = { new FriendlyButton(), new HostileButton(), new ObsequiousButton() };

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
				m_spaceflightController.m_buttonController.UpdateButtons( m_buttons );

				// let the buttons know we are hailing (not responding)
				FriendlyButton.m_isResponding = false;
				HostileButton.m_isResponding = false;
				ObsequiousButton.m_isResponding = false;

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
