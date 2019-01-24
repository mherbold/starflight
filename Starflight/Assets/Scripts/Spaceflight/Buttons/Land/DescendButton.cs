
public class DescendButton : ShipButton
{
	public override string GetLabel()
	{
		return "Descend";
	}

	public override bool Execute()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// update the messages display
		SpaceflightController.m_instance.m_messages.ChangeText( "Computing descent profile..." );

		// set the landing coordinates
		SpaceflightController.m_instance.m_planetside.SetLandingCoordinates( playerData.m_general.m_selectedLatitude, playerData.m_general.m_selectedLongitude );

		// start the landing animation
		SpaceflightController.m_instance.m_playerCamera.StartAnimation( "Landing (Planetside)" );

		// start the landing sound
		SoundController.m_instance.PlaySound( SoundController.Sound.PlanetLanding );

		// stop the music
		MusicController.m_instance.ChangeToTrack( MusicController.Track.None );

		return false;
	}
}
