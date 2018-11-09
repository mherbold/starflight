
using UnityEngine;
using System;

[Serializable]

public class PD_General
{
	public enum Location
	{
		Starport,
		DockingBay,
		JustLaunched,
		StarSystem,
		Hyperspace,
		InOrbit,
		OnPlanet,
		Encounter,
	}

	// the player location
	public Location m_location;

	// the player coordinates
	public Vector3 m_coordinates;

	// the last known coordinates for various locations
	public Vector3 m_lastStarportCoordinates;
	public Vector3 m_lastHyperspaceCoordinates;
	public Vector3 m_lastStarSystemCoordinates;
	public Vector3 m_lastEncounterCoordinates;

	// game time stuff
	public string m_currentStardateYMD;
	public string m_currentStardateDHMY;

	public int m_day;
	public int m_hour;
	public int m_minute;
	public int m_second;
	public int m_millisecond;

	public float m_gameTime;

	// the current star we are in (or the last star we visited if we are in hyperspace)
	public int m_currentStarId;

	// the current planet we are visiting (or the last planet we visited)
	public int m_currentPlanetId;

	// the current encounter we are in (or the last encounter we were in)
	public int m_currentEncounterId;

	// keep track of the player's current speed and maximum speed
	public float m_currentSpeed;
	public float m_currentMaximumSpeed;

	// keep track of the player's current direction
	public Vector3 m_currentDirection;

	// this resets everything to initial game state
	public void Reset()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// reset location
		m_location = Location.Starport;

		// reset coordinates (standing in front of the operations door)
		m_coordinates = new Vector3( -35.18f, 0.0f, 20.86f );

		// reset last coordinates
		m_lastStarportCoordinates = m_coordinates;
		m_lastHyperspaceCoordinates = Tools.GameToWorldCoordinates( new Vector3( 125.0f, 0.0f, 100.0f ) );
		m_lastStarSystemCoordinates = Vector3.zero;
		m_lastEncounterCoordinates = Vector3.zero;

		// reset the current stardate
		m_currentStardateYMD = "4620-01-01";
		m_currentStardateDHMY = "01.00-01-4620";

		// reset the current game time
		m_day = 0;
		m_hour = 0;
		m_minute = 0;
		m_second = 0;
		m_millisecond = 0;

		// reset current ids
		m_currentStarId = gameData.m_misc.m_arthStarId;
		m_currentPlanetId = 0;
		m_currentEncounterId = 0;

		// facing north
		m_currentDirection = Vector3.forward;

		// not moving
		m_currentSpeed = 0.0f;
		m_currentMaximumSpeed = 10.0f;
	}
	
	// this updates the game time
	public void UpdateGameTime( float deltaTime )
	{
		// we want 1 year of arth time to be equal to 50 hours of game play time
		var scale = ( 365.0f * 24.0f ) / 50.0f;

		deltaTime *= scale;

		// convert deltaTime to milliseconds as an integer
		var deltaMilliseconds = Mathf.RoundToInt( deltaTime * 1000.0f );

		// update the day hour minute second and millisecond
		m_millisecond += deltaMilliseconds;
		m_second += m_millisecond / 1000;
		m_millisecond %= 1000;
		m_minute += m_second / 60;
		m_second %= 60;
		m_hour += m_minute / 60;
		m_minute %= 60;
		m_day += m_hour / 24;
		m_hour %= 24;

		// update the game time (represented as days with fractional precision up to seconds)
		m_gameTime = (float) m_day + ( (float) m_hour / 24 ) + ( (float) m_minute / ( 60 * 24 ) ) + ( (float) m_second / ( 60 * 60 * 24 ) );

		// update the current stardate
		var dateTime = new DateTime( 4620, 1, 1 );
		dateTime = dateTime.AddDays( m_day );
		dateTime = dateTime.AddHours( m_hour );
		m_currentStardateYMD = dateTime.ToString( "yyyy-MM-dd" );
		m_currentStardateDHMY = dateTime.ToString( "dd.HH-MM-yyyy" );
	}
}
