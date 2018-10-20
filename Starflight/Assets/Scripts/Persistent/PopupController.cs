
using UnityEngine;
using TMPro;

public class PopupController : MonoBehaviour
{
	// static reference to this instance
	public static PopupController m_instance;

	// the pop up message box
	public GameObject m_popup;
	public TextMeshProUGUI m_popupMessage;
	public RectTransform m_popupFill;

	// unity awake
	void Awake()
	{
		// remember this instance to this
		m_instance = this;
	}

	// unity start
	void Start()
	{
		m_popup.SetActive( false );
	}

	// find out whether or not the pop up dialog is active
	public bool IsActive()
	{
		return m_popup.activeInHierarchy;
	}

	// show the pop up dialog
	public void ShowPopup( string message, float progress )
	{
		// make it visible
		m_popup.SetActive( true );

		// update the message
		m_popupMessage.text = message;

		// update the progress bar
		m_popupFill.anchorMax = new Vector2( progress, 1.0f );
	}

	// hide the pop up dialog
	public void HidePopup()
	{
		// make it invisible
		m_popup.SetActive( false );
	}
}
