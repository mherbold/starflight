using UnityEngine;

public class SoundController : MonoBehaviour
{
	public enum Sound
	{
		Click,
		Activate,
		Deactivate,
		Update,
		Error,
		Close,
		Open,
		Transport,
		DockingBayDoorOpen,
		DockingBayDoorClose,
		Decompression,
		Launch,
		Countdown,
		EnterWarp,
		ExitWarp,
		Scanning,
		Count
	};

	// static reference to this instance
	public static SoundController m_instance;

	// this is the max number of sounds we can play at the same time
	public int m_maxSimultaneousSounds = 8;

	// all of our sound clips
	public AudioClip m_click;
	public AudioClip m_activate;
	public AudioClip m_deactivate;
	public AudioClip m_update;
	public AudioClip m_error;
	public AudioClip m_close;
	public AudioClip m_open;
	public AudioClip m_transport;
	public AudioClip m_dockingBayDoorOpen;
	public AudioClip m_dockingBayDoorClose;
	public AudioClip m_decompression;
	public AudioClip m_launch;
	public AudioClip m_countdown;
	public AudioClip m_enterWarp;
	public AudioClip m_exitWarp;
	public AudioClip m_scanning;

	// the sound list (to use with the enum)
	AudioClip[] m_soundList;

	// the audio sources
	class AudioSourceController
	{
		// the audio source
		public AudioSource m_audioSource;

		// how loud to play the sound
		public float m_volume;

		// are we fading this sound out?
		public bool m_isFadingOut;

		// the fade out time
		public float m_fadeOutTime;

		public AudioSourceController( GameObject gameObject )
		{
			m_audioSource = gameObject.AddComponent<AudioSource>();
		}

		public bool IsFree()
		{
			return !m_audioSource.isPlaying;
		}

		public bool IsPlaying( AudioClip clip )
		{
			if ( m_audioSource.isPlaying )
			{
				if ( m_audioSource.clip == clip )
				{
					return true;
				}
			}

			return false;
		}

		public void Play( AudioClip clip, float volume )
		{
			m_volume = volume;

			m_audioSource.clip = clip;
			m_audioSource.volume = m_volume;

			m_audioSource.Play();
		}

		public void FadeOut( float fadeOutTime )
		{
			m_isFadingOut = true;
			m_fadeOutTime = fadeOutTime;
		}

		public void Update()
		{
			if ( m_isFadingOut )
			{
				if ( m_audioSource.isPlaying )
				{
					m_audioSource.volume -= Time.deltaTime * m_volume / m_fadeOutTime;

					if ( m_audioSource.volume <= 0.0f )
					{
						m_audioSource.volume = 0.0f;

						m_audioSource.Stop();

						m_isFadingOut = false;
					}
				}
				else
				{
					m_isFadingOut = false;
				}
			}
		}
	};

	AudioSourceController[] m_audioSourceControllerList;

	// unity awake
	void Awake()
	{
		// remember this instance to this
		m_instance = this;

		// create enough audio sources to be able to play the maximum number of sounds at once
		m_audioSourceControllerList = new AudioSourceController[ m_maxSimultaneousSounds ];

		for ( int i = 0; i < m_maxSimultaneousSounds; i++ )
		{
			m_audioSourceControllerList[ i ] = new AudioSourceController( gameObject );
		}

		// build the sound list
		m_soundList = new AudioClip[ (int) Sound.Count ];

		m_soundList[ (int) Sound.Click ] = m_click;
		m_soundList[ (int) Sound.Activate ] = m_activate;
		m_soundList[ (int) Sound.Deactivate ] = m_deactivate;
		m_soundList[ (int) Sound.Update ] = m_update;
		m_soundList[ (int) Sound.Error ] = m_error;
		m_soundList[ (int) Sound.Close ] = m_close;
		m_soundList[ (int) Sound.Open ] = m_open;
		m_soundList[ (int) Sound.Transport ] = m_transport;
		m_soundList[ (int) Sound.DockingBayDoorOpen ] = m_dockingBayDoorOpen;
		m_soundList[ (int) Sound.DockingBayDoorClose ] = m_dockingBayDoorClose;
		m_soundList[ (int) Sound.Decompression ] = m_decompression;
		m_soundList[ (int) Sound.Launch ] = m_launch;
		m_soundList[ (int) Sound.Countdown ] = m_countdown;
		m_soundList[ (int) Sound.EnterWarp ] = m_enterWarp;
		m_soundList[ (int) Sound.ExitWarp ] = m_exitWarp;
		m_soundList[ (int) Sound.Scanning ] = m_scanning;
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
		foreach ( AudioSourceController audioSourceController in m_audioSourceControllerList )
		{
			audioSourceController.Update();
		}
	}

	// play a sound
	public void PlaySound( Sound sound )
	{
		// go through each audio source controller
		foreach ( AudioSourceController audioSourceController in m_audioSourceControllerList )
		{
			// is this audio source playing something?
			if ( audioSourceController.IsFree() )
			{
				// play the sound!
				// TODO - use the volume level
				audioSourceController.Play( m_soundList[ (int) sound ], 1.0f );

				// nothing more to do here
				return;
			}
		}

		// we have failed to find a free audio source - complain
		Debug.Log( "Could not find a free audio source to play sound " + sound );
	}

	// stop a sound
	public void StopSound( Sound sound )
	{
		// go through each audio source controller
		foreach ( AudioSourceController audioSourceController in m_audioSourceControllerList )
		{
			// is this audio source playing this sound?
			if ( audioSourceController.IsPlaying( m_soundList[ (int) sound ] ) )
			{
				// fade out the sound
				audioSourceController.FadeOut( 1.0f );
			}
		}
	}
}
