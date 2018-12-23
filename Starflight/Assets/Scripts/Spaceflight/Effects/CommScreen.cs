
using UnityEngine;

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
	MorePPEffects.Waves m_waves;
	MorePPEffects.Headache m_headache;

	// our fade in timer
	float m_timer;

	// are we fading in?
	bool m_fadingIn;

	// our random waves and headache timers
	float m_waveTimer;
	float m_headacheTimer;

	// are we doing a random waves or headache?
	bool m_doingWaves;
	bool m_doingHeadache;

	// unity awake
	void Awake()
	{
		// grab all the wave components (there could be more than one, we just want the last one)
		var waveComponents = GetComponents<MorePPEffects.Waves>();

		// grab the camera fx components
		m_waves = waveComponents[ waveComponents.Length - 1 ];
		m_headache = GetComponent<MorePPEffects.Headache>();
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
			m_waves.strengthX = Mathf.Lerp( 3.0f, 0.0f, strength );
			m_headache.strength = Mathf.Lerp( 10.0f, 0.0f, strength );

			// are we done?
			if ( m_timer >= m_duration )
			{
				// yes - we are no longer fading in
				m_fadingIn = false;

				// initialize random wave and headache timers
				m_waveTimer = Random.Range( 3.0f, 30.0f );
				m_headacheTimer = Random.Range( 3.0f, 30.0f );
			}
		}
		else
		{
			if ( m_doingWaves )
			{
				m_waveTimer += Time.deltaTime;

				var strength = 1.0f - Mathf.Abs( 1.0f - Mathf.Clamp( m_waveTimer * 2.0f / m_randomWaveDuration, 0.0f, 2.0f ) );

				m_waves.strengthX = Mathf.Lerp( 0.0f, 3.0f, strength );

				if ( m_waveTimer >= m_randomWaveDuration )
				{
					m_doingWaves = false;

					m_waveTimer = Random.Range( 3.0f, 30.0f );
				}
			}
			else
			{
				m_waveTimer -= Time.deltaTime;

				if ( m_waveTimer <= 0.0f )
				{
					m_doingWaves = true;
					m_waveTimer = 0.0f;

					SoundController.m_instance.PlaySound( SoundController.Sound.WarbleShort );
				}
			}

			if ( m_doingHeadache )
			{
				m_headacheTimer += Time.deltaTime;

				var strength = 1.0f - Mathf.Abs( 1.0f - Mathf.Clamp( m_headacheTimer * 2.0f / m_randomHeadacheDuration, 0.0f, 2.0f ) );

				m_headache.strength = Mathf.Lerp( 0.0f, 3.0f, strength );

				if ( m_headacheTimer >= m_randomHeadacheDuration )
				{
					m_doingHeadache = false;

					m_headacheTimer = Random.Range( 3.0f, 30.0f );
				}
			}
			else
			{
				m_headacheTimer -= Time.deltaTime;

				if ( m_headacheTimer <= 0.0f )
				{
					m_doingHeadache = true;
					m_headacheTimer = 0.0f;

					SoundController.m_instance.PlaySound( SoundController.Sound.StaticShort );
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

		// fade in the viewer
		m_spaceflightController.m_map.StartFade( 1.0f, m_fadeInDuration );

		// play some sounds
		SoundController.m_instance.PlaySound( SoundController.Sound.WarbleLong );
		SoundController.m_instance.PlaySound( SoundController.Sound.StaticLong );
	}
}
