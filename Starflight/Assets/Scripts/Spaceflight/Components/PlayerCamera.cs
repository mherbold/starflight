
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	// the camera
	public Camera m_camera;

	// the space warp effect
	public SpaceWarp m_spaceWarp;

	// fire particle effects
	public ParticleSystem[] m_fireParticleSystems;

	// the object to follow
	public GameObject m_followGameObject;

	// the follow offset
	public Vector3 m_followOffset;

	// the follow rotation
	public Quaternion m_followRotation;

	// whether or not we are in the terrain vehicle mode
	public bool m_terrainVehicleMode;

	// the camera animation controller
	Animator m_animator;

	// true if an animation is currently playing
	bool m_animationIsPlaying;

	// unity awake
	void Awake()
	{
		// get the animator component
		m_animator = GetComponent<Animator>();
	}

	// unity late update
	void LateUpdate()
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

		// are we following an object?
		if ( m_followGameObject != null )
		{
			// yes - are we in the terrain vehicle mode?
			if ( m_terrainVehicleMode )
			{
				// yes - ok build the camera position
				var playerCameraPosition = m_followGameObject.transform.position + m_followOffset;

				// for now just use the follow offset
				transform.localPosition = playerCameraPosition;
				transform.localRotation = Quaternion.Euler( -45.0f, 0.0f, 0.0f );
			}
			else
			{
				// no - ok build the camera position
				var playerCameraPosition = m_followGameObject.transform.position + m_followOffset;

				// update the player camera transform
				transform.localPosition = playerCameraPosition;
				transform.localRotation = m_followRotation;

				// update the clipping planes
				m_camera.farClipPlane = Mathf.Max( 2048.0f, m_followOffset.y + 1536.0f );
			}
		}
	}

	// update the follow game object, follow offset, and the terrain vehicle mode flag
	public void SetCameraFollow( GameObject followGameObject, Vector3 followOffset, Quaternion followRotation, bool terrainVehicleMode )
	{
		m_followGameObject = followGameObject;
		m_terrainVehicleMode = terrainVehicleMode;
		m_followOffset = followOffset;
		m_followRotation = followRotation;

		// do we want to follow a game object?
		if ( m_followGameObject == null )
		{
			// no - are we currently playing an animation?
			if ( !m_animationIsPlaying )
			{
				// no - immediately set the transform to the follow offset and reset the camera rotation to looking straight down
				transform.localPosition = m_followOffset;
				transform.localRotation = m_followRotation;
			}
		}
	}

	// start a camera animation
	public void StartAnimation( string animationName )
	{
		// are we already playing an animation?
		if ( !m_animationIsPlaying )
		{
			// no - reset the camera to animation position
			SetCameraFollow( null, Vector3.zero, Quaternion.identity, false );

			// play the new animation
			m_animator.Play( animationName );

			// let the camera animation take over again
			m_animator.StartPlayback();

			// remember that we are playing an animation now
			m_animationIsPlaying = true;
		}
	}

	// stop the current camera animation
	public void StopAnimation()
	{
		m_animator.StopPlayback();

		m_animationIsPlaying = false;
	}

	//
	// everything below this point are animation event callbacks
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
