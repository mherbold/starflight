
public class MedicalButton : ShipButton
{
	public override string GetLabel()
	{
		return "Medical";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Medical );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the personnel file on this officer
		PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.Doctor );

		// set the name of the officer
		m_spaceflightController.m_buttonController.ChangeOfficerText( "Doctor " + personnelFile.m_name );

		return true;
	}
}
