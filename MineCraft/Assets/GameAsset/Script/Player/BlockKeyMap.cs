using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource/KeyMapConfig")]
public class BlockKeyMap : ScriptableObject
{
    [SerializeField]
    [Tooltip("BlockType None for random")]
    private KeyMapInfo[] keysMap;
}

[System.Serializable]
public class KeyMapInfo
{
    public BlockType blockType;
    public KeyCode KeyCode;
}
