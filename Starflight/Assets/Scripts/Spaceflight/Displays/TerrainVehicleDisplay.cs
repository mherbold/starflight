
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainVehicleDisplay : ShipDisplay
{
	// the status values text
	public TextMeshProUGUI m_statusValues;

	// the vitality names text
	public TextMeshProUGUI m_vitalityNames;

	// the vitality values text
	public TextMeshProUGUI m_vitalityValues;

	// the vitality bars
	public Image[] m_vitalityBars;

	PD_Personnel.PD_PersonnelFile[] m_personnelFiles;

	int m_numCrewInList;

	// for gizmo drawing
	Vector3[] m_debugVectors;

	public static readonly string[] c_cardinalDirections = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };

	TerrainVehicleDisplay()
	{
		m_debugVectors = new Vector3[ 2 ];
	}

	// the display label
	public override string GetLabel()
	{
		return "Status";
	}

	public override void Show()
	{
		base.Show();

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// (re)allocate the personnel file array
		m_personnelFiles = new PD_Personnel.PD_PersonnelFile[ (int) PD_CrewAssignment.Role.Count ];

		m_numCrewInList = 0;

		// go through each crew member
		for ( var i = PD_CrewAssignment.Role.First; i < PD_CrewAssignment.Role.Count; i++ )
		{
			var personnelFile = playerData.m_crewAssignment.GetPersonnelFile( i );

			bool alreadyThere = false;

			for ( var j = 0; j < m_numCrewInList; j++ )
			{
				if ( m_personnelFiles[ j ].m_fileId == personnelFile.m_fileId )
				{
					alreadyThere = true;
					break;
				}
			}

			if ( !alreadyThere )
			{
				m_personnelFiles[ m_numCrewInList++ ] = personnelFile;
			}
		}

		// update the crew member list
		m_vitalityNames.text = "";
		m_vitalityValues.text = "";

		for ( var i = 0; i < (int) PD_CrewAssignment.Role.Count; i++ )
		{
			if ( i < m_numCrewInList )
			{
				if ( i > 0 )
				{
					m_vitalityNames.text += "\n";
					m_vitalityValues.text += "\n";
				}

				m_vitalityNames.text += m_personnelFiles[ i ].m_name;
				m_vitalityValues.text += Mathf.CeilToInt( m_personnelFiles[ i ].m_vitality ) + "%";

				m_vitalityBars[ i ].enabled = true;
			}
			else
			{
				m_vitalityBars[ i ].enabled = false;
			}
		}
	}

	public override void Update()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// calculate ship coordinates
		var shipCoordinates = Tools.LatLongToWorldCoordinates( playerData.m_general.m_selectedLatitude, playerData.m_general.m_selectedLongitude );

		shipCoordinates = SpaceflightController.m_instance.m_disembarked.ApplyElevation( shipCoordinates, false );

		// calculate vector from TV to ship coordinates
		var vectorToShip = shipCoordinates - playerData.m_general.m_lastDisembarkedCoordinates;

		// how far is it in kilometers?
		var distanceInKm = vectorToShip.magnitude * 225.0f / 2048.0f - 2.0f;

		if ( distanceInKm < 0.0f )
		{
			distanceInKm = 0.0f;
		}

		// convert rotation to euler angles
		var eulerAngles = Quaternion.FromToRotation( Vector3.forward, vectorToShip ).eulerAngles;

		// convert euler angles to cardinal directions
		var index = Mathf.FloorToInt( ( eulerAngles.y - 22.5f ) / 45.0f ) + 1;

		var direction = c_cardinalDirections[ index ];

		// date and time
		m_statusValues.text = playerData.m_general.m_currentStardateDHMY + "\n";

		// get the amount of fuel remaining as a percent
		var percentFuelRemaining = playerData.m_terrainVehicle.GetPercentFuelRemaining();

		if ( percentFuelRemaining <= -5 )
		{
			m_statusValues.text += "<color=yellow>None</color>\n";
		}
		else if ( percentFuelRemaining <= 0 )
		{
			m_statusValues.text += "<color=red>Reserve</color>\n";
		}
		else
		{
			m_statusValues.text += percentFuelRemaining + "%\n";
		}

		// get the current terrain vehicle efficiency at this elevation
		var fuelEfficiency = SpaceflightController.m_instance.m_terrainVehicle.GetFuelEfficiency();

		m_statusValues.text += Mathf.RoundToInt( fuelEfficiency * 100.0f ) + "%\n";

		// get the amount of cargo space left as a percentage
		var percentRemainingVolume = playerData.m_terrainVehicle.GetPercentRemainingVolume();

		m_statusValues.text += ( 100 - percentRemainingVolume ) + "% Full\n";

		m_statusValues.text += Mathf.RoundToInt( distanceInKm ) + " KM. " + direction;

		m_debugVectors[ 0 ] = shipCoordinates;
		m_debugVectors[ 1 ] = playerData.m_general.m_lastDisembarkedCoordinates;
	}

#if UNITY_EDITOR

	// draw gizmos to help debug the game
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;

		for ( var i = 0; i < m_debugVectors.Length; i += 2 )
		{
			Gizmos.DrawLine( m_debugVectors[ i ], m_debugVectors[ i + 1 ] );
		}
	}

#endif
}
