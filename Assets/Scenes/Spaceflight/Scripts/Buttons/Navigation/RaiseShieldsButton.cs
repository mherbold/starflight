
public class RaiseShieldsButton : ShipButton
{
	public override string GetLabel()
	{
		return "Raise Shields";
	}

	public override bool Execute()
	{
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_ship.m_shieldingClass == 0 )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			m_spaceflightController.m_spaceflightUI.ChangeMessageText( "Ship is not equipped with shields." );

			m_spaceflightController.m_buttonController.UpdateButtonSprites();

			return false;
		}

		return false;
	}
}
