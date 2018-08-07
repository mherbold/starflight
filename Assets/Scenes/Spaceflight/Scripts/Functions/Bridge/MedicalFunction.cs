﻿
using UnityEngine;
using UnityEngine.EventSystems;

public class MedicalFunction : ButtonFunction
{
	private readonly ButtonFunction[] m_buttonFunctions = { null, null, new BridgeFunction() };
	private readonly string[] m_buttonLabels = { "Examine", "Treat", "Bridge" };

	public override void Execute()
	{
		// play the activate sound
		m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Activate );

		// change the button functions and labels
		m_spaceflightController.UpdateButtonList( m_buttonFunctions, m_buttonLabels );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the personnel file on our captain
		Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( CrewAssignment.Role.Doctor );

		// set the name of the captain
		m_spaceflightController.m_currentOfficer.text = "Doctor " + personnelFile.m_name;
	}

	public override void Cancel()
	{
	}
}
