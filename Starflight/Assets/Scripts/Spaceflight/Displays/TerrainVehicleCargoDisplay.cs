
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainVehicleCargoDisplay : ShipDisplay
{
	TerrainVehicleCargoDisplay()
	{
	}

	// the display label
	public override string GetLabel()
	{
		return "Cargo";
	}

	public override void Show()
	{
		base.Show();
	}

	public override void Update()
	{
	}
}
