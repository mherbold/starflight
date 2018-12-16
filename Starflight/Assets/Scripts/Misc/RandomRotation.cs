
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

	// unity start
	void Start()
	{
		m_targetYaw = 0.0f;
		m_targetPitch = 0.0f;
		m_targetRoll = 0.0f;

		m_originalRotation = transform.localRotation;
		m_initialRotation = Quaternion.identity;
		m_currentRotation = Quaternion.identity;

		m_nextInterval = Random.Range( m_minimumInterval, m_maximumInterval );
	}

	// unity update
	void Update()
	{
		// update the interval timer
		m_nextInterval -= Time.deltaTime;

		// is it time to select a new orientation?
		if ( m_nextInterval < 0.0f )
		{
			// yes - do it
			ChangeDirection();
		}

		// update the rotation timer
		m_rotationTimer += Time.deltaTime;

		// calculate the target rotation
		var targetRotation = Quaternion.Euler( m_targetPitch, m_targetYaw, m_targetRoll );

		// smoothstep the rotation timer
		var smoothedRotationTimer = Mathf.SmoothStep( 0.0f, 1.0f, m_rotationTimer / m_rotationTime );

		// calculate the new current rotation
		m_currentRotation = Quaternion.Slerp( m_initialRotation, targetRotation, smoothedRotationTimer );

		transform.localRotation = m_currentRotation * m_originalRotation;
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

		// set the next time we will pick a new orientation
		m_nextInterval = Random.Range( m_minimumInterval, m_maximumInterval );

		// reset the rotation timer
		m_rotationTimer = 0.0f;
	}
}
