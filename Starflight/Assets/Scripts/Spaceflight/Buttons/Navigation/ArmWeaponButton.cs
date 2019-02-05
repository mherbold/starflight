
public class ArmWeaponButton : ShipButton
{
	public override string GetLabel()
	{
		return "Arm Weapon";
	}

	public override bool Execute()
	{
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( ( playerData.m_playerShip.m_laserCannonClass == 0 ) && ( playerData.m_playerShip.m_missileLauncherClass == 0 ) )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			SpaceflightController.m_instance.m_messages.Clear();

			SpaceflightController.m_instance.m_messages.AddText( "<color=white>Ship is not equipped with weapons.</color>" );

			SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();
		}
		else
		{
			playerData.m_playerShip.ArmWeapons();

			SoundController.m_instance.PlaySound( SoundController.Sound.Update );

			SpaceflightController.m_instance.m_buttonController.SetManeuverButtons();
		}

		return false;
	}
}
