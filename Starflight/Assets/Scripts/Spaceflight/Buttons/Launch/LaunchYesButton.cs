
using UnityEngine;

public class LaunchYesButton : ShipButton
{
	public override string GetLabel()
	{
		return "Yes";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		if ( playerData.m_general.m_location == PD_General.Location.DockingBay )
		{
			// update the messages log
			SpaceflightController.m_instance.m_messages.ChangeText( "<color=white>Opening docking bay doors...</color>" );

			// open the docking bay doors
			SpaceflightController.m_instance.m_dockingBay.StartLaunchAnimation();
		}
		else
		{
			// run the launching camera animation
			SpaceflightController.m_instance.m_playerCamera.StartAnimation( "Launching (Planetside)" );
		}

		// stop the music
		MusicController.m_instance.ChangeToTrack( MusicController.Track.None );

		return true;
	}

	public override bool Update()
	{
		// don't let the default ship button update run
		return true;
	}
}
