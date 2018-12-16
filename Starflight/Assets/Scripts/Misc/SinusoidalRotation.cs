
using UnityEngine;

public class SinusoidalRotation : MonoBehaviour
{
	// how fast to rotate to the new orientation (in seconds)
	public float m_rotationTime = 1.0f;

	// allowed range for the new orientation
	public float m_rangeYaw = 30.0f;
	public float m_rangePitch = 30.0f;
	public float m_rangeRoll = 0.0f;

	// rotations
	Quaternion m_originalRotation;

	// the current offset into the sine wave
	float m_angle;

	// unity start
	void Start()
	{
		m_originalRotation = transform.localRotation;
	}

	// unity update
	void Update()
	{
		// update the rotation timer
		m_angle += 2.0f * Mathf.PI * ( Time.deltaTime / m_rotationTime );

		// calculate the sine wave
		var sine = Mathf.SmoothStep( -1.0f, 1.0f, Mathf.Sin( m_angle ) * 0.5f + 0.5f );

		// calculate the new current rotation
		var rotation = Quaternion.Euler( m_rangePitch * sine, m_rangeYaw * sine, m_rangeRoll * sine );

		transform.localRotation = rotation * m_originalRotation;
	}
}
