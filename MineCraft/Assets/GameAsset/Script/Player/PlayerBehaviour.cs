using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerBehaviour : MonoBehaviour
{
    private PlayerController player;

    private void Awake()
    {
        if (player == null)
        {
            player = gameObject.GetComponent<PlayerController>();
        }
    }
    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnLeftMouseClick()
    {
        if (player.InputManager.LeftMouseClick)
        {
        }
    }

    private void OnRightMouseClick()
    {
        if (player.InputManager.RightMouseClick)
        {
        }
    }

    private void OnKey1Down()
    {
        if (player.InputManager.Key1Down)
        {
        }
    }

    private void OnKey2Down()
    {
        if (player.InputManager.Key2Down)
        {
        }
    }

    private void OnKey3Down()
    {
        if (player.InputManager.Key3Down)
        {
        }
    }
}