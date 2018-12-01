
using UnityEngine;

abstract public class ShipButton
{
	// convenient access to the spaceflight controller
	protected SpaceflightController m_spaceflightController;

	public ShipButton()
	{
		var controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );

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

	public virtual bool Update()
	{
		return false;
	}
}
