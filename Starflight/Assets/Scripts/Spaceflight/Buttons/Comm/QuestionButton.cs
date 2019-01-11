
public class QuestionButton : ShipButton
{
	public override string GetLabel()
	{
		return "Question";
	}

	public override bool Execute()
	{
		// change the buttons
		SpaceflightController.m_instance.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.AskQuestion );

		return false;
	}
}
