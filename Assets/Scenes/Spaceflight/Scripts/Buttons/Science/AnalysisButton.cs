
using UnityEngine;
using UnityEngine.EventSystems;

public class AnalysisButton : Button
{
	public override string GetLabel()
	{
		return "Analysis";
	}

	public override bool Execute()
	{
		if ( !m_spaceflightController.m_hasCurrentSenorReading )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "I need a current senor reading.";

			m_spaceflightController.UpdateButtonSprites();

			return false;
		}

		return false;
	}
}
