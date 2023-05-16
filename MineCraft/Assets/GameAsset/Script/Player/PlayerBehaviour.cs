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
}