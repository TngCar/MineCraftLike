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
    private VisualEffect brokenVfx;
    [SerializeField]
    private float brokenVfxPlayTime = 1.5f;

    [HideInInspector]
    public Action<Block> BrokenCallback = null;

    protected Coroutine BrockenCoroutine;
    public virtual BlockType BlockType => type;

    #region Unity method
    protected virtual void Awake()
    {
        OnInit();
    }

    protected virtual void OnEnable()
    {
        if (brokenVfx)
        {
            brokenVfx.gameObject.SetActive(false);
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
    public virtual void OnBroken()
    {
        if (brokenVfx)
        {
            brokenVfx.gameObject.SetActive(true);
        }
        StartCoroutine(BrokenDelay());
    }

    protected IEnumerator BrokenDelay()
    {
        yield return new WaitForSeconds(brokenVfxPlayTime);

        if (brokenVfx)
        {
            brokenVfx.Stop();
        }
        SpawnController.Instance.OnBlockBroken(this);

    }

}


