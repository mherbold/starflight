
public class RaiseShieldsButton : ShipButton
{
	public override string GetLabel()
	{
		return "Raise Shields";
	}

	public override bool Execute()
	{
		var playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_playerShip.m_shieldingClass == 0 )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			SpaceflightController.m_instance.m_messages.Clear();

			SpaceflightController.m_instance.m_messages.AddText( "<color=white>Ship is not equipped with shields.</color>" );

			SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();
		}
		else
		{
			playerData.m_playerShip.RaiseShields();

			SoundController.m_instance.PlaySound( SoundController.Sound.Update );

			SpaceflightController.m_instance.m_buttonController.SetManeuverButtons();
		}

		return false;
	}
}
