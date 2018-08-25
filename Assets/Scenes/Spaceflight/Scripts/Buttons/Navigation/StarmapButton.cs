
public class StarmapButton : ShipButton
{
	public override string GetLabel()
	{
		return "Starmap";
	}

	public override bool Execute()
	{
		if ( !m_spaceflightController.m_inHyperspace )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "We're not in hyperspace, captain.";

			m_spaceflightController.m_buttonController.UpdateButtonSprites();

			return false;
		}

		return false;
	}
}
