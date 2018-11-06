
using UnityEngine;
using System;

public class Radar : MonoBehaviour
{
	class Detection
	{
		public PD_Encounter m_encounter;
		public float m_timeSinceDetection;
		public MeshRenderer m_blip;
		public float m_initialOpacity;

		public Detection( MeshRenderer blip )
		{
			m_encounter = null;
			m_timeSinceDetection = 3600.0f;
			m_blip = blip;
			m_initialOpacity = 0.0f;
		}
	}

	public float m_maxDetectionDistance = 1024.0f;

	public MeshRenderer m_ring;
	public MeshRenderer m_sweep;
	public MeshRenderer[] m_blips;

	float m_sweepAngle;
	float m_timeSinceLastDetection;

	Detection[] m_detectionList;

	// unity start
	void Start()
	{
		// clone material and make them all invisible
		m_ring.material = new Material( m_ring.material );

		SetOpacity( m_ring.material, 0 );

		m_sweep.material = new Material( m_sweep.material );

		SetOpacity( m_sweep.material, 0 );

		foreach ( var blip in m_blips )
		{
			blip.material = new Material( blip.material );

			SetOpacity( blip.material, 0 );
		}

		// start sweep angle at zero
		m_sweepAngle = 0.0f;

		// allocate detection list
		m_detectionList = new Detection[ m_blips.Length ];

		for ( var i = 0; i < m_detectionList.Length; i++ )
		{
			m_detectionList[ i ] = new Detection( m_blips[ i ] );
		}
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
			SetOpacity( m_ring.material, 0 );

			// nothing more to do here
			return;
		}

		// rotate the sweep (60 degrees per second, 6 seconds = full sweep)
		m_sweepAngle += Time.deltaTime * 60.0f;

		if ( m_sweepAngle >= 180.0f )
		{
			m_sweepAngle -= 360.0f;
		}

		m_sweep.transform.localRotation = Quaternion.Euler( 0.0f, 0.0f, m_sweepAngle );

		// update detection list - drop out detections older than 6 seconds
		foreach ( var detection in m_detectionList )
		{
			detection.m_timeSinceDetection += Time.deltaTime;

			if ( detection.m_timeSinceDetection >= 6.0f )
			{
				detection.m_timeSinceDetection = 6.0f;

				detection.m_encounter = null;

				SetOpacity( detection.m_blip.material, 0 );
			}
			else if ( detection.m_timeSinceDetection >= 3.0f )
			{
				var opacity = Mathf.Lerp( detection.m_initialOpacity, 0.0f, ( detection.m_timeSinceDetection - 3.0f ) / 3.0f );

				SetOpacity( detection.m_blip.material, opacity );
			}
		}

		// figure out which coordinate to use for encounter distances
		var coordinates = ( playerData.m_general.m_location == PD_General.Location.Hyperspace ) ? playerData.m_general.m_hyperspaceCoordinates : playerData.m_general.m_starSystemCoordinates;

		// go through each potential encounter
		foreach ( var encounter in playerData.m_encounterList )
		{
			// update the distance to the encounter
			encounter.Update( playerData.m_general.m_location, playerData.m_general.m_currentStarId, coordinates );
		}

		// sort the results
		Array.Sort( playerData.m_encounterList );

		// go through each potential encounter
		foreach ( var encounter in playerData.m_encounterList )
		{
			// are they close enough for us to detect them?
			if ( encounter.GetDistance() > m_maxDetectionDistance )
			{
				// no - stop now (the list is sorted so all remaining encounters are further away)
				break;
			}

			// calculate the direction of the encounter relative to the player
			var encounterDirection = encounter.m_coordinates - coordinates;

			// calculate the angle of the encounter
			var angle = Vector3.SignedAngle( Vector3.forward, encounterDirection, Vector3.up );

			// is it close to our current sweep direction for this frame?
			if ( ( angle > m_sweepAngle - 15.0f ) && ( angle < m_sweepAngle ) )
			{
				// yes - go through detection list
				bool ignoreEncounter = false;

				foreach ( var detection in m_detectionList )
				{
					// is this an active detection?
					if ( detection.m_encounter == encounter )
					{
						// yes - did we recently detect this?
						if ( detection.m_timeSinceDetection < 3.0f )
						{
							// yes - just reset the time since detection and ignore this encounter
							detection.m_timeSinceDetection = 0.0f;

							ignoreEncounter = true;

							break;
						}
					}
				}

				if ( ignoreEncounter )
				{
					continue;
				}

				// detection slot to use
				Detection detectionToUse = null;

				// go through detection list
				foreach ( var detection in m_detectionList )
				{
					// is this detection already in our list?
					if ( detection.m_encounter == encounter )
					{
						// yes - just update the same one
						detectionToUse = detection;
						break;
					}
				}

				// still need to find a detection slot?
				if ( detectionToUse == null )
				{
					// yes - go through the detection list again
					foreach ( var detection in m_detectionList )
					{
						// is this slot available?
						if ( detection.m_encounter == null )
						{
							// yes - use this one
							detectionToUse = detection;
							break;
						}
					}
				}

				// did we find a slot to use?
				if ( detectionToUse != null )
				{
					// calculate the blip opacity
					var opacity = Mathf.Lerp( 0.3f, 1.0f, 1.0f - ( encounter.GetDistance() / m_maxDetectionDistance ) );

					// yes - update the blip material
					SetOpacity( detectionToUse.m_blip.material, opacity );

					// set the rotation (position really) of the blip
					detectionToUse.m_blip.transform.localRotation = Quaternion.Euler( 0.0f, 0.0f, angle );

					// reset time since detection
					detectionToUse.m_timeSinceDetection = 0.0f;

					// remember the encounter
					detectionToUse.m_encounter = encounter;

					// remember the opacity
					detectionToUse.m_initialOpacity = opacity;

					// reset time since last detection
					m_timeSinceLastDetection = 0.0f;

					// play the radar blip sound
					SoundController.m_instance.PlaySound( SoundController.Sound.RadarBlip, Mathf.Pow( opacity, 2.0f ) );
				}
			}
		}

		{
			m_timeSinceLastDetection = Mathf.Min( 3600.0f, m_timeSinceLastDetection + Time.deltaTime );

			var opacity = Mathf.Lerp( 1.0f, 0.0f, ( m_timeSinceLastDetection - 12.0f ) / 6.0f );

			SetOpacity( m_ring.material, opacity );
			SetOpacity( m_sweep.material, opacity );
		}
	}

	void SetOpacity( Material material, float opacity )
	{
		var color = material.GetColor( "SF_AlbedoColor" );

		color.a = opacity;

		material.SetColor( "SF_AlbedoColor", color );
	}
}
