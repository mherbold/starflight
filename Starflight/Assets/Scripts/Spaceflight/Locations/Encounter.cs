
using UnityEngine;

using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Encounter : MonoBehaviour
{
	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// the speed the alien ships move at
	public float m_alienShipSpeed;

	// the rate the alien ships turn at
	public float m_alienShipTurnRate;

	// the camera dolly speed
	public float m_cameraDollySpeed;

	// how often to update the target coordinates
	public float m_targetCoordinateUpdateFrequency;

	// the main encounter map
	public GameObject m_main;

	// the race models
	public GameObject[] m_alienRaceModelList;

	// alien ship models (need 8)
	public GameObject[] m_alienShipModelList;

	// template models that we will clone as needed (need 23)
	public GameObject[] m_alienShipModelTemplate;

	// the current encounter data (both player and game)
	public PD_Encounter m_pdEncounter;
	public GD_Encounter m_gdEncounter;

	// did we just enter an encounter from another location? (this will be false when reloading a game)
	bool m_justEnteredEncounter;

	// alien ship data
	PD_AlienShip[] m_alienShipList;

	// do we want to update the alien ships? (false if we are in communications)
	bool m_updateShips;

	// current dolly distance
	float m_currentDollyDistance;

	// the conversation
	string m_conversation;

	// are the aliens waiting for a response?
	bool m_waitingForResponse;

	// does the player want to communicate?
	bool m_playerWantsToCommunicate;

	// are we already in video chat?
	bool m_inVideoChat;

	// the player's chosen stance
	GD_Comm.Stance m_stance;

	// what the player wants to know about
	GD_Comm.Subject m_subject;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
		// don't do anything if we have a panel open
		if ( PanelController.m_instance.HasActivePanel() )
		{
			return;
		}

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// update the time to the next action
		m_pdEncounter.m_timeToNextAction -= Time.deltaTime;

		// update comm logic
		switch ( m_gdEncounter.m_race )
		{
			case GameData.Race.Mechan:

				UpdateMechans();
				break;
		}

		// do we want to update the ships (both alien and player)?
		if ( m_updateShips )
		{
			// update the position and rotation of the active alien ship models
			for ( var alienShipIndex = 0; alienShipIndex < m_alienShipModelList.Length; alienShipIndex++ )
			{
				var alienShipModel = m_alienShipModelList[ alienShipIndex ];

				if ( alienShipModel.activeInHierarchy )
				{
					var alienShip = m_alienShipList[ alienShipIndex ];

					var vessel = gameData.m_vesselList[ alienShip.m_vesselId ];

					switch ( vessel.m_race )
					{
						case GameData.Race.Mechan:
							UpdateMechanAlienShip( alienShip, alienShipModel );
							break;

						default:
							DefaultAlienShipUpdate( alienShip, alienShipModel );
							break;
					}
				}
			}

			// finalize alien ships and camera transform
			FinalizeAlienShips();

			// has the player left the encounter?
			if ( playerData.m_general.m_coordinates.magnitude >= 4096.0f )
			{
				// calculate the normalized exit direction vector
				var exitDirection = Vector3.Normalize( playerData.m_general.m_coordinates );

				// was the last location in hyperspace?
				if ( playerData.m_general.m_lastLocation == PD_General.Location.Hyperspace )
				{
					// yes - update the last hyperspace coordinates
					playerData.m_general.m_lastHyperspaceCoordinates += exitDirection * m_spaceflightController.m_encounterRange * 1.25f;
				}
				else 
				{
					// no - update the last star system coordinates
					playerData.m_general.m_lastStarSystemCoordinates += exitDirection * m_spaceflightController.m_encounterRange * 1.25f;
				}

				// yes - switch back to the last location
				m_spaceflightController.SwitchLocation( playerData.m_general.m_lastLocation );
			}
		}
	}

	// call this to let us know we have just entered an encounter
	public void JustEnteredEncounter()
	{
		m_justEnteredEncounter = true;
	}

	// call this to hide the encounter stuff
	public void Hide()
	{
		if ( !gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Hiding the encounter location." );

		// hide the encounter objects
		gameObject.SetActive( false );

		// slide the message box back in
		m_spaceflightController.m_messages.SlideIn();
	}

	// call this to show the encounter stuff
	public void Show()
	{
		if ( gameObject.activeInHierarchy )
		{
			return;
		}

		Debug.Log( "Showing the encounter location." );

		// show the hyperspace objects
		gameObject.SetActive( true );

		// show the main encounter location stuff
		m_main.SetActive( true );

		// make sure the camera is at the right height above the zero plane
		m_currentDollyDistance = 1024.0f;

		m_spaceflightController.m_player.DollyCamera( m_currentDollyDistance );
		m_spaceflightController.m_player.SetClipPlanes( 512.0f, 1536.0f );

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// move the ship to where we are in the encounter
		m_spaceflightController.m_player.transform.position = playerData.m_general.m_coordinates = playerData.m_general.m_lastEncounterCoordinates;

		// calculate the new rotation of the player
		var newRotation = Quaternion.LookRotation( playerData.m_general.m_currentDirection, Vector3.up );

		// update the player rotation
		m_spaceflightController.m_player.m_ship.rotation = newRotation;

		// unfreeze the player
		m_spaceflightController.m_player.Unfreeze();

		// fade in the map
		m_spaceflightController.m_map.StartFade( 1.0f, 2.0f );

		// show the status display
		m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_statusDisplay );

		// initialize the encounter
		Initialize();

		// center the encounter coordinates on the player
		m_pdEncounter.m_currentCoordinates = playerData.m_general.m_coordinates;

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Encounter );

		// play the alarm sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Alarm );

		// let the player know we've just entered encounter mode
		m_spaceflightController.m_messages.ChangeText( "<color=white>Scanners indicate unidentified object!</color>" );
	}

	// places ships etc at the start of the encounter
	void Initialize()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get the current encounter id
		var encounterId = playerData.m_general.m_currentEncounterId;

		// get to the encounter game data
		m_gdEncounter = gameData.m_encounterList[ encounterId ];

		// find the encounter in the player data (the list is continually sorted by distance so we have to search)
		foreach ( var encounter in playerData.m_encounterList )
		{
			if ( encounter.m_encounterId == encounterId )
			{
				m_pdEncounter = encounter;
				break;
			}
		}

		// allocate array for alien ship list
		m_alienShipList = new PD_AlienShip[ m_alienShipModelList.Length ];

		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		// do we want to add the first round of alien ships?
		if ( m_justEnteredEncounter )
		{
			// yes - reset all of the alien ships
			foreach ( var alienShip in alienShipList )
			{
				alienShip.m_addedToEncounter = false;
			}

			// add the first round of alien ships to the encounter
			AddAlienShips();

			// we've added the first round
			m_justEnteredEncounter = false;

			// reset the encounter action
			m_pdEncounter.m_nextAction = 0;
			m_pdEncounter.m_timeToNextAction = 0.0f;
		}
		else
		{
			// no - just reset the alien ship models
			ResetAlienShipModels();
		}

		// reset the comm game data
		foreach ( var comm in gameData.m_commList )
		{
			comm.m_shown = false;
		}

		// reset other stuff
		m_conversation = "";
		m_updateShips = true;
		m_waitingForResponse = false;
		m_playerWantsToCommunicate = false;

		// TEMP
		m_pdEncounter.m_nextAction = 0;

		// if the next action is above 100 then we are in comms with the alien ships
		if ( m_pdEncounter.m_nextAction >= 100 )
		{
			ConnectToAliens();
		}
	}

	// adds a number of alien ships to the encounter - up to the maximum allowed by the encounter
	void AddAlienShips()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		// go through alien ship slots (up to the maximum allowed at once)
		for ( var alienShipIndex = 0; alienShipIndex < m_gdEncounter.m_maxNumShipsAtOnce; alienShipIndex++ )
		{
			// no - go through all of the alien ships in the encounter and add the next one
			foreach ( var alienShip in alienShipList )
			{
				// has this alien ship already been added to the encounter?
				if ( alienShip.m_addedToEncounter )
				{
					// yes - skip it
					continue;
				}

				// is this alien ship dead?
				if ( alienShip.m_isDead )
				{
					// yes - skip it
					continue;
				}

				// generate a random position inside of a unit circle
				var randomPosition = Random.insideUnitCircle;

				Vector3 coordinates;

				if ( m_justEnteredEncounter )
				{
					// put alien ship in area approximately in the correct direction of approach
					coordinates = new Vector3( randomPosition.x, 0.0f, randomPosition.y ) * 256.0f + Vector3.Normalize( m_pdEncounter.m_currentCoordinates - playerData.m_general.m_lastHyperspaceCoordinates ) * 4096.0f;
				}
				else
				{
					// put alien ship in a random position on a circle around the player
					coordinates = Vector3.Normalize( new Vector3( randomPosition.x, 0.0f, randomPosition.y ) ) * 4096.0f;
				}

				// make alien ship face the center of the encounter space
				var direction = -Vector3.Normalize( coordinates );

				// update the alien ship
				alienShip.m_coordinates = coordinates;
				alienShip.m_targetCoordinates = Vector3.zero;
				alienShip.m_currentDirection = direction;
				alienShip.m_lastDirection = direction;
				alienShip.m_currentBankingAngle = 0.0f;
				alienShip.m_timeSinceLastTargetCoordinateChange = (float) alienShipIndex / (float) m_gdEncounter.m_maxNumShipsAtOnce * m_targetCoordinateUpdateFrequency;
				alienShip.m_addedToEncounter = true;

				// we are done adding an alien ship to this slot
				break;
			}
		}

		// reset the alien ship models
		ResetAlienShipModels();
	}

	void ResetAlienShipModels()
	{
		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		// inactivate all of the alien ship models
		foreach ( var alienShipModel in m_alienShipModelList )
		{
			alienShipModel.SetActive( false );
		}

		// start adding alien ship models
		var alienShipIndex = 0;

		// go through all of the alien ships in the encounter
		foreach ( var alienShip in alienShipList )
		{
			// is this ship in the encounter and alive?
			if ( alienShip.m_addedToEncounter && !alienShip.m_isDead )
			{
				// get the model we will update
				var alienShipModel = m_alienShipModelList[ alienShipIndex ];

				// remove old model
				Tools.DestroyChildrenOf( alienShipModel );

				// reset the transform of the model
				alienShipModel.transform.SetPositionAndRotation( Vector3.zero, Quaternion.identity );

				// clone the model
				var alienShipModelTemplate = m_alienShipModelTemplate[ alienShip.m_vesselId ];

				Instantiate( alienShipModelTemplate, alienShipModelTemplate.transform.localPosition, alienShipModelTemplate.transform.localRotation, alienShipModel.transform );

				// show the model
				alienShipModel.SetActive( true );

				// remember the alien ship associated with this model
				m_alienShipList[ alienShipIndex ] = alienShip;

				// next!
				alienShipIndex++;
			}
		}
	}

	void UpdateMechans()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// used in a few places
		GD_Comm comm;

		// is it time to take the next action?
		if ( m_pdEncounter.m_timeToNextAction <= 0 )
		{
			// yes - what is the next action?
			switch ( m_pdEncounter.m_nextAction )
			{
				case 0:

					// determine opening stance
					if ( playerData.m_general.m_mechan9Unlocked )
					{
						m_pdEncounter.m_stance = GD_Comm.Stance.Friendly;
						m_pdEncounter.m_nextAction = 3;
					}
					else
					{
						m_pdEncounter.m_stance = GD_Comm.Stance.Neutral;
						m_pdEncounter.m_nextAction = 1;
					}

					m_pdEncounter.m_timeToNextAction = Random.Range( 3.0f, 15.0f );

					break;

				case 1:

					// "scan" the player
					if ( ClosestDistanceToAlienShip() < 512.0f )
					{
						SoundController.m_instance.PlaySound( SoundController.Sound.RadarBlip );

						m_spaceflightController.m_messages.ChangeText( "Captain, we're being scanned." );

						m_pdEncounter.m_nextAction = 2;
						m_pdEncounter.m_timeToNextAction = Random.Range( 3.0f, 10.0f );
					}

					break;

				case 2:

					// check if there is at least one human crew member
					if ( playerData.m_crewAssignment.HasAtLeastOneHumanCrew() )
					{
						// yes - hail the player
						m_pdEncounter.m_nextAction = 3;
					}
					else
					{
						// mechans no likey
						m_pdEncounter.m_stance = GD_Comm.Stance.Hostile;

						comm = FindComm( GD_Comm.Subject.Custom );

						ReceiveComm( comm );

						// terminate communications
						m_pdEncounter.m_nextAction = 900;
					}

					break;

				case 3:

					// does the player want to communicate?
					if ( m_playerWantsToCommunicate )
					{
						// connect!
						ConnectToAliens();

						m_pdEncounter.m_nextAction = 100;
					}
					else
					{
						// send greeting message
						comm = FindComm( GD_Comm.Subject.GreetingHail );

						ReceiveComm( comm );

						m_pdEncounter.m_timeToNextAction = Random.Range( 10.0f, 20.0f );
					}

					break;

				case 100:

					// answer any question from the player
					if ( ( m_subject >= GD_Comm.Subject.Themselves ) && ( m_subject <= GD_Comm.Subject.GeneralInfo ) )
					{
						comm = FindComm( m_subject );

						ReceiveComm( comm );

						m_subject = GD_Comm.Subject.Statement;

						m_pdEncounter.m_timeToNextAction = Random.Range( 10.0f, 20.0f );
					}

					break;

				case 900:

					// send communications terminated message
					comm = FindComm( GD_Comm.Subject.Terminate );

					ReceiveComm( comm );

					m_pdEncounter.m_nextAction = 101;
					m_pdEncounter.m_timeToNextAction = 3.0f;

					break;

				case 901:

					// close messages
					m_spaceflightController.m_messages.SlideIn();

					m_pdEncounter.m_nextAction = 999;

					break;

				case 999:

					// ships fly out of encounter zone
					m_pdEncounter.m_timeToNextAction = 60.0f;

					break;
			}
		}
	}

	// find and return a comm
	public GD_Comm FindComm( GD_Comm.Subject subject, bool playerComm = false )
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// which race and stance to use?
		GameData.Race race;
		GD_Comm.Stance stance;

		if ( playerComm )
		{
			// remember what the player just said
			m_subject = subject;

			race = GameData.Race.Human;
			stance = m_stance;

			if ( ( subject >= GD_Comm.Subject.Themselves ) && ( subject <= GD_Comm.Subject.GeneralInfo ) )
			{
				subject = GD_Comm.Subject.Question;
			}
		}
		else
		{
			race = m_gdEncounter.m_race;
			stance = m_pdEncounter.m_stance;
		}

		// go through each comm in the comm list and build a list of possible comms to choose from
		var possibleComms = new List<GD_Comm>();

		foreach ( var comm in gameData.m_commList )
		{
			if ( comm.m_race == race )
			{
				if ( comm.m_subject == subject )
				{
					if ( ( comm.m_stance == 0 ) || ( comm.m_stance == stance ) )
					{
						possibleComms.Add( comm );
					}
				}
			}
		}

		// make sure we have something to pick from!
		if ( possibleComms.Count == 0 )
		{
			Debug.Log( "Whoops - no suitable comm found! (" + m_gdEncounter.m_race + ", " + subject + ", " + m_pdEncounter.m_stance + ")" );

			return new GD_Comm( "ERROR" );
		}

		// if the subject is between 7 and 11 the comm is a response to a question, so show them in order
		if ( ( subject >= GD_Comm.Subject.Themselves ) && ( subject <= GD_Comm.Subject.GeneralInfo ) )
		{
			for ( int i = 0; i < 2; i++ )
			{
				int lastRaceCommId = playerData.m_general.m_lastRaceCommIds[ (int) m_gdEncounter.m_race, (int) subject ];

				foreach ( var comm in possibleComms )
				{
					if ( comm.m_raceCommId > lastRaceCommId )
					{
						playerData.m_general.m_lastRaceCommIds[ (int) m_gdEncounter.m_race, (int) subject ] = comm.m_raceCommId;

						return comm;
					}
				}

				// if we get here we have run out of responses, so start over (the next loop is guaranteed to succeed)
				// TODO: make use of subject 5 (no more info)
				playerData.m_general.m_lastRaceCommIds[ (int) m_gdEncounter.m_race, (int) subject ] = 0;
			}
		}

		// have we shown all possible comms for this subject?
		var alreadyShownThemAll = true;

		foreach ( var comm in possibleComms )
		{
			if ( !comm.m_shown )
			{
				alreadyShownThemAll = false;
				break;
			}
		}

		// if we have shown them all then reset them all
		if ( alreadyShownThemAll )
		{
			foreach ( var comm in possibleComms )
			{
				comm.m_shown = false;
			}
		}

		// remove all of the ones we've shown before from the list
		for ( var i = 0; i < possibleComms.Count; i++ )
		{
			if ( possibleComms[ i ].m_shown )
			{
				possibleComms.RemoveAt( i );

				i--;
			}
		}

		// now pick and return a random one
		var randomNumber = Random.Range( 0, possibleComms.Count - 1 );

		var selectedComm = possibleComms[ randomNumber ];

		selectedComm.m_shown = true;

		return selectedComm;
	}

	// adds some text to the conversation
	void ReceiveComm( GD_Comm comm )
	{
		// add a newline if there's stuff already in the comm log
		if ( m_conversation.Length > 0 )
		{
			m_conversation += "\n";
		}

		// add the text
		m_conversation += "<color=white>Receiving:</color>\n<color=#0a0>" + comm.m_text + "</color>";

		// update the message
		m_spaceflightController.m_messages.ChangeText( m_conversation );

		// slide the message box out
		m_spaceflightController.m_messages.SlideOut();

		// play the beep sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Beep );

		// if this wasn't a terminate message then aliens are waiting for a response
		m_waitingForResponse = ( comm.m_subject != GD_Comm.Subject.Terminate );
	}

	// adds some text to the conversation
	public void SendComm( GD_Comm comm )
	{
		if ( ( comm.m_subject != GD_Comm.Subject.Terminate ) || ( comm.m_race != GameData.Race.Human ) )
		{
			// get to the player data
			var playerData = DataController.m_instance.m_playerData;

			// add a newline if there's stuff already in the comm log
			if ( m_conversation.Length > 0 )
			{
				m_conversation += "\n";
			}

			// get to the captains's personnel file
			var personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.Captain );

			// parse the text (replace tokens with values)
			var text = comm.m_text;

			text = text.Replace( "&", "ISS " + playerData.m_playerShip.m_name );
			text = text.Replace( "*", personnelFile.m_name );

			if ( text.Contains( "/" ) )
			{
				if ( ( m_subject >= GD_Comm.Subject.Themselves ) && ( m_subject < GD_Comm.Subject.GeneralInfo ) )
				{
					string askingAbout = null;

					switch ( m_subject )
					{
						case GD_Comm.Subject.Themselves:
							askingAbout = "your race";
							break;

						case GD_Comm.Subject.OtherRaces:
							askingAbout = "other races";
							break;

						case GD_Comm.Subject.OldEmpire:
							askingAbout = "the old empire";
							break;

						case GD_Comm.Subject.TheAncients:
							askingAbout = "the Ancients";
							break;
					}

					text = text.Replace( "(", "" );
					text = text.Replace( ")", "" );
					text = text.Replace( "/", askingAbout );
				}
				else
				{
					var regex = new Regex( @"\(([^\}]+)\)" );

					text = regex.Replace( text, "" );
				}

				m_pdEncounter.m_timeToNextAction = Random.Range( 2.0f, 5.0f );
			}

			// add the text
			m_conversation += "<color=white>Transmitting:</color>\n<color=#0aa>" + text + "</color>";

			// update the message
			m_spaceflightController.m_messages.ChangeText( m_conversation );

			// slide the message box out
			m_spaceflightController.m_messages.SlideOut();

			// play the beep sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Beep );
		}

		// did communications terminate?
		if ( comm.m_subject == GD_Comm.Subject.Terminate )
		{
			// yes - get out of video chat
			DisconnectFromAliens();
		}
	}

	// enter video chat
	public void ConnectToAliens()
	{
		// stop updating the ships (both aliens and player)
		m_updateShips = false;

		// hide the player (camera and all)
		m_spaceflightController.m_player.Hide();

		// hide the encounter location
		m_main.SetActive( false );

		// instantly black out the viewer
		m_spaceflightController.m_map.StartFade( 0.0f, 0.0f );

		// show the race of the aliens in this encounter
		if ( m_alienRaceModelList[ (int) m_gdEncounter.m_race ] != null )
		{
			m_alienRaceModelList[ (int) m_gdEncounter.m_race ].SetActive( true );
		}
		else
		{
			Debug.Log( "Sorry, alien race " + m_gdEncounter.m_race + " is not available to display yet!" );
		}

		// change the buttons
		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );

		// we are in video chat now
		m_inVideoChat = true;
	}

	// exit video chat
	public void DisconnectFromAliens()
	{
		// hide the race of the aliens in this encounter
		if ( m_alienRaceModelList[ (int) m_gdEncounter.m_race ] != null )
		{
			m_alienRaceModelList[ (int) m_gdEncounter.m_race ].SetActive( false );
		}

		// show the encounter location
		m_main.SetActive( true );

		// show the player (camera and all)
		m_spaceflightController.m_player.Show();

		// start updating the ships again
		m_updateShips = true;

		// change the buttons
		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Bridge );

		// we are not in video chat any more
		m_inVideoChat = false;
	}

	// call this to find out if there are living alien ships in the encounter
	public bool HasLivingAlienShips()
	{
		// go through each alien ship
		foreach ( var alienShip in m_pdEncounter.m_alienShipList )
		{
			// has this alien ship been added to the encounter?
			if ( alienShip.m_addedToEncounter )
			{
				// is this alien ship still living?
				if ( !alienShip.m_isDead )
				{
					// yep!
					return true;
				}
			}
		}

		// nope!
		return false;
	}

	// check if the aliens are waiting for a response
	public bool IsWaitingForResponse()
	{
		return m_waitingForResponse;
	}

	// return true if we are in video chat already
	public bool InVideoChat()
	{
		return m_inVideoChat;
	}

	// hail (or respond to) the alien ships
	public void Hail( GD_Comm.Stance stance, bool responding )
	{
		// set the stance to what the player selected
		m_stance = stance;

		// are we not in video chat?
		if ( !m_inVideoChat )
		{
			// no - pick the hail message subject based on whether we are responding or hailing
			var comm = FindComm( responding ? GD_Comm.Subject.GreetingResponse : GD_Comm.Subject.GreetingHail, true );

			SendComm( comm );

			// the player wants to communicate
			m_playerWantsToCommunicate = true;
		}
	}

	void UpdateMechanAlienShip( PD_AlienShip alienShip, GameObject alienShipModel )
	{
		BuzzPlayer( alienShip, alienShipModel, 1.0f );
	}

	void DefaultAlienShipUpdate( PD_AlienShip alienShip, GameObject alienShipModel )
	{
		BuzzPlayer( alienShip, alienShipModel, 1.0f );
	}

	void BuzzPlayer( PD_AlienShip alienShip, GameObject alienShipModel, float alienShipSpeedMultiplier )
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		MoveAlienShip( alienShip, alienShipModel, alienShipSpeedMultiplier, playerData.m_general.m_coordinates );
	}

	void MoveAlienShip( PD_AlienShip alienShip, GameObject alienShipModel, float alienShipSpeedMultiplier, Vector3 targetCoordinates )
	{
		// update time since we changed target coordinates for this alien ship
		alienShip.m_timeSinceLastTargetCoordinateChange += Time.deltaTime;

		// change target coordinates every so often
		if ( alienShip.m_timeSinceLastTargetCoordinateChange >= m_targetCoordinateUpdateFrequency )
		{
			var randomCoordinates = Random.insideUnitCircle;

			alienShip.m_targetCoordinates = targetCoordinates + Vector3.Normalize( new Vector3( randomCoordinates.x, 0.0f, randomCoordinates.y ) ) * 256.0f;

			alienShip.m_timeSinceLastTargetCoordinateChange -= m_targetCoordinateUpdateFrequency;
		}

		// steer the alien ship towards the target coordinates
		var desiredDirection = Vector3.Normalize( alienShip.m_targetCoordinates - alienShip.m_coordinates );

		alienShip.m_currentDirection = Vector3.Slerp( alienShip.m_currentDirection, desiredDirection, Time.deltaTime * m_alienShipTurnRate );

		// move the alien ship forward
		alienShip.m_coordinates += alienShip.m_currentDirection * Time.deltaTime * m_alienShipSpeed * alienShipSpeedMultiplier;
	}

	void FinalizeAlienShips()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// remember the extents
		var xExtent = 0.0f;
		var zExtent = 0.0f;

		// update the position and rotation of the active alien ship models
		for ( var alienShipIndex = 0; alienShipIndex < m_alienShipModelList.Length; alienShipIndex++ )
		{
			var alienShipModel = m_alienShipModelList[ alienShipIndex ];

			if ( alienShipModel.activeInHierarchy )
			{
				// get to the alien ship
				var alienShip = m_alienShipList[ alienShipIndex ];

				// set the rotation of the ship
				alienShipModel.transform.rotation = Quaternion.LookRotation( alienShip.m_currentDirection, Vector3.up );

				// get the number of degrees we are turning the ship (compared to the last frame)
				var bankingAngle = Vector3.SignedAngle( alienShip.m_currentDirection, alienShip.m_lastDirection, Vector3.up );

				// scale the angle enough so we actually see the ship banking (but max it out at 60 degrees in either direction)
				bankingAngle = Mathf.Max( -60.0f, Mathf.Min( 60.0f, bankingAngle * 48.0f ) );

				// interpolate towards the new banking angle
				alienShip.m_currentBankingAngle = Mathf.Lerp( alienShip.m_currentBankingAngle, bankingAngle, Time.deltaTime );

				// save the last direction
				alienShip.m_lastDirection = alienShip.m_currentDirection;

				// bank the ship based on the calculated angle
				alienShipModel.transform.rotation = Quaternion.AngleAxis( alienShip.m_currentBankingAngle, alienShip.m_currentDirection ) * alienShipModel.transform.rotation;

				// set the position of the ship
				alienShipModel.transform.position = alienShip.m_coordinates;

				// figure out how far away from the player this alien ship is
				var playerToShip = alienShip.m_coordinates - playerData.m_general.m_coordinates;

				xExtent = Mathf.Max( xExtent, Mathf.Abs( playerToShip.x ) );
				zExtent = Mathf.Max( zExtent, Mathf.Abs( playerToShip.z ) );
			}
		}

		// add some space around the extents
		xExtent += 192.0f;
		zExtent += 192.0f;

		// recalculate what the camera distance from the zero plane should be
		var verticalFieldOfView = m_spaceflightController.m_map.m_playerCamera.fieldOfView * Mathf.Deg2Rad;
		var horizontalFieldOfView = 2.0f * Mathf.Atan( Mathf.Tan( verticalFieldOfView * 0.5f ) * m_spaceflightController.m_map.m_playerCamera.aspect );
		var horizontalAngle = Mathf.Deg2Rad * ( 180.0f - 90.0f - horizontalFieldOfView * Mathf.Rad2Deg * 0.5f );
		var verticalAngle = Mathf.Deg2Rad * ( 180.0f - 90.0f - verticalFieldOfView * Mathf.Rad2Deg * 0.5f );
		var tanHorizontalAngle = Mathf.Tan( horizontalAngle );
		var tanVerticalAngle = Mathf.Tan( verticalAngle );

		var targetDollyDistance = Mathf.Max( xExtent * tanHorizontalAngle, zExtent * tanVerticalAngle, 1024.0f );

		// slowly dolly the camera
		m_currentDollyDistance = Mathf.Lerp( m_currentDollyDistance, targetDollyDistance, Time.deltaTime * m_cameraDollySpeed );

		m_spaceflightController.m_player.DollyCamera( m_currentDollyDistance );
	}

	// return the distance to the nearest alien ship
	float ClosestDistanceToAlienShip()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// closest distance is "infinity"
		var closestDistance = float.MaxValue;

		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		foreach ( var alienShip in alienShipList )
		{
			if ( alienShip.m_addedToEncounter && !alienShip.m_isDead )
			{
				var distance = Vector3.Distance( alienShip.m_coordinates, playerData.m_general.m_coordinates );

				if ( distance < closestDistance )
				{
					closestDistance = distance;
				}
			}
		}

		return closestDistance;
	}

	// update the current scanner selection (0 = player's ship, 1...n = alien ships)
	public int UpdateScannerSelection( int currentSelection, int selectChangeDirection )
	{
		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		// do we have the player's ship currently selected?
		if ( currentSelection == 0 )
		{
			// yes - move the current selection to the beginning or the ending of the alien ship list
			if ( selectChangeDirection > 0 )
			{
				currentSelection = -1;
			}
			else
			{
				currentSelection = alienShipList.Length;
			}
		}
		else
		{
			currentSelection--;
		}

		do
		{
			currentSelection += selectChangeDirection;

			if ( ( currentSelection < 0 ) || ( currentSelection == alienShipList.Length ) )
			{
				return 0;
			}
		}
		while ( alienShipList[ currentSelection ].m_addedToEncounter == false );

		return currentSelection + 1;
	}

	// gets the coordinates for the current selection in the encounter
	public Vector3 GetScannerPosition( int currentSelection )
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		// is the current selection the player's ship?
		if ( currentSelection == 0 )
		{
			// yes - just return the position of the player
			return playerData.m_general.m_coordinates;
		}

		// return the position of the alien ship
		return alienShipList[ currentSelection - 1 ].m_coordinates;
	}

	// start the scanning cinematic
	public void StartScanning( int currentSelection )
	{
		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		// was the player ship selected?
		if ( currentSelection == 0 )
		{
			// yes - let the player know
			SoundController.m_instance.PlaySound( SoundController.Sound.Error );

			m_spaceflightController.m_messages.ChangeText( "<color=red>That is your ship.</color>" );

			// deactivate the sensor button
			m_spaceflightController.m_buttonController.DeactivateButton();

			// show the status display
			m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_statusDisplay );
		}
		else
		{
			// is the alien ship dead?
			if ( alienShipList[ currentSelection - 1 ].m_isDead )
			{
				// yes - scanning of debris not implemented yet
				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_messages.ChangeText( "<color=red>Not yet implemented.</color>" );

				// deactivate the sensor button
				m_spaceflightController.m_buttonController.DeactivateButton();

				// show the status display
				m_spaceflightController.m_displayController.ChangeDisplay( m_spaceflightController.m_displayController.m_statusDisplay );
			}
			else
			{
				// no - get the vessel id
				var scanType = (SensorsDisplay.ScanType) alienShipList[ currentSelection - 1 ].m_vesselId;

				// start the scan
				m_spaceflightController.m_displayController.m_sensorsDisplay.StartScanning( scanType, 1, 20, 0, 100 );
			}
		}
	}
}
