
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

	public virtual string GetButtonLabel()
	{
		return "???";
	}

	public virtual void Execute()
	{
	}

	public virtual void Cancel()
	{
	}

	public virtual bool Update()
	{
		return false;
	}
}
