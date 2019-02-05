
public class LaunchNoButton : ShipButton
{
	public override string GetLabel()
	{
		return "No";
	}

	public override bool Execute()
	{
		// restore the bridge buttons
		SpaceflightController.m_instance.m_buttonController.SetBridgeButtons();

		// clear the messages
		SpaceflightController.m_instance.m_messages.Clear();

		// play the deactivate sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );

		return false;
	}
}
