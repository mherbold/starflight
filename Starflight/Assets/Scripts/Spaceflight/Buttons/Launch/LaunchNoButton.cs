
public class LaunchNoButton : ShipButton
{
	public override string GetLabel()
	{
		return "No";
	}

	public override bool Execute()
	{
		// restore the bridge buttons
		SpaceflightController.m_instance.m_buttonController.RestoreBridgeButtons();

		// clear the messages
		SpaceflightController.m_instance.m_messages.ChangeText( "" );

		// play the deactivate sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );

		return false;
	}
}
