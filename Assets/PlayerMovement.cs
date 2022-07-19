using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private CharacterController controller;
	[SerializeField]
	public float speed = 12f;

	[SerializeField]
	private Vector3 velocity;
	[SerializeField]
	private bool isGrounded;
	[SerializeField]
	private float gravity = -9.81f;
	[SerializeField]
	private Transform groundCheck;
	[SerializeField]
	private float groundDistance = 0.4f;
	[SerializeField]
	private LayerMask groundMask;

	// Update is called once per frame
	void Update()
    {
		// Check if player is grounded
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		// Get keyboard input
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		// Move player
		Vector3 move = transform.right * horizontal + transform.forward * vertical;
		controller.Move(move * speed * Time.deltaTime);

		// Apply gravity to player
		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f;
		} else if (!isGrounded)
		{
			velocity.y += gravity * Time.deltaTime;
			controller.Move(velocity * Time.deltaTime);
		}
	}
}
