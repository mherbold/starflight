
public class DistressButton : ShipButton
{
	public override string GetLabel()
	{
		return "Distress";
	}

	public override bool Execute()
	{
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		m_spaceflightController.m_messages.ChangeText( "<color=red>Not yet implemented.</color>" );

		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
