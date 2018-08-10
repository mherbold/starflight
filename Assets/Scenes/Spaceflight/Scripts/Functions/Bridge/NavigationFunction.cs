
using UnityEngine;
using UnityEngine.EventSystems;

public class NavigationFunction : ButtonFunction
{
	private readonly ButtonFunction[] m_buttonFunctions = { new ManeuverFunction(), new StarmapFunction(), new RaiseShieldsFunction(), new ArmWeaponFunction(), new CombatFunction(), new BridgeFunction() };

	public override string GetButtonLabel()
	{
		return "Navigation";
	}

	public override void Execute()
	{
		// change the button functions and labels
		m_spaceflightController.UpdateButtonFunctions( m_buttonFunctions );

		// get to the player data
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		// get the personnel file on our captain
		Personnel.PersonnelFile personnelFile = playerData.m_crewAssignment.GetPersonnelFile( CrewAssignment.Role.Navigator );

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
