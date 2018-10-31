
public class LandButton : ShipButton
{
	private readonly ShipButton[] m_buttons = { new SelectSiteButton(), new DescendButton(), new AbortButton() };

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
			case PD_General.Location.JustLaunched:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_messages.ChangeText( "<color=white>We can't land on Arth.</color>" );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;

			case PD_General.Location.InOrbit:

				// show the terrian map display
				m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_terrainMapDisplay );

				// change the buttons
				m_spaceflightController.m_buttonController.UpdateButtons( m_buttons );

				return true;

			default:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_messages.ChangeText( "<color=white>We're not in orbit.</color>" );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

				break;
		}

		return false;
	}
}
