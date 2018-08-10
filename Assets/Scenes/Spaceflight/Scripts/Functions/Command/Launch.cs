
using UnityEngine;
using UnityEngine.EventSystems;

public class LaunchFunction : ButtonFunction
{
	private readonly ButtonFunction[] m_buttonFunctions = { new LaunchYesFunction(), new LaunchNoFunction() };

	public override string GetButtonLabel()
	{
		return "Launch";
	}

	public override void Execute()
	{
		if ( m_spaceflightController.m_inDockingBay )
		{
			m_spaceflightController.m_messages.text = "Confirm launch?";

			m_spaceflightController.UpdateButtonFunctions( m_buttonFunctions );
		}
	}
}
