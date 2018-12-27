
using UnityEngine;
using MorePPEffects;

public class CommScreen : MonoBehaviour
{
	// convenient access to the spaceflight controller
	public SpaceflightController m_spaceflightController;

	// how long the effect lasts
	public float m_duration = 5.0f;

	// how long to fade in
	public float m_fadeInDuration = 1.0f;

	// how long to do those random waves and headaches
	public float m_randomWaveDuration = 0.5f;
	public float m_randomHeadacheDuration = 0.5f;

	// camera fx components we will be controlling
	ColoredRays m_coloredRays;
	Waves m_waves;
	Headache m_headache;
	LowResolution m_lowResolution;

	// our fade in timer
	float m_timer;

	// are we fading in?
	bool m_fadingIn;

	// the current training level of the comms officer
	float m_trainingLevel;

	// our random waves and headache timers
	float m_waveTimer;
	float m_headacheTimer;

	// are we doing a random waves or headache?
	bool m_doingWaves;
	bool m_doingHeadache;

	// unity awake
	void Awake()
	{
		// create the components
		m_waves = gameObject.AddComponent<Waves>();
		m_headache = gameObject.AddComponent<Headache>();
		m_coloredRays = gameObject.AddComponent<ColoredRays>();
		m_lowResolution = gameObject.AddComponent<LowResolution>();
	}

	// unity update
	void Update()
	{
		// are we fading in?
		if ( m_fadingIn )
		{
			// update the timer
			m_timer += Time.deltaTime;

			// calculate new strength of effects
			var strength = Mathf.Clamp( m_timer / m_duration, 0.0f, 1.0f );

			// update strength of effects
			m_waves.strengthX = Mathf.SmoothStep( 3.0f, 0.0f, strength );
			m_headache.strength = Mathf.SmoothStep( 10.0f, 0.0f, strength );

			// are we done?
			if ( m_timer >= m_duration )
			{
				// yes - we are no longer fading in
				m_fadingIn = false;

				// initialize random wave and headache timers
				m_waveTimer = m_trainingLevel * Random.Range( 3.0f, 30.0f );
				m_headacheTimer = m_trainingLevel * Random.Range( 3.0f, 30.0f );
			}
		}
		else
		{
			if ( m_doingWaves )
			{
				m_waveTimer += Time.deltaTime;

				var strength = 1.0f - Mathf.Abs( 1.0f - Mathf.Clamp( m_waveTimer * 2.0f / m_randomWaveDuration, 0.0f, 2.0f ) );

				m_waves.strengthX = Mathf.SmoothStep( 0.0f, 3.0f, strength );

				if ( m_waveTimer >= m_randomWaveDuration )
				{
					m_doingWaves = false;

					m_waveTimer = m_trainingLevel * Random.Range( 3.0f, 30.0f );
				}
			}
			else
			{
				m_waveTimer -= Time.deltaTime;

				if ( m_waveTimer <= 0.0f )
				{
					m_doingWaves = true;
					m_waveTimer = 0.0f;

					PlaySound( SoundController.Sound.WarbleShort );
				}
			}

			if ( m_doingHeadache )
			{
				m_headacheTimer += Time.deltaTime;

				var strength = 1.0f - Mathf.Abs( 1.0f - Mathf.Clamp( m_headacheTimer * 2.0f / m_randomHeadacheDuration, 0.0f, 2.0f ) );

				m_headache.strength = Mathf.SmoothStep( 0.0f, 10.0f, strength );

				if ( m_headacheTimer >= m_randomHeadacheDuration )
				{
					m_doingHeadache = false;

					m_headacheTimer = m_trainingLevel * Random.Range( 3.0f, 30.0f );
				}
			}
			else
			{
				m_headacheTimer -= Time.deltaTime;

				if ( m_headacheTimer <= 0.0f )
				{
					m_doingHeadache = true;
					m_headacheTimer = 0.0f;

					PlaySound( SoundController.Sound.StaticShort );
				}
			}
		}
	}

	// unity onenable
	void OnEnable()
	{
		// reset our timer
		m_timer = 0;

		// we are fading in
		m_fadingIn = true;

		// do we have a spaceflight controller (we could be in the test level where there isn't one)
		if ( m_spaceflightController != null )
		{
			// yes - fade in the viewer
			m_spaceflightController.m_map.StartFade( 1.0f, m_fadeInDuration );
		}

		// play some sounds
		PlaySound( SoundController.Sound.WarbleLong );
		PlaySound( SoundController.Sound.StaticLong );

		// the training level for the test scene
		m_trainingLevel = 0.5f;

		// do we have a player (are we not in test scene?)
		if ( DataController.m_instance != null )
		{
			// get to the player data
			var playerData = DataController.m_instance.m_playerData;

			// configure the video effects based on the training level of the comm officer
			var personnelFile = playerData.m_crewAssignment.GetPersonnelFile( PD_CrewAssignment.Role.CommunicationsOfficer );

			// convert the communications training level to a 1 to 0 range
			m_trainingLevel = personnelFile.m_communications / 250.0f;
		}

		// re-range tranining level
		m_trainingLevel = Mathf.Lerp( 0.1f, 1.0f, m_trainingLevel );

		// colored rays
		m_coloredRays.strength = 0.5f + 4.5f * ( 1.0f - m_trainingLevel );

		// waves
		m_waves.strengthY = 0.0f;
		m_waves.frequencyX = 100.0f;

		// low resolution
		if ( m_trainingLevel < 0.75f )
		{
			m_lowResolution.enabled = true;

			m_lowResolution.resolutionX = Mathf.FloorToInt( 512.0f * m_trainingLevel );
			m_lowResolution.resolutionY = m_lowResolution.resolutionX;
		}
		else
		{
			m_lowResolution.enabled = false;
		}
	}

	// play a sound if we have a sound controller
	void PlaySound( SoundController.Sound sound )
	{
		if ( SoundController.m_instance != null )
		{
			SoundController.m_instance.PlaySound( sound );
		}
	}
}
