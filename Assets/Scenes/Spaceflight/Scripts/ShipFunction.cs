
using UnityEngine;
using UnityEngine.EventSystems;

abstract public class ShipFunction
{
	// convenient access to the spaceflight controller
	protected SpaceflightController m_spaceflightController;

	public ShipFunction()
	{
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );

		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();
	}

	public abstract void Execute();

	public abstract void Cancel();
}
