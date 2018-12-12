
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
	public Vector3 m_rotationSpeed;

	Vector3 m_currentRotation;
	Quaternion m_originalRotation;

	void Start()
	{
		m_originalRotation = transform.localRotation;
	}

	void Update()
	{
		m_currentRotation += m_rotationSpeed * Time.deltaTime;

		if ( m_currentRotation.x >= 360.0f )
		{
			m_currentRotation.x -= 360.0f;
		}

		if ( m_currentRotation.y >= 360.0f )
		{
			m_currentRotation.y -= 360.0f;
		}

		if ( m_currentRotation.z >= 360.0f )
		{
			m_currentRotation.z -= 360.0f;
		}

		transform.localRotation = m_originalRotation * Quaternion.Euler( m_currentRotation.x, m_currentRotation.y, m_currentRotation.z );
	}
}
