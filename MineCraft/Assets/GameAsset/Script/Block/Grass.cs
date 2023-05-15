using UnityEngine;

public class Grass : Block
{
    protected override void OnInit()
    {
        if (type != BlockType.Grass)
        {
            Debug.LogError($"Set type for Grass incorrect. curent type is {type}, please set {BlockType.Grass}");
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
