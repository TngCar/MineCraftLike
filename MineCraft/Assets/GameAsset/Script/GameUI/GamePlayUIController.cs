using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GamePlayUIController : MonoBehaviour
{
    [SerializeField]
    private Image attackImage;

    [SerializeField]
    private BlockSelectUI[] blockItems;
    [SerializeField]
    private Color selectColor;
    [SerializeField]
    private Color unselectColor;

    private BlockType currentType = BlockType.None;

    public static Action<float> attackHoldTime;

    private float fillTime = 0;
    private float timer;
    // Use this for initialization
    void Start()
    {
        attackImage.fillAmount = 0;
        attackHoldTime = (t) =>
        {
            fillTime = t;
            timer = fillTime;
        };

#if UNITY_EDITOR
        // check duplicate config
        if (blockItems.Length <= 0 ||
            blockItems.Length != blockItems.GroupBy(x => x.BlockType).Select(y => y.First()).Count())
        {
            Debug.LogError("BlockSelectUIController: blockItems is empty or duplicate item");
            return;
        }
#endif
        bool hasRandom = false;
        for (int i = 0; i < blockItems.Length; ++i)
        {
            if (blockItems[i].BlockType == BlockType.None)
            {
                hasRandom = true;
                currentType = BlockType.None;
                blockItems[i].OnSelectItemHandle(selectColor);
            }
            else
            {
                blockItems[i].OnSelectItemHandle(unselectColor);
            }
        }

        if (!hasRandom)
        {
            Debug.LogWarning("Don't have Random select in config");
            var block = blockItems[Random.Range(0, blockItems.Length)];
            block.OnSelectItemHandle(selectColor);
            currentType = block.BlockType;
        }
    }

    public void OnBlockSelectChange(BlockType blockType)
    {
        if (currentType == blockType) return;

        // querry never null
        blockItems.First(x => x.BlockType == currentType).OnSelectItemHandle(unselectColor);

        if (blockItems.Any(x => x.BlockType == blockType))
        {
            var block = blockItems.First(x => x.BlockType == blockType);
            block.OnSelectItemHandle(selectColor);
            currentType = block.BlockType;
        }
        else
        {
            Debug.LogError($"Can't select block {blockType}");
        }
    }

    private void Update()
    {
        if (fillTime == 0 || timer <= 0)
        {
            attackImage.fillAmount = 0;
        }
        else
        {
            timer -= Time.deltaTime;
            attackImage.fillAmount = timer / fillTime;
        }
    }
}
