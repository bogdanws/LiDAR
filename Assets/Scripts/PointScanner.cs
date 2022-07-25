using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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
	private Transform raycastStart;
	[SerializeField]
	[Tooltip("Empty Gameobject inside player used for raycast during sweep scan.")]
	private Transform raycastHorizontal;
	/*[SerializeField]
	private TrailRenderer scanTrail;*/
	[SerializeField]
	[Tooltip("Scan delay.")]
	private float scanDelay = 0.005f;
	[Tooltip("Cooldown for sphere scan.")]
	private float sphereDelay = 1f;
	[SerializeField]
	[Tooltip("What layers the raycast collides with.")]
	private LayerMask mask;
	[SerializeField]
	[Tooltip("Layer that the points are on.")]
	private LayerMask pointsMask;
	[SerializeField]
	[Tooltip("Gameobject used to group all points under 1 parent.")]
	private GameObject hitParent;
	[SerializeField]
	[Tooltip("Prefab of point.")]
	private GameObject hitPoint;
	[SerializeField]
	[Tooltip("Material of point.")]
	private Material hitPointMaterial;

	private float lastScanTime = 0f;
	private bool horizontalScanner = false;
	private float lastSphereScanTime = 0f;

	private List<Vector3> tr = new List<Vector3>();
	private List<Color> cl = new List<Color>();

	// Maximum points after which old ones start getting deleted
	public static int maxPoints = 20000;
	Queue<GameObject> points = new Queue<GameObject>(); // queue that stores all the points

	private void Start()
	{
		generateMaterials();
	}

	// Instantiate materials at runtime for batching to work
	private Material redMat;
	private void generateMaterials()
	{
		redMat = Instantiate(hitPointMaterial);
		redMat.color = Color.red;
	}

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

		if (Input.GetMouseButton(2) && !horizontalScanner) // if currently holding middle button
		{
			// check if delay passed
			if (lastSphereScanTime + sphereDelay < Time.time)
			{
				StartCoroutine(sphereScan());
			}
		}

	}

	private IEnumerator horizontalScan()
	{
		horizontalScanner = true;
		inputActions.Disable();

		for (int i = -15; i <= 15; i++)
		{
			for (int j = -20; j <= 20; j++)
			{
				// Rotate raycastHorizontal object along lines
				raycastHorizontal.transform.eulerAngles = new Vector3(i + raycastStart.transform.eulerAngles.x, j + raycastStart.transform.eulerAngles.y, raycastStart.transform.eulerAngles.z);
				doRayCast(raycastHorizontal.transform.forward);
			}

			yield return new WaitForSeconds(0.04f);
		}
		
		horizontalScanner = false;
		inputActions.Enable();
	}

	private IEnumerator sphereScan()
	{
		lastSphereScanTime = Time.time;

		for (int i = -85; i <= 85; i += 2)
		{
			if (i < -25 || i > 25 && i <= 82)
			{
				i += 3;
			} else if (i < -20 || i > 20)
			{
				i += 1;
			}
			for (int j = -180; j <= 179; j += 5)
			{
				// Rotate raycastHorizontal object along lines
				raycastHorizontal.transform.eulerAngles = new Vector3(i, j, 0);
				doRayCast(raycastHorizontal.transform.forward);
			}
		}

		yield return null;
	}

	private GameObject doRayCast(Vector3 direction)
	{
		GameObject point = null;
		lastScanTime = Time.time; // update delay time

		// start raycast
		if (Physics.Raycast(raycastStart.position, direction, out RaycastHit hit, 50f, mask))
		{
			if (Physics.OverlapSphereNonAlloc(hit.point, 0.011f, new Collider[8], pointsMask) >= 8) { return point; }

			switch (hit.collider.tag)
			{
				// if hit world object (exclude normal entities and stuff)
				case "World":
					point = Instantiate(hitPoint, hit.point, Quaternion.LookRotation(hit.normal)); // instantiate object
					point.transform.parent = hitParent.transform; // set parent to hitParent
					break;
				// if hit enemy
				case "Enemy":
					point = Instantiate(hitPoint, hit.point, Quaternion.LookRotation(hit.normal)); // instantiate object
					point.transform.parent = hitParent.transform; // set parent to hitParent
					point.GetComponent<Renderer>().material = redMat;
					point.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
					break;
			}
		}

		points.Enqueue(point);
		while (points.Count > maxPoints) // remove previous points until max limit is reached
		{
			Destroy(points.Dequeue());
		}
		return point;
	}

	private Vector3 GetCircularDirection()
	{
		Vector3 direction = raycastStart.transform.forward;

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
