
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensorsDisplay : ShipDisplay
{
	public enum ScanType
	{
		Planet
	};

	// the mass text
	public TextMeshProUGUI m_massText;

	// the bio / min text
	public TextMeshProUGUI m_bioMinText;

	// the scan image (this should be using custom mask image shader)
	public Image m_image;

	// the mask for the planet scan type
	public Texture m_planetMaskTexture;

	// how fast to cycle the colors
	public float m_colorCycleSpeed;

	// minimum duration for scanning
	public float m_minDuration;

	// maximum duration for scanning
	public float m_maxDuration;

	// do we have sensor data to analyze?
	public bool m_hasSensorData;

	// what are we scanning
	public ScanType m_scanType;

	// are we running the cinematics?
	bool m_isDoingCinematics;

	// have we stopped the sensor sound yet?
	bool m_soundStopped;

	// our timer
	float m_timer;

	// unity start
	public override void Start()
	{
		// get to the material
		Material material = m_image.material;

		// replace it with a copy (so when we modify it it doesn't modify the original material)
		material = new Material( material );

		// set the copy into image
		m_image.material = material;
	}

	// unity update
	public override void Update()
	{
		// are we doing the cinematics?
		if ( m_isDoingCinematics )
		{
			// yes -update the timer
			m_timer += Time.deltaTime;

			// switch to the scan type update subroutine
			switch ( m_scanType )
			{
				case ScanType.Planet:

					UpdatePlanetScan();

					break;
			}
		}
	}

	// the display label
	public override string GetLabel()
	{
		return "Sensors";
	}

	// show (but set the scan type before calling this)
	public override void Show()
	{
		// call base show
		base.Show();

		// reset the cinematics timer
		m_timer = 0.0f;

		// start the cinematics
		m_isDoingCinematics = true;
		m_soundStopped = false;

		// switch to the scan type initialize subroutine
		switch ( m_scanType )
		{
			case ScanType.Planet:

				InitializePlanetScan();
				break;
		}
	}

	// hide
	public override void Hide()
	{
		// call base hide
		base.Hide();

		// no more sensor data available for analysis
		m_hasSensorData = false;
	}

	// planet scan initialize
	void InitializePlanetScan()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the planet we are currently orbiting about
		Planet m_planet = gameData.m_planetList[ playerData.m_starflight.m_currentPlanetId ];

		// get to the material
		Material material = m_image.material;

		// swap the mask to the planet mask
		material.SetTexture( "_MaskTex", m_planetMaskTexture );

		// change the size of the image based on the size of the planet
		m_image.transform.localScale = m_planet.GetScale() / 320.0f * 0.5f + new Vector3( 0.5f, 0.5f, 0.5f );
	}

	// planet scan update
	void UpdatePlanetScan()
	{
		// get to the game data
		GameData gameData = DataController.m_instance.m_gameData;

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the planet we are currently orbiting about
		Planet m_planet = gameData.m_planetList[ playerData.m_starflight.m_currentPlanetId ];

		// calculate the duration of the scan
		float scanDuration = Mathf.Lerp( m_minDuration, m_maxDuration, ( m_planet.m_bioDensity + m_planet.m_mineralDensity ) / 200.0f );

		// call the generic scan update routine (returns true when it is time to stop the scan)
		if ( UpdateScan( scanDuration ) )
		{
			// get the planet info
			string atmosphere = m_planet.GetAtmosphereText();
			string hydrosphere = m_planet.GetHydrosphereText();
			string lithosphere = m_planet.GetLithosphereText();

			// update the messages text
			m_spaceflightController.m_spaceflightUI.ChangeMessageText( "Atmosphere:\n<color=white>" + atmosphere + "</color>\nHydrosphere:\n<color=white>" + hydrosphere + "</color>\nLithosphere:\n<color=white>" + lithosphere + "</color>" );
		}

		// calculate the scanned amounts
		float scannedMass = Mathf.Lerp( 1.0f, m_planet.m_mass, Mathf.Pow( m_timer / scanDuration, 4.0f ) );
		float scannedBio = Mathf.Lerp( 0.0f, m_planet.m_bioDensity, m_timer / scanDuration );
		float scannedMinerals = Mathf.Lerp( 0.0f, m_planet.m_mineralDensity, m_timer / scanDuration );

		// update the mass text
		int massLength = Mathf.RoundToInt( scannedMass ).ToString().Length;
		int massPower = 18 + massLength - 1;
		int massBase = Mathf.FloorToInt( scannedMass / Mathf.Pow( 10.0f, massLength - 1 ) );
		m_massText.text = "Mass: <color=\"white\">" + massBase + "x10<sup>" + massPower + "</sup></color> Tons";

		// update the bio text
		m_bioMinText.text = "Bio: <color=\"white\">" + Mathf.RoundToInt( scannedBio ) + "%</color>   Min: <color=\"white\">" + Mathf.RoundToInt( scannedMinerals ) + "%</color>";
	}

	// the generic scan update
	bool UpdateScan( float scanDuration )
	{
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

			// we have something to analyze
			m_hasSensorData = true;
		}

		// calculate the new color multiplier for the noise
		float r = ( alpha + ( 1 - alpha ) * Mathf.Abs( Mathf.Sin( m_timer * m_colorCycleSpeed + ( 2.0f * Mathf.PI / 3.0f ) * 1.0f ) ) ) * alpha;
		float g = ( alpha + ( 1 - alpha ) * Mathf.Abs( Mathf.Sin( m_timer * m_colorCycleSpeed + ( 2.0f * Mathf.PI / 3.0f ) * 2.0f ) ) ) * alpha;
		float b = ( alpha + ( 1 - alpha ) * Mathf.Abs( Mathf.Sin( m_timer * m_colorCycleSpeed + ( 2.0f * Mathf.PI / 3.0f ) * 3.0f ) ) ) * alpha;

		// set the new image color
		m_image.color = new Color( r, g, b );

		// give the main (noise) texture a random posititon
		m_image.material.SetTextureOffset( "_MainTex", new Vector2( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) ) );

		// let the caller know if it is time to stop the scan cinematics
		return !m_isDoingCinematics;
	}
}
