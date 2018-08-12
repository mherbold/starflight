
using UnityEngine;

abstract public class Display
{
	// convenient access to the spaceflight controller
	protected SpaceflightController m_spaceflightController;

	public Display()
	{
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );

		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();
	}

	public virtual string GetLabel()
	{
		return "???";
	}

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
	}
}
