using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

	private float lastScanTime = 0f;
	private bool horizontalScanner = false;

	

	private void Update()
	{
		float mouseScroll = Input.GetAxis("Mouse ScrollWheel"); // change scan spread
		if (mouseScroll != 0f)
		{
			spread = Mathf.Clamp(spread + mouseScroll / 2, 0.05f, 0.5f);
		}

		if (Input.GetMouseButtonDown(1) && !horizontalScanner) // if currently holding secondary button
		{
			// check if delay passed
			if (lastScanTime + scanDelay < Time.time)
			{
				StartCoroutine(horizontalScan());
			}
		}


		if (Input.GetMouseButton(0) && !horizontalScanner) // if currently holding shoot button
		{
			// check if delay passed
			if (lastScanTime + scanDelay < Time.time)
			{
				doRayCast(GetCircularDirection());
			}
		}
	}

	private IEnumerator horizontalScan()
	{
		horizontalScanner = true;
		inputActions.Disable();

		Transform raycastStartTransform = raycastStartPos.transform;

		for (int i = -15; i <= 15; i++)
		{
			for (int j = -20; j <= 20; j++)
			{
				// Rotate raycastPointer object along lines
				Vector3 eulerAngles = raycastStartTransform.eulerAngles;
				raycastPointer.transform.eulerAngles = new Vector3(i + eulerAngles.x, j + eulerAngles.y, eulerAngles.z);
				doRayCast(raycastStartTransform.forward);
			}

			yield return new WaitForSeconds(0.04f);
		}
		
		horizontalScanner = false;
		inputActions.Enable();
	}

	private void doRayCast(Vector3 direction)
	{
		lastScanTime = Time.time; // update delay time

		// start raycast
		if (Physics.Raycast(raycastStartPos.position, direction, out RaycastHit hit, 50f, mask))
		{
			//if (Physics.OverlapSphereNonAlloc(hit.point, 0.011f, new Collider[8], pointsMask) >= 8) { return; }

			switch (hit.collider.tag)
			{
				// if hit world object (exclude normal entities and stuff)
				case "World":
					trRenderer.AddTriangle(hit.point, Quaternion.LookRotation(hit.normal));
					break;
				// if hit enemy
				case "Enemy":
					/*point.GetComponent<Renderer>().material = redMat;
					point.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);*/
					trRenderer.AddTriangle(hit.point, Quaternion.LookRotation(hit.normal));
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
