
using UnityEngine;
using UnityEngine.EventSystems;

public class ScienceFunction : ButtonFunction
{
	private readonly ButtonFunction[] m_buttonFunctions = { new SensorsFunction(), new AnalysisFunction(), null, new BridgeFunction() };
	private readonly string[] m_buttonLabels = { "Sensors", "Analysis", "Status", "Bridge" };

	public override void Execute()
	{
		// play the activate sound
		m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Activate );

		// change the button functions and labels
		m_spaceflightController.UpdateButtonList( m_buttonFunctions, m_buttonLabels );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the personnel file on our captain
		Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( CrewAssignment.Role.ScienceOfficer );

		// set the name of the captain
		m_spaceflightController.m_currentOfficer.text = "Officer " + personnelFile.m_name;
	}

	public override void Cancel()
	{
	}
}
