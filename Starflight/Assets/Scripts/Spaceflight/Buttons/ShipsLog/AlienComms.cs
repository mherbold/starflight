
public class AlienCommsButton : ShipButton
{
	public override string GetLabel()
	{
		return "Alien Comms";
	}

	public override bool Execute()
	{
		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.AlienComms );

		return false;
	}
}
