
public class ArmWeaponButton : ShipButton
{
	public override string GetLabel()
	{
		return "Arm Weapon";
	}

	public override bool Execute()
	{
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( ( playerData.m_ship.m_laserCannonClass == 0 ) && ( playerData.m_ship.m_missileLauncherClass == 0 ) )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			m_spaceflightController.m_spaceflightUI.ChangeMessageText( "<color=white>Ship is not equipped with weapons.</color>" );

			m_spaceflightController.m_buttonController.UpdateButtonSprites();
		}

		return false;
	}
}
