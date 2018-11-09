
using UnityEngine;

public class LaunchYesButton : ShipButton
{
	// whether or not we have started the countdown
	bool m_countdownStarted;

	// keep track of what countdown number we are showing
	int m_lastCountdownNumberShown;

	// our launch timer
	float m_timer;

	public override string GetLabel()
	{
		return "Yes";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_general.m_location == PD_General.Location.DockingBay )
		{
			// update the messages log
			m_spaceflightController.m_messages.ChangeText( "<color=white>Opening docking bay doors...</color>" );

			// reset the last countdown number shown
			m_lastCountdownNumberShown = 0;

			if ( !m_spaceflightController.m_skipCinematics )
			{
				// open the docking bay doors
				m_spaceflightController.m_dockingBay.OpenDockingBayDoors();

				// play the launch sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Launch );
			}
		}
		else
		{
			// TODO: Launching from planet cinematics
		}

		// reset the timer
		m_timer = 0.0f;

		// we haven't started the countdown just yet
		m_countdownStarted = false;

		// stop the music
		MusicController.m_instance.ChangeToTrack( MusicController.Track.None );

		return true;
	}

	public override bool Update()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// keep track of the cutscene time
		m_timer += Time.deltaTime * ( m_spaceflightController.m_skipCinematics ? 20.0f : 1.0f );

		// at 15 seconds begin the countdown...
		if ( m_timer >= 15.0f )
		{
			// check if we have have not played the countdown sound yet
			if ( !m_countdownStarted )
			{
				if ( !m_spaceflightController.m_skipCinematics )
				{
					// play the countdown sound
					SoundController.m_instance.PlaySound( SoundController.Sound.Countdown );
				}

				// remember that we've already started playing the countdown sound
				m_countdownStarted = true;
			}

			// do countdown text animation
			int currentNumber = Mathf.FloorToInt( 21.5f - m_timer );

			// check if we are showing the countdown text
			if ( ( currentNumber >= 1 ) && ( currentNumber <= 5 ) )
			{
				if ( currentNumber != m_lastCountdownNumberShown )
				{
					// remember the current number
					m_lastCountdownNumberShown = currentNumber;

					// animate it
					m_spaceflightController.m_countdown.SetCountdownText( currentNumber.ToString() );
				}
			}
			else
			{
				// have we reached zero in the countdown?
				if ( currentNumber <= 0 )
				{
					// are we in the docking bay?
					if ( playerData.m_general.m_location == PD_General.Location.DockingBay )
					{
						// yes - fade the map out
						m_spaceflightController.m_map.StartFade( 0.0f, 7.0f );

						// yes - update the messages text
						m_spaceflightController.m_messages.ChangeText( "<color=white>Leaving starport...</color>" );

						// figure out how much to move the ship forward by (with an exponential acceleration curve)
						float y = Mathf.Pow( ( m_timer - 20.5f ) * 5.0f, 2.0f );

						// fudge the speed of the ship (to make infinite starfield appear)
						playerData.m_general.m_currentMaximumSpeed = 128.0f;
						playerData.m_general.m_currentSpeed = Mathf.Lerp( 0.0f, playerData.m_general.m_currentMaximumSpeed, ( m_timer - 20.5f ) / 2.0f );

						// update the position of the camera
						m_spaceflightController.m_player.DollyCamera( m_spaceflightController.m_dockingBay.GetParkedPosition() - y );

						// have we reached the end of the launch trip?
						if ( y >= 1024.0f )
						{
							Debug.Log( "At end of launch cinematics - switching to the just launched location." );

							// calculate the new position of the player (just north of the arth spaceport)
							Vector3 playerPosition = gameData.m_planetList[ gameData.m_misc.m_arthPlanetId ].GetPosition();
							playerPosition.y = 0.0f;
							playerPosition.z += 128.0f;

							// update the player's system coordinates to the new position
							playerData.m_general.m_lastStarSystemCoordinates = playerPosition;

							// update the player location
							m_spaceflightController.SwitchLocation( PD_General.Location.JustLaunched );

							// restore the bridge buttons (this also ends the launch function)
							m_spaceflightController.m_buttonController.RestoreBridgeButtons();
						}
					}
					else
					{
						// TODO: Launching from planet animation
					}
				}
			}
		}

		// returning true prevents the default spaceflight update from running
		return true;
	}
}
