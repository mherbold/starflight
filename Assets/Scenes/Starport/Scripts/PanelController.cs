
using UnityEngine;

public class PanelController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public GameObject m_panelGameObject;

	// functions that need to be overridden to do anything useful
	public virtual void Show()
	{
	}

	public virtual void Hide()
	{
	}

	public virtual void FinishedOpeningUI()
	{
	}

	public virtual void FinishedClosingUI()
	{
	}

	// call this to start opening the ui
	public void StartOpeningUI()
	{
		// activate the UI
		m_panelGameObject.SetActive( true );

		// tell the panel to start animating to the "open" position
		Animator animator = m_panelGameObject.transform.Find( "Panel" ).gameObject.GetComponent<Animator>();
		animator.SetBool( "Open", true );

		// make sure nothing is selected
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject( null );
	}

	// this is called when the ui is finished opening
	public void FinishOpeningUI()
	{
		// call the virtual function
		FinishedOpeningUI();
	}

	// call this to start closing the ui
	public void StartClosingUI()
	{
		// tell the panel to start animating to the "closed" position
		Animator animator = m_panelGameObject.transform.Find( "Panel" ).gameObject.GetComponent<Animator>();
		animator.SetBool( "Open", false );
	}

	// this is called when the ui is finished closing
	public void FinishClosingUI()
	{
		// call the virtual function
		FinishedClosingUI();

		// deactivate the UI
		m_panelGameObject.SetActive( false );
	}
}
