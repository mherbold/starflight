
using UnityEngine;

public class MechanHead : MonoBehaviour
{
	public float m_lerpSpeed;
	public float m_jitterSpeed;
	public float m_yawJitterScale;
	public float m_pitchJitterScale;

	Quaternion m_originalRotation;
	Quaternion m_currentRotation;
	Quaternion m_startRotation;

	float m_currentYaw;
	float m_currentPitch;
	float m_lerpTime;
	float m_nextChangeTime;

	// unity start
	void Start()
	{
		m_originalRotation = transform.localRotation;
		m_currentRotation = Quaternion.identity;
		m_startRotation = Quaternion.identity;

		ChangeDirection();
	}

	// unity update
	void Update()
	{
		m_nextChangeTime -= Time.deltaTime;

		if ( m_nextChangeTime < 0.0f )
		{
			ChangeDirection();
		}

		m_lerpTime += Time.deltaTime;

		var jitterTime = Time.time * m_jitterSpeed;

		var jitterYaw = Mathf.PerlinNoise( jitterTime, 0.0f ) * m_yawJitterScale;
		var jitterPitch = Mathf.PerlinNoise( 0.0f, jitterTime ) * m_pitchJitterScale;

		var targetRotation = Quaternion.Euler( m_currentPitch + jitterPitch, m_currentYaw + jitterYaw, 0.0f );

		m_currentRotation = Quaternion.Slerp( m_startRotation, targetRotation, Mathf.SmoothStep( 0.0f, 1.0f, m_lerpTime * m_lerpSpeed ) );

		transform.localRotation = m_currentRotation * m_originalRotation;
	}

	// pick a random direction
	void ChangeDirection()
	{
		m_startRotation = m_currentRotation;

		m_currentYaw = Random.Range( -1.0f, 1.0f );
		m_currentYaw = m_currentYaw * m_currentYaw * m_currentYaw * 30.0f;

		m_currentPitch = Random.Range( 0.0f, 1.0f );
		m_currentPitch = m_currentPitch * m_currentPitch * 10.0f;

		m_nextChangeTime = Random.Range( 3.0f, 15.0f );

		m_lerpTime = 0.0f;
	}
}
