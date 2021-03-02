
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SaveGamePanel : Panel
{
	// the description for each save game slot
	public TextMeshProUGUI[] m_slotDescriptionText;

	// the "active" text object array
	public GameObject[] m_slotActiveText;

	// the overlay panel
	public GameObject m_overlayPanel;

	// the switch panel
	public GameObject m_switchPanel;

	// the copy panel
	public GameObject m_copyPanel;

	// the reset panel
	public GameObject m_resetPanel;

	// the switch button
	public Button m_switchButton;

	// the copy button
	public Button m_copyButton;

	// the reset button
	public Button m_resetButton;

	// the resume button
	public Button m_resumeButton;

	// the quit button
	public Button m_quit;

	// the switch cancel button
	public Button m_switchCancelButton;

	// the copy cancel button
	public Button m_copyCancelButton;

	// the reset no button
	public Button m_resetNoButton;

	// the callback object
	object m_callbackObject;

	// call this to open this panel
	public override bool Open()
	{
		// base panel open
		base.Open();

		// hide all of the pop up panels
		m_overlayPanel.SetActive( false );
		m_switchPanel.SetActive( false );
		m_copyPanel.SetActive( false );
		m_resetPanel.SetActive( false );

		// enable the main menu buttons
		EnableMenuButtons( true );

		// update the active slot indicators
		UpdateActive();

		// update the save game descriptions
		UpdateDescriptions();

		// select the resume button by default
		m_resumeButton.Select();

		// panel was opened
		return true;
	}

	// this is called when the closing animation is done
	public override void Closed()
	{
		// base planel closed
		base.Closed();

		// if we have a callback object let it know we have closed the panel
		if ( ( m_callbackObject != null ) && !m_callbackObject.Equals( null ) )
		{
			// get the object type
			var type = m_callbackObject.GetType();

			// get the method
			var method = type.GetMethod( "PanelWasClosed" );

			// invoke the method
			method.Invoke( m_callbackObject, null );
		}
	}

	// called when player clicks on switch
	public void SwitchClicked()
	{
		InputController.m_instance.Debounce();

		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// disable the menu buttons
		EnableMenuButtons( false );

		// activate the overlay
		m_overlayPanel.SetActive( true );

		// activate the switch panel
		m_switchPanel.SetActive( true );

		// select the cancel button by default
		m_switchCancelButton.Select();

		// play the activate sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// called when player clicks on copy
	public void CopyClicked()
	{
		InputController.m_instance.Debounce();

		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// disable the menu buttons
		EnableMenuButtons( false );

		// activate the overlay
		m_overlayPanel.SetActive( true );

		// activate the copy panel
		m_copyPanel.SetActive( true );

		// select the cancel button by default
		m_copyCancelButton.Select();

		// play the activate sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// called when player clicks on reset
	public void ResetClicked()
	{
		InputController.m_instance.Debounce();

		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// disable the menu buttons
		EnableMenuButtons( false );

		// activate the overlay
		m_overlayPanel.SetActive( true );

		// activate the reset panel
		m_resetPanel.SetActive( true );

		// select the no button by default
		m_resetNoButton.Select();

		// play the activate sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// called when player clicks on resume
	public void ResumeClicked()
	{
		InputController.m_instance.Debounce();

		// close this panel
		PanelController.m_instance.Close();
	}

	// called when the player clicks on quit
	public void QuitClicked()
	{
		InputController.m_instance.Debounce();

		// save the game
		DataController.m_instance.SaveActiveGame();

		// quit the game
		Application.Quit();
	}

	// called when player switches save game slots
	public void SwitchSaveGame( int i )
	{
		// do we want to switch to a different save game slot?
		if ( i != DataController.m_instance.m_activeSaveGameSlotNumber )
		{
			// yes - tell the data controller to switch
			DataController.m_instance.SetTargetSaveGameSlotNumber( i );

			// play the update sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Update );

			// close this panel
			PanelController.m_instance.Close();
		}
		else
		{
			// no - return to the main menu
			CancelClicked();
		}
	}

	// called when player copies the save game
	public void CopySaveGame( int i )
	{
		// do we want to copy to a different save game slot?
		if ( i != DataController.m_instance.m_activeSaveGameSlotNumber )
		{
			// yes - tell the data controller to copy
			DataController.m_instance.CopyActiveSaveGameSlot( i );

			// update the save game descriptions
			UpdateDescriptions();

			// enable the main menu buttons
			EnableMenuButtons( true );

			// hide the overlay
			m_overlayPanel.SetActive( false );

			// hide the copy panel
			m_copyPanel.SetActive( false );

			// select the resume button by default
			m_resumeButton.Select();

			// play the update sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Update );
		}
		else
		{
			// no - return to the main menu
			CancelClicked();
		}
	}

	// called when player wants to reset the current game
	public void ResetGame()
	{
		// tell the data controller to reset the game
		DataController.m_instance.ResetGame();

		// play the update sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Update );

		// close this panel
		PanelController.m_instance.Close();
	}

	// called when player clicks on cancel (or "no")
	public void CancelClicked()
	{
		InputController.m_instance.Debounce();

		// deselect all the buttons
		EventSystem.current.SetSelectedGameObject( null );

		// enable the menu buttons
		EnableMenuButtons( true );

		// hide the overlay
		m_overlayPanel.SetActive( false );

		// hide all of the pop up panels
		m_switchPanel.SetActive( false );
		m_copyPanel.SetActive( false );
		m_resetPanel.SetActive( false );

		// select the resume button by default
		m_resumeButton.Select();

		// play the deactivate sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
	}

	// pass in the object you want to update when the panel has closed
	public void SetCallbackObject( object callbackObject )
	{
		m_callbackObject = callbackObject;
	}

	// call this to update the active slot indicators
	void UpdateActive()
	{
		for ( var i = 0; i < DataController.c_numSaveGameSlots; i++ )
		{
			m_slotActiveText[ i ].SetActive( i == DataController.m_instance.m_activeSaveGameSlotNumber );
		}
	}

	// call this to update the save game descriptions
	void UpdateDescriptions()
	{
		// go through each save game slot
		for ( var i = 0; i < DataController.c_numSaveGameSlots; i++ )
		{
			// get the player data for that save game slot
			var playerData = DataController.m_instance.m_playerDataList[ i ];

			// the date and time
			var description = "Date: <color=\"blue\">" + playerData.m_general.m_currentStardateYMD + "</color>   Time: <color=\"blue\">" + playerData.m_general.m_hour.ToString( "D2" ) + ":" + playerData.m_general.m_minute.ToString( "D2" ) + "</color>\n";

			// the location
			description += "Location: <color=\"blue\">";

			Vector3 coordinates;

			switch ( playerData.m_general.m_location )
			{
				case PD_General.Location.Starport:
					description += "Starport</color>\n";
					break;
				case PD_General.Location.DockingBay:
					description += "Docking Bay</color>\n";
					break;
				case PD_General.Location.JustLaunched:
					description += "Just Launched</color>\n";
					break;
				case PD_General.Location.StarSystem:
					coordinates = Tools.WorldToGameCoordinates( playerData.m_general.m_lastStarSystemCoordinates );
					description += "Star System</color>   Coordinates: <color=\"blue\">" + Mathf.RoundToInt( coordinates.x ) + ", " + Mathf.RoundToInt( coordinates.z ) + "</color>\n";
					break;
				case PD_General.Location.Hyperspace:
					coordinates = Tools.WorldToGameCoordinates( playerData.m_general.m_lastHyperspaceCoordinates );
					description += "Hyperspace</color>   Coordinates: <color=\"blue\">" + Mathf.RoundToInt( coordinates.x ) + ", " + Mathf.RoundToInt( coordinates.z ) + "</color>\n";
					break;
				case PD_General.Location.InOrbit:
					description += "In Orbit</color>\n";
					break;
				case PD_General.Location.Planetside:
					description += "Planetside</color>\n";
					break;
				case PD_General.Location.Encounter:
					description += "In Encounter</color>\n";
					break;
			}

			// the ship name and cargo
			var shipName = ( playerData.m_playerShip.m_name == "" ) ? "No Name" : playerData.m_playerShip.m_name;
			var numCargoPods = ( playerData.m_playerShip.m_numCargoPods == 0 ) ? "None" : playerData.m_playerShip.m_numCargoPods.ToString();
			var cargoUsage = (float) playerData.m_playerShip.m_volumeUsed / (float) playerData.m_playerShip.m_volume * 100.0f;

			description += "Ship: <color=\"blue\">" + shipName + "</color>";
			description += "   Cargo Pods: <color=\"blue\">" + numCargoPods + "</color>";
			description += "   Cargo: <color=\"blue\">" + cargoUsage.ToString( "N1" ) + "% Full</color>\n";

			// ship part classes
			description += "Engines: <color=\"blue\">" + playerData.m_playerShip.GetEngines().m_name + "</color>";
			description += "   Shields: <color=\"blue\">" + playerData.m_playerShip.GetShielding().m_name + "</color>";
			description += "   Armor: <color=\"blue\">" + playerData.m_playerShip.GetArmor().m_name + "</color>";
			description += "   Missle Launcher: <color=\"blue\">" + playerData.m_playerShip.GetMissileLauncher().m_name + "</color>";
			description += "   Laser Cannon: <color=\"blue\">" + playerData.m_playerShip.GetLaserCannon().m_name + "</color>";

			// replace the description text for this save game slot
			m_slotDescriptionText[ i ].text = description;
		}
	}

	// call this to enable or disable the main menu buttons
	void EnableMenuButtons( bool enable )
	{
		m_switchButton.interactable = enable;
		m_copyButton.interactable = enable;
		m_resetButton.interactable = enable;
		m_resumeButton.interactable = enable;
	}
}
