
using UnityEngine;

public class UISoundController : MonoBehaviour
{
	public enum UISound
	{
		None,
		Click,
		Activate,
		Deactivate,
		Update,
		Error,
	};

	// public stuff we want to set using the editor
	public AudioClip m_clickAudioClip;
	public AudioClip m_updateAudioClip;
	public AudioClip m_activateAudioClip;
	public AudioClip m_deactivateAudioClip;
	public AudioClip m_errorAudioClip;

	// private stuff we don't want the editor to see
	private AudioSource m_audioSource;
	private UISound m_soundToPlay;

	// this is called by unity once at the start of the level
	private void Start()
	{
		// create an audio source on the game object we are attached to
		m_audioSource = gameObject.AddComponent<AudioSource>();

		// don't play any sounds on awake
		m_audioSource.playOnAwake = false;

		// we are not wanting to play any sound at this time
		m_soundToPlay = UISound.None;
	}

	// this is called by unity every frame
	private void Update()
	{
		// do we want to play a sound now?
		if ( m_soundToPlay != UISound.None )
		{
			// play the selected clip
			switch ( m_soundToPlay )
			{
				case UISound.Click: m_audioSource.PlayOneShot( m_clickAudioClip ); break;
				case UISound.Activate: m_audioSource.PlayOneShot( m_activateAudioClip ); break;
				case UISound.Deactivate: m_audioSource.PlayOneShot( m_deactivateAudioClip ); break;
				case UISound.Update: m_audioSource.PlayOneShot( m_updateAudioClip ); break;
				case UISound.Error: m_audioSource.PlayOneShot( m_errorAudioClip ); break;
			}

			// reset to no sound
			m_soundToPlay = UISound.None;
		}
	}

	// call this to queue up a sound
	public void Play( UISound soundIndex )
	{
		// update sound to play only if it is a higher index
		if ( soundIndex > m_soundToPlay )
		{
			m_soundToPlay = soundIndex;

			// Debug.Log( "m_soundToPlay = " + m_soundToPlay );
		}
	}

	// call this to cancel all sounds
	public void CancelSounds()
	{
		m_soundToPlay = UISound.None;
	}
}
