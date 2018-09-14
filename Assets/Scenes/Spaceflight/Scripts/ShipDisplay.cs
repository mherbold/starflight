
using UnityEngine;

abstract public class ShipDisplay : MonoBehaviour
{
	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// unity start
	public virtual void Start()
	{
	}

	// unity update
	public virtual void Update()
	{
	}

	// returns the display label
	public virtual string GetLabel()
	{
		return "???";
	}

	// call this to show the display
	public virtual void Show()
	{
		gameObject.SetActive( true );
	}

	// call this to hide the display
	public virtual void Hide()
	{
		gameObject.SetActive( false );
	}
}
