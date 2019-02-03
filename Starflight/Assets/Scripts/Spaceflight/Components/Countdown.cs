
using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
	// the countdown text
	TextMeshProUGUI m_countdown;

	// the countdown animator
	Animator m_animator;

	// the current count
	int m_count;

	// whether or not we have been initialized already
	bool m_initialized;

	// call this to hide the countdown object
	public void Hide()
	{
		// disable the countdown object
		gameObject.SetActive( false );
	}

	// call this to show the countdown object and start the animation
	public void Show()
	{
		Initialize();

		gameObject.SetActive( true );

		m_count = 5;

		m_countdown.text = m_count.ToString();

		m_animator.Play( "Countdown" );
	}

	// initialize the countdown
	void Initialize()
	{
		if ( m_initialized )
		{
			return;
		}

		m_countdown = GetComponent<TextMeshProUGUI>();

		m_animator = GetComponent<Animator>();

		m_initialized = true;
	}

	// everything below this point are animator callbacks

	public void DecrementCountdownText()
	{
		m_count--;

		m_countdown.text = m_count.ToString();
	}

	public void StopCountdown()
	{
		Hide();
	}
}
