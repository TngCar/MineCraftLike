using UnityEngine;

public enum BlockType
{
    None,
    Sand,
    Stone,
    Grass,
    // add new Item above this line

    // Hide on Inspector
    [InspectorName(null)]
    Count
}