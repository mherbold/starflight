
using UnityEngine;
using TMPro;

public class Messages : MonoBehaviour
{
	// text elements
	public TextMeshProUGUI m_messages;

	// unity awake
	void Awake()
	{
	}

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
	}

	// call this to change the message text
	public void ChangeText( string newMessage )
	{
		m_messages.text = newMessage;
	}
}
