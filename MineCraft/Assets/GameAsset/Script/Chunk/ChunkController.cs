using UnityEngine;


public class ChunkController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("layer order up to down")]
    private BlockType[] layerTypes;
    [SerializeField]
    [Tooltip("Number of block")]
    private Vector3Int layerDimension = Vector3Int.one;
    [SerializeField]
    private Vector3  blockDimension  = Vector3Int.one;

    public BlockType[] LayerTypes => layerTypes;
    public Vector3Int LayerDimension => layerDimension;
    public Vector3 BlockDimension => blockDimension;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

[System.Serializable]
public class ChunkConfig
{
    public ChunkController chunkTemplate;
    [Tooltip("dimention amount chunk fill. xy <=> xz of vector3")]
    public Vector2Int dimension;
}
