
using UnityEngine;
using System;

[Serializable]

public class Starflight
{
	public string m_currentStardate;

	public int m_day;
	public int m_hour;
	public int m_minute;
	public int m_second;
	public int m_millisecond;

	public float m_gameTime;

	public void Reset()
	{
		// reset the current stardate
		m_currentStardate = "4620-01-01";

		// reset the current game time
		m_day = 0;
		m_hour = 0;
		m_minute = 0;
		m_second = 0;
		m_millisecond = 0;
	}
	
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
		dateTime.AddDays( m_day );
		m_currentStardate = dateTime.ToString( "yyyy-MM-dd" );
	}
}
