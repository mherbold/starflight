
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
	public Vector3 m_rotationSpeed;
	public bool m_worldSpace;

	Quaternion m_currentRotation;

	void Start()
	{
		m_currentRotation = ( m_worldSpace ) ? transform.rotation : transform.localRotation;
	}

	void Update()
	{
		m_currentRotation = Quaternion.Euler( m_rotationSpeed * Time.deltaTime ) * m_currentRotation;

		if ( m_worldSpace )
		{
			transform.rotation = m_currentRotation;
		}
		else
		{
			transform.localRotation = m_currentRotation;
		}
	}
}
