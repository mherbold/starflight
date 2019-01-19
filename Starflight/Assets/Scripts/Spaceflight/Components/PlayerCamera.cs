
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

	// how quickly to ramp up the rumble effect
	public float m_rumbleRampUpTime;

	// how quickly to ramp down the rumble effect
	public float m_rumbleRampDownTime;

	// maximum strength of the rumble shake effect
	public Vector3 m_maxRumbleStrength;

	// the frequency of the rumble shake effect
	public float m_rumbleFrequency;

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

	// true if we are ramping up the rumble effect
	bool m_isEnteringRumble;

	// true if we ar raming down the rumble effect
	bool m_isExitingRumble;

	// the rumble timers
	float m_rumbleTimer;
	float m_rumbleExitTime;

	// the current rumble strength
	Vector3 m_rumbleStrength;

	// give the rumble some randomness
	float m_rumbleRandomOffset;

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

		// rumble effect
		if ( m_isEnteringRumble || m_isExitingRumble || ( m_rumbleStrength != Vector3.zero ) )
		{
			m_rumbleTimer += Time.deltaTime;

			if ( m_isEnteringRumble )
			{
				m_rumbleStrength = Vector3.Lerp( Vector3.zero, m_maxRumbleStrength, m_rumbleTimer / m_rumbleRampUpTime );

				if ( m_rumbleTimer >= m_rumbleRampUpTime )
				{
					m_isEnteringRumble = false;
				}
			}
			else if ( m_isExitingRumble )
			{
				m_rumbleStrength = Vector3.Lerp( m_maxRumbleStrength, Vector3.zero, ( m_rumbleTimer - m_rumbleExitTime ) / m_rumbleRampDownTime );

				if ( ( m_rumbleTimer - m_rumbleExitTime ) >= m_rumbleRampDownTime )
				{
					m_isExitingRumble = false;
				}
			}
		}
	}

	// unity late update (apply rumble additively after animation controller has run)
	void LateUpdate()
	{
		if ( m_rumbleStrength != Vector3.zero )
		{
			var shakeX = ( m_fastNoise.GetNoise( m_rumbleTimer * m_rumbleFrequency, m_rumbleRandomOffset + 1000.0f ) ) * m_rumbleStrength.x;
			var shakeY = ( m_fastNoise.GetNoise( m_rumbleTimer * m_rumbleFrequency, m_rumbleRandomOffset + 2000.0f ) ) * m_rumbleStrength.y;
			var shakeZ = ( m_fastNoise.GetNoise( m_rumbleTimer * m_rumbleFrequency, m_rumbleRandomOffset + 3000.0f ) ) * m_rumbleStrength.z;

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
		SpaceflightController.m_instance.m_viewport.StartFade( 0.0f, 7.0f );

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

		// override what the planetside show() does for the skybox blend factor (avoid momentary flash)
		StarflightSkybox.m_instance.m_currentBlendFactor = 0.0f;
	}

	public void SwitchToInOrbitLocation()
	{
		SpaceflightController.m_instance.SwitchLocation( PD_General.Location.InOrbit );
	}

	public void StartRumble()
	{
		m_rumbleTimer = 0.0f;
		m_rumbleRandomOffset = Random.Range( 0.0f, 1000.0f );

		m_isEnteringRumble = true;
	}

	public void StopRumble()
	{
		m_rumbleExitTime = m_rumbleTimer;

		m_isExitingRumble = true;
	}
}
