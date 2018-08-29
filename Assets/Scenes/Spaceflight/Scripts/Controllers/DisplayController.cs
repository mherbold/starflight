
using UnityEngine;
using TMPro;

public class DisplayController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public TextMeshProUGUI m_displayLabel;
	public GameObject m_status;
	public GameObject m_system;

	// displays
	public StatusDisplay m_statusDisplay;
	public SystemDisplay m_systemDisplay;
	public ShipDisplay m_currentDisplay;

	// unity awake
	void Awake()
	{
		// create displays
		m_statusDisplay = new StatusDisplay( m_status );
		m_systemDisplay = new SystemDisplay( m_system );
	}

	// unity start
	void Start()
	{
		// status display should be first
		ChangeDisplay( m_statusDisplay );
	}

	// unity update
	void Update()
	{
		// update the current display
		m_currentDisplay.Update();
	}

	// change the current display to a different one
	public void ChangeDisplay( ShipDisplay newDisplay )
	{
		// inactivate all of the display UI
		m_statusDisplay.Stop();
		m_systemDisplay.Stop();

		// change the current display
		m_currentDisplay = newDisplay;

		// fire it up
		m_currentDisplay.Start();

		// update the display label
		m_displayLabel.text = m_currentDisplay.GetLabel();

		// run the update
		m_currentDisplay.Update();
	}
}
