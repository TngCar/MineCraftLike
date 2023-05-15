using System.Collections.Generic;
using UnityEngine;


public class ChunkLayer : MonoBehaviour
{
    private List<Vector3> bloksPosition;
    // Use this for initialization
    private void Awake()
    {
        bloksPosition = new(16);
        foreach (Transform child in transform)
        {
            bloksPosition.Add(child.position);
        }
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
