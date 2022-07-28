using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PointScanner : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Rays spread in circle scan mode")]
	private float spread = 0.1f;
	[SerializeField]
	[Tooltip("Reference to movement input system asset in order to disable movement while scanning.")]
	private InputActionAsset inputActions;
	[SerializeField]
	[Tooltip("Where to start raycasting from.")]
	private Transform raycastStartPos;
	[SerializeField]
	[Tooltip("Empty Gameobject inside player used for raycast during sweep scan.")]
	private Transform raycastPointer;
	/*[SerializeField]
	private TrailRenderer scanTrail;*/
	[SerializeField]
	[Tooltip("Scan delay.")]
	private float scanDelay = 0.005f;
	[SerializeField]
	[Tooltip("What layers the raycast collides with.")]
	private LayerMask mask;
	[SerializeField]
	[Tooltip("TriangleMeshRenderer component on a GameObject used to render the triangles")]
	private TriangleMeshRenderer trRenderer;

	private float _lastScanTime = 0f;
	private bool _horizontalScanner = false;

	

	private void Update()
	{
		float mouseScroll = Input.GetAxis("Mouse ScrollWheel"); // change scan spread
		if (mouseScroll != 0f)
		{
			spread = Mathf.Clamp(spread + mouseScroll / 2, 0.05f, 0.5f);
		}

		if (Input.GetMouseButtonDown(1) && !_horizontalScanner) // if currently holding secondary button
		{
			// check if delay passed
			if (_lastScanTime + scanDelay < Time.time)
			{
				StartCoroutine(HorizontalScan());
			}
		}


		if (Input.GetMouseButton(0) && !_horizontalScanner) // if currently holding shoot button
		{
			// check if delay passed
			if (_lastScanTime + scanDelay < Time.time)
			{
				DoRayCast(GetCircularDirection());
			}
		}
	}

	private IEnumerator HorizontalScan()
	{
		_horizontalScanner = true;
		inputActions.Disable();

		for (int i = -15; i <= 15; i++)
		{
			for (int j = -20; j <= 20; j++)
			{

				// Rotate raycastHorizontal object along lines
				raycastPointer.transform.eulerAngles = new Vector3(i + raycastStartPos.transform.eulerAngles.x + Random.Range(-1.5f, 1.5f), j + raycastStartPos.transform.eulerAngles.y + Random.Range(-1.5f, 1.5f), raycastStartPos.transform.eulerAngles.z);
				DoRayCast(raycastPointer.transform.forward);
			}

			yield return new WaitForSeconds(0.04f);
		}

		_horizontalScanner = false;
		inputActions.Enable();
	}

	private void DoRayCast(Vector3 direction)
	{
		_lastScanTime = Time.time; // update delay time

		// start raycast
		if (Physics.Raycast(raycastStartPos.position, direction, out RaycastHit hit, 50f, mask))
		{
			//if (Physics.OverlapSphereNonAlloc(hit.point, 0.011f, new Collider[8], pointsMask) >= 8) { return; }

			switch (hit.collider.tag)
			{
				// if hit world object (exclude normal entities and stuff)
				case "World":
					trRenderer.AddTriangle(hit.point, Quaternion.LookRotation(hit.normal), new Color32(255, 255, 255, 255));
					break;
				// if hit enemy
				case "Enemy":
					/*point.GetComponent<Renderer>().material = redMat;
					point.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);*/
					trRenderer.AddTriangle(hit.point, Quaternion.LookRotation(hit.normal), new Color32(255, 0, 0, 255), 2.5f);
					break;
			}
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
}
