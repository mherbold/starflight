
public class NavigationButton : ShipButton
{
	public override string GetLabel()
	{
		return "Navigation";
	}

	public override bool Execute()
	{
		// change the buttons
		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Navigation );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the personnel file on this officer
		PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.Navigator );

		// set the name of the officer
		SpaceflightController.m_instance.m_buttonController.ChangeOfficerText( "Officer " + personnelFile.m_name );

		return true;
	}
}
