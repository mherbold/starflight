
public class DisarmWeaponButton : ShipButton
{
	public override string GetLabel()
	{
		return "Disarm Weapon";
	}

	public override bool Execute()
	{
		var playerData = DataController.m_instance.m_playerData;

		playerData.m_playerShip.m_weaponsAreArmed = false;

		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );

		SpaceflightController.m_instance.m_buttonController.SetManeuverButtons();

		return false;
	}
}
