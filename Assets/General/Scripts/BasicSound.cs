
using UnityEngine;

public class BasicSound : MonoBehaviour
{
	// public stuff we want to set using the editor
	public int m_numberOfAudioSources;
	public AudioClip[] m_audioClip;

	// private stuff we don't want the editor to see
	private AudioSource [] m_audioSource;

	// this is called by unity once at the start of the level
	private void Start()
	{
		// create audio sources on the game object we are attached to
		m_audioSource = new AudioSource[ m_numberOfAudioSources ];

		for ( int i = 0; i < m_numberOfAudioSources; i++ )
		{
			// create a new audio source
			m_audioSource[ i ] = gameObject.AddComponent<AudioSource>();

			// don't play any sounds on awake
			m_audioSource[ i ].playOnAwake = false;
		}
	}

	// call this to play the sound now
	public void PlayOneShot( int soundIndex, int audioSourceIndex = 0 )
	{
		m_audioSource[ audioSourceIndex ].PlayOneShot( m_audioClip[ soundIndex ], 1.0f );
	}
}
