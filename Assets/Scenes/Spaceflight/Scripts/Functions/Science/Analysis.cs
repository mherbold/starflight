
using UnityEngine;
using UnityEngine.EventSystems;

public class AnalysisFunction : ButtonFunction
{
	public override void Execute()
	{
		if ( !m_spaceflightController.m_hasCurrentSenorReading )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "I need a current senor reading.";
		}
	}

	public override void Cancel()
	{
	}
}
