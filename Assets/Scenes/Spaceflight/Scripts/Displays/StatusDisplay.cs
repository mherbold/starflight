
using TMPro;

public class StatusDisplay : ShipDisplay
{
	// the values text
	public TextMeshProUGUI m_values;

	// unity update
	public override void Update()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// update the date
		m_values.text = playerData.m_starflight.m_currentStardateDHMY + "\n";

		// TODO: do actual damage text
		m_values.text += "None\n";

		// update the cargo usage
		// TODO: change the color depending on how full the storage is
		float cargoUsage = (float) playerData.m_ship.m_volumeUsed / (float) playerData.m_ship.m_volume * 100.0f;
		m_values.text += cargoUsage.ToString( "N1" ) + "% Full\n";

		// get to the endurium in the ship storage
		ElementReference elementReference = playerData.m_ship.m_elementStorage.Find( 5 );

		// update the amount of energy remaining
		if ( elementReference == null )
		{
			m_values.text += "None\n";
		}
		else
		{
			float energyAmount = (float) elementReference.m_volume / 10.0f;

			// TODO: change the color depending on the amount remaining
			m_values.text += energyAmount.ToString( "N1" ) + "M<sup>3</sup>\n";
		}

		// TODO: actual shields text
		m_values.text += "Down\n";

		// TODO: actual weapons text
		m_values.text += "Unarmed\n";
	}

	// the status display label
	public override string GetLabel()
	{
		return "Status";
	}
}
