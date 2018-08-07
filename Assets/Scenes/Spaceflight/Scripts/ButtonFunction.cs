
using UnityEngine;

abstract public class ButtonFunction
{
	// convenient access to the spaceflight controller
	protected SpaceflightController m_spaceflightController;

	public ButtonFunction()
	{
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );

		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();
	}

	public abstract void Execute();

	public abstract void Cancel();
}
