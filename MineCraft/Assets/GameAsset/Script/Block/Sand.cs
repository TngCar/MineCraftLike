using UnityEngine;


public sealed class Sand : Block
{
    protected override void OnInit()
    {
        if (type != BlockType.Sand)
        {
            Debug.LogError($"Set type for Grass incorrect. curent type is {type}, please set {BlockType.Sand}");
        }
    }
}
