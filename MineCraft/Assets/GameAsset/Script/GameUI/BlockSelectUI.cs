using UnityEngine;
using UnityEngine.UI;


public class BlockSelectUI : MonoBehaviour
{
    [SerializeField]
    private BlockType blockType;
    [SerializeField]
    private Image hilightImage;

    public BlockType BlockType => blockType;

    // Use this for initialization

    public void OnSelectItemHandle(Color color)
    {
        if (hilightImage)
        {
            hilightImage.color = color;
        }
    }
}
