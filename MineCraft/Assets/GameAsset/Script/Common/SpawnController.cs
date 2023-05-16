using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnController : Singleton<SpawnController>
{
    [SerializeField]
    private Block[] blockTemplates;
    [SerializeField]
    private ChunkConfig chunkConfig;

    private GameObject emptyGameObject;
    private GameObject chunkSpawnedContainer;
    private SpawnGridPlatformXZ spawnGridXZ;

    private Dictionary<BlockType, ObjectPool<Block>> blocksPool;

    /// <summary>
    /// please use the blocksToSpawn to sapwn on pool
    /// </summary>
    private Dictionary<BlockType, Block> blocksToSpawn;
    public BlockType CurrentBlockType { get; set; } = BlockType.None;
    public List<Block> BlocksSpawned { get; private set; }

    protected override void OnInit()
    {
#if UNITY_EDITOR
        if (blockTemplates.Length <= 0 ||
            blockTemplates.Length != blockTemplates.GroupBy(x => x.BlockType).Select(y => y.First()).Count())
        {
            Debug.LogError("SpawnController: blocksTemplate is empty or duplicate item");
            return;
        }
#endif
        BlocksSpawned = new();
        emptyGameObject = new GameObject();
        chunkSpawnedContainer = new GameObject();
#if UNITY_EDITOR
        chunkSpawnedContainer.name = nameof(chunkSpawnedContainer);
#endif

        blocksToSpawn = blockTemplates.ToDictionary(x => x.BlockType, x => x);

        blocksPool = new();
        foreach (var b in blocksToSpawn)
        {
            blocksPool[b.Key] = new ObjectPool<Block>(
                () => Instantiate(b.Value)
                , b =>
                {
                    b.gameObject.SetActive(true);
                    b.BrokenCallback = OnBlockBroken;
                }
                , b =>
                {
                    b.gameObject.SetActive(false);
                    b.BrokenCallback = null;
                });
        }

        OnSpawnChunk();
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {

    }

    /// <summary>
    /// BlockType.None is for Random type
    /// </summary>
    /// <param name="spawnPos"></param>
    /// <param name="blockType"></param>
    public void OnSpawnBlock(Vector3 spawnPos, BlockType blockType = BlockType.None)
    {
        if (blockType == BlockType.None)
        {
            blockType = (BlockType)Random.Range((int)BlockType.None + 1, (int)BlockType.Count);
        }

        var block = blocksPool[blockType].Get();
        block.transform.position = spawnPos;
    }

    /// <summary>
    /// for random position
    /// </summary>
    /// <param name="blockType"></param>
    public void OnSpawnBlock(BlockType blockType)
    {
        
    }

    public void OnBlockBroken(Block block)
    {
        // release pool instead of Destroy Object
        blocksPool[block.BlockType].Release(block);
    }

    private void OnSpawnChunk()
    {
        spawnGridXZ = new();
        ChunkController firstChunkLayout = null;
        int dimension = chunkConfig.dimension.x * chunkConfig.dimension.y;
        int dimensionX = chunkConfig.dimension.x;
        for (int i = 0; i < dimension; ++i)
        {
            ChunkController chunk;
            if (firstChunkLayout == null)
            {
                // create first chunk
                chunk = Instantiate(chunkConfig.chunkTemplate, chunkSpawnedContainer.transform);
                for (int j = 0; j < chunk.LayerTypes.Length; ++j)
                {
                    // offset for up to down
                    var layerOffsetY = chunk.LayerTypes.Length - 1 - j;
                    OnSpawnChunkLayer(chunk.transform, chunk.LayerTypes[j], chunk.LayerDimension, chunk.BlockDimension, layerOffsetY);
                }

                firstChunkLayout = chunk;
            }
            else
            {
                // clone  other chunk from first chunk
                chunk = Instantiate(firstChunkLayout, chunkSpawnedContainer.transform);
            }

            // set chunk position
            chunk.transform.localPosition = new Vector3(
                 chunk.transform.localPosition.x + chunk.LayerDimension.x * (i % dimensionX)
                , chunk.transform.localPosition.y
                , chunk.transform.localPosition.z + chunk.LayerDimension.z * (i / dimensionX)
             );

#if UNITY_EDITOR
            chunk.name = $"Chunk_{i % dimensionX},{i / dimensionX}";
#endif
        }
    }

    private void OnSpawnChunkLayer(in Transform chunk, in BlockType layerType, in Vector3Int layerDimension, in Vector3 blockDimension, in int layerOffsetY)
    {
        var layer = Instantiate(emptyGameObject, chunk);
#if UNITY_EDITOR
        layer.name = $"Layer_{layerType}";
#endif
        // fill block on layer
        var layerDime = layerDimension.x * layerDimension.z;
        for (int i = 0; i < layerDime; ++i)
        {
            var offsetX = blockDimension.x * (i % layerDimension.x);
            var offsetZ = blockDimension.z * (i / layerDimension.x);

            OnSpawnBlockIntolayer(layer.transform, layerType, offsetX, offsetZ);
        }

        // set layer position
        var layerLocalPos = layer.transform.localPosition;
        layer.transform.localPosition = new Vector3(
            layerLocalPos.x
            , layerLocalPos.y + layerDimension.y * layerOffsetY
            , layerLocalPos.z
            );
    }

    private void OnSpawnBlockIntolayer(in Transform layer, in BlockType layerType, in float offsetX, in float offsetZ)
    {
        if (blocksPool.ContainsKey(layerType))
        {
            var block = blocksPool[layerType].Get();
            block.transform.parent = layer.transform;
            block.transform.localPosition = new Vector3(
                block.transform.localPosition.x + offsetX
                , block.transform.localPosition.y
                , block.transform.localPosition.z + offsetZ
            );

            BlocksSpawned.Add(block);
        }
        else
        {
            Debug.LogError($"Block Type: {layerType} don't config check NOW !");
        }
    }
}

public class SpawnGridPlatformXZ
{
    public (float x, float z) beginPosition { get; set; } = (0, 0);
    public (float x, float z) endPosition { get; set; } = (0, 0);
    public (float x, float z) cellSize { get; set; } = (0, 0);
    //public float beginPosX { get; set; } = 0;
    //public float beginPosZ { get; set; } = 0;
    //public float endPosX { get; set; } = 0;
    //public float endPosZ { get; set; } = 0;
    //public float cellSizeX { get; set; } = 0;
    //public float cellSizeZ { get; set; } = 0;
}
