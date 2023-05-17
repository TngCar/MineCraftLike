using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerBehaviour : MonoBehaviour
{
    private PlayerController player;

    private Block blockTarget = null;

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
        if (player.InputManager.LeftMouseClick)
        {
            Debug.Log("PlayerBehaviour OnLeftMouseClick");
            OnActackRandomHandle();
        }

        if (player.InputManager.LeftMouseRelease)
        {
            Debug.Log("PlayerBehaviour OnLeftMouseRelease");
            OnCancleAttact();
        }
    }
    public void OnActackRandomHandle()
    {
        // block's layer is 6, please see on  unity editor
        Collider[] blockColliders = Physics.OverlapSphere(transform.position, player.DetectMaxRange, 1 << 6);
        if (blockColliders.Length > 0)
        {
            var block = blockColliders[Random.Range(0, blockColliders.Length)].GetComponent<Block>();
            if (block)
            {
                blockTarget = block;
                blockTarget.OnBreakProgressStart(() => { blockTarget = null; });
            }
        }
    }

    private void OnCancleAttact()
    {
        if (blockTarget != null)
        {
           blockTarget.OnCancleBreakProgress();
        }
    }
}