using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private Camera camera;

    public CharacterController CharacterController => characterController;
    public InputManager InputManager { get; set; }
 
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
