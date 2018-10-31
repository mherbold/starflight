
using UnityEngine;
using TMPro;

public class StatusDisplay : ShipDisplay
{
	// the values text
	public TextMeshProUGUI m_values;

	// the missile launcher
	public GameObject m_missileLauncher;

	// the laser cannon
	public GameObject m_laserCannon;

	// the cargo pods
	public GameObject[] m_cargoPods;

	// unity start
	public override void Start()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// show only as many cargo pods as we have purchased
		for ( int cargoPodId = 0; cargoPodId < m_cargoPods.Length; cargoPodId++ )
		{
			m_cargoPods[ cargoPodId ].SetActive( cargoPodId < playerData.m_playerShip.m_numCargoPods );
		}

		// hide or show the missile launchers depending on if we have them
		m_missileLauncher.SetActive( playerData.m_playerShip.m_missileLauncherClass > 0 );

		// hide or show the missile launchers depending on if we have them
		m_laserCannon.SetActive( playerData.m_playerShip.m_laserCannonClass > 0 );
	}

	// unity update
	public override void Update()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// update the date
		m_values.text = playerData.m_general.m_currentStardateDHMY + "\n";

		// TODO: do actual damage text
		m_values.text += "None\n";

		// update the cargo usage
		// TODO: change the color depending on how full the storage is
		float cargoUsage = (float) playerData.m_playerShip.m_volumeUsed / (float) playerData.m_playerShip.m_volume * 100.0f;
		m_values.text += cargoUsage.ToString( "N1" ) + "% Full\n";

		// get to the endurium in the ship storage
		PD_ElementReference elementReference = playerData.m_playerShip.m_elementStorage.Find( 5 );

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
