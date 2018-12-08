
public class QuestionButton : ShipButton
{
	public override string GetLabel()
	{
		return "Question";
	}

	public override bool Execute()
	{
		// change the buttons
		m_spaceflightController.m_buttonController.ChangeButtonSet( ButtonController.ButtonSet.AskQuestion );

		return false;
	}
}
