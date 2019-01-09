
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

	// alien ship data
	PD_AlienShip[] m_alienShipList;

	// did we just enter this encounter from hyperspace?
	bool m_justEntered;

	// current dolly distance
	float m_currentDollyDistance;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
		// turn off all of the alien ship model templates
		foreach ( var alienShipModelTemplate in m_alienShipModelTemplate )
		{
			alienShipModelTemplate.SetActive( false );
		}
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

		// update race encounter
		switch ( m_gdEncounter.m_race )
		{
			case GameData.Race.Elowan:

				UpdateElowanEncounter();
				break;

			case GameData.Race.Gazurtoid:

				UpdateGazurtoidEncounter();
				break;

			case GameData.Race.Mechan:

				UpdateMechanEncounter();
				break;

			case GameData.Race.Mysterion:

				UpdateMysterionEncounter();
				break;

			case GameData.Race.NomadProbe:

				UpdateNomadProbeEncounter();
				break;

			case GameData.Race.Spemin:

				UpdateSpeminEncounter();
				break;

			case GameData.Race.Thrynn:

				UpdateThrynnEncounter();
				break;

			case GameData.Race.Velox:

				UpdateVeloxEncounter();
				break;

			case GameData.Race.VeloxProbe:

				UpdateVeloxProbeEncounter();
				break;

			case GameData.Race.Minstrel:

				UpdateMinstrelEncounter();
				break;
		}

		// do we want to update the ships (both alien and player)?
		if ( !m_pdEncounter.m_connected )
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
	public void JustEntered()
	{
		m_justEntered = true;
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

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// show the hyperspace objects
		gameObject.SetActive( true );

		// show the main encounter location stuff
		m_main.SetActive( true );

		// make sure the camera is at the right height above the zero plane
		m_currentDollyDistance = 1024.0f;

		m_spaceflightController.m_player.DollyCamera( m_currentDollyDistance );
		m_spaceflightController.m_player.SetClipPlanes( 512.0f, 1536.0f );

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

		// play the star system music track
		MusicController.m_instance.ChangeToTrack( MusicController.Track.Encounter );

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

		// did we just enter this encounter from hyperspace?
		if ( m_justEntered )
		{
			// yes - reset all of the alien ships
			foreach ( var alienShip in alienShipList )
			{
				alienShip.m_addedToEncounter = false;
			}

			// add the first round of alien ships to the encounter
			AddAlienShips();

			// center the encounter coordinates on the player
			var encounterLocation = m_pdEncounter.GetLocation();

			if ( encounterLocation == PD_General.Location.Hyperspace )
			{
				m_pdEncounter.SetCoordinates( playerData.m_general.m_lastHyperspaceCoordinates );
			}
			else if ( encounterLocation == PD_General.Location.StarSystem )
			{
				m_pdEncounter.SetCoordinates( playerData.m_general.m_lastStarSystemCoordinates );
			}

			// reset the conversation
			m_pdEncounter.ResetConversation();

			// reset the various encounter stuff
			m_pdEncounter.m_shownCommList.Clear();
			m_pdEncounter.m_connected = false;
			m_pdEncounter.m_disconnected = false;
			m_pdEncounter.m_connectionTimer = 0.0f;
			m_pdEncounter.m_aliensWantToConnect = false;
			m_pdEncounter.m_playerWantsToConnect = false;
			m_pdEncounter.m_lastSubjectFromPlayer = GD_Comm.Subject.None;
			m_pdEncounter.m_lastQuestionFromAliens = 0;
			m_pdEncounter.m_alreadyScanned = false;
			m_pdEncounter.m_scanTimer = 0.0f;
			m_pdEncounter.m_conversationTimer = 0.0f;
			m_pdEncounter.m_alienStance = GD_Comm.Stance.Neutral;
			m_pdEncounter.m_playerStance = GD_Comm.Stance.Neutral;
			m_pdEncounter.m_questionLikelihood = 50;
			m_pdEncounter.m_numCorrectAnswers = 0;
			m_pdEncounter.m_mechan9NoHumansWarningDone = false;

			// initialize race encounter
			switch ( m_gdEncounter.m_race )
			{
				case GameData.Race.Elowan:

					InitializeElowanEncounter();
					break;

				case GameData.Race.Gazurtoid:

					InitializeGazurtoidEncounter();
					break;

				case GameData.Race.Mechan:

					InitializeMechanEncounter();
					break;

				case GameData.Race.Mysterion:

					InitializeMysterionEncounter();
					break;

				case GameData.Race.NomadProbe:

					InitializeNomadProbeEncounter();
					break;

				case GameData.Race.Spemin:

					InitializeSpeminEncounter();
					break;

				case GameData.Race.Thrynn:

					InitializeThrynnEncounter();
					break;

				case GameData.Race.Velox:

					InitializeVeloxEncounter();
					break;

				case GameData.Race.VeloxProbe:

					InitializeVeloxProbeEncounter();
					break;

				case GameData.Race.Minstrel:

					InitializeMinstrelEncounter();
					break;
			}

			// play the alarm sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Alarm );

			// reset flag
			m_justEntered = false;
		}
		else
		{
			// no - just reset the alien ship models
			ResetAlienShipModels();
		}

		// are we are already connected to the aliens (from save game)?
		if ( m_pdEncounter.m_connected )
		{
			// yes - reconnect to the aliens immediately (reuse conversation string)
			Connect();
		}

		// show the conversation string
		m_spaceflightController.m_messages.ChangeText( m_pdEncounter.m_conversation );

		// slide the message box out
		m_spaceflightController.m_messages.SlideOut();

		// force an update now
		Update();
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

				if ( m_justEntered )
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

	// update and position 3d models based on encounter alien ship data
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

				var clonedAlienShipModel = Instantiate( alienShipModelTemplate, alienShipModelTemplate.transform.localPosition, alienShipModelTemplate.transform.localRotation, alienShipModel.transform );

				clonedAlienShipModel.SetActive( true );

				// show the model
				alienShipModel.SetActive( true );

				// remember the alien ship associated with this model
				m_alienShipList[ alienShipIndex ] = alienShip;

				// next!
				alienShipIndex++;
			}
		}
	}

	// initialize encounter with elowan
	void InitializeElowanEncounter()
	{
		DefaultEncounterInitialize();
	}

	// initialize encounter with gazurtoid
	void InitializeGazurtoidEncounter()
	{
		DefaultEncounterInitialize();
	}

	// initialize encounter with mechans
	void InitializeMechanEncounter()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// set scan timer
		m_pdEncounter.m_scanTimer = Random.Range( 5.0f, 90.0f );

		// set statement timer
		m_pdEncounter.m_conversationTimer = Random.Range( 5.0f, 30.0f );

		// determine initial stance
		if ( playerData.m_general.m_mechan9Unlocked )
		{
			// player has unlocked the mechans
			m_pdEncounter.m_alienStance = GD_Comm.Stance.Friendly;

			// start at 0% question likelihood
			m_pdEncounter.m_questionLikelihood = 0;
		}
		else if ( playerData.m_crewAssignment.HasAtLeastOneHumanCrew() )
		{
			// neutral since there is at least one human crew member
			m_pdEncounter.m_alienStance = GD_Comm.Stance.Neutral;

			// set connection timer
			m_pdEncounter.m_connectionTimer = Random.Range( 120.0f, 300.0f );

			// start at 50% question likelihood
			m_pdEncounter.m_questionLikelihood = 50;
		}
		else
		{
			// hostile since there are no human crew members
			m_pdEncounter.m_alienStance = GD_Comm.Stance.Hostile;

			// set connection timer
			m_pdEncounter.m_connectionTimer = Random.Range( 60.0f, 120.0f );

			// start at 0% question likelihood
			m_pdEncounter.m_questionLikelihood = 0;
		}

		// mechans have not warned the player yet
		m_pdEncounter.m_mechan9NoHumansWarningDone = false;
	}

	// initialize encounter with mysterion
	void InitializeMysterionEncounter()
	{
		DefaultEncounterInitialize();
	}

	// initialize encounter with nomad probe
	void InitializeNomadProbeEncounter()
	{
		DefaultEncounterInitialize();
	}

	// initialize encounter with spemin
	void InitializeSpeminEncounter()
	{
		DefaultEncounterInitialize();
	}

	// initialize encounter with thrynn
	void InitializeThrynnEncounter()
	{
		DefaultEncounterInitialize();
	}

	// initialize encounter with velox
	void InitializeVeloxEncounter()
	{
		DefaultEncounterInitialize();
	}

	// initialize encounter with velox probe
	void InitializeVeloxProbeEncounter()
	{
		DefaultEncounterInitialize();
	}

	// initialize encounter with minstrel
	void InitializeMinstrelEncounter()
	{
		DefaultEncounterInitialize();
	}

	// the default encounter initialization
	void DefaultEncounterInitialize()
	{
		// set statement timer
		m_pdEncounter.m_conversationTimer = Random.Range( 5.0f, 30.0f );

		// set connection timer
		m_pdEncounter.m_connectionTimer = Random.Range( 120.0f, 300.0f );

		// start at 50% question likelihood
		m_pdEncounter.m_questionLikelihood = 50;
	}

	// update encounter with elowan
	void UpdateElowanEncounter()
	{
		DefaultEncounterUpdate();
	}

	// update encounter with gazurtoid
	void UpdateGazurtoidEncounter()
	{
		DefaultEncounterUpdate();
	}

	// update encounter with mechans
	void UpdateMechanEncounter()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// did we warn the player yet?
		if ( !m_pdEncounter.m_mechan9NoHumansWarningDone )
		{
			// no - did we scan the player?
			if ( m_pdEncounter.m_alreadyScanned )
			{
				// yes - did we do the scan at least 3 seconds ago?
				if ( m_pdEncounter.m_scanTimer < -3.0f )
				{
					// yes - is there at least one crew member?
					if ( playerData.m_crewAssignment.HasAtLeastOneHumanCrew() )
					{
						// yes - nothing to do
					}
					else
					{
						// no - show no humans warning message
						AddComm( GD_Comm.Subject.Custom, false );
					}

					// update flag
					m_pdEncounter.m_mechan9NoHumansWarningDone = true;
				}
			}
		}

		// did the mechans ask a question?
		if ( m_pdEncounter.m_lastQuestionFromAliens != 0 )
		{
			// yes - did the player answer?
			if ( m_pdEncounter.m_lastSubjectFromPlayer != GD_Comm.Subject.None )
			{
				// yes - get the correct answer
				GD_Comm.Subject correctAnswer = GD_Comm.Subject.Yes;

				switch ( m_pdEncounter.m_lastQuestionFromAliens )
				{
					case 101:
					case 102:
					case 103:
					case 105:
						correctAnswer = GD_Comm.Subject.No;
						break;
				}

				// did the player answer correctly?
				if ( m_pdEncounter.m_lastSubjectFromPlayer == correctAnswer )
				{
					// yes - add to the number of correct answers
					m_pdEncounter.m_numCorrectAnswers++;

					// did the player answer enough questions correctly?
					if ( m_pdEncounter.m_numCorrectAnswers == 5 )
					{
						// yes - yay! we are are friends!
						playerData.m_general.m_mechan9Unlocked = true;

						// mechans are friendly now
						m_pdEncounter.m_alienStance = GD_Comm.Stance.Friendly;

						// no more questions from the mechans
						m_pdEncounter.m_questionLikelihood = 0;
					}
				}
				else
				{
					// no - oh boy mechans hate the player now
					m_pdEncounter.m_alienStance = GD_Comm.Stance.Hostile;

					// no more questions from the mechans
					m_pdEncounter.m_questionLikelihood = 0;

					// mechans want to terminate comms shortly
					m_pdEncounter.m_connectionTimer = 15.0f;
				}
			}
		}

		// do the default encounter stuff
		if ( DefaultEncounterUpdate() )
		{
			// mechans more likely to ask questions as time goes by to try and figure out who the player is
			if ( m_pdEncounter.m_alienStance == GD_Comm.Stance.Neutral )
			{
				m_pdEncounter.m_questionLikelihood += 25;
			}
		}
	}

	// update encounter with mysterion
	void UpdateMysterionEncounter()
	{
		DefaultEncounterUpdate();
	}

	// update encounter with nomad probe
	void UpdateNomadProbeEncounter()
	{
		DefaultEncounterUpdate();
	}

	// update encounter with spemin
	void UpdateSpeminEncounter()
	{
		DefaultEncounterUpdate();
	}

	// update encounter with thrynn
	void UpdateThrynnEncounter()
	{
		DefaultEncounterUpdate();
	}

	// update encounter with velox
	void UpdateVeloxEncounter()
	{
		DefaultEncounterUpdate();
	}

	// update encounter with velox probe
	void UpdateVeloxProbeEncounter()
	{
		DefaultEncounterUpdate();
	}

	// update encounter with minstrel
	void UpdateMinstrelEncounter()
	{
		DefaultEncounterUpdate();
	}

	// default encounter update
	bool DefaultEncounterUpdate()
	{
		// keep track of whether the aliens said something
		bool aliensSaidSomething = false;

		// update scan timer
		m_pdEncounter.m_scanTimer -= Time.deltaTime;

		// have the aliens already scanned us?
		if ( !m_pdEncounter.m_alreadyScanned )
		{
			// no - did the timer expire?
			if ( m_pdEncounter.m_scanTimer <= 0.0f )
			{
				// yes - are the aliens close enough to scan?
				if ( ClosestDistanceToAlienShip() < 512.0f )
				{
					// yes - play the scan sound
					SoundController.m_instance.PlaySound( SoundController.Sound.RadarBlip );

					// add to the conversation
					m_pdEncounter.AddToConversation( "Captain, we're being scanned." );

					// update the message
					m_spaceflightController.m_messages.ChangeText( m_pdEncounter.m_conversation );

					// update the flag
					m_pdEncounter.m_scanTimer = 0.0f;
					m_pdEncounter.m_alreadyScanned = true;
				}
			}
		}

		// have we already disconnected?
		if ( !m_pdEncounter.m_disconnected )
		{
			// did the aliens ask a question?
			if ( m_pdEncounter.m_lastQuestionFromAliens == 0 )
			{
				// no - update the statement timer
				m_pdEncounter.m_conversationTimer -= Time.deltaTime;

				// is it time for an update?
				if ( m_pdEncounter.m_conversationTimer <= 0.0f )
				{
					// reset the conversation timer
					m_pdEncounter.m_conversationTimer = Random.Range( 10.0f, 30.0f );

					// are we connected?
					if ( m_pdEncounter.m_connected )
					{
						// the aliens will have said something
						aliensSaidSomething = true;

						// was there a pending question from the player?
						if ( ( m_pdEncounter.m_lastSubjectFromPlayer >= GD_Comm.Subject.Themselves ) && ( m_pdEncounter.m_lastSubjectFromPlayer <= GD_Comm.Subject.TheAncients ) )
						{
							// yes - answer the question
							AddComm( m_pdEncounter.m_lastSubjectFromPlayer, false );

							// there is no longer have a pending question
							m_pdEncounter.m_lastSubjectFromPlayer = GD_Comm.Subject.None;
						}
						else
						{
							// no - do we want to do a question?
							if ( Random.Range( 1, 101 ) > m_pdEncounter.m_questionLikelihood )
							{
								// no - do a normal statement
								AddComm( GD_Comm.Subject.Statement, false );
							}
							else
							{
								// yes - ask a question
								AddComm( GD_Comm.Subject.Question, false );
							}
						}
					}
					else
					{
						// are we already connected?
						if ( m_pdEncounter.m_connected )
						{
							// do a normal statement
							AddComm( GD_Comm.Subject.Statement, false );
						}
						else
						{
							// did the player hail?
							if ( m_pdEncounter.m_playerWantsToConnect )
							{
								// yes - do a response
								AddComm( GD_Comm.Subject.GreetingResponse, false );
							}
							else
							{
								// no - do a hail
								AddComm( GD_Comm.Subject.GreetingHail, false );
							}
						}
					}
				}

				// are we connected?
				if ( m_pdEncounter.m_connected )
				{
					// yes - update connection timer
					m_pdEncounter.m_connectionTimer -= Time.deltaTime;

					// did the timer expire?
					if ( m_pdEncounter.m_connectionTimer <= 0.0f )
					{
						// yes - the aliens want to terminate the conversation
						AddComm( GD_Comm.Subject.Terminate, false );
					}
				}
			}
		}

		// did the aliens ask a question?
		if ( m_pdEncounter.m_lastQuestionFromAliens != 0 )
		{
			// yes - did the player answer?
			if ( m_pdEncounter.m_lastSubjectFromPlayer != GD_Comm.Subject.None )
			{
				// yes - forget the question (it should already have been handled before calling this default encounter update function)
				m_pdEncounter.m_lastQuestionFromAliens = 0;
			}
		}

		// let the caller know if the aliens said something
		return aliensSaidSomething;
	}

	// mechan ship movement
	void UpdateMechanAlienShip( PD_AlienShip alienShip, GameObject alienShipModel )
	{
		BuzzPlayer( alienShip, alienShipModel, 1.0f );
	}

	// default alien ship movement
	void DefaultAlienShipUpdate( PD_AlienShip alienShip, GameObject alienShipModel )
	{
		BuzzPlayer( alienShip, alienShipModel, 1.0f );
	}

	// moves the alien ship towards the player
	void BuzzPlayer( PD_AlienShip alienShip, GameObject alienShipModel, float alienShipSpeedMultiplier )
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// move the alien ship towards the player
		MoveAlienShip( alienShip, alienShipModel, alienShipSpeedMultiplier, playerData.m_general.m_coordinates );
	}

	// moves the alien ship towards the target coordinates
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

	// find a comm based on the subject
	GD_Comm FindComm( GD_Comm.Subject subject, bool outgoing )
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// which race and stance to use?
		GameData.Race race;
		GD_Comm.Stance stance;

		if ( outgoing )
		{
			// remember what the player just said
			m_pdEncounter.m_lastSubjectFromPlayer = subject;

			race = GameData.Race.Human;
			stance = m_pdEncounter.m_playerStance;

			if ( ( subject >= GD_Comm.Subject.Themselves ) && ( subject <= GD_Comm.Subject.TheAncients ) )
			{
				subject = GD_Comm.Subject.Question;
			}
		}
		else
		{
			race = m_gdEncounter.m_race;
			stance = m_pdEncounter.m_alienStance;
		}

		// go through each comm in the comm list and build a list of possible comms to choose from
		var possibleComms = new List<GD_Comm>();

		foreach ( var comm in gameData.m_commList )
		{
			if ( comm.m_homeworld || comm.m_surrender )
			{
				continue;
			}

			if ( ( comm.m_race == race ) && ( comm.m_subject == subject ) && ( comm.m_stance == stance ) )
			{
				possibleComms.Add( comm );
			}
		}

		// if we didn't find anything then try the neutral stance
		if ( possibleComms.Count == 0 )
		{
			foreach ( var comm in gameData.m_commList )
			{
				if ( comm.m_homeworld || comm.m_surrender )
				{
					continue;
				}

				if ( ( comm.m_race == race ) && ( comm.m_subject == subject ) && ( comm.m_stance == GD_Comm.Stance.Neutral ) )
				{
					possibleComms.Add( comm );
				}
			}
		}

		// if the subject is between 7 and 11 the comm is a response to a question, so show them in order
		if ( ( subject >= GD_Comm.Subject.Themselves ) && ( subject <= GD_Comm.Subject.TheAncients ) )
		{
			int lastCommId = playerData.m_general.m_lastCommIds[ (int) race, (int) subject ];

			foreach ( var comm in possibleComms )
			{
				if ( comm.m_id > lastCommId )
				{
					playerData.m_general.m_lastCommIds[ (int) m_gdEncounter.m_race, (int) subject ] = comm.m_id;

					return comm;
				}
			}

			// they have run out of answers to this question so reset the list
			playerData.m_general.m_lastCommIds[ (int) m_gdEncounter.m_race, (int) subject ] = 0;

			// return an "i dont know" response
			return FindComm( GD_Comm.Subject.NoMoreInformation, false );
		}

		// make sure we have something to pick from!
		if ( possibleComms.Count == 0 )
		{
			Debug.Log( "Whoops - no suitable comm found! (" + race + ", " + subject + ", " + stance + ")" );

			return new GD_Comm( "ERROR" );
		}

		// have we shown all possible comms for this subject?
		var alreadyShownThemAll = true;

		foreach ( var comm in possibleComms )
		{
			if ( !m_pdEncounter.m_shownCommList.Contains( comm.m_id ) )
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
				m_pdEncounter.m_shownCommList.Remove( comm.m_id );
			}
		}

		// remove all of the ones we've shown before from the list
		for ( var i = 0; i < possibleComms.Count; i++ )
		{
			if ( m_pdEncounter.m_shownCommList.Contains( possibleComms[ i ].m_id ) )
			{
				possibleComms.RemoveAt( i );

				i--;
			}
		}

		// now pick and return a random one
		var randomNumber = Random.Range( 0, possibleComms.Count );

		var selectedComm = possibleComms[ randomNumber ];

		m_pdEncounter.m_shownCommList.Add( selectedComm.m_id );

		return selectedComm;
	}

	// adds some text to the conversation
	public void AddComm( GD_Comm.Subject subject, bool outgoing )
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// get to the captains's personnel file
		var personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.Captain );

		// find a comm
		var comm = FindComm( subject, outgoing );

		// copy the comm text (because we will replace tokens)
		var text = comm.m_text;

		// replace tokens with values
		text = text.Replace( "&", "ISS " + playerData.m_playerShip.m_name );
		text = text.Replace( "*", personnelFile.m_name );

		// check for the slash token (replace with subject)
		if ( text.Contains( "/" ) )
		{
			// process tokens
			if ( m_pdEncounter.m_lastSubjectFromPlayer != GD_Comm.Subject.GeneralInfo )
			{
				string askingAbout = null;

				switch ( m_pdEncounter.m_lastSubjectFromPlayer )
				{
					case GD_Comm.Subject.Themselves:
						askingAbout = "your race";
						break;

					case GD_Comm.Subject.OtherRaces:
						askingAbout = "other races";
						break;

					case GD_Comm.Subject.OldEmpire:
						askingAbout = "the Old Empire";
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
		}

		// is this an outgoing message (from player)?
		if ( outgoing )
		{
			// yes - update timer so alien response doesn't take too long
			m_pdEncounter.m_conversationTimer += 5.0f;

			if ( m_pdEncounter.m_conversationTimer > 10.0f )
			{
				m_pdEncounter.m_conversationTimer = Random.Range( 5.0f, 10.0f );
			}

			// finalize text
			text = "<color=white>Transmitting:</color>\n<color=#0aa>" + text + "</color>";
		}
		else
		{
			// get the comm text
			var commText = comm.m_text;

			// get possible garble words for this race
			var possibleGarbles = new List<GD_Garble>();

			foreach ( var garble in gameData.m_garbleList )
			{
				if ( garble.m_race == m_gdEncounter.m_race )
				{
					possibleGarbles.Add( garble );
				}
			}

			// did we find any?
			if ( possibleGarbles.Count > 0 )
			{
				// yes - get the personnel file of the comm officer
				personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.CommunicationsOfficer );

				// compute the training level (in 0 to 1 range)
				var trainingLevel = personnelFile.m_communications / 250.0f;

				// create and seed the noise generator
				var fastNoise = new FastNoise( comm.m_id );

				fastNoise.SetNoiseType( FastNoise.NoiseType.Value );
				fastNoise.SetFrequency( 1.0f );

				// split the text into words
				var splitCommText = commText.Split();

				// start building the new comm text
				var garbledCommText = new List<string>();

				// go through each word
				var commWordIndex = 0;
				var lastWordHadSpecialCharacter = false;
				var lastWordWasGarbled = false;

				foreach ( var commWord in splitCommText )
				{
					commWordIndex++;

					// get the difficulty level of this word
					var difficulty = fastNoise.GetNoise( commWordIndex, 0 ) * 0.45f + 0.5f;

					// is this word too hard for our poor comm officer?
					if ( difficulty > trainingLevel )
					{
						// yes - pick number of syllables (based on race)
						int minSyllables = 1;
						int maxSyllables = 1;

						switch ( m_gdEncounter.m_race )
						{
							case GameData.Race.Gazurtoid:
								minSyllables = 5;
								maxSyllables = 10;
								break;

							case GameData.Race.Mysterion:
								minSyllables = 5;
								maxSyllables = 5;
								break;

							case GameData.Race.Spemin:
								minSyllables = 1;
								maxSyllables = 3;
								break;

							case GameData.Race.Velox:
								minSyllables = 1;
								maxSyllables = 5;
								break;

							case GameData.Race.Elowan:
								minSyllables = 1;
								maxSyllables = 4;
								break;

							case GameData.Race.Thrynn:
								minSyllables = 1;
								maxSyllables = 5;
								break;
						}

						var syllables = minSyllables + Mathf.FloorToInt( 0.25f + ( ( maxSyllables + 0.5f ) * ( fastNoise.GetNoise( 0, commWordIndex ) * 0.5f + 0.5f ) ) );

						// start building the garble word
						var garbledWord = "";

						for ( var syllableIndex = 0; syllableIndex < syllables; syllableIndex++ )
						{
							var garbleIndex = Mathf.FloorToInt( Mathf.Lerp( 0, possibleGarbles.Count, fastNoise.GetNoise( commWordIndex, syllableIndex ) * 0.5f + 0.5f ) );

							garbledWord += possibleGarbles[ garbleIndex ].m_text;
						}

						// fix case of word
						garbledWord = garbledWord.ToLower();

						if ( ( garbledCommText.Count == 0 ) || lastWordHadSpecialCharacter )
						{
							garbledWord = garbledWord[ 0 ].ToString().ToUpper() + garbledWord.Substring( 1 );
						}

						// add on the garbled word
						garbledCommText.Add( garbledWord );

						// this word does not have a special character
						lastWordHadSpecialCharacter = false;

						// this is a garbled word
						lastWordWasGarbled = true;
					}
					else
					{
						// no - just add on the word
						garbledCommText.Add( commWord );

						// check if this word has a special character at the end
						if ( !System.Char.IsLetter( commWord[ commWord.Length - 1 ] ) )
						{
							lastWordHadSpecialCharacter = true;
						}

						// this isn't a garbled word
						lastWordWasGarbled = false;
					}
				}

				// put everything back together
				commText = System.String.Join( " ", garbledCommText );

				// add a period (or a question mark) if the last word was garbled
				if ( lastWordWasGarbled )
				{
					commText += ( subject == GD_Comm.Subject.Question ) ? "?" : ".";
				}
			}

			// finalize text
			text = "<color=white>Receiving:</color>\n<color=#0a0>" + commText + "</color>";

			// was this a question?
			if ( subject == GD_Comm.Subject.Question )
			{
				// yes - change the button set
				m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.AnswerQuestion );

				// forget the last subject from the player
				m_pdEncounter.m_lastSubjectFromPlayer = GD_Comm.Subject.None;

				// remember the question from the aliens
				m_pdEncounter.m_lastQuestionFromAliens = comm.m_id;
			}
		}

		// add the text
		m_pdEncounter.AddToConversation( text );

		// update the message
		m_spaceflightController.m_messages.ChangeText( m_pdEncounter.m_conversation );

		// play the beep sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Beep );

		// are we trying to connect?
		if ( ( subject == GD_Comm.Subject.GreetingHail ) || ( subject == GD_Comm.Subject.GreetingResponse ) )
		{
			if ( outgoing )
			{
				// allow the aliens to connect to the player
				m_pdEncounter.m_playerWantsToConnect = true;
			}
			else
			{
				// allow the player to connect to the aliens
				m_pdEncounter.m_aliensWantToConnect = true;
			}

			// do both sides want to connect?
			if ( m_pdEncounter.m_playerWantsToConnect && m_pdEncounter.m_aliensWantToConnect )
			{
				// yes - make it happen
				Connect();
			}
		}

		// did we want the communications to terminate?
		if ( subject == GD_Comm.Subject.Terminate )
		{
			// yes - get out of video chat
			Disconnect();
		}
	}

	// connect the aliens and the players
	public void Connect()
	{
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

		// are the aliens trying to ask a question that we haven't answered yet?
		if ( ( m_pdEncounter.m_lastQuestionFromAliens != 0 ) && ( m_pdEncounter.m_lastSubjectFromPlayer == GD_Comm.Subject.None ) )
		{
			// yes - change to the answer question button set
			m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.AnswerQuestion );
		}
		else
		{
			// no - change to the normal comm buttons
			m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Comm );
		}

		// we are connected now
		m_pdEncounter.m_connected = true;
	}

	// disconnect the aliens and the players
	public void Disconnect()
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

		// change the buttons
		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.Bridge );

		// we are not connected any more
		m_pdEncounter.m_connected = false;
		m_pdEncounter.m_disconnected = true;

		// no one wants to connect any more
		m_pdEncounter.m_playerWantsToConnect = false;
		m_pdEncounter.m_aliensWantToConnect = false;
	}

	// call this to find out if there are living alien ships in the encounter
	public bool HasLivingAlienShips()
	{
		// get to the list of alien ships
		var alienShipList = m_pdEncounter.GetAlienShipList();

		// go through each alien ship
		foreach ( var alienShip in alienShipList )
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

	// whether or not the aliens want to connect
	public bool AliensWantToConnect()
	{
		return m_pdEncounter.m_aliensWantToConnect;
	}

	// whether or not we are connected to the aliens
	public bool IsConnected()
	{
		return m_pdEncounter.m_connected;
	}

	// hail (or respond to) the alien ships
	public void Hail( GD_Comm.Stance stance, bool responding )
	{
		// set the stance to what the player selected
		m_pdEncounter.m_playerStance = stance;

		// have we connected already?
		if ( !m_pdEncounter.m_connected )
		{
			// no - send the hail (responding or hailing)
			AddComm( responding ? GD_Comm.Subject.GreetingResponse : GD_Comm.Subject.GreetingHail, true );
		}
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
