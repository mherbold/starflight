
using UnityEngine;
using UnityEngine.EventSystems;

public class LaunchButton : Button
{
	private readonly Button[] m_buttons = { new LaunchYesButton(), new LaunchNoButton() };

	public override string GetLabel()
	{
		return "Launch";
	}

	public override bool Execute()
	{
		if ( m_spaceflightController.m_inDockingBay )
		{
			m_spaceflightController.m_messages.text = "Confirm launch?";

			m_spaceflightController.UpdateButtons( m_buttons );

			return true;
		}

		return false;
	}
}
