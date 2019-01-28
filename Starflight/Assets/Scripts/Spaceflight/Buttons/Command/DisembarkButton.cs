
using UnityEngine;

public class DisembarkButton : ShipButton
{
	public override string GetLabel()
	{
		return "Disembark";
	}

	public override bool Execute()
	{
		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		switch ( playerData.m_general.m_location )
		{
			case PD_General.Location.DockingBay:

				// Debug.Log( "Player is disembarking - switching to starport." );

				// play the update sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Update );

				// make sure we are standing in the middle of the transporter pad
				playerData.m_general.m_lastStarportCoordinates = new Vector3( 0.0f, 0.5f, 0.0f );

				// switch to the starport location
				SpaceflightController.m_instance.SwitchLocation( PD_General.Location.Starport );

				return true;

			case PD_General.Location.JustLaunched:
			case PD_General.Location.InOrbit:
			case PD_General.Location.StarSystem:
			case PD_General.Location.Hyperspace:
			case PD_General.Location.Encounter:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				SpaceflightController.m_instance.m_messages.ChangeText( "<color=white>We can't disembark in space!</color>" );

				SpaceflightController.m_instance.m_buttonController.UpdateButtonSprites();

				break;

			case PD_General.Location.Planetside:

				// play the update sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Update );

				// move the player to the arth ship coordinates on the surface
				playerData.m_general.m_lastDisembarkedCoordinates = Tools.TerrainToWorldCoordinates( playerData.m_general.m_selectedLatitude, playerData.m_general.m_selectedLongitude );

				// update the terrain grid
				SpaceflightController.m_instance.m_disembarked.UpdateTerrainGridNow();

				// switch locations
				SpaceflightController.m_instance.SwitchLocation( PD_General.Location.Disembarked );

				break;
		}

		return false;
	}

	public override bool Update()
	{
		// always return true to prevent player from chagning buttons while transitioning to the docking bay
		return true;
	}
}
