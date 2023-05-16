using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Block : MonoBehaviour
{
    [SerializeField]
    protected BlockType type = BlockType.None;
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

    #region Unity method
    protected virtual void Awake()
    {
        OnInit();
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
        if (collision.collider.CompareTag(TagsDefined.TAG_WEAPON))
        {
            //breakTween = DOVirtual.DelayedCall(breakTime, () => BrokenCallback?.Invoke(this));
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(TagsDefined.TAG_WEAPON))
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


