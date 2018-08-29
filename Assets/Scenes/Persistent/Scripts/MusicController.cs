using UnityEngine;

public class MusicController : MonoBehaviour
{
	public enum Track
	{
		None,
		Intro,
		Starport,
		DockingBay,
		StarSystem,
		Hyperspace,
		Count
	};

	public enum Phase
	{
		None,
		FadingOut,
		FadingIn
	};

	// static reference to this instance
	public static MusicController m_instance;

	// how loud we want to play music
	public float m_volume = 0.35f;

	// how quickly we want to fade out and then into a new track
	public float m_trackChangeTime = 2.0f;

	// all of our music clips
	public AudioClip[] m_musicTrackList;

	// the audio source
	AudioSource m_audioSource;

	// the track we are currently playing
	Track m_currentTrack;

	// the track we want to play next
	Track m_nextTrack;

	// the current music volume
	float m_currentVolume;

	// the volume we have started transitioning from
	float m_transitionVolume;

	// the current transition timer
	float m_timer;

	// the current transition phase
	Phase m_phase;

	// the constructor
	void Awake()
	{
		// remember this instance to this
		m_instance = this;

		// create the audio source
		m_audioSource = gameObject.AddComponent<AudioSource>();

		// we are not currently playing anything
		m_currentTrack = m_nextTrack = Track.None;
	}

	// use this for initialization
	void Start()
	{
		// make sure we have the right number of music tracks set up in the editor
		if ( m_musicTrackList.Length != (int) Track.Count )
		{
			Debug.Log( "The number of music tracks should be " + Track.Count + "!" );
		}
	}

	// update is called once per frame
	void Update()
	{
		// check if we are transitioning to a new track
		if ( m_phase != Phase.None )
		{
			// yep - update the timer
			m_timer += Time.deltaTime;

			// normalize timer to 0..1 range
			float normalizedTime = m_timer / m_trackChangeTime;

			// process fading out transition
			if ( m_phase == Phase.FadingOut )
			{
				// calculate the new volume
				m_currentVolume = Mathf.Lerp( m_transitionVolume, 0.0f, normalizedTime / 0.5f );

				// are we done fading out?
				if ( normalizedTime >= 0.5f )
				{
					// update the current track
					m_currentTrack = m_nextTrack;

					// reset the next track to none
					m_nextTrack = Track.None;

					// do we have a next track?
					if ( m_currentTrack == Track.None )
					{
						// no - end the transition now
						m_phase = Phase.None;

						// Debug.Log( "No music being played now." );

						m_audioSource.Stop();
					}
					else
					{
						// yes - switch to the fading in phase
						m_phase = Phase.FadingIn;

						// play the new track
						m_audioSource.clip = m_musicTrackList[ (int) m_currentTrack ];
						m_audioSource.loop = true;

						m_audioSource.Play();

						// Debug.Log( "Fading in " + m_currentTrack + "." );
					}
				}
			}

			// process fading in transition
			if ( m_phase == Phase.FadingIn )
			{
				// calculate the new volume
				m_currentVolume = Mathf.Lerp( 0.0f, m_volume, ( normalizedTime - 0.5f ) / 0.5f );

				// are we done fading in?
				if ( normalizedTime >= 1.0f )
				{
					// yes - end the transition
					m_phase = Phase.None;

					// Debug.Log( "Fade in complete." );
				}
			}

			// update the music volume
			m_audioSource.volume = m_currentVolume;
		}
	}

	// switch to a new track
	public void ChangeToTrack( Track track )
	{
		// make sure we aren't already transitioning to a new track
		if ( m_phase != Phase.None )
		{
			Debug.Log( "Attempting to switch to a new music track while already in transition!" );
		}
		else
		{
			// Debug.Log( "Switching to music track " + track + "." );

			// don't do anything if we are already playing this track
			if ( track != m_currentTrack )
			{
				// has the audio source started playing?
				if ( !m_audioSource.isPlaying )
				{
					// nope - go ahead and fire up the new track without any transition
					m_currentTrack = track;
					m_audioSource.clip = m_musicTrackList[ (int) m_currentTrack ];

					if ( track == Track.Intro )
					{
						m_audioSource.volume = m_currentVolume = 1.0f;
						m_audioSource.loop = false;
					}
					else
					{
						m_audioSource.volume = m_currentVolume = m_volume;
						m_audioSource.loop = true;
					}

					m_audioSource.Play();

					// Debug.Log( "Playing " + track + " right away because there is no current music being played." );
				}
				else
				{
					// yep - start transitioning to the new track
					m_transitionVolume = m_currentVolume;
					m_nextTrack = track;
					m_phase = Phase.FadingOut;
					m_timer = 0.0f;

					// Debug.Log( "Fading out music track " + m_currentTrack + "." );
				}
			}
		}
	}
}
