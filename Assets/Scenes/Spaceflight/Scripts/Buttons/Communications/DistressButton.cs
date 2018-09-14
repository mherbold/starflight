
public class DistressButton : ShipButton
{
	public override string GetLabel()
	{
		return "Distress";
	}

	public override bool Execute()
	{
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		m_spaceflightController.m_spaceflightUI.ChangeMessageText( "Not yet implemented." );

		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
