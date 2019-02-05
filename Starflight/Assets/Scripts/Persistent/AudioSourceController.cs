
using UnityEngine;

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

	// create the audio source component
	public AudioSourceController( GameObject gameObject )
	{
		m_audioSource = gameObject.AddComponent<AudioSource>();
	}

	// returns true if the audio source is not currently playing any sound
	public bool IsFree()
	{
		return !m_audioSource.isPlaying;
	}

	// returns true if this audio source is playing the given audio clip
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

	// use this audio source to play an audio clip at the given volume and pitch
	public void Play( AudioClip clip, float volume, float pitch, bool loop )
	{
		m_volume = volume;

		m_audioSource.clip = clip;
		m_audioSource.volume = m_volume;
		m_audioSource.pitch = pitch;
		m_audioSource.loop = loop;

		m_audioSource.Play();
	}

	// start fading out this audio source
	public void FadeOut( float fadeOutTime )
	{
		m_isFadingOut = true;
		m_fadeOutTime = fadeOutTime;
	}

	// change the pitch of this audio source
	public void ChangePitch( float pitch )
	{
		m_audioSource.pitch = pitch;
	}

	// unity fixed update
	public void FixedUpdate()
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
}
