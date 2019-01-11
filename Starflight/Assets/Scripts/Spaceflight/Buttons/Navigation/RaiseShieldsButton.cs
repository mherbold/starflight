
public class RaiseShieldsButton : ShipButton
{
	public override string GetLabel()
	{
		return "Raise Shields";
	}

	public override bool Execute()
	{
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_playerShip.m_shieldingClass == 0 )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			SpaceflightController.m_instance.m_messages.ChangeText( "<color=white>Ship is not equipped with shields.</color>" );

			SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();
		}

		return false;
	}
}
