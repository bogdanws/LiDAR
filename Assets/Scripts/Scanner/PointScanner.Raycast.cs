using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
/*
 * Raycast and scanning
 */
namespace lidar
{
    public partial class PointScanner : MonoBehaviour
    {
	    [Header("Raycast")]
	    [SerializeField]
	    [Tooltip("Where to start raycasting from.")]
	    private Transform raycastStartPos;

	    [SerializeField]
	    [Tooltip("Empty Gameobject inside player used for raycast during sweep scan.")]
	    private Transform raycastPointer;

	    [SerializeField]
	    [Tooltip("What layers the raycast collides with.")]
	    private LayerMask mask;

	    [Header("Scanning")]
	    [SerializeField]
	    [Tooltip("Rays spread in circle scan mode")]
	    private float spread = 0.1f;

	    [SerializeField]
	    [Tooltip("Scan delay.")]
	    private float scanDelay = 0.003f;

	    private float lastScanTime = 0f;
	    private bool horizontalScanner = false;


	    private void DoRayCast(Vector3 direction) {
		    lastScanTime = Time.time; // update delay time

		    // start raycast
		    if (Physics.Raycast(raycastStartPos.position, direction, out RaycastHit hit, 20f, mask)) {
			    //if (Physics.OverlapSphereNonAlloc(hit.point, 0.011f, new Collider[8], pointsMask) >= 8) { return; }

			    if (allowLineUpdate) // if should draw line
			    {
				    allowLineUpdate = false;
				    StartCoroutine(drawLine(transform.position, hit.point));
			    }

			    switch (hit.collider.tag) {
				    // if hit world object (exclude normal entities and stuff)
				    case "World":
					    chunkManager.AddTriangle(hit.point, Quaternion.LookRotation(hit.normal), new Color32(255, 255, 255, 255));
					    break;
				    // if hit enemy
				    case "Enemy":
					    //chunkManager.AddTriangle(hit.point, Quaternion.LookRotation(hit.normal), new Color32(255, 0, 0, 255), 5f);
					    if (allowNormalEffects)
					    {
						    isEntityScanned = true;
						    allowNormalEffects = false;

						    hit.collider.GetComponent<MeshRenderer>().enabled = true;

						    playEntityScanBackground(entityScareTime);
						    enableEntityScanDistortion_Shake();

						    Utility.Invoke(this, () =>
						    {
							    allowNormalEffects = true;
							    Application.Quit();
						    }, entityScareTime);
					    }
					    break;
			    }
		    }
	    }


		private IEnumerator HorizontalScan() {
		    horizontalScanner = true;
		    // slow down mouse and movement speed
		    FirstPersonController.scanMouseSpeed = 0.1f;
		    FirstPersonController.scanMoveSpeed = 0.1f;

		    // play scanner audio
		    scanAudioSource.Play();
		    scanAudioSource.volume = maxAudioVolume;

		    StartCoroutine(FadeIn(0.1f)); // fade in audio

		    for (float i = -20; i <= 20; i += 1.5f) {
			    bool foundLine = false; // used to track whether raycast hit line after middle of for is reached
			    for (float j = -25; j <= 25; j += 1.5f) {
				    // Rotate raycastHorizontal object along lines
				    raycastPointer.transform.eulerAngles = new Vector3(i + raycastStartPos.transform.eulerAngles.x + Random.Range(-1.5f, 1.5f), j + raycastStartPos.transform.eulerAngles.y + Random.Range(-1.5f, 1.5f), raycastStartPos.transform.eulerAngles.z);

				    if (j >= 0 && !foundLine) {
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

	    private Vector3 GetCircularDirection() {
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
}
