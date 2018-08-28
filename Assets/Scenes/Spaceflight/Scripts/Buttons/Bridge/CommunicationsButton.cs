
public class CommunicationsButton : ShipButton
{
	private readonly ShipButton[] m_buttons = { new HailButton(), new DistressButton(), new BridgeButton() };

	public override string GetLabel()
	{
		return "Communications";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.UpdateButtons( m_buttons );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the personnel file on this officer
		Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( CrewAssignment.Role.CommunicationsOfficer );

		// set the name of the officer
		m_spaceflightController.m_currentOfficer.text = "Officer " + personnelFile.m_name;

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
