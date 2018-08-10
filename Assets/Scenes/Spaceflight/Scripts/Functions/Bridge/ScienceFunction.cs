
using UnityEngine;
using UnityEngine.EventSystems;

public class ScienceFunction : ButtonFunction
{
	private readonly ButtonFunction[] m_buttonFunctions = { new SensorsFunction(), new AnalysisFunction(), new StatusFunction(), new BridgeFunction() };

	public override string GetButtonLabel()
	{
		return "Science";
	}

	public override void Execute()
	{
		// change the button functions and labels
		m_spaceflightController.UpdateButtonFunctions( m_buttonFunctions );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the personnel file on our captain
		Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( CrewAssignment.Role.ScienceOfficer );

		// set the name of the captain
		m_spaceflightController.m_currentOfficer.text = "Officer " + personnelFile.m_name;
	}

	public override void Cancel()
	{
		// play the deactivate sound
		m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Deactivate );

		// return to the bridge
		m_spaceflightController.RestoreBridgeButtons();
	}
}
