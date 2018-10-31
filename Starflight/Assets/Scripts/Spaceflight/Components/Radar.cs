
using UnityEngine;

public class Radar : MonoBehaviour
{
	public SVGImage m_radar;

	// unity start
	void Start()
	{
		// hide the radar outline
		m_radar.color = new Color( 1, 1, 1, 0 );
	}

	// unity update
	void Update()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// radar is visible only in hyperspace or in the star system
		if ( ( playerData.m_general.m_location != PD_General.Location.Hyperspace ) && ( playerData.m_general.m_location != PD_General.Location.StarSystem ) )
		{
			// hide the radar outline
			m_radar.color = new Color( 1, 1, 1, 0 );

			// nothing more to do here
			return;
		}

		// go through each potential encounter
		foreach ( var encounter in playerData.m_encounterList )
		{
			// skip encounter if we are not even in the same location
			if ( encounter.GetLocation() != playerData.m_general.m_location )
			{
				continue;
			}

			// if location is star system then check if we are in that star system
			if ( playerData.m_general.m_location == PD_General.Location.StarSystem )
			{
				if ( encounter.GetStarId() != playerData.m_general.m_currentStarId )
				{
					continue;
				}
			}

			// are they close enough for us to detect them?

		}
	}
}
