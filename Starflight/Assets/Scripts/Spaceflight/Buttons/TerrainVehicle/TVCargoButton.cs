
public class TVCargoButton : ShipButton
{
	public override string GetLabel()
	{
		return "Cargo";
	}

	public override bool Execute()
	{
		return true;
	}

	public override bool Update()
	{
		// check if we want to turn off the map
		if ( InputController.m_instance.m_submit )
		{
			// debounce the input
			InputController.m_instance.Debounce();

			// deactivate the current button
			SpaceflightController.m_instance.m_buttonController.DeactivateButton();

			// play the deactivate sound
			SoundController.m_instance.PlaySound( SoundController.Sound.Deactivate );
		}
		else
		{
		}

		// returning true prevents the default spaceflight update from running
		return true;
	}
}
