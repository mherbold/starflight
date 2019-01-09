
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensorsDisplay : ShipDisplay
{
	public enum ScanType
	{
		Player,
		SpeminTransport,
		SpeminScout,
		SpeminWarship,
		MechanScout,
		ElowanTransport,
		ElowanScout,
		ElowanWarship,
		ThrynnTransport,
		ThrynnScout,
		ThrynnWarship,
		VeloxiTransport,
		VeloxiScout,
		VeloxiWarship,
		GazurtoidScout,
		GazurtoidWarship,
		UhlekScout,
		UhlekWarship,
		VeloxiDrone,
		NomadProbe,
		Mysterion,
		TheEnterprise,
		Minstrel,
		NoahTransport,
		Debris,
		Planet,
		Unknown
	};

	// the mass text
	public TextMeshProUGUI m_massText;

	// the bio / min text
	public TextMeshProUGUI m_bioMinText;

	// the instructions text
	public TextMeshProUGUI m_instructionsText;

	// the background image
	public Image m_backgroundImage;

	// the scan image (this should be using custom mask image shader)
	public Image m_maskImage;

	// the background textures for the various scan types
	public Texture[] m_backgroundTextures;

	// the masks for the various scan types
	public Texture[] m_maskTextures;

	// how fast to cycle the colors
	public float m_colorCycleSpeed;

	// minimum duration for scanning
	public float m_minDuration;

	// maximum duration for scanning
	public float m_maxDuration;

	// do we have sensor data to analyze?
	public bool m_hasSensorData;

	// what are we currently scanning
	public ScanType m_scanType;

	// the background material
	Material m_backgroundMaterial;

	// the mask material
	Material m_maskMaterial;

	// are we running the cinematics?
	bool m_isDoingCinematics;

	// mass power base (18 for planets, 1 for ships)
	int m_massPowerBase;

	// mass of object we are scanning
	int m_mass;

	// bio density of object we are scanning
	int m_bioDensity;

	// mineral density of object we are scanning
	int m_mineralDensity;

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
		if ( !m_isDoingCinematics )
		{
			// no - don't do anything here
			return;
		}

		// yes -update the timer
		m_timer += Time.deltaTime;

		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// calculate the duration of the scan
		var scanDuration = Mathf.Lerp( m_minDuration, m_maxDuration, ( m_bioDensity + m_mineralDensity ) / 200.0f );

		// calculate alpha
		var alpha = Mathf.SmoothStep( 0.0f, 1.0f, m_timer / scanDuration );

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
		var r = ( alpha + ( 1 - alpha ) * Mathf.Abs( Mathf.Sin( m_timer * m_colorCycleSpeed + ( 2.0f * Mathf.PI / 3.0f ) * 1.0f ) ) ) * alpha;
		var g = ( alpha + ( 1 - alpha ) * Mathf.Abs( Mathf.Sin( m_timer * m_colorCycleSpeed + ( 2.0f * Mathf.PI / 3.0f ) * 2.0f ) ) ) * alpha;
		var b = ( alpha + ( 1 - alpha ) * Mathf.Abs( Mathf.Sin( m_timer * m_colorCycleSpeed + ( 2.0f * Mathf.PI / 3.0f ) * 3.0f ) ) ) * alpha;

		// set the new image color
		m_maskImage.color = new Color( r, g, b );

		// give the main (noise) texture a random posititon
		m_maskMaterial.SetTextureOffset( "_MainTex", new Vector2( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) ) );

		// is the cinematics done?
		if ( !m_isDoingCinematics )
		{
			// what were we scanning?

			switch ( m_scanType )
			{
				case ScanType.Planet:
				{
					// get the current planet
					var planet = gameData.m_planetList[ playerData.m_general.m_currentPlanetId ];

					// get the planet info
					var atmosphere = planet.GetAtmosphereText();
					var hydrosphere = planet.GetHydrosphereText();
					var lithosphere = planet.GetLithosphereText();

					// update the messages text
					m_spaceflightController.m_messages.ChangeText( "Atmosphere:\n<color=white>" + atmosphere + "</color>\nHydrosphere:\n<color=white>" + hydrosphere + "</color>\nLithosphere:\n<color=white>" + lithosphere + "</color>" );

					break;
				}

				default:
				{
					// display the ship information
					var vessel = gameData.m_vesselList[ (int) m_scanType ];

					string text = "Object Constituents:";

					if ( vessel.m_enduriumVolume > 0 )
					{
						var elementName = gameData.m_elementList[ gameData.m_misc.m_enduriumElementId ].m_name;

						text += "\n<color=white>" + elementName + "</color>";
					}

					if ( vessel.m_elementVolumeA > 0 )
					{
						var elementName = gameData.m_elementList[ vessel.m_elementIdA ].m_name;

						text += "\n<color=white>" + elementName + "</color>";
					}

					if ( vessel.m_elementVolumeB > 0 )
					{
						var elementName = gameData.m_elementList[ vessel.m_elementIdB ].m_name;

						text += "\n<color=white>" + elementName + "</color>";
					}

					if ( vessel.m_elementVolumeC > 0 )
					{
						var elementName = gameData.m_elementList[ vessel.m_elementIdC ].m_name;

						text += "\n<color=white>" + elementName + "</color>";
					}

					// update the messages text
					m_spaceflightController.m_messages.ChangeText( text );

					break;
				}
			}
		}

		// calculate the scanned amounts
		var scannedMass = Mathf.Lerp( 1.0f, m_mass, Mathf.Pow( m_timer / scanDuration, 4.0f ) );
		var scannedBio = Mathf.Lerp( 0.0f, m_bioDensity, m_timer / scanDuration );
		var scannedMinerals = Mathf.Lerp( 0.0f, m_mineralDensity, m_timer / scanDuration );

		// update the mass text
		var massLength = Mathf.RoundToInt( scannedMass ).ToString().Length;
		var massPower = m_massPowerBase + massLength - 1;
		var massBase = Mathf.FloorToInt( scannedMass / Mathf.Pow( 10.0f, massLength - 1 ) );
		m_massText.text = "Mass: <color=\"white\">" + massBase + "x10<sup>" + massPower + "</sup></color> Tons";

		// update the bio text
		m_bioMinText.text = "Bio: <color=\"white\">" + Mathf.RoundToInt( scannedBio ) + "%</color>   Min: <color=\"white\">" + Mathf.RoundToInt( scannedMinerals ) + "%</color>";
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

		// make a copy of the background material so the file doesn't get updated
		if ( m_backgroundMaterial == null )
		{
			m_backgroundMaterial = new Material( m_backgroundImage.material );
			m_backgroundImage.material = m_backgroundMaterial;
		}

		// make a copy of the mask material so the file doesn't get updated
		if ( m_maskMaterial == null )
		{
			m_maskMaterial = new Material( m_maskImage.material );
			m_maskImage.material = m_maskMaterial;
		}

		// hide the top and bottom text
		m_massText.gameObject.SetActive( false );
		m_bioMinText.gameObject.SetActive( false );

		// hide the background and mask
		m_backgroundImage.gameObject.SetActive( false );
		m_maskImage.gameObject.SetActive( false );

		// show the instructions text
		m_instructionsText.gameObject.SetActive( true );
	}

	// hide
	public override void Hide()
	{
		// call base hide
		base.Hide();

		// no more sensor data available for analysis
		m_hasSensorData = false;
	}

	// call this to start the scanning cinematics
	public void StartScanning( ScanType scanType, int massPowerBase, int mass, int bioDensity, int mineralDensity )
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// if we don't have a mask for this scan type then change it to the unknown mask
		if ( m_maskTextures[ (int) scanType ] == null )
		{
			scanType = ScanType.Unknown;
		}

		// remember the scan type, mass, bio density, and mineral density
		m_scanType = scanType;
		m_massPowerBase = massPowerBase;
		m_mass = mass;
		m_bioDensity = bioDensity;
		m_mineralDensity = mineralDensity;

		// reset the cinematics timer
		m_timer = 0.0f;

		// start the cinematics
		m_isDoingCinematics = true;
		m_soundStopped = false;

		// set the correct background texture for the scan type
		m_backgroundMaterial.SetTexture( "SF_AlbedoMap", m_backgroundTextures[ (int) m_scanType ] );

		// set the correct mask texture for the scan type
		m_maskMaterial.SetTexture( "_MaskTex", m_maskTextures[ (int) m_scanType ] );

		// reset background and mask image scale
		m_maskImage.transform.localScale = m_backgroundImage.transform.localScale = Vector3.one;

		// if we are scanning a planet scale the mask image
		if ( m_scanType == ScanType.Planet )
		{
			// get the planet we are currently orbiting about
			var planet = gameData.m_planetList[ playerData.m_general.m_currentPlanetId ];

			// change the size of the background and mask images based on the size of the planet
			m_backgroundImage.transform.localScale = m_maskImage.transform.localScale = planet.GetScale() / 320.0f * 0.5f + new Vector3( 0.5f, 0.5f, 0.5f );
		}

		// play the scanning sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Scanning );

		// show the top and bottom text
		m_massText.gameObject.SetActive( true );
		m_bioMinText.gameObject.SetActive( true );

		// show the background and mask
		m_backgroundImage.gameObject.SetActive( true );
		m_maskImage.gameObject.SetActive( true );

		// hide the instructions text
		m_instructionsText.gameObject.SetActive( false );
	}
}
