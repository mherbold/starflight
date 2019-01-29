
public class LookButton : ShipButton
{
	public override string GetLabel()
	{
		return "Look";
	}

	public override bool Execute()
	{
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		SpaceflightController.m_instance.m_messages.ChangeText( "<color=red>Not yet implemented.</color>" );

		SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
