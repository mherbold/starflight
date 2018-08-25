
using System;

[Serializable]

public class Star
{
	public const int c_maxNumPlanets = 8;

	public int m_id;
	public int m_xCoordinate;
	public int m_yCoordinate;
	public string m_class;
	public int m_daysToNextFlare;
	public int m_daysSincePreviousFlare;
	public int m_yearOfNextFlare;
	public int m_monthOfNextFlare;
	public int m_dayOfNextFlare;

	public Planet[] m_planetList;

	public void Initialize( GameData gameData )
	{
		// allocate planets array
		m_planetList = new Planet[ 8 ];

		// initialize planets array with null (means no planet in that orbit)
		for ( int i = 0; i < c_maxNumPlanets; i++ )
		{
			m_planetList[ i ] = null;
		}

		// find planets and put them in the right spot in the planets array
		foreach ( Planet planet in gameData.m_planetList )
		{
			if ( planet.m_starId == m_id )
			{
				m_planetList[ planet.m_orbitPosition ] = planet;
			}
		}
	}
}
