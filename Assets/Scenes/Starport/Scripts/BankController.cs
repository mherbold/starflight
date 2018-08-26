
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BankController : DoorController
{
	// public stuff we want to set using the editor
	public Button m_exitButton;
	public TextMeshProUGUI m_dateListText;
	public TextMeshProUGUI m_transactionsListText;
	public TextMeshProUGUI m_amountListText;
	public TextMeshProUGUI m_currentBalanceText;
	public GameObject m_dateMask;

	// call this to show the operations ui
	public override void Show()
	{
		// start the opening animation
		StartOpeningUI();

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
	}

	// call this to hide the operations ui
	public override void Hide()
	{
		// lose the focus
		LoseFocus();

		// start the closing animation
		StartClosingUI();
	}

	// call this to take control
	public void TakeFocus()
	{
		// turn on controller navigation of the UI
		EventSystem.current.sendNavigationEvents = true;

		// automatically select the "notices" button for the player
		m_exitButton.Select();

		// cancel the ui sounds
		m_starportController.m_uiSoundController.CancelSounds();
	}

	// call this to give up control
	public void LoseFocus()
	{
		// turn off controller navigation of the UI
		EventSystem.current.sendNavigationEvents = false;
	}
	
	// this is called when the ui has finished animating to the open state
	public override void FinishedOpeningUI()
	{
		// take the focus
		TakeFocus();
	}

	// this is called when the ui has finished animating to the close state
	public override void FinishedClosingUI()
	{
		// give the focus back to the starport controller
		m_starportController.TakeFocus();
	}

	// this is called if we clicked on the exit button
	public void ExitClicked()
	{
		// close this ui
		Hide();
	}
}
