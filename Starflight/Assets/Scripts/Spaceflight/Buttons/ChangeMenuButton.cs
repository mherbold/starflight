
public class ChangeMenuButton : ShipButton
{
	private string m_label;
	private ButtonController.ButtonSet m_buttonSet;

	public ChangeMenuButton( string label, ButtonController.ButtonSet buttonSet )
	{
		m_label = label;
		m_buttonSet = buttonSet;
	}

	public override string GetLabel()
	{
		return m_label;
	}

	public override bool Execute()
	{
		// do we want to change to the bridge button set?
		if ( m_buttonSet == ButtonController.ButtonSet.Bridge )
		{
			// yes - special case handling for that button set
			SpaceflightController.m_instance.m_buttonController.SetBridgeButtons();
		}
		else
		{
			// no - just switch the menu buttons to the one we specified in the constructor
			SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( m_buttonSet );
		}

		return false;
	}
}
