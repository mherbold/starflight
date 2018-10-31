
using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
	// the countdown text
	public TextMeshProUGUI m_countdown;

	// our countdown text animation timer
	float m_countdownTimer;

	// whether or not we are currently animating the countdown text
	bool m_animatingCountdownText;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
		// we are not currently animating the countdown text
		m_animatingCountdownText = false;

		// disable the countdown text object
		m_countdown.gameObject.SetActive( false );
	}

	// unity update
	void Update()
	{
		// are we animating the countdown text?
		if ( m_animatingCountdownText )
		{
			// update the timer
			m_countdownTimer += Time.deltaTime;

			// animate the opacity and the size of the numbers
			if ( m_countdownTimer < 0.1f )
			{
				// fade in
				m_countdown.alpha = m_countdownTimer * 10.0f;
				m_countdown.fontSize = 240.0f;
			}
			else if ( m_countdownTimer < 1.0f )
			{
				// fade out and shrink
				m_countdown.alpha = 1.0f - ( m_countdownTimer - 0.1f ) / 0.9f;
				m_countdown.fontSize = 140.0f + m_countdown.alpha * 100.0f;
			}
			else
			{
				// stop animating the countdown text
				m_animatingCountdownText = false;

				// disable the countdown text object
				m_countdown.gameObject.SetActive( false );
			}
		}
	}

	// call this to display the countdown text and animate it
	public void SetCountdownText( string newText )
	{
		// change the countdown text
		m_countdown.text = newText;

		// make it active
		m_countdown.gameObject.SetActive( true );

		// reset the timer
		m_countdownTimer = 0.0f;

		// we are now animating the countdown text
		m_animatingCountdownText = true;
	}
}
