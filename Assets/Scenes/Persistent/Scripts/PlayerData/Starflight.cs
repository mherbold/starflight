
using UnityEngine;
using System;

[Serializable]

public class Starflight
{
	public enum Location
	{
		Starport,
		DockingBay,
		JustLaunched,
		StarSystem,
		Hyperspace,
		InOrbit,
		OnPlanet
	}

	// the player location
	public Location m_location;

	// the player coordinates
	public SerializableVector3 m_starportCoordinates;
	public SerializableVector3 m_systemCoordinates;
	public SerializableVector3 m_hyperspaceCoordinates;

	// game time stuff
	public string m_currentStardateYMD;
	public string m_currentStardateDHMY;

	public int m_day;
	public int m_hour;
	public int m_minute;
	public int m_second;
	public int m_millisecond;

	public float m_gameTime;

	// temporary - this will be replaced
	public bool m_hasCurrentSenorReading;

	// the current star we are in (or the last star we visited if we are in hyperspace)
	public int m_currentStarId;

	// the current planet we are visiting (or the last planet we visited)
	public int m_currentPlanetId;

	// keep track of the player's current speed
	public float m_currentSpeed;

	// keep track of the player's current direction
	public SerializableVector3 m_currentDirection;

	// this resets everything to initial game state
	public void Reset()
	{
		// reset location
		m_location = Location.Starport;

		// reset coordinates
		m_starportCoordinates = new Vector3( 0.0f, 0.0f, 0.0f );
		m_systemCoordinates = new Vector3( 0.0f, 0.0f, 0.0f );
		m_hyperspaceCoordinates = new Vector3( 0.0f, 0.0f, 0.0f );

		// reset the current stardate
		m_currentStardateYMD = "4620-01-01";

		// reset the current game time
		m_day = 0;
		m_hour = 0;
		m_minute = 0;
		m_second = 0;
		m_millisecond = 0;

		// reset flags
		m_hasCurrentSenorReading = false;

		// reset star id
		GameData gameData = DataController.m_instance.m_gameData;
		m_currentStarId = gameData.m_misc.m_arthStarId;

		// facing north
		m_currentDirection = Vector3.forward;

		// not moving
		m_currentSpeed = 0.0f;
	}
	
	// this updates the game time
	public void UpdateGameTime( float deltaTime )
	{
		// we want 1 year of arth time to be equal to 50 hours of game play time
		float scale = ( 365.0f * 24.0f ) / 50.0f;

		deltaTime *= scale;

		// convert deltaTime to milliseconds as an integer
		int deltaMilliseconds = Mathf.RoundToInt( deltaTime * 1000.0f );

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
		DateTime dateTime = new DateTime( 4620, 1, 1 );
		dateTime = dateTime.AddDays( m_day );
		dateTime = dateTime.AddHours( m_hour );
		m_currentStardateYMD = dateTime.ToString( "yyyy-MM-dd" );
		m_currentStardateDHMY = dateTime.ToString( "dd.HH-MM-yyyy" );
	}
}
