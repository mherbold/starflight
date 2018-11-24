
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
			m_blip = blip;

			Reset();
		}

		public void Reset()
		{
			m_encounter = null;
			m_timeSinceDetection = 3600.0f;
			m_initialOpacity = 0.0f;
		}
	}

	public float m_maxHyperspaceDetectionDistance = 1024.0f;
	public float m_maxStarSystemDetectionDistance = 4096.0f;

	public MeshRenderer m_ring;
	public MeshRenderer m_sweep;
	public MeshRenderer[] m_blips;

	float m_sweepAngle;
	float m_timeSinceLastDetection;

	Detection[] m_detectionList;

	SpaceflightController m_spaceflightController;

	// unity start
	void Start()
	{
		// get access to the spaceflight controller
		var controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );

		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();

		// clone materials
		m_ring.material = new Material( m_ring.material );
		m_sweep.material = new Material( m_sweep.material );

		foreach ( var blip in m_blips )
		{
			blip.material = new Material( blip.material );
		}

		// start sweep angle at zero
		m_sweepAngle = 0.0f;

		// allocate detection list
		m_detectionList = new Detection[ m_blips.Length ];

		for ( var i = 0; i < m_detectionList.Length; i++ )
		{
			m_detectionList[ i ] = new Detection( m_blips[ i ] );
		}

		// make everything invisible
		Show();
	}

	// unity update
	void Update()
	{
		float opacity;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// update time since last detection
		m_timeSinceLastDetection = Mathf.Min( 3600.0f, m_timeSinceLastDetection + Time.deltaTime );

		// rotate the sweep (60 degrees per second, 6 seconds = full sweep)
		m_sweepAngle += Time.deltaTime * 60.0f;

		if ( m_sweepAngle >= 180.0f )
		{
			m_sweepAngle -= 360.0f;
		}

		m_sweep.transform.localRotation = Quaternion.Euler( 0.0f, 0.0f, m_sweepAngle );

		// sort the encounter list by distance
		Array.Sort( playerData.m_encounterList );

		// get our current radar detection distance
		var maxDetectionDistance = ( playerData.m_general.m_location == PD_General.Location.Hyperspace ) ? m_maxHyperspaceDetectionDistance : m_maxStarSystemDetectionDistance;

		// go through each potential encounter
		foreach ( var encounter in playerData.m_encounterList )
		{
			// are they close enough for us to detect them?
			if ( encounter.GetDistance() > maxDetectionDistance )
			{
				// no - stop now (the list is sorted so all remaining encounters are further away)
				break;
			}

			// calculate the direction of the encounter relative to the player
			var encounterDirection = encounter.m_currentCoordinates - playerData.m_general.m_coordinates;

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
					opacity = Mathf.Lerp( 0.3f, 1.0f, 1.0f - ( encounter.GetDistance() / maxDetectionDistance ) );

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

					// let the player know we've detected motion
					m_spaceflightController.m_messages.ChangeText( "<color=white>Motion detected.</color>" );
				}
			}
		}

		// get the current map fade amount
		var currentMapFadeAmount = m_spaceflightController.m_map.GetCurrentFadeAmount();

		// go through each detection in the detection list
		foreach ( var detection in m_detectionList )
		{
			// is this blip active?
			if ( detection.m_encounter != null )
			{
				// yes - update the time since detected
				detection.m_timeSinceDetection += Time.deltaTime;

				// has it been more than 6 seconds?
				if ( detection.m_timeSinceDetection >= 6.0f )
				{
					// yes - drop this blip (free it up)
					detection.m_timeSinceDetection = 6.0f;

					detection.m_encounter = null;

					Tools.SetOpacity( detection.m_blip.material, 0 );
				}
				else
				{
					// has it been more than 3 seconds?
					if ( detection.m_timeSinceDetection >= 3.0f )
					{
						// yes - calculate the fade amount
						opacity = Mathf.Lerp( detection.m_initialOpacity, 0.0f, ( detection.m_timeSinceDetection - 3.0f ) / 3.0f );
					}
					else
					{
						// no - it should be fully visible
						opacity = detection.m_initialOpacity;
					}

					// update the opacity of the blip
					Tools.SetOpacity( detection.m_blip.material, opacity * currentMapFadeAmount );
				}
			}
		}

		// update the opacity of the ring and sweep indicator
		opacity = Mathf.Lerp( 1.0f, 0.0f, ( m_timeSinceLastDetection - 12.0f ) / 6.0f );

		Tools.SetOpacity( m_ring.material, opacity * currentMapFadeAmount );
		Tools.SetOpacity( m_sweep.material, opacity * currentMapFadeAmount );
	}

	// hide the radar
	public void Hide()
	{
		// make this game object not active
		gameObject.SetActive( false );
	}

	// show the radar
	public void Show()
	{
		// make this game object active
		gameObject.SetActive( true );

		// reset the time since last detection
		m_timeSinceLastDetection = 3600.0f;

		// make everything invisible to start with
		Tools.SetOpacity( m_ring.material, 0 );
		Tools.SetOpacity( m_sweep.material, 0 );

		foreach ( var blip in m_blips )
		{
			Tools.SetOpacity( blip.material, 0 );
		}

		// reset all detections
		if ( m_detectionList != null )
		{
			foreach ( var detection in m_detectionList )
			{
				detection.Reset();
			}
		}
	}
}
