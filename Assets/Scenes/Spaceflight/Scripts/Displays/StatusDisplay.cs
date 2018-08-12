
using UnityEngine;

public class StatusDisplay : Display
{
	public StatusDisplay( GameObject rootGameObject ) : base( rootGameObject )
	{
	}

	public override string GetLabel()
	{
		return "Status";
	}

	public override void Start()
	{
		m_rootGameObject.SetActive( true );
	}

	public override void Update()
	{
	}
}
