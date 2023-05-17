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
	protected Collider collider;
	[SerializeField]
	protected float breakTime = 1.0f;
	[SerializeField]
	[Tooltip("VFX must be set play on Awake")]
	private VisualEffect breakVfx;
	[SerializeField]
	private float breakVfxPlayTime = 1.5f;

	protected Coroutine breakProgressCoroutine;

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

		if (collider == null)
		{
			collider = gameObject.GetComponent<Collider>();
		}
	}

	protected virtual void OnEnable()
	{
		if (collider)
		{
			collider.enabled = true;
		}

		if (breakVfx)
		{
			breakVfx.gameObject.SetActive(false);
		}
	}

	//protected void OnCollisionEnter(Collision collision)
	//{
	//    if (collision.collider.CompareTag(TagsDefined.TAG_BLOCK))
	//    {
	//        //breakTween = DOVirtual.DelayedCall(breakTime, () => BrokenCallback?.Invoke(this));
	//    }
	//}

	//protected void OnCollisionExit(Collision collision)
	//{
	//    if (collision.collider.CompareTag(TagsDefined.TAG_BLOCK))
	//    {
	//        //breakTween.Kill();
	//    }
	//}
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
		// collider disable during vfx playing
		// for playe not detect again;
		// will enable it when block spawn by pool
		collider.enabled = false;
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
		BrokenCallback?.Invoke(this);
	}

	public void OnCancleBreakProgress()
	{
		if (breakProgressCoroutine != null)
		{
			Debug.Log("Cancle BreakProgress");
			StopCoroutine(breakProgressCoroutine);
			breakProgressCoroutine = null;
		}
	}

	public void OnBreakProgressStart(Action completeCallback = null)
	{
		breakProgressCoroutine = StartCoroutine(WaitBreakComplete(completeCallback));
	}

	protected IEnumerator WaitBreakComplete(Action completeCallback)
	{
		Debug.Log($"WaitBreakComplete start count down, keep mouse {breakTime}s ");
		yield return new WaitForSeconds(breakTime);

		// double check
		if (breakProgressCoroutine != null)
		{
			completeCallback?.Invoke();
			OnBreak();
			Debug.Log($"WaitBreakComplete Block Broken!");
		}
		else
		{
			Debug.Log($"WaitBreakComplete end, but progress has been cancle");
		}
	}
}


