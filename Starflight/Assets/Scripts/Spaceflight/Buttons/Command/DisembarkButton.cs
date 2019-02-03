
using UnityEngine;

public class DisembarkButton : ShipButton
{
	// if this is true we are transitioning to disembarked
	bool m_isTransitioning;

	// what are we transitioning to?
	PD_General.Location m_nextLocation;

	// the message state
	int m_messageState;

	// the message timer
	float m_messageTimer;

	// get the label for this button
	public override string GetLabel()
	{
		return "Disembark";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		switch ( playerData.m_general.m_location )
		{
			case PD_General.Location.DockingBay:

				// Debug.Log( "Player is disembarking - switching to starport." );

				// play the update sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Update );

				// make sure we are standing in the middle of the transporter pad
				playerData.m_general.m_lastStarportCoordinates = new Vector3( 0.0f, 0.5f, 0.0f );

				// switch to the starport location
				SpaceflightController.m_instance.SwitchLocation( PD_General.Location.Starport );

				return true;

			case PD_General.Location.JustLaunched:
			case PD_General.Location.InOrbit:
			case PD_General.Location.StarSystem:
			case PD_General.Location.Hyperspace:
			case PD_General.Location.Encounter:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				SpaceflightController.m_instance.m_messages.ChangeText( "<color=white>We can't disembark in space!</color>" );

				SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

				break;

			case PD_General.Location.Planetside:

				// move the player to the arth ship coordinates on the surface
				playerData.m_general.m_lastDisembarkedCoordinates = Tools.LatLongToWorldCoordinates( playerData.m_general.m_selectedLatitude, playerData.m_general.m_selectedLongitude );

				// nudge the terrain vehicle to the south a bit
				playerData.m_general.m_lastDisembarkedCoordinates += Vector3.back * 48.0f;

				// update the terrain grid
				SpaceflightController.m_instance.m_disembarked.UpdateTerrainGridNow();

				// fade the map to black
				SpaceflightController.m_instance.m_viewport.StartFade( 0.0f, 2.0f );

				// we are now transitioning
				m_isTransitioning = true;

				// transition to the star system
				m_nextLocation = PD_General.Location.Disembarked;

				// display message
				SpaceflightController.m_instance.m_messages.ChangeText( "<color=white>Stand by, scanning planet...</color>" );

				// reset the message state and timer
				m_messageState = 0;
				m_messageTimer = 0.0f;

				return true;
		}

		return false;
	}

	public override bool Update()
	{
		// are we currently transitioning?
		if ( m_isTransitioning )
		{
			m_messageTimer += Time.deltaTime;

			if ( m_messageTimer >= 0.75f )
			{
				m_messageTimer -= 0.75f;

				switch ( m_messageState )
				{
					case 0:
						SpaceflightController.m_instance.m_messages.AddText( "<color=white>Auto sampling devices activated.</color>" );
						break;

					case 1:
						SpaceflightController.m_instance.m_messages.AddText( "<color=white>Initiating hull integrity check.</color>" );
						break;
				}

				m_messageState++;
			}

			// has the map stopped fading yet?
			if ( !SpaceflightController.m_instance.m_viewport.IsFading() )
			{
				// we are not transitioning any more
				m_isTransitioning = false;

				// switch to the next location
				SpaceflightController.m_instance.SwitchLocation( m_nextLocation );
			}
		}

		// always return true to prevent player from chagning buttons while transitioning to the docking bay
		return true;
	}
}
