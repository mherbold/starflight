
using UnityEngine;
using UnityEngine.EventSystems;

public class ArmWeaponFunction : ButtonFunction
{
	public override void Execute()
	{
		PlayerData playerData = PersistentController.m_instance.m_playerData;

		if ( ( playerData.m_shipConfiguration.m_laserCannonClass == 0 ) && ( playerData.m_shipConfiguration.m_missileLauncherClass == 0 ) )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "Ship is not equipped with weapons.";
		}
	}

	public override void Cancel()
	{
	}
}
