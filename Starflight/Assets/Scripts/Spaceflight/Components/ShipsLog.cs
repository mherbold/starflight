
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using System.Collections.Generic;

public class ShipsLog : MonoBehaviour
{
	[SerializeField] GameObject m_selectionXform = default;

	[SerializeField] Image m_upArrowImage = default;
	[SerializeField] Image m_downArrowImage = default;

	[SerializeField] TextMeshProUGUI m_entries = default;
	[SerializeField] TextMeshProUGUI m_message = default;

	[SerializeField] GameObject m_entriesMask = default;

	List<PD_ShipsLog.Entry> m_entryList;

	int m_currentIndex = 0;

	bool m_initialized = false;

	float m_currentEntriesOffset;

	Vector3 m_baseSelectionOffsetMin;
	Vector3 m_baseSelectionOffsetMax;

	float m_ignoreControllerTimer;

	// unity update
	void Update()
	{
		// update the ignore controller timer
		m_ignoreControllerTimer = Mathf.Max( 0.0f, m_ignoreControllerTimer - Time.deltaTime );

		// if the player hits cancel (esc) or submit (enter) close the ships log
		if ( InputController.m_instance.m_cancel || InputController.m_instance.m_submit )
		{
			Hide();

			InputController.m_instance.Debounce();

			SpaceflightController.m_instance.m_buttonController.DeactivateButton();
		}
		else
		{
			// do we want to ignore the controls?
			if ( m_ignoreControllerTimer == 0.0f )
			{
				// no - get the controller stick position
				var controllerY = InputController.m_instance.m_y;

				// check if we want to change entries
				if ( ( controllerY < -0.5f ) || ( controllerY > 0.5f ) )
				{
					// yes - remember the current index
					var currentIndex = m_currentIndex;

					// ignore the controller for a bit
					m_ignoreControllerTimer = 0.15f;

					// update the current index
					if ( controllerY > 0.5f )
					{
						m_currentIndex--;
					}
					else
					{
						m_currentIndex++;
					}

					// don't let the index go out of range
					if ( m_currentIndex < 0 )
					{
						m_currentIndex = 0;
					}
					else if ( m_currentIndex >= m_entryList.Count )
					{
						m_currentIndex = m_entryList.Count - 1;
					}

					// did the index change?
					if ( m_currentIndex != currentIndex )
					{
						// yes - update the display
						UpdateDisplay();

						// make some noise
						SoundController.m_instance.PlaySound( SoundController.Sound.Click );
					}
				}
			}
		}
	}

	// initialize
	public void Initialize()
	{
		// do this only if we haven't already initialized
		if ( !m_initialized )
		{
			// initialize only once
			m_initialized = true;

			// remember the base selection offset
			var rectTransform = m_selectionXform.GetComponent<RectTransform>();

			m_baseSelectionOffsetMin = rectTransform.offsetMin;
			m_baseSelectionOffsetMax = rectTransform.offsetMax;
		}
	}

	// hide the ships log
	public void Hide()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// unpause the game
		SpaceflightController.m_instance.m_gameIsPaused = false;

		// make this game object not active
		gameObject.SetActive( false );

		// show the radar (if we are in star system or hyperspace)
		if ( ( playerData.m_general.m_location == PD_General.Location.StarSystem ) || ( playerData.m_general.m_location == PD_General.Location.Hyperspace ) )
		{
			SpaceflightController.m_instance.m_radar.Hide();
		}
	}

	// show the ships log
	public void Show( List<PD_ShipsLog.Entry> entryList )
	{
		// initialize this component
		Initialize();

		// pause the game
		SpaceflightController.m_instance.m_gameIsPaused = true;

		// make this game object active
		gameObject.SetActive( true );

		// hide the radar
		SpaceflightController.m_instance.m_radar.Hide();

		// play a sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Beep );

		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;

		// remember which list of entries we want to show
		m_entryList = entryList;

		// is the list empty?
		if ( m_entryList.Count == 0 )
		{
			// yes - tell the player there's nothing in here
			m_entries.text = "";
			m_message.text = "There are no entries in this log.";

			// hide the selection xform
			m_selectionXform.SetActive( false );
		}
		else
		{
			// populate the entries
			m_entries.text = "";

			foreach ( var entry in m_entryList )
			{
				if ( m_entries.text != "" )
				{
					m_entries.text += "\n";
				}

				m_entries.text += entry.m_stardate;
			}

			// select the first entry
			m_currentIndex = 0;

			// update the display
			UpdateDisplay();

			// show the selection xform
			m_selectionXform.SetActive( true );
		}
	}

	// returns true if the ships log is open
	public bool IsOpen()
	{
		return gameObject.activeInHierarchy;
	}

	void UpdateDisplay()
	{
		// get the entry
		var entry = m_entryList[ m_currentIndex ];

		// update the message
		m_message.text = "<color=#0ff>" + entry.m_header + "</color>\n" + entry.m_message;

		// calculate the height of each entry
		var rowHeight = m_entries.renderedHeight / m_entryList.Count;

		// show the up arrow only if the first item is not selected
		m_upArrowImage.gameObject.SetActive( ( m_currentIndex > 0 ) );

		// show the down arrow only if the last part is not selected
		m_downArrowImage.gameObject.SetActive( m_currentIndex < ( m_entryList.Count - 1 ) );

		// set the position of the selector
		var selectorOffset = m_currentIndex * rowHeight;

		// force the text object to update (so we can get the correct height)
		m_entries.ForceMeshUpdate();

		// force the canvas to update
		Canvas.ForceUpdateCanvases();

		// get the height of the entries mask
		var entriesMaskHeight = m_entriesMask.GetComponent<RectTransform>().rect.height;

		// make sure the selector is visible
		while ( ( selectorOffset < ( m_currentEntriesOffset + rowHeight ) ) && ( m_currentEntriesOffset > 0.0f ) )
		{
			m_currentEntriesOffset -= rowHeight;
		}

		while ( ( selectorOffset > ( entriesMaskHeight + m_currentEntriesOffset - rowHeight ) ) && ( m_currentEntriesOffset < ( ( rowHeight * m_entryList.Count ) - entriesMaskHeight ) ) )
		{
			m_currentEntriesOffset += rowHeight;
		}
		
		// scroll the entries
		m_entries.rectTransform.offsetMax = new Vector3( 0.0f, m_currentEntriesOffset, 0.0f );

		var rectTransform = m_selectionXform.GetComponent<RectTransform>();

		rectTransform.offsetMin = m_baseSelectionOffsetMin + new Vector3( 0.0f, m_currentEntriesOffset - selectorOffset, 0.0f );
		rectTransform.offsetMax = m_baseSelectionOffsetMax + new Vector3( 0.0f, m_currentEntriesOffset - selectorOffset, 0.0f );
	}
}
