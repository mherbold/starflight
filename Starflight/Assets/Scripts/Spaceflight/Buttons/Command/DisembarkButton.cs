
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

				Debug.Log( "Player is disembarking - switching to starport." );

				// play the update sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Update );

				// make sure we are standing in the middle of the transporter pad
				playerData.m_general.m_lastStarportCoordinates = new Vector3( 0.0f, 0.5f, 0.0f );

				// switch to the starport location
				m_spaceflightController.SwitchLocation( PD_General.Location.Starport );

				return true;

			case PD_General.Location.JustLaunched:
			case PD_General.Location.InOrbit:
			case PD_General.Location.StarSystem:
			case PD_General.Location.Hyperspace:
			case PD_General.Location.Encounter:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_messages.ChangeText( "<color=white>We can't disembark in space!</color>" );

				m_spaceflightController.m_buttonController.UpdateButtonSprites();

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
