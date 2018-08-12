
using UnityEngine;
using UnityEngine.EventSystems;

public class SystemDisplay : Display
{
	public override string GetLabel()
	{
		return "System";
	}

	public override void Start()
	{
		m_spaceflightController.m_systemDisplayUI.SetActive( true );
	}

	public override void Update()
	{
	}
}
