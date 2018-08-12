
using UnityEngine;

abstract public class Button
{
	// convenient access to the spaceflight controller
	protected SpaceflightController m_spaceflightController;

	public Button()
	{
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );

		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();
	}

	public virtual string GetLabel()
	{
		return "???";
	}

	public virtual bool Execute()
	{
		return false;
	}

	public virtual void Cancel()
	{
	}

	public virtual bool Update()
	{
		return false;
	}
}
