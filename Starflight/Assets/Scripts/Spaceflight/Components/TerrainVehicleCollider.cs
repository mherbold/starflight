
using UnityEngine;

public class TerrainVehicleCollider : MonoBehaviour
{
	public TerrainVehicle m_terrainVehicle;

	void OnCollisionStay( Collision other )
	{
		if ( other.gameObject.tag == "Terrain Vehicle" )
		{
			var contact = other.GetContact( 0 );

			m_terrainVehicle.AddPushBack( contact.normal, contact.separation );
		}
	}
}
