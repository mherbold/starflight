
using UnityEngine;
using TMPro;

public class DisplayController : MonoBehaviour
{
	// the display label
	public TextMeshProUGUI m_displayLabel;

	// the controllers for each display
	public StatusDisplay m_statusDisplay;
	public SystemDisplay m_systemDisplay;
	public SensorsDisplay m_sensorsDisplay;
	public TerrainMapDisplay m_terrainMapDisplay;

	// the current display
	ShipDisplay m_currentDisplay;

	// unity start
	void Start()
	{
		// show the status display by default
		if ( m_currentDisplay == null )
		{
			ChangeDisplay( m_statusDisplay );
		}
	}

	// change the current display to a different one
	public void ChangeDisplay( ShipDisplay newDisplay )
	{
		// Debug.Log( "Showing display " + newDisplay );

		// inactivate all of the display UI
		m_statusDisplay.Hide();
		m_systemDisplay.Hide();
		m_sensorsDisplay.Hide();
		m_terrainMapDisplay.Hide();

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
