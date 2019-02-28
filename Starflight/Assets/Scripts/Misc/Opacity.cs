
using UnityEngine;
using System.Collections.Generic;

public class Opacity : MonoBehaviour
{
	// the initial opacity
	[SerializeField] float m_initialOpacity = 1.0f;

	// how bright to make the lights
	[SerializeField] float m_lightIntensityMultiplier = 1.0f;

	// the current opacity
	float m_currentOpacity;

	// whether or not we changed the opacity
	bool m_opacityChanged;

	// all of the game objects we want to control the opacity of
	List<Material> m_materialList;
	List<Light> m_lightList;

	Opacity()
	{
		m_materialList = new List<Material>();
		m_lightList = new List<Light>();
	}

	// unity start
	void Start()
	{
		Initialize( gameObject );

		m_currentOpacity = m_initialOpacity;
		m_opacityChanged = true;
	}

	// unity update
	void Update()
	{
		if ( m_opacityChanged )
		{
			foreach ( var material in m_materialList )
			{
				Tools.SetOpacity( material, m_currentOpacity );
			}

			foreach ( var light in m_lightList )
			{
				light.intensity = m_currentOpacity * m_lightIntensityMultiplier;
			}

			m_opacityChanged = false;
		}
	}

	public void SetOpacity( float opacity )
	{
		m_currentOpacity = opacity;
		m_opacityChanged = true;
	}

	void Initialize( GameObject gameObject )
	{
		var meshRenderer = gameObject.GetComponent<MeshRenderer>();

		if ( meshRenderer != null )
		{
			meshRenderer.material = new Material( meshRenderer.material );

			m_materialList.Add( meshRenderer.material );
		}

		var light = gameObject.GetComponent<Light>();

		if ( light != null )
		{
			m_lightList.Add( light );
		}

		for ( var i = 0; i < gameObject.transform.childCount; i++ )
		{
			var childTransform = gameObject.transform.GetChild( i );

			Initialize( childTransform.gameObject );
		}
	}
}
