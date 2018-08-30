
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceflightUI : MonoBehaviour
{
	// text elements
	public TextMeshProUGUI m_messages;
	public TextMeshProUGUI m_currentOfficer;
	public TextMeshProUGUI m_coordinates;

	// the map object
	public RawImage m_map;

	// the countdown text
	public TextMeshProUGUI m_countdown;

	// our countdown text animation timer
	float m_timer;

	// whether or not we are currently animating the countdown text
	bool m_animatingCountdownText;

	// convenient access to the spaceflight controller
	SpaceflightController m_spaceflightController;

	// unity awake
	private void Awake()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();
	}

	// unity start
	void Start()
	{
		// we are not currently animating the countdown text
		m_animatingCountdownText = false;
	}

	// unity update
	void Update()
	{
		// are we animating the countdown text?
		if ( m_animatingCountdownText )
		{
			// update the timer
			m_timer += Time.deltaTime;

			// animate the opacity and the size of the numbers
			if ( m_timer < 0.1f )
			{
				// fade in
				m_countdown.alpha = m_timer * 10.0f;
				m_countdown.fontSize = 240.0f;
			}
			else if ( m_timer < 1.0f )
			{
				// fade out and shrink
				m_countdown.alpha = 1.0f - ( m_timer - 0.1f ) / 0.9f;
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

	// call this to fade in or out the map
	public void FadeMap( float alpha )
	{
		m_map.color = new Color( alpha, alpha, alpha );
	}

	// call this to change the current officer text
	public void ChangeOfficerText( string newOfficer )
	{
		m_currentOfficer.text = newOfficer;
	}

	// call this to change the message text
	public void ChangeMessageText( string newMessage )
	{
		m_messages.text = newMessage;
	}

	// call this to display the countdown text and animate it
	public void SetCountdownText( string newText )
	{
		// change the countdown text
		m_countdown.text = newText;

		// make it active
		m_countdown.gameObject.SetActive( true );

		// reset the timer
		m_timer = 0.0f;

		// we are now animating the countdown text
		m_animatingCountdownText = true;
	}

	// call this to update the coordinates above the map display
	public void UpdateCoordinates()
	{
		PlayerData playerData = DataController.m_instance.m_playerData;

		Vector3 gameCoordinates = Tools.WorldToGameCoordinates( playerData.m_starflight.m_hyperspaceCoordinates );

		int x = Mathf.RoundToInt( gameCoordinates.x );
		int y = Mathf.RoundToInt( gameCoordinates.z );

		m_coordinates.text = x.ToString() + "   " + y.ToString();
	}
}
