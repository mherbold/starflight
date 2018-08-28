
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

	// this is called by unity before start
	private void Awake()
	{
	}

	// this is called by unity once at the start of the level
	private void Start()
	{
		// displays
		m_statusDisplay = new StatusDisplay( m_status );
		m_systemDisplay = new SystemDisplay( m_system );

		// status display should be first
		ChangeDisplay( m_statusDisplay );
	}

	// this is called by unity every frame
	private void Update()
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
