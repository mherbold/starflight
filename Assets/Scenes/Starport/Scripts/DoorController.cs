
using UnityEngine;

public class DoorController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public GameObject m_doorGameObject;

	// stuff we want derived controllers to use
	public StarportController m_starportController { get; protected set; }
	public bool m_hasFocus { get; private set; }

	// private stuff we don't want the editor to see
	private Animator m_animator;

	// this is called by unity before start
	protected virtual void Awake()
	{
		// get access to the starport controller
		m_starportController = GetComponent<StarportController>();

		// get access to the animator
		if ( m_doorGameObject != null )
		{
			m_animator = m_doorGameObject.transform.Find( "Panel" ).gameObject.GetComponent<Animator>();
		}
	}

	// this is called by unity once at the start of the level
	protected virtual void Start()
	{
		// hide the ui
		if ( m_doorGameObject != null )
		{
			m_doorGameObject.SetActive( false );
		}

		// this door does not have the focus
		m_hasFocus = false;
	}

	// functions that need to be overridden for the door to do anything useful
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
		m_doorGameObject.SetActive( true );

		// tell the panel to start animating to the "open" position
		m_animator.SetBool( "Open", true );

		// make sure nothing is selected
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject( null );

		// play the compressed air sound
		m_starportController.m_basicSound.PlayOneShot( 0 );
	}

	// this is called when the ui is finished opening
	public void FinishOpeningUI()
	{
		// call the virtual function
		FinishedOpeningUI();

		// activate the door ui controls
		m_hasFocus = true;
	}

	// call this to start closing the ui
	public void StartClosingUI()
	{
		// deactivate the door ui controls
		m_hasFocus = false;

		// tell the panel to start animating to the "closed" position
		m_animator.SetBool( "Open", false );

		// play the escaping air sound
		m_starportController.m_basicSound.PlayOneShot( 1 );
	}

	// this is called when the ui is finished closing
	public void FinishClosingUI()
	{
		// call the virtual function
		FinishedClosingUI();

		// deactivate the UI
		m_doorGameObject.SetActive( false );
	}
}
