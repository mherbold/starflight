
using UnityEngine;
using UnityEngine.EventSystems;

public class MedicalButton : Button
{
	private readonly Button[] m_buttons = { new ExamineButton(), new TreatButton(), new BridgeButton() };

	public override string GetLabel()
	{
		return "Medical";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.UpdateButtons( m_buttons );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the personnel file on this officer
		Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( CrewAssignment.Role.Doctor );

		// set the name of the officer
		m_spaceflightController.m_currentOfficer.text = "Doctor " + personnelFile.m_name;

		return true;
	}

	public override void Cancel()
	{
		// play the deactivate sound
		m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Deactivate );

		// return to the bridge
		m_spaceflightController.m_buttonController.RestoreBridgeButtons();
	}
}
