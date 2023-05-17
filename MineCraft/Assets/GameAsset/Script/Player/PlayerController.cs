using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private Camera camera;
    [SerializeField]
    private float attackRange = 2f;
    [SerializeField]
    private float detectMaxRange = 200f;

    public CharacterController CharacterController => characterController;
    public InputManager InputManager { get; set; }
    public float AttackRange => attackRange;
    public float DetectMaxRange { get => detectMaxRange; set => detectMaxRange = value; }

    public Camera Camera => camera;

    // Use this for initialization
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // switch to palyer Camera
        if (Camera.main != null)
        {
            Camera.main.enabled = false;
        }

        camera.enabled = true;
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
