
using UnityEngine;
using UnityEngine.EventSystems;

public class ScienceButton : Button
{
	private readonly Button[] m_buttons = { new SensorsButton(), new AnalysisButton(), new StatusButton(), new BridgeButton() };

	public override string GetLabel()
	{
		return "Science";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.UpdateButtons( m_buttons );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the personnel file on this officer
		Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( CrewAssignment.Role.ScienceOfficer );

		// set the name of the officer
		m_spaceflightController.m_currentOfficer.text = "Officer " + personnelFile.m_name;

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
