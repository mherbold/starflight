
using UnityEngine;

public class JustLaunched : MonoBehaviour
{
	// convenient access to the spaceflight controller
	SpaceflightController m_spaceflightController;

	// unity awake
	private void Awake()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();
	}

	// unity start
	void Start()
	{

	}

	// unity update
	void Update()
	{

	}

	// call this to switch to the just launched mode
	public void Show()
	{
		// update the system controller
		m_spaceflightController.m_systemController.EnterSystem();

		// make sure the map is NOT visible
		m_spaceflightController.m_spaceflightUI.FadeMap( 0.0f );
	}
}
