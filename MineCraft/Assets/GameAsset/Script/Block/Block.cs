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

	protected Coroutine breakProgressCoroutine;

	public BlockType BlockType => type;
	public Rigidbody Rigid => rigid;
	public float BreakTime => breakTime;

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
		GameManager.Instance.OnBreakBlock(this);
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


