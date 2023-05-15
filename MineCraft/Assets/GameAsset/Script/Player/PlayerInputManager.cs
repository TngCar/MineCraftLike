using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class PlayerInputManager : MonoBehaviour
{
    public bool Jump => Input.GetButtonDown("Jump");

    public bool RightMouseClick => Input.GetButtonDown("Mouse1");

    public bool LeftMouseClick => Input.GetButtonDown("Mouse0");

    public bool Key1Down => Input.GetKeyDown(KeyCode.Alpha1);
    
    public bool Key2Down => Input.GetKeyDown(KeyCode.Alpha2);

    public bool Key3Down => Input.GetKeyDown(KeyCode.Alpha3);

    public float Vertical => Input.GetAxisRaw("Vertical");

    public float Horizontal => Input.GetAxisRaw("Horizontal");

    public float MouseY => Input.GetAxis("Mouse Y");


    public float MouseX => Input.GetAxis("Mouse X");
}
