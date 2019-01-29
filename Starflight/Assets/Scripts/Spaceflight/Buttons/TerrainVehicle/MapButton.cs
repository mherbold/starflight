
using UnityEngine;

public class MapButton : ShipButton
{
	float m_followDistance;

	public override string GetLabel()
	{
		return "Map";
	}

	public override bool Execute()
	{
		// reset the follow distance
		m_followDistance = 192.0f;

		// get the terrain vehicle game object
		var gameObject = SpaceflightController.m_instance.m_playerCamera.GetCameraFollowGameObject();

		// position the camera directly above the tv
		SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( gameObject, Vector3.up * m_followDistance );

		// return true to keep the button lit and active
		return true;
	}

	public override bool Update()
	{
		// get the terrain vehicle game object
		var gameObject = SpaceflightController.m_instance.m_playerCamera.GetCameraFollowGameObject();

		// check if we want to stop moving
		if ( InputController.m_instance.m_submit )
		{
			// debounce the input
			InputController.m_instance.Debounce();

			// turn off the engines
			SpaceflightController.m_instance.m_terrainVehicle.TurnOffEngines();

			// deactivate the current button
			SpaceflightController.m_instance.m_buttonController.DeactivateButton();

			// reset the camera follow
			SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( gameObject, new Vector3( 0.0f, 75.0f, -75.0f ) );
		}
		else
		{
			// get the controller stick position
			float z = InputController.m_instance.m_y;

			// check if the move vector will actually move the ship (that the controller is not centered)
			if ( ( z < -0.5f ) || ( z > 0.5f ) )
			{
				// change the follow distance
				m_followDistance -= z * 150.0f * Time.deltaTime;

				// restrict follow distance
				m_followDistance = Mathf.Clamp( m_followDistance, 192.0f, 512.0f );

				// update the camera position
				SpaceflightController.m_instance.m_playerCamera.SetCameraFollow( gameObject, Vector3.up * m_followDistance );
			}
		}

		// returning true prevents the default spaceflight update from running
		return true;
	}
}
