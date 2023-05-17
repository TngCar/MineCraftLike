using UnityEngine;

public sealed class Stone : Block
{
    protected override void OnInit()
    {
        if (type != BlockType.Stone)
        {
            Debug.LogError($"Set type for Grass incorrect. curent type is {type}, please set {BlockType.Stone}");
        }
    }
}
