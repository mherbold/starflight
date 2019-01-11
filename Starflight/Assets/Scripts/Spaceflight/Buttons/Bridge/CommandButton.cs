
public class CommandButton : ShipButton
{
	public override string GetLabel()
	{
		return "Command";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// change the buttons
		switch ( playerData.m_general.m_location )
		{
			case PD_General.Location.DockingBay:
				SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.CommandA );
				break;

			default:
				SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.CommandB );
				break;
		}

		// get the personnel file on our captain
		PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.Captain );

		// set the name of the captain
		SpaceflightController.m_instance.m_buttonController.ChangeOfficerText( "Captain " + personnelFile.m_name );

		return true;
	}
}
