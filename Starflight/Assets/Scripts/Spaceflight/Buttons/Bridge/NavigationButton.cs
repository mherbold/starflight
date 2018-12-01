
public class NavigationButton : ShipButton
{
	private readonly ShipButton[] m_buttons = { new ManeuverButton(), new StarmapButton(), new RaiseShieldsButton(), new ArmWeaponButton(), new CombatButton(), new BridgeButton() };

	public override string GetLabel()
	{
		return "Navigation";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.UpdateButtons( m_buttons );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the personnel file on this officer
		PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.Navigator );

		// set the name of the officer
		m_spaceflightController.m_buttonController.ChangeOfficerText( "Officer " + personnelFile.m_name );

		return true;
	}
}
