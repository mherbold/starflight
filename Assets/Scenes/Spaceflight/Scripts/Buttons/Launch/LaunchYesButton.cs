﻿
using UnityEngine;

public class LaunchYesButton : ShipButton
{
	private float m_launchTimer;
	private bool m_countdownStarted;

	public override string GetLabel()
	{
		return "Yes";
	}

	public override bool Execute()
	{
		if ( m_spaceflightController.m_inDockingBay )
		{
			// change to the arth system
			GameData gameData = DataController.m_instance.m_gameData;
			m_spaceflightController.m_systemController.EnterSystem( gameData.m_misc.m_arthStarId );

			// update the messages log
			m_spaceflightController.m_messages.text = "Opening docking bay doors...";

			if ( !m_spaceflightController.m_skipCinematics )
			{
				// play the docking bay door open sound
				SoundController.m_instance.PlaySound( SoundController.Sound.DockingBayDoorOpen );

				// play the decompression sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Decompression );

				// play the launch sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Launch );

				// open the top docking bay door
				m_spaceflightController.m_dockingBayDoorTop.gameObject.SetActive( true );
				m_spaceflightController.m_dockingBayDoorTop.Play( "Open" );

				// open the bottom docking bay door
				m_spaceflightController.m_dockingBayDoorBottom.gameObject.SetActive( true );
				m_spaceflightController.m_dockingBayDoorBottom.Play( "Open" );

				// fire up the particle system
				m_spaceflightController.m_decompressionParticleSystem.gameObject.SetActive( true );
				m_spaceflightController.m_decompressionParticleSystem.Play();
			}
		}
		else
		{
			// TODO: Launching from planet cinematics
		}

		// reset some private members
		m_launchTimer = 0.0f;
		m_countdownStarted = false;

		// stop the music
		MusicController.m_instance.ChangeToTrack( MusicController.Track.None );

		return true;
	}

	public override bool Update()
	{
		// keep track of the cutscene time
		m_launchTimer += Time.deltaTime * ( m_spaceflightController.m_skipCinematics ? 20.0f : 1.0f );

		// at 15 seconds begin the countdown...
		if ( m_launchTimer >= 15.0f )
		{
			// check if we have have not played the countdown sound yet
			if ( !m_countdownStarted )
			{
				if ( !m_spaceflightController.m_skipCinematics )
				{
					// play the countdown sound
					SoundController.m_instance.PlaySound( SoundController.Sound.Countdown );
				}

				// turn off the decompression particle system
				m_spaceflightController.m_decompressionParticleSystem.gameObject.SetActive( false );

				// remember that we've already started playing the countdown sound
				m_countdownStarted = true;
			}

			// do countdown text animation
			int currentNumber = Mathf.FloorToInt( 21.5f - m_launchTimer );

			// check if we are showing the countdown text
			if ( ( currentNumber >= 1 ) && ( currentNumber <= 5 ) )
			{
				// yes - make it active
				m_spaceflightController.m_countdown.gameObject.SetActive( true );

				// update the text to the current number
				m_spaceflightController.m_countdown.text = currentNumber.ToString();

				// animate the opacity and the size of the numbers
				float deltaTime = m_launchTimer - ( 15.5f + ( 5 - currentNumber ) );

				if ( deltaTime < 0.1f )
				{
					// fade in
					m_spaceflightController.m_countdown.alpha = deltaTime * 10.0f;
					m_spaceflightController.m_countdown.fontSize = 240.0f;
				}
				else
				{
					// fade out and shrink
					m_spaceflightController.m_countdown.alpha = 1.0f - ( deltaTime - 0.1f ) / 0.9f;
					m_spaceflightController.m_countdown.fontSize = 140.0f + m_spaceflightController.m_countdown.alpha * 100.0f;
				}
			}
			else
			{
				// we're not showing the numbers so hide the countdown text
				m_spaceflightController.m_countdown.gameObject.SetActive( false );

				// have we reached zero in the countdown?
				if ( currentNumber <= 0 )
				{
					if ( m_spaceflightController.m_inDockingBay )
					{
						// yes - update the messages text
						m_spaceflightController.m_messages.text = "Leaving starport...";

						// figure out how much to move the ship forward by (with an exponential acceleration curve)
						float y = ( m_launchTimer - 20.5f ) * 5.0f;

						y *= y;

						// have we reached the end of the launch trip?
						if ( y >= 1000.0f )
						{
							// yep - fix us at exactly 1500 units above the zero plane
							y = 1000.0f;

							// hide the docking bay doors
							m_spaceflightController.m_dockingBayDoorTop.gameObject.SetActive( false );
							m_spaceflightController.m_dockingBayDoorBottom.gameObject.SetActive( false );

							// update some flags
							m_spaceflightController.m_inDockingBay = false;
							m_spaceflightController.m_justLaunched = true;

							// calculate the new position of the player (just north of the arth spaceport)
							Vector3 playerPosition = m_spaceflightController.m_systemController.m_planetController[ 2 ].transform.position;
							playerPosition.y = 0.0f;
							playerPosition.z += 250.0f;

							// update the players position
							m_spaceflightController.m_player.transform.position = playerPosition;

							// restore the bridge buttons (this also ends the launch function)
							m_spaceflightController.m_buttonController.RestoreBridgeButtons();
						}

						// update the position of the camera
						Vector3 cameraPosition = new Vector3( 0.0f, 2500.0f - y, 0.0f );
						m_spaceflightController.m_camera.transform.localPosition = cameraPosition;

						// fade the map overlay
						float fade = 1.0f - ( y / 1000.0f );
						m_spaceflightController.m_mapRawImage.color = new Color( fade, fade, fade );
					}
					else
					{
						// TODO: Launching from planet animation
					}
				}
			}
		}
		else
		{
			// hide the countdown text
			m_spaceflightController.m_countdown.gameObject.SetActive( false );
		}

		// returning true prevents the default spaceflight update from running
		return true;
	}
}
