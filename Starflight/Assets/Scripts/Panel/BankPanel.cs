
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BankPanel : Panel
{
	// buttons
	public Button m_exitButton;

	// text
	public TextMeshProUGUI m_dateListText;
	public TextMeshProUGUI m_transactionsListText;
	public TextMeshProUGUI m_amountListText;
	public TextMeshProUGUI m_currentBalanceText;

	// the date mask
	public GameObject m_dateMask;

	// the astronaut controller
	public AstronautController m_astronautController;

	// panel open
	public override bool Open()
	{
		// base panel open
		base.Open();

		// get access to the bank player data
		Bank bank = DataController.m_instance.m_playerData.m_bank;

		// update the date, transactions, and amount list text
		m_dateListText.text = "";
		m_transactionsListText.text = "";
		m_amountListText.text = "";

		for ( int transactionId = 0; transactionId < bank.m_transactionList.Count; transactionId++ )
		{
			Bank.Transaction transaction = bank.m_transactionList[ transactionId ];

			DateTime dateTime = DateTime.ParseExact( transaction.m_stardate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );
			m_dateListText.text += dateTime.ToShortDateString();
			m_transactionsListText.text += transaction.m_description;
			m_amountListText.text += transaction.m_amount;

			if ( transactionId < ( bank.m_transactionList.Count - 1 ) )
			{
				m_dateListText.text += Environment.NewLine;
				m_transactionsListText.text += Environment.NewLine;
				m_amountListText.text += Environment.NewLine;
			}
		}

		// update the current balance text
		m_currentBalanceText.text = "Your current balance is " + string.Format( "{0:n0}", bank.m_currentBalance ) + " M.U.";

		// force the text object to update (so we can get the correct height)
		m_currentBalanceText.ForceMeshUpdate();

		// force the canvas to update so we can get the height of the date viewport
		Canvas.ForceUpdateCanvases();

		// get the height of the date viewport
		float viewportHeight = m_dateMask.GetComponent<RectTransform>().rect.height;

		// calculate the offset we need to show the bottom of the list
		float offset = Mathf.Max( 0.0f, m_dateListText.renderedHeight - viewportHeight );

		// move up the text in all 3 columns
		m_dateListText.rectTransform.offsetMax = new Vector3( 0.0f, offset, 0.0f );
		m_transactionsListText.rectTransform.offsetMax = new Vector3( 0.0f, offset, 0.0f );
		m_amountListText.rectTransform.offsetMax = new Vector3( 0.0f, offset, 0.0f );

		// automatically select the "exit" button for the player
		m_exitButton.Select();

		// panel was opened
		return true;
	}

	// panel closed
	public override void Closed()
	{
		// base panel closed
		base.Closed();

		// let the starport controller know
		m_astronautController.PanelWasClosed();
	}

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		// close this panel
		PanelController.m_instance.Close();
	}
}
