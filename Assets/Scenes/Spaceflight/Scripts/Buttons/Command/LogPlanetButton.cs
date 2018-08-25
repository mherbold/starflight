
public class LogPlanetButton : ShipButton
{
	public override string GetLabel()
	{
		return "Log Planet";
	}

	public override bool Execute()
	{
		if ( !m_spaceflightController.m_inOrbit )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "We're not in orbit.";

			m_spaceflightController.m_buttonController.UpdateButtonSprites();

			return false;
		}

		return false;
	}
}
