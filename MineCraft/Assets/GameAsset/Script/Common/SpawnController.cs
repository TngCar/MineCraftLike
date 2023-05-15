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

	private Dictionary<BlockType, ObjectPool<Block>> blocksPool;

	/// <summary>
	/// please use the blocksToSpawn to sapwn on pool
	/// </summary>
	private Dictionary<BlockType, Block> blocksToSpawn;
	public BlockType currentBlockType { get; set; } = BlockType.None;
	public List<Block> BlocksSpawned { get; private set; }

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
		emptyGameObject = new GameObject();
		chunkSpawnedContainer = new GameObject();
#if UNITY_EDITOR
		chunkSpawnedContainer.name = nameof(chunkSpawnedContainer);
#endif
		BlocksSpawned = new();

		blocksToSpawn = blockTemplates.ToDictionary(x => x.BlockType, x => x);

		blocksPool = new();
		foreach (var b in blocksToSpawn)
		{
			blocksPool[b.Key] = new ObjectPool<Block>(
				() => Instantiate(b.Value)
				, b => b.gameObject.SetActive(true)
				, b => b.gameObject.SetActive(false));
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

	public void OnBlockBroken(Block block)
	{
		// release pool instead of Destroy Object
		blocksPool[block.BlockType].Release(block);
	}

	private void OnSpawnChunk()
	{
		int dimensionX = chunkConfig.dimension.x;

		for (int i = 0; i < chunkConfig.dimension.x * chunkConfig.dimension.y; i++)
		{
			var chunk = Instantiate(chunkConfig.chunkTemplate, chunkSpawnedContainer.transform);
			for (int j = 0; j < chunk.LayerTypes.Length; j++)
			{
				// offset for up to down
				var layerOffsetY = chunk.LayerTypes.Length - 1 - j;
				OnSpawnChunkLayer(chunk.transform, chunk.LayerTypes[j], chunk.LayerDimension, chunk.BlockDimension, layerOffsetY);
			}

			// set chunk position
			chunk.transform.localPosition = new Vector3(
				 chunk.transform.localPosition.x + chunk.LayerDimension.x * (i % dimensionX)
				, chunk.transform.localPosition.y
				, chunk.transform.localPosition.z + chunk.LayerDimension.z * (i / dimensionX)
			 );
		}
	}

	private void OnSpawnChunkLayer(in Transform chunk, in BlockType layerType, in Vector3Int layerDimension, in Vector3 blockDimension, in int layerOffsetY)
	{
		var layer = Instantiate(emptyGameObject, chunk);
#if UNITY_EDITOR
		layer.name = $"Layer_{layerType}";
#endif
		// fill block on layer
		for (int i = 0; i < layerDimension.x * layerDimension.z; i++)
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
