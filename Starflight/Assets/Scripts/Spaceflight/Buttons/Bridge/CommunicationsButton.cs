
public class CommunicationsButton : ShipButton
{
	private readonly ShipButton[] m_buttonSet1 = { new HailButton(), new DistressButton(), new BridgeButton() };
	private readonly ShipButton[] m_buttonSet2 = { new RespondButton(), new DistressButton(), new BridgeButton() };

	bool m_showingRespondButton;

	public override string GetLabel()
	{
		return "Communications";
	}

	public override bool Execute()
	{
		// do we want to show the respond button?
		m_showingRespondButton = m_spaceflightController.m_encounter.IsWaitingForResponse();

		// decide which button set to use
		var buttonSet = ( m_showingRespondButton ) ? m_buttonSet2 : m_buttonSet1;

		// change the buttons
		m_spaceflightController.m_buttonController.UpdateButtons( buttonSet );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the personnel file on this officer
		PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.CommunicationsOfficer );

		// set the name of the officer
		m_spaceflightController.m_buttonController.ChangeOfficerText( "Officer " + personnelFile.m_name );

		return true;
	}

	public override bool Update()
	{
		// did the encounter change to waiting for response?
		if ( m_showingRespondButton != m_spaceflightController.m_encounter.IsWaitingForResponse() )
		{
			// do we want to show the respond button?
			m_showingRespondButton = m_spaceflightController.m_encounter.IsWaitingForResponse();

			// decide which button set to use
			var buttonSet = ( m_showingRespondButton ) ? m_buttonSet2 : m_buttonSet1;

			// change the buttons
			m_spaceflightController.m_buttonController.UpdateButtons( buttonSet );
		}

		return false;
	}
}
