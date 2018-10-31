
public class MedicalButton : ShipButton
{
	private readonly ShipButton[] m_buttons = { new ExamineButton(), new TreatButton(), new BridgeButton() };

	public override string GetLabel()
	{
		return "Medical";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.UpdateButtons( m_buttons );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the personnel file on this officer
		PD_Personnel.PD_PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.Doctor );

		// set the name of the officer
		m_spaceflightController.m_buttonController.ChangeOfficerText( "Doctor " + personnelFile.m_name );

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
