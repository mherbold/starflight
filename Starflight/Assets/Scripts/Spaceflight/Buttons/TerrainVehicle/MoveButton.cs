
using UnityEngine;

public class MoveButton : ShipButton
{
	public override string GetLabel()
	{
		return "Move";
	}

	public override bool Execute()
	{
		// return true to keep the button lit and active
		return true;
	}

	public override bool Update()
	{
		// get to the player data
		var playerData = DataController.m_instance.m_playerData;

		// check if we want to stop moving
		if ( InputController.m_instance.m_submit )
		{
			// debounce the input
			InputController.m_instance.Debounce();

			// turn off the engines
			SpaceflightController.m_instance.m_terrainVehicle.TurnOffEngines();

			// remove the "active" dot from the current button
			SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

			// play the deactivate sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
		}
		else
		{
			// get the controller stick position
			float x = InputController.m_instance.m_x;
			float z = InputController.m_instance.m_y;

			// create our 3d move vector from the controller position
			Vector3 moveVector = new Vector3( x, 0.0f, z );

			// check if the move vector will actually move the ship (that the controller is not centered)
			if ( moveVector.magnitude > 0.5f )
			{
				// normalize the move vector to a length of 1.0
				moveVector.Normalize();

				// update the direction
				playerData.m_general.m_currentDirection = Vector3.Slerp( playerData.m_general.m_currentDirection, moveVector, Time.deltaTime * 2.0f );

				// turn the engines on
				SpaceflightController.m_instance.m_terrainVehicle.TurnOnEngines();
			}
			else
			{
				// turn the engines off
				SpaceflightController.m_instance.m_terrainVehicle.TurnOffEngines();
			}
		}

		// returning true prevents the default spaceflight update from running
		return true;
	}
}
