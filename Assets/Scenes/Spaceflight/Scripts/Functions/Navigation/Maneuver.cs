
using UnityEngine;
using UnityEngine.EventSystems;

public class ManeuverFunction : ButtonFunction
{
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

			// show the player on the map
			m_spaceflightController.m_player.SetActive( true );
		}
	}

	public override bool Update()
	{
		// returning true prevents the default spaceflight update from running
		return true;
	}
}
