using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/*
 * Rendering and visual
 */
namespace lidar
{
    public partial class PointScanner : MonoBehaviour
    {
	    [Header("Rendering")]
	    [SerializeField]
	    [Tooltip("ChunkRenderer used to render the triangle on the corresponding mesh")]
	    private ChunkManager chunkManager;

	    [SerializeField]
	    [Tooltip("Line renderer used to draw the lines for the LiDAR gun")]
	    private LineRenderer lineRenderer;

	    [SerializeField] private PostProcessVolume postProcess;
	    private LensDistortion lensDistortion;
	    private bool allowLineUpdate = true;
	    private bool drawNextLine = true;


	    void enableEntityScanDistortion_Shake() {
		    lensDistortion.active = true;
		    CameraShake.instance.Shake(0.25f, entityScareTime);
		    StartCoroutine(increaseLensDistortion(entityScareTime));
		    Utility.Invoke(this,
			    () => { CameraShake.instance.Shake(0.08f, entityPostScareTime); } // shake camera less after intense shaking is over
			    , entityScareTime);
	    }

	    private IEnumerator increaseLensDistortion(float time)
	    {
		    float initialScale = lensDistortion.scale.value;

		    float endTime = Time.time + time;

			while (Time.time < endTime)
		    {
			    lensDistortion.scale.value += 0.01f;
			    yield return new WaitForSeconds(0.05f);
		    }

		    lensDistortion.scale.value = initialScale;
	    }

		private IEnumerator drawLine(Vector3 startPos, Vector3 endPos) {
		    if (!allowNormalEffects) // if entity was scanned, disable the rendering of line
		    {
			    lineRenderer.SetPosition(0, Vector3.zero);
			    lineRenderer.SetPosition(1, Vector3.zero);
			    yield break;
		    }

		    if (!drawNextLine) yield break; // if line renderer didn't finish last render

		    drawNextLine = false;
		    lineRenderer.SetPosition(0, startPos);
		    lineRenderer.SetPosition(1, endPos);
		    yield return new WaitForSeconds(Math.Max(Time.deltaTime * 4f, 0.04f));
		    lineRenderer.SetPosition(0, Vector3.zero);
		    lineRenderer.SetPosition(1, Vector3.zero);
		    drawNextLine = true;
	    }
	}
}
