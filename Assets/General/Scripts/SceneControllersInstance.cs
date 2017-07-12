
using UnityEngine;

class SceneControllersInstance : MonoBehaviour
{
	// static reference to this instance
	public static SceneControllersInstance m_instance;

	// this is called by unity before start
	private void Awake()
	{
		// remember this instance to this game object
		m_instance = this;
	}
}
