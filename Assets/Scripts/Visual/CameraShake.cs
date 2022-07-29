using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
	private CinemachineVirtualCamera cinecamera;
	public static CameraShake instance { get; private set; }

	private void Start()
	{
		instance = this;
		cinecamera = GetComponent<CinemachineVirtualCamera>();
	}

	public void Shake(float intensity, float time)
	{
		CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
			cinecamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

		// TODO: RESTORE DEFAULT VALUES AFTER TIME PASSES
		cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
		cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 30f;
	}

}
