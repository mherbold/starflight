
using UnityEngine;
using UnityEngine.UI;

public class RandomScaleOffset : MonoBehaviour
{
	// our material
	Material m_material;

	// unity start
	void Start()
	{
		// try to find a material
		var image = GetComponent<Image>();

		if ( image != null )
		{
			m_material = new Material( image.material );

			image.material = m_material;
		}
		else
		{
			var meshRenderer = GetComponent<MeshRenderer>();

			if ( meshRenderer != null )
			{
				m_material = new Material( meshRenderer.material );

				meshRenderer.material = m_material;
			}
			else
			{
				Debug.Log( "Could not find component to apply the random scale offset to." );
			}
		}
	}

	// unity update
	void Update()
	{
		// give the albedo map a random offset
		m_material.SetVector( "SF_AlbedoMapScaleOffset", new Vector4( 1.0f, 1.0f, Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) ) );
	}
}
