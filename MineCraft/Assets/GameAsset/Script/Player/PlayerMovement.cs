using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private PlayerController player;
	[SerializeField]
	private float lookRotateSpeed = 1.0f;
	[SerializeField]
	[Range(0, 90)]
	private float lookRotateLimitTopAngle = 45f;
	[SerializeField]
	[Range(0, 90)]
	private float lookRotateLimitBottomAngle = 75f;
	[SerializeField]
	private float jumpSpeed = 5f;
	[SerializeField]
	private float moveSpeed = 2f;

	private Vector3 velocity;
	private float velocityYPreviousFrame;
	private Vector3 moveDir;
	private float mouseYOffset;

	public float LookRotateSpeed => lookRotateSpeed;
	public float LookRotateLimitTopAngle => lookRotateLimitTopAngle;
	public float LookRotateLimitBottomAngle => lookRotateLimitBottomAngle;

	private void Awake()
	{
		if (player == null)
		{
			player = gameObject.GetComponent<PlayerController>();
		}	
}

	private void Update()
	{
		UpdateMovement();
		UpdateLookRotation();
	}

	private void UpdateMovement()
	{
		velocityYPreviousFrame = velocity.y;
		// player can update direction when jump
		moveDir.Set(player.InputManager.Horizontal, 0f, player.InputManager.Vertical);
		velocity = moveSpeed * ((transform.right * moveDir.x) + (transform.forward * moveDir.z));

		if (player.CharacterController.isGrounded)
		{
			if (player.InputManager.Jump)
			{
				velocity.y = jumpSpeed;
			}
			else
			{
				velocity.y = 0f;
			}
		}
		else
		{
			velocity.y = velocityYPreviousFrame + Physics.gravity.y * Time.deltaTime;
		}

		player.CharacterController.Move(velocity * Time.deltaTime);
	}

	private void UpdateLookRotation()
	{
		mouseYOffset += -player.InputManager.MouseY * lookRotateSpeed;
		mouseYOffset = Mathf.Clamp(mouseYOffset, -lookRotateLimitTopAngle, lookRotateLimitBottomAngle); ;

		// camera rotate top - down
		player.Camera.transform.localRotation = Quaternion.Euler(mouseYOffset, 0f, 0f);

		// player rotate right - left
		transform.rotation *= Quaternion.Euler(0, player.InputManager.MouseX * lookRotateSpeed, 0f);
	}
}
