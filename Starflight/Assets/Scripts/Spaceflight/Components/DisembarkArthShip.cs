
using UnityEngine;

public class DisembarkArthShip : MonoBehaviour
{
	public bool m_terrainVehicleIsInside;

	void OnTriggerEnter( Collider other )
	{
		if ( other.gameObject.tag == "Terrain Vehicle" )
		{
			m_terrainVehicleIsInside = true;
		}
	}

	void OnTriggerExit( Collider other )
	{
		if ( other.gameObject.tag == "Terrain Vehicle" )
		{
			m_terrainVehicleIsInside = false;
		}
	}
}
