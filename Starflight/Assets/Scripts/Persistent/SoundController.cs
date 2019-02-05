
using UnityEngine;

public class SoundController : MonoBehaviour
{
	public enum Sound
	{
		Activate,
		Alarm,
		Beep,
		Click,
		Close,
		Countdown,
		Deactivate,
		Decompression,
		DieselEngine,
		DockingBayDoorClose,
		DockingBayDoorOpen,
		EnterWarp,
		Error,
		ExitWarp,
		Launch,
		Open,
		PlanetLanding,
		PlanetLaunching,
		RadarBlip,
		Scanning,
		StaticLong,
		StaticShort,
		Transport,
		Update,
		WarbleLong,
		WarbleShort,
		Count
	};

	// static reference to this instance
	public static SoundController m_instance;

	// this is the max number of sounds we can play at the same time
	public int m_maxSimultaneousSounds = 16;

	// all of our sound clips
	public AudioClip m_activate;
	public AudioClip m_alarm;
	public AudioClip m_beep;
	public AudioClip m_click;
	public AudioClip m_close;
	public AudioClip m_countdown;
	public AudioClip m_deactivate;
	public AudioClip m_decompression;
	public AudioClip m_dieselEngine;
	public AudioClip m_dockingBayDoorClose;
	public AudioClip m_dockingBayDoorOpen;
	public AudioClip m_enterWarp;
	public AudioClip m_error;
	public AudioClip m_exitWarp;
	public AudioClip m_launch;
	public AudioClip m_open;
	public AudioClip m_planetLanding;
	public AudioClip m_planetLaunching;
	public AudioClip m_radarBlip;
	public AudioClip m_scanning;
	public AudioClip m_staticLong;
	public AudioClip m_staticShort;
	public AudioClip m_transport;
	public AudioClip m_update;
	public AudioClip m_warbleLong;
	public AudioClip m_warbleShort;

	// the sound list (to use with the enum)
	AudioClip[] m_soundList;

	// our audio source controllers
	AudioSourceController[] m_audioSourceControllerList;

	// unity awake
	void Awake()
	{
		// remember this instance
		m_instance = this;

		// create enough audio sources to be able to play the maximum number of sounds at once
		m_audioSourceControllerList = new AudioSourceController[ m_maxSimultaneousSounds ];

		for ( var i = 0; i < m_maxSimultaneousSounds; i++ )
		{
			m_audioSourceControllerList[ i ] = new AudioSourceController( gameObject );
		}

		// build the sound list
		m_soundList = new AudioClip[ (int) Sound.Count ];

		m_soundList[ (int) Sound.Activate ] = m_activate;
		m_soundList[ (int) Sound.Alarm ] = m_alarm;
		m_soundList[ (int) Sound.Beep ] = m_beep;
		m_soundList[ (int) Sound.Click ] = m_click;
		m_soundList[ (int) Sound.Close ] = m_close;
		m_soundList[ (int) Sound.Countdown ] = m_countdown;
		m_soundList[ (int) Sound.Deactivate ] = m_deactivate;
		m_soundList[ (int) Sound.Decompression ] = m_decompression;
		m_soundList[ (int) Sound.DieselEngine ] = m_dieselEngine;
		m_soundList[ (int) Sound.DockingBayDoorClose ] = m_dockingBayDoorClose;
		m_soundList[ (int) Sound.DockingBayDoorOpen ] = m_dockingBayDoorOpen;
		m_soundList[ (int) Sound.EnterWarp ] = m_enterWarp;
		m_soundList[ (int) Sound.Error ] = m_error;
		m_soundList[ (int) Sound.ExitWarp ] = m_exitWarp;
		m_soundList[ (int) Sound.Launch ] = m_launch;
		m_soundList[ (int) Sound.Open ] = m_open;
		m_soundList[ (int) Sound.PlanetLanding ] = m_planetLanding;
		m_soundList[ (int) Sound.PlanetLaunching ] = m_planetLaunching;
		m_soundList[ (int) Sound.RadarBlip ] = m_radarBlip;
		m_soundList[ (int) Sound.Scanning ] = m_scanning;
		m_soundList[ (int) Sound.StaticLong ] = m_staticLong;
		m_soundList[ (int) Sound.StaticShort ] = m_staticShort;
		m_soundList[ (int) Sound.Transport ] = m_transport;
		m_soundList[ (int) Sound.Update ] = m_update;
		m_soundList[ (int) Sound.WarbleLong ] = m_warbleLong;
		m_soundList[ (int) Sound.WarbleShort ] = m_warbleShort;
	}

	// unity fixed update
	void FixedUpdate()
	{
		foreach ( var audioSourceController in m_audioSourceControllerList )
		{
			audioSourceController.FixedUpdate();
		}
	}

	// play a sound at the given volume and pitch
	public void PlaySound( Sound sound, float volume = 1.0f, float pitch = 1.0f, bool loop = false )
	{
		// go through each audio source controller
		foreach ( var audioSourceController in m_audioSourceControllerList )
		{
			// is this audio source playing something?
			if ( audioSourceController.IsFree() )
			{
				Debug.Log( "Playing sound " + sound );

				// play the sound!
				audioSourceController.Play( m_soundList[ (int) sound ], volume, pitch, loop );

				// nothing more to do here
				return;
			}
		}

		// we have failed to find a free audio source - complain
		Debug.Log( "Could not find a free audio source to play sound " + sound );
	}

	// stop a sound (stops all audio sources playing the given sound)
	public void StopSound( Sound sound )
	{
		// go through each audio source controller
		foreach ( var audioSourceController in m_audioSourceControllerList )
		{
			// is this audio source playing this sound?
			if ( audioSourceController.IsPlaying( m_soundList[ (int) sound ] ) )
			{
				// fade out the sound
				audioSourceController.FadeOut( 1.0f );
			}
		}
	}

	// stop all sounds
	public void StopAllLoopingSounds()
	{
		// go through each audio source controller
		foreach ( var audioSourceController in m_audioSourceControllerList )
		{
			if ( audioSourceController.IsLooping() )
			{
				audioSourceController.FadeOut( 0.25f );
			}
		}
	}

	// change the pitch of a sound (changes the pitch on all audio sources playing the given sound)
	public void SetFrequency( Sound sound, float pitch = 1.0f )
	{
		// go through each audio source controller
		foreach ( var audioSourceController in m_audioSourceControllerList )
		{
			// is this audio source playing this sound?
			if ( audioSourceController.IsPlaying( m_soundList[ (int) sound ] ) )
			{
				// fade out the sound
				audioSourceController.ChangePitch( pitch );
			}
		}
	}
}
