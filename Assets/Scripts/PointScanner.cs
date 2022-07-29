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

public class PointScanner : MonoBehaviour
{
	[Header("Raycast")]
	[SerializeField] [Tooltip("Where to start raycasting from.")]
	private Transform raycastStartPos;
	[SerializeField] [Tooltip("Empty Gameobject inside player used for raycast during sweep scan.")]
	private Transform raycastPointer;
	[SerializeField] [Tooltip("What layers the raycast collides with.")]
	private LayerMask mask;

	[Header("Scanning")]
	[SerializeField] [Tooltip("Rays spread in circle scan mode")]
	private float spread = 0.1f;
	[SerializeField] [Tooltip("Scan delay.")]
	private float scanDelay = 0.003f;
	private float lastScanTime = 0f;
	private bool horizontalScanner = false;

	[Header("Rendering")]
	[SerializeField] [Tooltip("ChunkRenderer used to render the triangle on the corresponding mesh")]
	private ChunkManager chunkManager;
	[SerializeField] [Tooltip("Line renderer used to draw the lines for the LiDAR gun")]
	private LineRenderer lineRenderer;
	[SerializeField]
	private PostProcessVolume postProcess;
	private LensDistortion lensDistortion;
	private bool allowLineUpdate = true;
	private bool drawNextLine = true;

	[Header("Audio")]
	[SerializeField] [LabelText("Volume")] [Range(0, 1)] 
	private float maxAudioVolume = 0.7f;
	private AudioSource scanAudioSource;
	[SerializeField] [Tooltip("Audio source used to play background noises")]
	private AudioSource globalAudioSource;
	[SerializeField] 
	private AudioClip entityScanClip;
	private bool holdingScan = false;
	private bool isEntityScanned = false;

	private void Start()
	{
		scanAudioSource = GetComponent<AudioSource>();
		postProcess.profile.TryGetSettings(out lensDistortion);
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

	private IEnumerator HorizontalScan()
	{
		horizontalScanner = true;
		// slow down mouse and movement speed
		FirstPersonController.scanMouseSpeed = 0.1f;
		FirstPersonController.scanMoveSpeed = 0.1f;

		// play scanner audio
		scanAudioSource.Play();
		scanAudioSource.volume = maxAudioVolume;

		StartCoroutine(FadeIn(0.1f)); // fade in audio

		for (float i = -20; i <= 20; i += 1.5f)
		{
			bool foundLine = false; // used to track whether raycast hit line after middle of for is reached
			for (float j = -25; j <= 25; j += 1.5f)
			{
				// Rotate raycastHorizontal object along lines
				raycastPointer.transform.eulerAngles = new Vector3(i + raycastStartPos.transform.eulerAngles.x + Random.Range(-1.5f, 1.5f), j + raycastStartPos.transform.eulerAngles.y + Random.Range(-1.5f, 1.5f), raycastStartPos.transform.eulerAngles.z);

				if (j >= 0 && !foundLine)
				{
					allowLineUpdate = true;
					foundLine = true;
				}
				DoRayCast(raycastPointer.transform.forward);
			}
			yield return new WaitForSeconds(0.04f);
		}

		StartCoroutine(FadeOut(0.025f)); // fade out sound

		horizontalScanner = false;

		// reset mouse and movement speed
		FirstPersonController.scanMouseSpeed = 1;
		FirstPersonController.scanMoveSpeed = 1;
	}

	private void DoRayCast(Vector3 direction)
	{
		void enableEntityScanDistortion_Shake()
		{
			lensDistortion.active = true;
			CameraShake.instance.Shake(0.2f, 0f);
			StartCoroutine(increaseLensDistortion());
		}

		void playEntityScanBackground()
		{
			globalAudioSource.clip = entityScanClip;
			globalAudioSource.Play();
			scanAudioSource.Stop();
		}

		lastScanTime = Time.time; // update delay time

		// start raycast
		if (Physics.Raycast(raycastStartPos.position, direction, out RaycastHit hit, 20f, mask))
		{
			//if (Physics.OverlapSphereNonAlloc(hit.point, 0.011f, new Collider[8], pointsMask) >= 8) { return; }

			if (allowLineUpdate) // if should draw line
			{
				allowLineUpdate = false;
				StartCoroutine(drawLine(transform.position, hit.point));
			}

			switch (hit.collider.tag)
			{
				// if hit world object (exclude normal entities and stuff)
				case "World":
					chunkManager.AddTriangle(hit.point, Quaternion.LookRotation(hit.normal), new Color32(255, 255, 255, 255));
					break;
				// if hit enemy
				case "Enemy":
					chunkManager.AddTriangle(hit.point, Quaternion.LookRotation(hit.normal), new Color32(255, 0, 0, 255), 5f);
					if (!isEntityScanned) // TODO: FIX THIS MESS
					{
						isEntityScanned = true;

						playEntityScanBackground();
						enableEntityScanDistortion_Shake();
					}
					break;
			}
		}
	}

	private IEnumerator increaseLensDistortion()
	{
		while (true) // increase lens distortion indefinitely
		{
			lensDistortion.scale.value += 0.01f;
			yield return new WaitForSeconds(0.05f);
		}
	}

	private Vector3 GetCircularDirection()
	{
		Vector3 direction = raycastStartPos.transform.forward;

		// Add random spread to direction
		direction += new Vector3(
			Random.Range(-spread, spread),
			Random.Range(-spread, spread),
			Random.Range(-spread, spread)
		);
		direction.Normalize();

		return direction;
	}

	private IEnumerator drawLine(Vector3 startPos, Vector3 endPos)
	{
		if (isEntityScanned) // if entity was scanned, disable the rendering of line
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

	private IEnumerator FadeOut(float step)
	{
		if (isEntityScanned) // if entity was scanned, stop scanner audio
		{ scanAudioSource.Stop(); yield break;}

		while (scanAudioSource.volume != 0)
		{
			scanAudioSource.volume = Math.Clamp(scanAudioSource.volume - step, 0f, 1f);
			yield return new WaitForSeconds(0.01f);
		}
		scanAudioSource.Stop();
		scanAudioSource.volume = maxAudioVolume;
	}

	private IEnumerator FadeIn(float step)
	{
		if (isEntityScanned) // if entity was scanned, stop scanner audio
		{ scanAudioSource.Stop(); yield break; }

		scanAudioSource.volume = 0;
		scanAudioSource.Play();
		while (scanAudioSource.volume < maxAudioVolume)
		{
			scanAudioSource.volume = Math.Clamp(scanAudioSource.volume + step, 0f, 1f);
			yield return new WaitForSeconds(0.01f);
		}
	}
}

