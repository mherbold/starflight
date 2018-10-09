
public class RepairButton : ShipButton
{
	public override string GetLabel()
	{
		return "Repair";
	}

	public override bool Execute()
	{
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		m_spaceflightController.m_spaceflightUI.ChangeMessageText( "<color=red>Not yet implemented.</color>" );

		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
