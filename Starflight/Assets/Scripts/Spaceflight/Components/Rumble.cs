
using UnityEngine;

public class Rumble : MonoBehaviour
{
	// the frequency of the rumble effect
	public float m_frequency;

	// the current rumble effect strength
	public Vector3 m_strength;

	// fast noise
	FastNoise m_fastNoise;

	// unity awake
	void Awake()
	{
		// create the fast noise
		m_fastNoise = new FastNoise();
	}

	// unity late update
	void LateUpdate()
	{
		if ( m_strength.magnitude > 0.001f )
		{
			var shakeX = ( m_fastNoise.GetNoise( Time.time * m_frequency, 1000.0f ) ) * m_strength.x;
			var shakeY = ( m_fastNoise.GetNoise( Time.time * m_frequency, 2000.0f ) ) * m_strength.y;
			var shakeZ = ( m_fastNoise.GetNoise( Time.time * m_frequency, 3000.0f ) ) * m_strength.z;

			transform.localRotation *= Quaternion.Euler( shakeX, shakeY, shakeZ );
		}
	}
}
