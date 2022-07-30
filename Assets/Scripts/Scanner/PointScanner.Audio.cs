using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/*
 * Audio
 */
namespace lidar
{
    public partial class PointScanner : MonoBehaviour
    {
	    [Header("Audio")]
	    [SerializeField]
	    [LabelText("Volume")]
	    [Range(0, 1)]
	    private float maxAudioVolume = 0.7f;

	    private AudioSource scanAudioSource;

	    [SerializeField]
	    [Tooltip("Audio source used to play background noises")]
	    private AudioSource globalAudioSource;

	    [SerializeField] private AudioClip entityScanClip;
	    private bool holdingScan = false;


	    void playEntityScanBackground(float time) {
		    globalAudioSource.clip = entityScanClip;
		    globalAudioSource.Play();
		    scanAudioSource.Stop();

		    Utility.Invoke(this, () =>
		    {
			    globalAudioSource.volume = maxAudioVolume / 2f;
		    }, entityPostScareTime);
	    }

	    private IEnumerator FadeOut(float step) {
		    if (!allowNormalEffects) // if entity was scanned, stop scanner audio
		    {
			    scanAudioSource.Stop();
			    yield break;
		    }

		    while (scanAudioSource.volume != 0) {
			    scanAudioSource.volume = Math.Clamp(scanAudioSource.volume - step, 0f, 1f);
			    yield return new WaitForSeconds(0.01f);
		    }

		    scanAudioSource.Stop();
		    scanAudioSource.volume = maxAudioVolume;
	    }

	    private IEnumerator FadeIn(float step) {
		    if (!allowNormalEffects) // if entity was scanned, stop scanner audio
		    {
			    scanAudioSource.Stop();
			    yield break;
		    }

		    scanAudioSource.volume = 0;
		    scanAudioSource.Play();
		    while (scanAudioSource.volume < maxAudioVolume) {
			    scanAudioSource.volume = Math.Clamp(scanAudioSource.volume + step, 0f, 1f);
			    yield return new WaitForSeconds(0.01f);
		    }
	    }
	}
}
