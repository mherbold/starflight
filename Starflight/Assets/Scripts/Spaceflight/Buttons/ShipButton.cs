
abstract public class ShipButton
{
	public virtual string GetLabel()
	{
		return "???";
	}

	public virtual bool Execute()
	{
		return false;
	}

	public virtual bool Update()
	{
		return false;
	}
}
