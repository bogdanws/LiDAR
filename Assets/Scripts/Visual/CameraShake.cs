using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
	private CinemachineVirtualCamera cinecamera;
	private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
	public static CameraShake instance { get; private set; }

	private void Start()
	{
		instance = this;
		cinecamera = GetComponent<CinemachineVirtualCamera>();
		cinemachineBasicMultiChannelPerlin = cinecamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
	}

	public void Shake(float intensity, float time)
	{
		StartCoroutine(resetShake(cinemachineBasicMultiChannelPerlin.m_AmplitudeGain, cinemachineBasicMultiChannelPerlin.m_FrequencyGain, time));

		cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
		cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 30f;
	}

	private IEnumerator resetShake(float intensity, float frequency, float time)
	{
		yield return new WaitForSeconds(time);
		cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
		cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frequency;
	}

}
