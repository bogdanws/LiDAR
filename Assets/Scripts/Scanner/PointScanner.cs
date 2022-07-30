using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace lidar
{
	public partial class PointScanner : MonoBehaviour
	{
		// variables
		private bool isEntityScanned = false;
		private bool allowNormalEffects = true;
		[Tooltip("Time used to play the jumpscare effects")] private float entityScareTime = 4f;
		[Tooltip("Time used to play the post-jumpscare effects")] private float entityPostScareTime = 15f;

		private void Start()
		{
			// Get the audio source used for scanning sounds
			scanAudioSource = GetComponent<AudioSource>();

			// Get settings for post processing
			postProcess.profile.TryGetSettings(out lensDistortion);

			// Add 2 vertices to the line renderer
			lineRenderer.positionCount = 2;
		}


		private void Update()
		{
			float mouseScroll = Input.GetAxis("Mouse ScrollWheel"); // change scan spread
			if (mouseScroll != 0f)
			{
				spread = Mathf.Clamp(spread + mouseScroll / 2, 0.05f, 0.6f);
			}

			if (Input.GetMouseButtonDown(1) && !horizontalScanner) // if currently holding secondary button
			{
				// check if delay passed
				if (lastScanTime + scanDelay < Time.time)
				{
					StartCoroutine(HorizontalScan());
				}
			}


			if (Input.GetMouseButton(0) && !horizontalScanner) // if currently holding shoot button
			{
				if (!holdingScan)
				{
					holdingScan = true;
					scanAudioSource.Play();
				}

				// check if delay passed
				if (lastScanTime + scanDelay / 2 < Time.time)
				{
					allowLineUpdate = true;
					DoRayCast(GetCircularDirection());
				}
			}

			if (Input.GetMouseButtonUp(0) && holdingScan) // fade out scanner sound when player stops random scanning
			{
				holdingScan = false;
				StartCoroutine(FadeOut(0.1f));
			}
		}
	}
}