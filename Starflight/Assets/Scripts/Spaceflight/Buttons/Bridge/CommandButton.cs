
public class CommandButton : ShipButton
{
	private readonly ShipButton[] m_buttonSetA = { new LaunchButton(), new DisembarkButton(), new CargoButton(), new LogPlanetButton(), new ShipsLogButton(), new BridgeButton() };
	private readonly ShipButton[] m_buttonSetB = { new LandButton(), new DisembarkButton(), new CargoButton(), new LogPlanetButton(), new ShipsLogButton(), new BridgeButton() };

	public override string GetLabel()
	{
		return "Command";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// change the buttons
		switch ( playerData.m_starflight.m_location )
		{
			case PD_General.Location.DockingBay:
				m_spaceflightController.m_buttonController.UpdateButtons( m_buttonSetA );
				break;

			default:
				m_spaceflightController.m_buttonController.UpdateButtons( m_buttonSetB );
				break;
		}

		// get the personnel file on our captain
		PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.Captain );

		// set the name of the captain
		m_spaceflightController.m_buttonController.ChangeOfficerText( "Captain " + personnelFile.m_name );

		return true;
	}

	public override void Cancel()
	{
		// play the deactivate sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );

		// return to the bridge
		m_spaceflightController.m_buttonController.RestoreBridgeButtons();
	}
}
