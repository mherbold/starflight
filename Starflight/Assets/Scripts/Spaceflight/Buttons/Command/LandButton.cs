
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

		switch ( playerData.m_general.m_location )
		{
			case PD_General.Location.JustLaunched:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				SpaceflightController.m_instance.m_messages.Clear();

				SpaceflightController.m_instance.m_messages.AddText( "<color=white>We can't land on Arth.</color>" );

				SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

				break;

			case PD_General.Location.InOrbit:

				// show the terrian map display
				SpaceflightController.m_instance.m_displayController.ChangeDisplay( SpaceflightController.m_instance.m_displayController.m_terrainMapDisplay );

				// change the buttons
				SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Land );

				return true;

			default:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				SpaceflightController.m_instance.m_messages.Clear();

				SpaceflightController.m_instance.m_messages.AddText( "<color=white>We're not in orbit.</color>" );

				SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

				break;
		}

		return false;
	}
}
