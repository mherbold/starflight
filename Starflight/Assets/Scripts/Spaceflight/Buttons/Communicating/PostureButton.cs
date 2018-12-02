
public class PostureButton : ShipButton
{
	public override string GetLabel()
	{
		return "Posture";
	}

	public override bool Execute()
	{
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		m_spaceflightController.m_messages.ChangeText( "<color=red>Not yet implemented.</color>" );

		m_spaceflightController.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
