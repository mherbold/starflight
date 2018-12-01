
public class RespondButton : ShipButton
{
	private readonly ShipButton[] m_buttons = { new FriendlyButton(), new HostileButton(), new ObsequiousButton() };

	public override string GetLabel()
	{
		return "Respond";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.UpdateButtons( m_buttons );

		// let the buttons know we are responding (not hailing)
		FriendlyButton.m_isResponding = true;
		HostileButton.m_isResponding = true;
		ObsequiousButton.m_isResponding = true;

		return true;
	}
}
