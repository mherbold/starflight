using System;

public class ShipCargoButton : ShipButton
{
	public override string GetLabel()
	{
		return "Cargo";
	}

	public override bool Execute()
	{
		SoundController.m_instance.PlaySound( SoundController.Sound.Error );

		SpaceflightController.m_instance.m_messages.Clear();

		SpaceflightController.m_instance.m_messages.RenderTable(GetCargoDataTable(), new int[] { 55, 25, 20}, new TMPro.TextAlignmentOptions[] { TMPro.TextAlignmentOptions.TopLeft, TMPro.TextAlignmentOptions.TopRight, TMPro.TextAlignmentOptions.TopRight});

		SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

		return false;
	}

	private string[] GetCargoDataTable()
    {
		string itemColumn = "ITEM\n";
		string volumeColumn = "VOLUME\n";
		string valueColumn = "VALUE\n";
		foreach (PD_ElementReference elementRef in DataController.m_instance.m_playerData.m_playerShip.m_elementStorage.m_elementList)
        {
			itemColumn += "   " + elementRef.GetElementGameData().m_name + "\n";
			volumeColumn += elementRef.GetVolume() + "\n";
			valueColumn += elementRef.GetElementGameData().m_actualValue + "\n";
		}
		return new string[] { itemColumn, volumeColumn, valueColumn };
    }
}
