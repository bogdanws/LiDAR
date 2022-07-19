using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class gun : MonoBehaviour
{
	[SerializeField]
	private float spread = 0.1f;
	[SerializeField]
	private Transform raycastStart;
	[SerializeField]
	private Transform raycastHorizontal;
	/*[SerializeField]
	private TrailRenderer scanTrail;*/
	[SerializeField]
	private float scanDelay = 0.005f;
	[SerializeField]
	private LayerMask mask;
	[SerializeField]
	private LayerMask pointsMask;
	[SerializeField]
	private GameObject hitParent;
	[SerializeField]
	private GameObject hitPoint;
	[SerializeField]
	private Material hitPointMaterial;

	private float lastScanTime = 0f;
	private bool horizontalScanner = false;

	[SerializeField]
	public int maxPoints = 5000;
	Queue<GameObject> points = new Queue<GameObject>();

	private void Start()
	{
		generateMaterials();
	}

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
	}

	private IEnumerator horizontalScan()
	{
		horizontalScanner = true;
		//inputActions.Disable();

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
		//inputActions.Enable();
	}

	private GameObject doRayCast(Vector3 direction)
	{
		GameObject point = null;
		lastScanTime = Time.time; // update delay time

		// start raycast
		if (Physics.Raycast(raycastStart.position, direction, out RaycastHit hit, 50f, mask))
		{

			//if (Physics.OverlapSphereNonAlloc(hit.point, 0.02f, new Collider[4], pointsMask) >= 4) { return point; }

			if (hit.collider.tag == "World") // if hit world object (exclude normal entities and stuff)
			{
				point = Instantiate(hitPoint, hit.point, Quaternion.LookRotation(hit.normal)); // instantiate object
				point.transform.parent = hitParent.transform; // set parent to hitParent
			}
			else if (hit.collider.tag == "Enemy") // if hit enemy
			{
				point = Instantiate(hitPoint, hit.point, Quaternion.LookRotation(hit.normal)); // instantiate object
				point.transform.parent = hitParent.transform; // set parent to hitParent
				point.GetComponent<Renderer>().material = redMat;
				point.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
				//foreach (Renderer r in point.GetComponentsInChildren<Renderer>()) r.material = redMat;
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
