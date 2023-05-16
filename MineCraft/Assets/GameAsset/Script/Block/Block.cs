using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Block : MonoBehaviour
{
    [SerializeField]
    protected BlockType type = BlockType.None;
    [SerializeField]
    protected Rigidbody rigid;
    [SerializeField]
    protected float breakTime = 1.0f;
    [SerializeField]
    [Tooltip("VFX must be set play on Awake")]
    private VisualEffect breakVfx;
    [SerializeField]
    private float breakVfxPlayTime = 1.5f;

    protected Coroutine breakCoroutine;

    public Action<Block> BrokenCallback { get; set; } = null;

    public BlockType BlockType => type;
    public Rigidbody Rigid => rigid;

    #region Unity method
 
    private void Awake()
    {
        OnInit();

        if (rigid == null)
        {
            rigid = gameObject.GetComponent<Rigidbody>();
            if (rigid == null)
            {
                rigid = gameObject.AddComponent<Rigidbody>();
            }
        }
    }

    protected virtual void OnEnable()
    {
        if (breakVfx)
        {
            breakVfx.gameObject.SetActive(false);
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(TagsDefined.TAG_BLOCK))
        {
            //breakTween = DOVirtual.DelayedCall(breakTime, () => BrokenCallback?.Invoke(this));
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(TagsDefined.TAG_BLOCK))
        {
            //breakTween.Kill();
        }
    }
    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion // Unity method

    #region  abstract method
    protected abstract void OnInit();

    #endregion // abstract method
    public virtual void OnBreak()
    {
        if (breakVfx)
        {
            breakVfx.gameObject.SetActive(true);
        }

        StartCoroutine(BreakDelay());
    }

    protected IEnumerator BreakDelay()
    {
        yield return new WaitForSeconds(breakVfxPlayTime);

        if (breakVfx)
        {
            breakVfx.Stop();
        }
        //SpawnController.Instance.OnBlockBroken(this);
        BrokenCallback?.Invoke(this);
    }

    public void OnCancleBreakProgress()
    {
        if (breakCoroutine != null)
        {
            Debug.Log("Cancle BreakProgress");
            StopCoroutine(breakCoroutine);
            breakCoroutine = null;
        }
    }
}


