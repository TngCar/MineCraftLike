using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerBehaviour : MonoBehaviour
{
    private PlayerController player;

    private Block blockTarget = null;
    private RaycastHit hit;

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
#if UNITY_EDITOR
        // draw red line in Scene window
        Debug.DrawRay(player.Camera.transform.position, player.Camera.transform.forward * player.AttackRange, Color.red, 0.5f);
#endif

        if (player.InputManager.LeftMouseRelease)
        {
            Debug.Log("PlayerBehaviour OnLeftMouseRelease");
            OnCancleAttact();
        }

        if (player.InputManager.LeftMouseClick)
        {
            Debug.Log("PlayerBehaviour OnLeftMouseClick");
            //blockTarget = null;
            if (!CanAttactTargetBlock())
                OnActackRandomHandle();
        }

    }

    private bool CanAttactTargetBlock()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, player.AttackRange, 1 << 6))
        {
            var block = hit.collider.gameObject.GetComponent<Block>();
            if (block != null)
            {
                OnLockTargetHandle(block);
                return true;
            }
        }

        return false;
    }

    private void OnActackRandomHandle()
    {
        // block's layer is 6, please see on  unity editor
        Collider[] blockColliders = Physics.OverlapSphere(transform.position, player.DetectMaxRange, 1 << 6);
        if (blockColliders.Length > 0)
        {
            var block = blockColliders[Random.Range(0, blockColliders.Length)].GetComponent<Block>();

            if (block != null)
            {
                OnLockTargetHandle(block);
            }
        }
    }

    private void OnCancleAttact()
    {
        if (blockTarget != null)
        {
            blockTarget.OnCancleBreakProgress();
            blockTarget = null;
            GamePlayUIController.attackHoldTime(0);
        }
    }

    private void OnLockTargetHandle(Block block)
    {
        if (blockTarget != null)
        {
            blockTarget.OnCancleBreakProgress();
        }

        blockTarget = block;
        blockTarget.OnBreakProgressStart(() => { blockTarget = null; });
        GamePlayUIController.attackHoldTime(blockTarget.BreakTime);

    }
}