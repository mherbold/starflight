
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensorsDisplay : ShipDisplay
{
	// the mass text
	public TextMeshProUGUI m_mass;

	// the bio / min text
	public TextMeshProUGUI m_bioMin;

	// the missile launcher
	public Image m_scanEffect;

	// how fast to cycle the colors
	public float m_colorCycleSpeed;

	// duration for scanning a 0% planet
	public float m_minDuration;

	// duration for scanning a 200% planet
	public float m_maxDuration;

	// do we have sensor data to analyze?
	public bool m_hasSensorData;

	// are we running the cinematics?
	bool m_isDoingCinematics;

	// have we stopped the sensor sound yet?
	bool m_soundStopped;

	// our timer
	float m_timer;

	// unity start
	public override void Start()
	{
	}

	// unity update
	public override void Update()
	{
		// are we doing the cinematics?
		if ( m_isDoingCinematics )
		{
			// yes -update the timer
			m_timer += Time.deltaTime;

			// get to the game data
			GameData gameData = DataController.m_instance.m_gameData;

			// get to the player data
			PlayerData playerData = DataController.m_instance.m_playerData;

			// get the planet we are currently orbiting about
			Planet m_planet = gameData.m_planetList[ playerData.m_starflight.m_currentPlanetId ];

			// calculate the duration of the scan
			float scanDuration = Mathf.Lerp( m_minDuration, m_maxDuration, ( m_planet.m_bioDensity + m_planet.m_mineralDensity ) / 200.0f );

			// calculate alpha
			float alpha = Mathf.SmoothStep( 0.0f, 1.0f, m_timer / scanDuration );
			
			// have we stopped the scanning sound yet?
			if ( !m_soundStopped )
			{
				// no - is it time to stop the sound?
				if ( m_timer > ( scanDuration - 1.0f ) )
				{
					// yes - stop it now
					m_soundStopped = true;

					// stop the scanning sound
					SoundController.m_instance.StopSound( SoundController.Sound.Scanning );
				}
			}

			// is it time to stop the cinematics?
			if ( m_timer >= scanDuration )
			{
				// cap the timer
				m_timer = scanDuration;

				// the cinematics is over - set alpha to 1
				alpha = 1.0f;

				// turn off cinematics
				m_isDoingCinematics = false;

				// deactivate the sensor button
				m_spaceflightController.m_buttonController.DeactivateButton();

				// play the activate sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Activate );

				string atmosphere = m_planet.GetAtmosphereText();
				string hydrosphere = m_planet.GetHydrosphereText();
				string lithosphere = m_planet.GetLithosphereText();

				// update the messages text
				m_spaceflightController.m_spaceflightUI.ChangeMessageText( "Atmosphere:\n<color=white>" + atmosphere + "</color>\nHydrosphere:\n<color=white>" + hydrosphere + "</color>\nLithosphere:\n<color=white>" + lithosphere + "</color>" );

				// we have something to analyze
				m_hasSensorData = true;
			}

			// calculate new image color
			float r = ( alpha + ( 1 - alpha ) * Mathf.Abs( Mathf.Sin( m_timer * m_colorCycleSpeed + ( 2.0f * Mathf.PI / 3.0f ) * 1.0f ) ) ) * alpha;
			float g = ( alpha + ( 1 - alpha ) * Mathf.Abs( Mathf.Sin( m_timer * m_colorCycleSpeed + ( 2.0f * Mathf.PI / 3.0f ) * 2.0f ) ) ) * alpha;
			float b = ( alpha + ( 1 - alpha ) * Mathf.Abs( Mathf.Sin( m_timer * m_colorCycleSpeed + ( 2.0f * Mathf.PI / 3.0f ) * 3.0f ) ) ) * alpha;

			// set the new image color
			m_scanEffect.color = new Color( r, g, b );

			// spin the image randomly
			m_scanEffect.transform.localRotation = Quaternion.Euler( 0.0f, 0.0f, Random.Range( 0.0f, 360.0f ) );

			// calculate the scanned amounts
			float scannedMass = Mathf.Lerp( 1.0f, m_planet.m_mass, Mathf.Pow( m_timer / scanDuration, 4.0f ) );
			float scannedBio = Mathf.Lerp( 0.0f, m_planet.m_bioDensity, m_timer / scanDuration );
			float scannedMinerals = Mathf.Lerp( 0.0f, m_planet.m_mineralDensity, m_timer / scanDuration );

			// update the mass text
			int massLength = Mathf.RoundToInt( scannedMass ).ToString().Length;
			int massPower = 18 + massLength - 1;
			int massBase = Mathf.FloorToInt( scannedMass / Mathf.Pow( 10.0f, massLength - 1 ) );
			m_mass.text = "Mass: <color=\"white\">" + massBase + "x10<sup>" + massPower + "</sup></color> Tons";

			// update the bio text
			m_bioMin.text = "Bio: <color=\"white\">" + Mathf.RoundToInt( scannedBio ) + "%</color>   Min: <color=\"white\">" + Mathf.RoundToInt( scannedMinerals ) + "%</color>";
		}
	}

	// the display label
	public override string GetLabel()
	{
		return "Sensors";
	}

	// show
	public override void Show()
	{
		// call base show
		base.Show();

		// reset the cinematics timer
		m_timer = 0.0f;

		// start the cinematics
		m_isDoingCinematics = true;
		m_soundStopped = false;
	}

	// hide
	public override void Hide()
	{
		// call base hide
		base.Hide();

		// no more sensor data available for analysis
		m_hasSensorData = false;
	}
}
