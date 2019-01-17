
public class SelectSiteButton : ShipButton
{
	public override string GetLabel()
	{
		return "Select Site";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_displayController.m_terrainMapDisplay.ShowSelectSite();

		return true;
	}

	public override bool Update()
	{
		SpaceflightController.m_instance.m_displayController.m_terrainMapDisplay.MoveCrosshairs();

		if ( InputController.m_instance.m_submit )
		{
			InputController.m_instance.Debounce();

			SpaceflightController.m_instance.m_buttonController.DeactivateButton();

			SpaceflightController.m_instance.m_displayController.m_terrainMapDisplay.HideSelectSite();
		}

		return true;
	}
}
