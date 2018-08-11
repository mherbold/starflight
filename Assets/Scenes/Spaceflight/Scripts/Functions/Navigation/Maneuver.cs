
using UnityEngine;
using UnityEngine.EventSystems;

public class ManeuverFunction : ButtonFunction
{
	// keep track of the ship's current movement (for inertia)
	private Vector3 m_inertiaVector;

	public override string GetButtonLabel()
	{
		return "Maneuver";
	}

	public override void Execute()
	{
		if ( m_spaceflightController.m_inDockingBay )
		{
			m_spaceflightController.m_uiSoundController.Play( UISoundController.UISound.Error );

			m_spaceflightController.m_messages.text = "Standing by to launch.";

			m_spaceflightController.UpdateButtonSprites();
		}
		else if ( m_spaceflightController.m_justLaunched )
		{
			// clear out the just launched flag
			m_spaceflightController.m_justLaunched = false;

			// turn off the overlay
			m_spaceflightController.m_overlay.gameObject.SetActive( false );

			// show the ship on the map
			m_spaceflightController.m_ship.SetActive( true );

			// reset the inertia vector
			m_inertiaVector = Vector3.zero;
		}
	}

	public override bool Update()
	{
		// get the controller stick position
		float x = m_spaceflightController.m_inputManager.m_x;
		float z = m_spaceflightController.m_inputManager.m_y;

		// create our 3d move vector from the controller position
		Vector3 moveVector = new Vector3( x, 0.0f, z );

		// check if the move vector will actually move the ship (controller is not centered)
		if ( moveVector.magnitude > 0.5f )
		{
			// normalize the move vector to a length of 1.0 - so the ship will move the same distance in any direction
			moveVector.Normalize();

			// scale the move vector to the normal ship speed
			moveVector *= 5.0f;

			// update the inertia vector
			m_inertiaVector = Vector3.Slerp( m_inertiaVector, moveVector, Time.deltaTime * 2.0f );
		}
		else
		{
			// slow the ship to a stop
			m_inertiaVector = Vector3.Slerp( m_inertiaVector, Vector3.zero, Time.deltaTime );
		}

		// move the ship!
		m_spaceflightController.m_player.transform.position += m_inertiaVector;

		// rotate the ship towards the direction we want to move in (as long as it has some magnitude)
		if ( m_inertiaVector.magnitude > 0.01f )
		{
			Vector3 currentLookVector = m_spaceflightController.m_ship.transform.rotation * Vector3.forward;

			Vector3 newLookVector = Vector3.Slerp( currentLookVector, m_inertiaVector, Time.deltaTime * 4.0f );

			m_spaceflightController.m_ship.transform.rotation = Quaternion.LookRotation( newLookVector, Vector3.up );
		}

		// returning true prevents the default spaceflight update from running
		return true;
	}
}
