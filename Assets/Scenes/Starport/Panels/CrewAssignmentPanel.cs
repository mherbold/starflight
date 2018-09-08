
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrewAssignmentPanel : Panel
{
	enum State
	{
		MenuBar, AssignPersonnel
	}

	// public stuff we want to set using the editor
	public TextMeshProUGUI m_positionValuesText;
	public TextMeshProUGUI m_nameText;
	public TextMeshProUGUI m_skillValuesText;
	public TextMeshProUGUI m_messageText;
	public Image m_upArrowImage;
	public Image m_downArrowImage;
	public Button m_assignButton;
	public Button m_exitButton;
	public GameObject m_selectionXform;
	public GameObject m_personnelFileXform;
	public GameObject m_messageXform;
	public GameObject m_enabledArrowsGameObject;
	public GameObject m_disabledArrowsGameObject;
	public GameObject m_bottomPanelGameObject;

	// the starport controller
	public StarportController m_starportController;

	// private stuff we don't want the editor to see
	State m_currentState;
	CrewAssignment.Role m_currentRole;
	int m_currentPersonnelId;
	Vector3 m_baseSelectionOffsetMin;
	Vector3 m_baseSelectionOffsetMax;
	float m_ignoreControllerTimer;

	// constant values
	const int c_numSkills = 5;

	// unity awake
	void Awake()
	{
		// reset the ignore controller timer
		m_ignoreControllerTimer = 0.0f;

		// remember the base selection offset
		RectTransform rectTransform = m_selectionXform.GetComponent<RectTransform>();
		m_baseSelectionOffsetMin = rectTransform.offsetMin;
		m_baseSelectionOffsetMax = rectTransform.offsetMax;
	}

	// panel open
	public override bool Open()
	{
		// base panel open
		base.Open();

		// reset the current state
		SwitchToMenuBarState();

		// panel was opened
		return true;
	}

	// panel closed
	public override void Closed()
	{
		// base panel closed
		base.Closed();

		// let the starport controller know
		m_starportController.PanelWasClosed();
	}

	// panel tick
	public override void Tick()
	{
		// update the ignore controller timer
		m_ignoreControllerTimer = Mathf.Max( 0.0f, m_ignoreControllerTimer - Time.deltaTime );

		// call the proper update function based on our current state
		switch ( m_currentState )
		{
			case State.AssignPersonnel:
			{
				UpdateControllerForAssignPersonnelState();
				break;
			}
		}
	}

	// controller updates for when we are currently selecting a race
	public void UpdateControllerForAssignPersonnelState()
	{
		// get access to the personnel player data
		Personnel personnel = DataController.m_instance.m_playerData.m_personnel;

		// keep track if we have centered both x and y
		bool xIsCentered = false;
		bool yIsCentered = false;

		// check if we moved the stick left
		if ( InputController.m_instance.m_west )
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				ChangeCurrentPersonnelId( ( m_currentPersonnelId + personnel.m_personnelList.Count - 1 ) % personnel.m_personnelList.Count );

				UpdateDisplay();
			}
		}
		else if ( InputController.m_instance.m_east ) // check if we moved the stick right
		{
			if ( m_ignoreControllerTimer == 0.0f )
			{
				m_ignoreControllerTimer = 0.3f;

				ChangeCurrentPersonnelId( ( m_currentPersonnelId + 1 ) % personnel.m_personnelList.Count );

				UpdateDisplay();
			}
		}
		else // we have centered the stick
		{
			xIsCentered = true;
		}

		// change position only if x is centered
		if ( xIsCentered )
		{
			// check if we moved the stick down
			if ( InputController.m_instance.m_south )
			{
				if ( m_ignoreControllerTimer == 0.0f )
				{
					m_ignoreControllerTimer = 0.3f;

					if ( m_currentRole < ( CrewAssignment.Role.Length - 1 ) )
					{
						ChangeCurrentRole( m_currentRole + 1 );

						UpdateDisplay();
					}
				}
			}
			else if ( InputController.m_instance.m_north ) // check if we have moved the stick up
			{
				if ( m_ignoreControllerTimer == 0.0f )
				{
					m_ignoreControllerTimer = 0.3f;

					if ( m_currentRole > CrewAssignment.Role.First )
					{
						ChangeCurrentRole( m_currentRole - 1 );

						UpdateDisplay();
					}
				}
			}
			else // we have centered the stick
			{
				yIsCentered = true;
			}
		}

		// check if we have centered the stick
		if ( xIsCentered && yIsCentered )
		{
			m_ignoreControllerTimer = 0.0f;
		}

		// check if we have pressed the cancel button
		if ( InputController.m_instance.CancelWasPressed() )
		{
			SwitchToMenuBarState();

			SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
		}
	}

	// call this to switch to the menu bar state
	void SwitchToMenuBarState()
	{
		// change the current state
		m_currentState = State.MenuBar;

		// hide the selection bar
		m_selectionXform.SetActive( false );

		// hide the enabled arrows
		m_enabledArrowsGameObject.SetActive( false );

		// show the disabled arrows
		m_disabledArrowsGameObject.SetActive( false );

		// enable the exit button
		m_exitButton.interactable = true;

		// update the assigned crewmember list
		UpdateAssignedCrewmemberList();

		// get access to the personnel player data
		Personnel personnel = DataController.m_instance.m_playerData.m_personnel;

		// check if we have at least one living crewmember in personnel
		if ( personnel.AnyLiving() )
		{
			// hide the crewmember panel
			m_bottomPanelGameObject.SetActive( false );

			// show the personnel file
			m_personnelFileXform.SetActive( true );

			// hide the message
			m_messageXform.SetActive( false );

			// activate the assign button
			m_assignButton.interactable = true;

			// select the assign button
			m_assignButton.Select();
		}
		else
		{
			// show the crewmember panel
			m_bottomPanelGameObject.SetActive( true );

			// hide the personnel file
			m_personnelFileXform.SetActive( false );

			// show the message
			m_messageXform.SetActive( true );

			// update the message
			m_messageText.text = "Report to Personnel: There are no living crewmembers on file";

			// inactivate the assign button
			m_assignButton.interactable = false;

			// select the exit button
			m_exitButton.Select();
		}
	}

	// call this to switch to the select race state
	void SwitchToAssignPersonnelState()
	{
		// change the current state
		m_currentState = State.AssignPersonnel;

		// start with the captain
		ChangeCurrentRole( CrewAssignment.Role.Captain );

		// show the crewmember panel
		m_bottomPanelGameObject.SetActive( true );

		// show the selection xform
		m_selectionXform.SetActive( true );

		// get access to the personnel player data
		Personnel personnel = DataController.m_instance.m_playerData.m_personnel;

		// show the enabled arrows only if we have more than one personnel on file
		if ( personnel.m_personnelList.Count > 1 )
		{
			m_enabledArrowsGameObject.SetActive( true );
		}
		else
		{
			m_disabledArrowsGameObject.SetActive( true );
		}

		// disable the assign and exit buttons
		m_assignButton.interactable = false;
		m_exitButton.interactable = false;

		// update the display
		UpdateDisplay();
	}

	// update the assigned crewmember list
	void UpdateAssignedCrewmemberList()
	{
		// get access to the crew assignment player data
		CrewAssignment crewAssignment = DataController.m_instance.m_playerData.m_crewAssignment;

		// start with an empty text string
		m_positionValuesText.text = "";

		// go through each position
		for ( CrewAssignment.Role role = CrewAssignment.Role.First; role < CrewAssignment.Role.Length; role++ )
		{
			// get the file id for the assigned crewmember
			if ( crewAssignment.IsAssigned( role ) )
			{
				// get the personnel file for that role
				Personnel.PersonnelFile personnelFile = crewAssignment.GetPersonnelFile( role );

				// add the crewmember's name
				m_positionValuesText.text += personnelFile.m_name;
			}
			else
			{
				// add the not assigned text
				m_positionValuesText.text += "[Not Assigned]";
			}

			if ( role < ( CrewAssignment.Role.Length - 1 ) )
			{
				m_positionValuesText.text += Environment.NewLine;
			}
		}
	}

	void ChangeCurrentRole( CrewAssignment.Role role )
	{
		// update the current position index
		m_currentRole = role;

		// get access to the crew assignment player data
		CrewAssignment crewAssignment = DataController.m_instance.m_playerData.m_crewAssignment;

		// check if we have don't have someone assigned to this position
		if ( !crewAssignment.IsAssigned( m_currentRole ) )
		{
			// automatically select the first personnel file
			ChangeCurrentPersonnelId( 0, true );
		}
		else
		{
			// get the current file id for this position
			int fileId = crewAssignment.GetFileId( m_currentRole );

			// get access to the personnel player data
			Personnel personnel = DataController.m_instance.m_playerData.m_personnel;

			// update the current personnel id
			m_currentPersonnelId = personnel.GetPersonnelId( fileId );
		}

		// play a sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Update );
	}

	void ChangeCurrentPersonnelId( int personnelId, bool forceUpdate = false )
	{
		// don't do anything if we aren't changing the file index to a different one
		if ( ( personnelId != m_currentPersonnelId ) || forceUpdate )
		{
			// update the current personnel id
			m_currentPersonnelId = personnelId;

			// get access to the crew assignment player data
			CrewAssignment crewAssignment = DataController.m_instance.m_playerData.m_crewAssignment;

			// get access to the personnel player data
			Personnel personnel = DataController.m_instance.m_playerData.m_personnel;

			// get the personnel file
			Personnel.PersonnelFile personnelFile = personnel.m_personnelList[ m_currentPersonnelId ];

			// assign this personnel to this position
			crewAssignment.Assign( m_currentRole, personnelFile.m_fileId );

			// update the assigned crewmember list
			UpdateAssignedCrewmemberList();

			// play a sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Update );
		}
	}

	void UpdateDisplay()
	{
		// show the up arrow only if we are not at the first position index
		m_upArrowImage.gameObject.SetActive( m_currentRole != CrewAssignment.Role.First );

		// show the down arrow only if we are not at the last position index
		m_downArrowImage.gameObject.SetActive( m_currentRole != ( CrewAssignment.Role.Length - 1 ) );

		// put the position selection box in the right place
		float offset = (int) m_currentRole * m_positionValuesText.renderedHeight / (int) CrewAssignment.Role.Length;

		RectTransform rectTransform = m_selectionXform.GetComponent<RectTransform>();
		rectTransform.offsetMin = m_baseSelectionOffsetMin + new Vector3( 0.0f, -offset, 0.0f );
		rectTransform.offsetMax = m_baseSelectionOffsetMax + new Vector3( 0.0f, -offset, 0.0f );

		// get access to the personnel player data
		Personnel personnel = DataController.m_instance.m_playerData.m_personnel;

		// get the personnel file
		Personnel.PersonnelFile personnelFile = personnel.m_personnelList[ m_currentPersonnelId ];

		// update the crewmember name
		if ( personnelFile.m_vitality > 0 )
		{
			m_nameText.text = personnelFile.m_name + " - " + personnelFile.m_vitality + "% vitality";
		}
		else
		{
			m_nameText.text = personnelFile.m_name + " - DEAD";
		}

		// update the skill values
		m_skillValuesText.text = "";

		for ( int skillIndex = 0; skillIndex < c_numSkills; skillIndex++ )
		{
			m_skillValuesText.text += personnelFile.GetSkill( skillIndex ).ToString();

			if ( skillIndex < ( c_numSkills - 1 ) )
			{
				m_skillValuesText.text += Environment.NewLine;
			}
		}
	}

	// this is called if we clicked on the assign button
	public void AssignClicked()
	{
		// switch to the create select race state
		SwitchToAssignPersonnelState();

		// play a ui sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Activate );
	}

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		// close this panel
		PanelController.m_instance.Close();
	}
}
