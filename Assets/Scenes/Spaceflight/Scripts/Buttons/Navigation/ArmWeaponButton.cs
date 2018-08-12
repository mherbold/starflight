
using UnityEngine;
using UnityEngine.EventSystems;

public class ArmWeaponButton : Button
{
	public override string GetLabel()
	{
		return "Arm Weapon";
	}

	public override bool Execute()
	{
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		if ( ( playerData.m_shipConfiguration.m_laserCannonClass == 0 ) && ( playerData.m_shipConfiguration.m_missileLauncherClass == 0 ) )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "Ship is not equipped with weapons.";

			m_spaceflightController.m_buttonController.UpdateButtonSprites();

			return false;
		}

		return false;
	}
}
