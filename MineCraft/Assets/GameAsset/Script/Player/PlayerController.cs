using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private PlayerInputManager inputManager;

    [SerializeField]
    private Camera camera;

    public CharacterController CharacterController => characterController;
    public PlayerInputManager InputManager => inputManager;
 
    public Camera Camera => camera;

    // Use this for initialization
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
