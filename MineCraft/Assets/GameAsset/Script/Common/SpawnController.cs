using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnController : Singleton<SpawnController>
{
    [SerializeField]
    private Block[] blockTemplates;
    [SerializeField]
    private ChunkConfig[] chunksConfig;

    private GameObject emptyGameObj;
    //[SerializeField]
    //private VisualEffect GrassBrokenVfx;
    //[SerializeField]
    //private VisualEffect SandBrokenVfx;
    //[SerializeField]
    //private VisualEffect StoneBrokenVfx;

    private Dictionary<BlockType, ObjectPool<Block>> blocksPool;

    /// <summary>
    /// please use the blocksToSpawn to sapwn on pool
    /// </summary>
    private Dictionary<BlockType, Block> blocksToSpawn;
    public BlockType currentBlockType { get; set; } = BlockType.None;

    protected override void OnInit()
    {
#if UNITY_EDITOR
        if (blockTemplates.Length <= 0 ||
            blockTemplates.Length != blockTemplates.GroupBy(x => x.BlockType).Select(y => y.First()).Count())
        {
            Debug.LogError(" blocksTemplate is empty ");
            return;
        }
#endif
        emptyGameObj = new GameObject();
        blocksToSpawn = blockTemplates.ToDictionary(x => x.BlockType, x => x);

        blocksPool = new();
        foreach (var b in blocksToSpawn)
        {
            blocksPool[b.Key] = new ObjectPool<Block>(() => Instantiate(b.Value)
                    , b => b.gameObject.SetActive(true)
                    , b => b.gameObject.SetActive(false));
        }
        OnSpawnChunk();

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSpawnBlock(Vector3 spawnPos, BlockType blockType = BlockType.None)
    {
        if (blockType == BlockType.None)
        {
            blockType = (BlockType)Random.Range((int)BlockType.None + 1, (int)BlockType.Count);
        }

        var block = blocksPool[blockType].Get();
        block.transform.position = spawnPos;
    }

    public void OnBlockBroken(Block block)
    {
        blocksPool[block.BlockType].Release(block);
    }

    private void OnSpawnChunk()
    {
        for (int i = 0; i < chunksConfig.Length; i++)
        {
            var chunkAmount = chunksConfig[i].dimension.x * chunksConfig[i].dimension.y;
            int dimensionX = chunksConfig[i].dimension.x;
            int dimensionZ = chunksConfig[i].dimension.y;
            for (int k = 0; k < chunkAmount; k++)
            {
                var chunk = Instantiate(chunksConfig[i].chunkTemplate, transform);
                for (int j = 0; j < chunk.LayerTypes.Length; j++)
                {
                    var layer = Instantiate(emptyGameObj, chunk.transform);
                    OnSpawnChunkLayer(ref layer, chunk.LayerTypes[j], chunk.LayerDimension, chunk.BlockDimension);
                    var layerLocalPos = layer.transform.localPosition;
                    layer.transform.localPosition = new Vector3(
                        layerLocalPos.x
                        , layerLocalPos.y + chunk.LayerDimension.y * (chunk.LayerTypes.Length - 1 - j)
                        , layerLocalPos.z
                        );
                }

                chunk.transform.localPosition = new Vector3(
                     chunk.transform.localPosition.x + chunk.LayerDimension.x * (k % dimensionX)
                    , chunk.transform.localPosition.y
                    , chunk.transform.localPosition.z + chunk.LayerDimension.z * (k / dimensionZ)
                    );
            }
        }
    }

    private void OnSpawnChunkLayer(ref GameObject layer, in BlockType layerType, in Vector3Int layerDimension, in Vector3 blockDimension)
    {
        layer.name = $"Layer_{layerType}";
        for (int i = 0; i < layerDimension.x * layerDimension.z; i++)
        {
            if (blocksPool.ContainsKey(layerType))
            {
                var block = blocksPool[layerType].Get();
                block.transform.parent = layer.transform;
            }
            else
            {
                Debug.LogError($"Block Type: {layerType} don't config check NOW !");
            }
        }

        for (int i = 0; i < layer.transform.childCount; i++)
        {
            var child = layer.transform.GetChild(i);
            child.localPosition = new Vector3(
                child.localPosition.x + blockDimension.x * (i % layerDimension.x)
                , child.localPosition.y
                , child.localPosition.z + blockDimension.z * (i / layerDimension.z)
                );
        }


    }
}
