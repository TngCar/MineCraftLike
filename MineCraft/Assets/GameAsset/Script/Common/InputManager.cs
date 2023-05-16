using UnityEngine;


public class InputManager : MonoBehaviour
{
    public bool Jump => Input.GetButtonDown("Jump");

    public bool LeftMouseClick => Input.GetKeyDown(KeyCode.Mouse0);
    public bool LeftMouseRelease => Input.GetKeyUp(KeyCode.Mouse0);
    public bool RightMouseClick => Input.GetKeyDown(KeyCode.Mouse1);

    public bool Key4Up => Input.GetKeyUp(KeyCode.Alpha4);
    public bool Key1Up => Input.GetKeyUp(KeyCode.Alpha1);
    
    public bool Key2Up => Input.GetKeyUp(KeyCode.Alpha2);

    public bool Key3Up => Input.GetKeyUp(KeyCode.Alpha3);

    // W, S / arrow up , arrow down
    public float Vertical => Input.GetAxisRaw("Vertical");

    // A, D / <- , ->
    public float Horizontal => Input.GetAxisRaw("Horizontal");

    public float MouseY => Input.GetAxis("Mouse Y");

    public float MouseX => Input.GetAxis("Mouse X");
}
