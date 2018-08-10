
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandFunction : ButtonFunction
{
	private readonly ButtonFunction[] m_buttonFunctions = { new LaunchFunction(), new DisembarkFunction(), new CargoFunction(), new LogPlanetFunction(), new ShipsLogFunction(), new BridgeFunction() };

	public override string GetButtonLabel()
	{
		return "Command";
	}

	public override void Execute()
	{
		// change the button functions and labels
		m_spaceflightController.UpdateButtonFunctions( m_buttonFunctions );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the personnel file on our captain
		Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( CrewAssignment.Role.Captain );

		// set the name of the captain
		m_spaceflightController.m_currentOfficer.text = "Captain " + personnelFile.m_name;
	}

	public override void Cancel()
	{
		// play the deactivate sound
		m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Deactivate );

		// return to the bridge
		m_spaceflightController.RestoreBridgeButtons();
	}
}
