
using UnityEngine;

public class BasicSound : MonoBehaviour
{
	// public stuff we want to set using the editor
	public AudioClip[] m_audioClip;

	// private stuff we don't want the editor to see
	private AudioSource m_audioSource;

	// this is called by unity once at the start of the level
	private void Start()
	{
		// create an audio source on the game object we are attached to
		m_audioSource = gameObject.AddComponent<AudioSource>();

		// don't play any sounds on awake
		m_audioSource.playOnAwake = false;
	}

	// call this to play the sound now
	public void PlayOneShot( int soundIndex )
	{
		m_audioSource.PlayOneShot( m_audioClip[ soundIndex ], 1.0f );
	}
}
