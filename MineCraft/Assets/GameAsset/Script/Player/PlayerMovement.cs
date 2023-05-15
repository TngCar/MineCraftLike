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
    private float lookRotateLimitTopAngle = 45;
    [SerializeField]
    [Range(0, 90)]
    private float lookRotateLimitBottomAngle = 75;
    [SerializeField]
    private float jumpSpeed = 8f;


    private bool canMove = true;
    private Vector3 moveDirection = new Vector3();
    private Vector2 currentSpeed;
    private float mouseY;

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
        currentSpeed = canMove ? new Vector2(player.InputManager.Vertical, player.InputManager.Horizontal) : Vector2.zero;
        float movementDirectionY = moveDirection.y;
        moveDirection = (transform.forward * currentSpeed.x) + (transform.right * currentSpeed.y);
        if (player.CharacterController.isGrounded)
        {
            if (player.InputManager.Jump && canMove)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }
        }
        else
        {
            moveDirection.y += Physics.gravity.y * Time.deltaTime;
        }

        player.CharacterController.Move(moveDirection * Time.deltaTime);

    }

    private void UpdateLookRotation()
    {
        if (canMove)
        {
            mouseY += -player.InputManager.MouseY * lookRotateSpeed;
            mouseY = Mathf.Clamp(mouseY, -lookRotateLimitTopAngle, lookRotateLimitBottomAngle); ;

            // camera rotate top - down
            player.Camera.transform.localRotation = Quaternion.Euler(mouseY, 0, 0);

            // player rotate right - left
            transform.rotation *= Quaternion.Euler(0, player.InputManager.MouseX * lookRotateSpeed, 0);
        }
    }
}
