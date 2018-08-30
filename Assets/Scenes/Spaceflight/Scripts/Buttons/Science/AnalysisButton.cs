
public class AnalysisButton : ShipButton
{
	public override string GetLabel()
	{
		return "Analysis";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( !playerData.m_starflight.m_hasCurrentSenorReading )
		{
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			m_spaceflightController.m_spaceflightUI.ChangeMessageText( "I need a current senor reading." );

			m_spaceflightController.m_buttonController.UpdateButtonSprites();

			return false;
		}

		return false;
	}
}
