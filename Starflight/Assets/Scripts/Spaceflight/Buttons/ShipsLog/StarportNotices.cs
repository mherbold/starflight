
public class StarportNoticesButton : ShipButton
{
	public override string GetLabel()
	{
		return "Starport Notices";
	}

	public override bool Execute()
	{
		// show the ships log
		SpaceflightController.m_instance.m_shipsLog.Show();

		return false;
	}
}
