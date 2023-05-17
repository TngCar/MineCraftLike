using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnController : MonoBehaviour
{
	[SerializeField]
	private Block[] blockTemplates;
	[SerializeField]
	private ChunkConfig chunkConfig;

	private GameObject emptyGameObject;
	private GameObject chunkSpawnedContainer;
	private SpawnGridPlatformXZ spawnGridXZ = new();

	private Dictionary<BlockType, ObjectPool<Block>> blocksPool;

	/// <summary>
	/// please use the blocksToSpawn to sapwn on pool
	/// </summary>
	private Dictionary<BlockType, Block> blocksToSpawn;
	public BlockType CurrentBlockType { get; set; } = BlockType.None;
	public List<Block> BlocksSpawned { get; private set; }

	public Vector3 WorldBlockDimesion { get; set; } = Vector3.one;

	private void Awake()
	{
		OnInit();
	}

	private  void OnInit()
	{
#if UNITY_EDITOR
		if (blockTemplates.Length <= 0 ||
			blockTemplates.Length != blockTemplates.GroupBy(x => x.BlockType).Select(y => y.First()).Count())
		{
			Debug.LogError("SpawnController: blocksTemplate is empty or duplicate item");
			return;
		}
#endif
		WorldBlockDimesion = GameManager.Instance.WorldBlockDimension;
		BlocksSpawned = new();
		emptyGameObject = new GameObject();
		chunkSpawnedContainer = new GameObject();
#if UNITY_EDITOR
		chunkSpawnedContainer.name = nameof(chunkSpawnedContainer);
#endif
		InitBlocksPool();
		OnSpawnChunk();

		var gridBaseXZ = chunkSpawnedContainer.GetComponentsInChildren<Block>()
			.GroupBy(x => x.transform.position.y)
			.First()
			.Select(x => x.transform.position);

		if (gridBaseXZ.Any())
		{
			spawnGridXZ = new(gridBaseXZ, WorldBlockDimesion);
		}
		else
		{
			Debug.LogWarning("NO block Spawn on the Game ");
		}
	}

	private void InitBlocksPool()
	{
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
	}

	/// <summary>
	/// BlockType.None is for Random type
	/// </summary>
	/// <param name="spawnPos"></param>
	/// <param name="blockType"></param>
	public void OnSpawnBlock(Vector3 spawnPos, BlockType blockType = BlockType.None, bool? isKinematic = null)
	{
		if (blockType == BlockType.None)
		{
			blockType = (BlockType)Random.Range((int)BlockType.None + 1, (int)BlockType.Count);
		}

		var block = blocksPool[blockType].Get();
		block.Rigid.isKinematic = isKinematic ?? Random.value > 0.5f;
		block.transform.position = spawnPos;
	}

	/// <summary>
	/// for random position
	/// </summary>
	/// <param name="blockType"></param>
	public void OnSpawnBlockRandomPosition()
	{
		var spawnX = Random.Range(spawnGridXZ.BeginPosition.x, spawnGridXZ.EndPosition.x);
		var fixedX = spawnGridXZ.BeginPosition.x
			+ WorldBlockDimesion.x * Mathf.CeilToInt((spawnX - spawnGridXZ.BeginPosition.x) / WorldBlockDimesion.x);

		var spawnZ = Random.Range(spawnGridXZ.BeginPosition.z, spawnGridXZ.EndPosition.z);
		var fixedZ = spawnGridXZ.BeginPosition.z
			+ WorldBlockDimesion.z * Mathf.CeilToInt((spawnZ - spawnGridXZ.BeginPosition.z) / WorldBlockDimesion.z);

		OnSpawnBlock(new Vector3(fixedX, 2, fixedZ), CurrentBlockType);
	}

	public void OnBlockBroken(Block block)
	{
		// release pool instead of Destroy Object
		blocksPool[block.BlockType].Release(block);
	}

	private void OnSpawnChunk()
	{
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
					OnSpawnChunkLayer(chunk.transform, chunk.LayerTypes[j], chunk.LayerDimension, WorldBlockDimesion, layerOffsetY);
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
	public (float x, float z) BeginPosition { get; set; } = (0, 0);
	public (float x, float z) EndPosition { get; set; } = (0, 0);
	public (float x, float z) CellSize { get; set; } = (1, 1);

	public SpawnGridPlatformXZ() { }
	public SpawnGridPlatformXZ(IEnumerable<Vector3> dataIput, Vector3 cellDimestion)
	{
		if (dataIput == null || !dataIput.Any())
			return;
		
		Vector3 minPos = dataIput.First();
		Vector3 maxPos = dataIput.First();

		foreach (var pos in dataIput)
		{
			if (pos.x < minPos.x)
			{
				minPos = pos;
			}
			else if (pos.x == minPos.x)
			{
				if (pos.z < minPos.z)
				{
					minPos = pos;
				}
			}

			if (pos.x > maxPos.x)
			{
				maxPos = pos;
			}
			else if (pos.x == maxPos.x)
			{
				if (pos.z > maxPos.z)
				{
					maxPos = pos;
				}
			}
		}

		Debug.Log($"minPos: {minPos}, maxPos: {maxPos}");

		BeginPosition = (minPos.x, minPos.z);
		EndPosition = (maxPos.x, maxPos.z);
		CellSize = (cellDimestion.x, cellDimestion.z);
	}
}
