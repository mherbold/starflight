
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandButton : Button
{
	private readonly Button[] m_buttons = { new LaunchButton(), new DisembarkButton(), new CargoButton(), new LogPlanetButton(), new ShipsLogButton(), new BridgeButton() };

	public override string GetLabel()
	{
		return "Command";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.UpdateButtons( m_buttons );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the personnel file on our captain
		Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( CrewAssignment.Role.Captain );

		// set the name of the captain
		m_spaceflightController.m_currentOfficer.text = "Captain " + personnelFile.m_name;

		return true;
	}

	public override void Cancel()
	{
		// play the deactivate sound
		m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Deactivate );

		// return to the bridge
		m_spaceflightController.RestoreBridgeButtons();
	}
}
