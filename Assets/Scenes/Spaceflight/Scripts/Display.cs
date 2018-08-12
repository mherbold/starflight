
using UnityEngine;

abstract public class Display
{
	// convenient access to the spaceflight controller
	protected SpaceflightController m_spaceflightController;

	// the root display gameobject
	protected GameObject m_rootGameObject;

	public Display( GameObject rootGameObject )
	{
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );

		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();

		m_rootGameObject = rootGameObject;
	}

	public virtual string GetLabel()
	{
		return "???";
	}

	public virtual void Start()
	{
	}

	public virtual void Stop()
	{
		m_rootGameObject.SetActive( false );
	}

	public virtual void Update()
	{
	}
}
