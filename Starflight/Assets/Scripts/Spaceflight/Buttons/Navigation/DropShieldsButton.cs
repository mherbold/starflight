
public class DropShieldsButton : ShipButton
{
	public override string GetLabel()
	{
		return "Drop Shields";
	}

	public override bool Execute()
	{
		var playerData = DataController.m_instance.m_playerData;

		playerData.m_playerShip.DropShields();

		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );

		SpaceflightController.m_instance.m_buttonController.SetManeuverButtons();

		return false;
	}
}
