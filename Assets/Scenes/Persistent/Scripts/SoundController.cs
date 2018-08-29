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
		Decompression,
		Launch,
		Countdown,
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
	public AudioClip m_decompression;
	public AudioClip m_launch;
	public AudioClip m_countdown;

	// the audio source
	AudioSource[] m_audioSourceList;

	// the sound list (to use with the enum)
	AudioClip[] m_soundList;

	// unity awake
	void Awake()
	{
		// remember this instance to this
		m_instance = this;

		// create enough audio sources to be able to play the maximum number of sounds at once
		m_audioSourceList = new AudioSource[ m_maxSimultaneousSounds ];

		for ( int i = 0; i < m_maxSimultaneousSounds; i++ )
		{
			m_audioSourceList[ i ] = gameObject.AddComponent<AudioSource>();
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
		m_soundList[ (int) Sound.Decompression ] = m_decompression;
		m_soundList[ (int) Sound.Launch ] = m_launch;
		m_soundList[ (int) Sound.Countdown ] = m_countdown;
	}

	// use this for initialization
	void Start()
	{
	}

	// update is called once per frame
	void Update()
	{
	}

	// play a sound
	public void PlaySound( Sound sound )
	{
		// find the first free audio source (one that is not playing anything at the moment)
		foreach ( AudioSource audioSource in m_audioSourceList )
		{
			// is this audio source playing something?
			if ( !audioSource.isPlaying )
			{
				// no - great so switch the clip in this audio source
				audioSource.clip = m_soundList[ (int) sound ];

				// play the clip
				audioSource.Play();

				// debug info
				// Debug.Log( "Playing sound " + sound + "." );

				// nothing more to do here
				return;
			}
		}

		// we have failed to find a free audio source - complain
		Debug.Log( "Could not find a free audio source to play sound " + sound + "." );
	}
}
