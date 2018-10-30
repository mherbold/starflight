
public class LaunchButton : ShipButton
{
	private readonly ShipButton[] m_buttons = { new LaunchYesButton(), new LaunchNoButton() };

	public override string GetLabel()
	{
		return "Launch";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_starflight.m_location == PD_General.Location.DockingBay )
		{
			m_spaceflightController.m_spaceflightUI.ChangeMessageText( "<color=yellow>Confirm launch?</color>" );

			m_spaceflightController.m_buttonController.UpdateButtons( m_buttons );

			return true;
		}

		return false;
	}
}
