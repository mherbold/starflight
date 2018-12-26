
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
	// how often to select a new orientation
	public float m_minimumInterval = 2.0f;
	public float m_maximumInterval = 4.0f;

	// how fast to rotate to the new orientation (in seconds)
	public float m_rotationTime = 1.0f;

	// allowed range for the new orientation
	public float m_rangeYaw = 30.0f;
	public float m_rangePitch = 30.0f;
	public float m_rangeRoll = 0.0f;

	// the amount of jitter to have when in between rotations
	public float m_jitterScale = 0.1f;
	public float m_jitterSpeed = 1.0f;

	// the target yaw and pitch
	float m_targetYaw;
	float m_targetPitch;
	float m_targetRoll;

	// rotations
	Quaternion m_originalRotation;
	Quaternion m_initialRotation;
	Quaternion m_currentRotation;

	// timers
	float m_rotationTimer;
	float m_nextInterval;
	float m_jitterOffset;

	// unity start
	void Start()
	{
		m_targetYaw = 0.0f;
		m_targetPitch = 0.0f;
		m_targetRoll = 0.0f;

		m_originalRotation = transform.localRotation;
		m_initialRotation = Quaternion.identity;
		m_currentRotation = Quaternion.identity;

		m_rotationTimer = 0.0f;
		m_nextInterval = Random.Range( m_minimumInterval, m_maximumInterval );
		m_jitterOffset = 0.0f;
	}

	// unity update
	void Update()
	{
		// update the rotation timer
		m_rotationTimer += Time.deltaTime;

		// is it time to select a new orientation?
		if ( m_rotationTimer >= m_nextInterval )
		{
			// yes - do it
			ChangeDirection();
		}

		// smoothstep the rotation timer
		var smoothedRotationTimer = Mathf.SmoothStep( 0.0f, 1.0f, m_rotationTimer / m_rotationTime );

		// calculate the target rotation
		var targetRotation = Quaternion.Euler( m_targetPitch, m_targetYaw, m_targetRoll );

		// calculate the new current rotation
		m_currentRotation = Quaternion.Slerp( m_initialRotation, targetRotation, smoothedRotationTimer );

		// can we jitter?
		if ( m_rotationTimer > m_rotationTime )
		{
			// yes - compute jitter
			var jitterTimer = m_jitterOffset + m_rotationTimer * m_jitterSpeed;

			var jitterScale = m_jitterScale * Mathf.Sin( Mathf.PI * ( m_rotationTimer - m_rotationTime ) / ( m_nextInterval - m_rotationTime ) );

			var jitterPitch = ( Mathf.PerlinNoise( jitterTimer, 0.0f ) * 2.0f - 1.0f ) * m_rangePitch * jitterScale;
			var jitterYaw = ( Mathf.PerlinNoise( jitterTimer, jitterTimer ) * 2.0f - 1.0f ) * m_rangeYaw * jitterScale;
			var jitterRoll = ( Mathf.PerlinNoise( 0.0f, jitterTimer ) * 2.0f - 1.0f ) * m_rangeRoll * jitterScale;

			m_currentRotation *= Quaternion.Euler( jitterPitch, jitterYaw, jitterRoll );
		}

		// update the local rotation
		transform.localRotation = m_originalRotation * m_currentRotation;
	}

	// pick a random pitch and yaw
	void ChangeDirection()
	{
		// save the current orientation as the initial rotation
		m_initialRotation = m_currentRotation;

		// pick new yaw and pitch
		m_targetYaw = Random.Range( -1.0f, 1.0f ) * m_rangeYaw;
		m_targetPitch = Random.Range( -1.0f, 1.0f ) * m_rangePitch;
		m_targetRoll = Random.Range( -1.0f, 1.0f ) * m_rangeRoll;

		// set a new random jitter offset
		m_jitterOffset = Random.Range( 0.0f, 1000.0f );

		// reset the rotation timer
		m_rotationTimer = 0.0f;

		// set the next time we will pick a new orientation
		m_nextInterval = Random.Range( m_minimumInterval, m_maximumInterval );
	}
}
