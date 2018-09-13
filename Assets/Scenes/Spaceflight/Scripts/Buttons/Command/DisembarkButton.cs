
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

		switch ( playerData.m_starflight.m_location )
		{
			case Starflight.Location.DockingBay:

				// play the update sound
				SoundController.m_instance.PlaySound( SoundController.Sound.Update );

				// make sure we are standing in the middle of the transporter pad
				playerData.m_starflight.m_starportCoordinates = new Vector3( 0.0f, 0.5f, 0.0f );

				// switch to the starport location
				m_spaceflightController.SwitchLocation( Starflight.Location.Starport );

				return true;

			case Starflight.Location.JustLaunched:
			case Starflight.Location.InOrbit:
			case Starflight.Location.StarSystem:
			case Starflight.Location.Hyperspace:

				SoundController.m_instance.PlaySound( SoundController.Sound.Error );

				m_spaceflightController.m_spaceflightUI.ChangeMessageText( "We can't disembark in space!" );

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
