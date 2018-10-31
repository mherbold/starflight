
using UnityEngine;

public class Radar : MonoBehaviour
{
	public SVGImage m_ring;
	public SVGImage m_sweep;

	float m_sweepAngle;

	// unity start
	void Start()
	{
		// hide the radar outline
		m_ring.color = new Color( 1, 1, 1, 0 );

		// start sweep angle at zero
		m_sweepAngle = 0.0f;
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
			m_ring.color = new Color( 1, 1, 1, 0 );

			// nothing more to do here
			return;
		}

		// start counting detected encounters
		var encounterIndex = 0;

		// go through each potential encounter
		foreach ( var encounter in playerData.m_encounterList )
		{
			// skip encounter if we are not even in the same location
			if ( encounter.GetLocation() != playerData.m_general.m_location )
			{
				continue;
			}

			var distance = 0.0f;

			// if location is star system then check if we are in that star system
			if ( playerData.m_general.m_location == PD_General.Location.StarSystem )
			{
				if ( encounter.GetStarId() != playerData.m_general.m_currentStarId )
				{
					continue;
				}

				// calculate the distance
				distance = Vector3.Distance( playerData.m_general.m_starSystemCoordinates, encounter.m_coordinates );
			}
			else
			{
				// calculate the distance
				distance = Vector3.Distance( playerData.m_general.m_hyperspaceCoordinates, encounter.m_coordinates );
			}

			// are they close enough for us to detect them?
			if ( distance > 512.0f )
			{
				// no - skip this encounter
				continue;
			}
		}

		// rotate the sweep
		m_sweepAngle += Time.deltaTime * 30.0f;

		if ( m_sweepAngle >= 360.0f )
		{
			m_sweepAngle -= 360.0f;
		}

		m_sweep.rectTransform.localRotation = Quaternion.Euler( 0.0f, 0.0f, -m_sweepAngle );
	}
}
