
public class MapButton : ShipButton
{
	public override string GetLabel()
	{
		return "Map";
	}

	public override bool Execute()
	{
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		SpaceflightController.m_instance.m_messages.ChangeText( "<color=red>Not yet implemented.</color>" );

		SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
