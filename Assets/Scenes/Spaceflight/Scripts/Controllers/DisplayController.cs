
using UnityEngine;
using TMPro;

public class DisplayController : MonoBehaviour
{
	// the display label
	public TextMeshProUGUI m_displayLabel;

	// the controllers for each display
	public StatusDisplay m_statusDisplay;
	public SystemDisplay m_systemDisplay;

	// the current display
	ShipDisplay m_currentDisplay;

	// unity start
	void Start()
	{
		// status display should be first
		ChangeDisplay( m_statusDisplay );
	}

	// change the current display to a different one
	public void ChangeDisplay( ShipDisplay newDisplay )
	{
		// inactivate all of the display UI
		m_statusDisplay.Hide();
		m_systemDisplay.Hide();

		// change the current display
		m_currentDisplay = newDisplay;

		// fire it up
		m_currentDisplay.Show();

		// update the display label
		m_displayLabel.text = m_currentDisplay.GetLabel();

		// run the update
		m_currentDisplay.Update();
	}
}
