
public class ShipCargoButton : ShipButton
{
	public override string GetLabel()
	{
		return "Cargo";
	}

	public override bool Execute()
	{
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		SpaceflightController.m_instance.m_messages.Clear();

		SpaceflightController.m_instance.m_messages.AddText( "<color=red>Not yet implemented.</color>" );

		SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

		return false;
	}
}
