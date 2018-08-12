
using UnityEngine;
using UnityEngine.EventSystems;

public class StatusDisplay : Display
{
	public override string GetLabel()
	{
		return "Status";
	}

	public override void Start()
	{
		m_spaceflightController.m_statusDisplayUI.SetActive( true );
	}

	public override void Update()
	{
	}
}
