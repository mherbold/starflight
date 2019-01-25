
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	// the warp shader we want to use
	public Shader m_shader;

	// how fast to enter and exit warp
	public float m_enterWarpTransitionTime = 1.0f;
	public float m_exitWarpTransitionTime = 1.0f;

	// how strong the warp effect should be
	public float m_warpStrength;

	// the frequency of the rumble effect
	public float m_rumbleFrequency;

	// the current rumble effect strength
	public Vector3 m_rumbleStrength;

	// fire particle effects
	public ParticleSystem[] m_fireParticleSystems;

	// material we will create for the space warp
	Material m_material;

	// true if we are currently entering the space warp
	bool m_isEnteringWarp;

	// true if we are currently exiting the space warp
	bool m_isExitingWarp;

	// warp effect timer
	float m_warpTimer;

	// the animation controller
	Animator m_animator;

	// fast noise
	FastNoise m_fastNoise;

	// true if an animation is currently playing
	bool m_animationIsPlaying;

	// unity awake
	void Awake()
	{
		// creat a new material using the given shader
		m_material = new Material( m_shader );

		// get to the animator component
		m_animator = GetComponent<Animator>();

		// create the fast noise
		m_fastNoise = new FastNoise();
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
		// update animator
		if ( m_animationIsPlaying )
		{
			// get the animator state info
			var animatorStateInfo = m_animator.GetCurrentAnimatorStateInfo( 0 );

			// check if we have finished playing the animation
			if ( animatorStateInfo.normalizedTime >= animatorStateInfo.length )
			{
				m_animationIsPlaying = false;
			}
		}

		// warp effect
		if ( m_isEnteringWarp )
		{
			m_warpTimer += Time.deltaTime;

			float warpStrength = Mathf.Lerp( 0.0f, m_warpStrength, m_warpTimer / m_enterWarpTransitionTime );

			if ( m_warpTimer >= m_enterWarpTransitionTime )
			{
				m_isEnteringWarp = false;
			}

			m_material.SetFloat( "_WarpStrength", warpStrength );
		}
		else if ( m_isExitingWarp )
		{
			m_warpTimer += Time.deltaTime;

			float warpStrength = Mathf.SmoothStep( m_warpStrength, 0.0f, m_warpTimer / m_exitWarpTransitionTime );

			if ( m_warpTimer >= m_exitWarpTransitionTime )
			{
				m_isExitingWarp = false;
			}

			m_material.SetFloat( "_WarpStrength", warpStrength );
		}
	}

	// unity late update (apply rumble additively after animation controller has run)
	void LateUpdate()
	{
		if ( m_rumbleStrength.magnitude > 0.001f )
		{
			var shakeX = ( m_fastNoise.GetNoise( Time.time * m_rumbleFrequency, 1000.0f ) ) * m_rumbleStrength.x;
			var shakeY = ( m_fastNoise.GetNoise( Time.time * m_rumbleFrequency, 2000.0f ) ) * m_rumbleStrength.y;
			var shakeZ = ( m_fastNoise.GetNoise( Time.time * m_rumbleFrequency, 3000.0f ) ) * m_rumbleStrength.z;

			transform.localRotation *= Quaternion.Euler( shakeX, shakeY, shakeZ );
		}
	}

	// unity on render image
	void OnRenderImage( RenderTexture source, RenderTexture destination )
	{
		Graphics.Blit( source, destination, m_material );
	}

	// call this to start adding the warp effect
	public void EnterWarp()
	{
		m_isEnteringWarp = true;
		m_warpTimer = 0.0f;
	}

	// call this to start exiting the warp effect
	public void ExitWarp()
	{
		m_isExitingWarp = true;
		m_warpTimer = 0.0f;
	}

	public void StartAnimation( string animationName )
	{
		// are we already playing an animation?
		if ( !m_animationIsPlaying )
		{
			// no - play the new animation
			m_animator.Play( animationName );

			// remember that we are playing an animation now
			m_animationIsPlaying = true;
		}
	}

	//
	// everything below this point are the different animation callbacks for the player camera
	//

	public void BeginCountdown()
	{
		// play the countdown sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Countdown );

		// start the countdown animation
		SpaceflightController.m_instance.m_countdown.Show();
	}

	public void PlayerIsLaunching()
	{
		// yes - start fading the map out
		SpaceflightController.m_instance.m_viewport.StartFade( 0.0f, 5.0f );

		// yes - update the messages text
		SpaceflightController.m_instance.m_messages.ChangeText( "<color=white>Leaving starport...</color>" );
	}

	public void PlayerHasLaunched()
	{
		// get to the game data
		var gameData = DataController.m_instance.m_gameData;

		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// are we in the docking bay location?
		if ( playerData.m_general.m_location == PD_General.Location.DockingBay )
		{
			// yes - calculate the new position of the player (just north of the arth spaceport)
			Vector3 playerPosition = gameData.m_planetList[ gameData.m_misc.m_arthPlanetId ].GetPosition();

			playerPosition.y = 0.0f;
			playerPosition.z += 128.0f;

			// update the player's system coordinates to the new position
			playerData.m_general.m_lastStarSystemCoordinates = playerPosition;

			// update the player location
			SpaceflightController.m_instance.SwitchLocation( PD_General.Location.JustLaunched );

			// restore the bridge buttons (this also ends the launch function)
			SpaceflightController.m_instance.m_buttonController.RestoreBridgeButtons();
		}
		else
		{
			// change to the alternate command button set
			SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.CommandB );
		}
	}

	public void PlayerHasLanded()
	{
		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.CommandA );

		SpaceflightController.m_instance.m_messages.ChangeText( "Safe landing, captain." );

		SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
	}
	
	public void StartingDescent()
	{
		SpaceflightController.m_instance.m_messages.ChangeText( "Autopilot engaged. Descending..." );

		SoundController.m_instance.PlaySound( SoundController.Sound.Beep );
	}

	public void RetroRocketsFiring()
	{
		SpaceflightController.m_instance.m_messages.ChangeText( "Retro rockets firing..." );

		SoundController.m_instance.PlaySound( SoundController.Sound.Beep );
	}

	public void SwitchToPlanetsideLocation()
	{
		SpaceflightController.m_instance.SwitchLocation( PD_General.Location.Planetside );
	}

	public void SwitchToInOrbitLocation()
	{
		SpaceflightController.m_instance.SwitchLocation( PD_General.Location.InOrbit );
	}

	public void StartDustStorm()
	{
		SpaceflightController.m_instance.m_planetside.StartDustStorm();
	}

	public void StartFireParticleSystems()
	{
		foreach ( var particleSystem in m_fireParticleSystems )
		{
			particleSystem.Play();
		}
	}

	public void StopFireParticleSystems()
	{
		foreach ( var particleSystem in m_fireParticleSystems )
		{
			particleSystem.Stop();
		}
	}
}
