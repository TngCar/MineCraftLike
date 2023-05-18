using UnityEngine;

public enum BlockType
{
    None,
    Grass,
    Sand,
    Stone,
    // add new Item above this line

    // Hide on Inspector
    [InspectorName(null)]
    Count
}