
using UnityEngine;

public class Mechan : MonoBehaviour
{
	public Transform m_head;
	public Transform m_torso;

	public float m_lerpSpeed;
	public float m_headJitterSpeed;
	public float m_jitterScaleYaw;
	public float m_jitterScalePitch;

	Quaternion m_headRotationOriginal;
	Quaternion m_headRotationCurrent;
	Quaternion m_headRotationStart;

	Quaternion m_torsoRotationOriginal;
	Quaternion m_torsoRotationCurrent;
	Quaternion m_torsoRotationStart;

	float m_currentYaw;
	float m_currentPitch;
	float m_lerpTime;
	float m_nextChangeTime;

	// unity start
	void Start()
	{
		m_headRotationOriginal = m_head.transform.localRotation;
		m_headRotationCurrent = Quaternion.identity;
		m_headRotationStart = Quaternion.identity;

		m_torsoRotationOriginal = m_torso.transform.localRotation;
		m_torsoRotationCurrent = Quaternion.identity;
		m_torsoRotationStart = Quaternion.identity;

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

		var jitterTime = Time.time * m_headJitterSpeed;

		var jitterYaw = Mathf.PerlinNoise( jitterTime, 0.0f ) * m_jitterScaleYaw;
		var jitterPitch = Mathf.PerlinNoise( 0.0f, jitterTime ) * m_jitterScalePitch;

		var targetHeadRotation = Quaternion.Euler( m_currentPitch + jitterPitch, m_currentYaw + jitterYaw, 0.0f );
		var targetTorsoRotation = Quaternion.Euler( 0.0f, m_currentYaw * 0.25f, 0.0f );

		var smoothedLerpTime = Mathf.SmoothStep( 0.0f, 1.0f, m_lerpTime * m_lerpSpeed );

		m_headRotationCurrent = Quaternion.Slerp( m_headRotationStart, targetHeadRotation, smoothedLerpTime );
		m_torsoRotationCurrent = Quaternion.Slerp( m_torsoRotationStart, targetTorsoRotation, smoothedLerpTime );

		m_head.transform.localRotation = m_headRotationCurrent * m_headRotationOriginal;
		m_torso.transform.localRotation = m_torsoRotationCurrent * m_torsoRotationOriginal;
	}

	// pick a random direction
	void ChangeDirection()
	{
		m_headRotationStart = m_headRotationCurrent;
		m_torsoRotationStart = m_torsoRotationCurrent;

		m_currentYaw = Random.Range( -1.0f, 1.0f );
		m_currentYaw = m_currentYaw * m_currentYaw * m_currentYaw * 30.0f;

		m_currentPitch = Random.Range( 0.0f, 1.0f );
		m_currentPitch = m_currentPitch * m_currentPitch * 10.0f;

		m_nextChangeTime = Random.Range( 3.0f, 15.0f );

		m_lerpTime = 0.0f;
	}
}
