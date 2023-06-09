﻿using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private SpawnController spawnController;
    [SerializeField]
    private GamePlayUIController blockSelectUIController;
    [SerializeField]
    private Vector3 worldBlockDimension = Vector3.one;

    public Vector3 WorldBlockDimension => worldBlockDimension;
    public InputManager InputManager => inputManager;

    protected override void OnInit()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        if (inputManager == null)
        {
            inputManager = gameObject.GetComponent<InputManager>();
        }

        if (blockSelectUIController == null)
        {
            blockSelectUIController = gameObject.GetComponent<GamePlayUIController>();
        }

        if (spawnController == null)
        {
            spawnController = FindAnyObjectByType<SpawnController>();
        }

        player.InputManager = inputManager;

    }


    // Update is called once per frame
    private void Update()
    {
        OnLeftMouseClick();
        OnRightMouseClick();
        OnKey1Up();
        OnKey2Up();
        OnKey3Up();
        OnKey4Up();
    }

    #region Input Handle
    private void OnLeftMouseClick()
    {
        if (inputManager.LeftMouseClick)
        {
            Debug.Log("PlayerBehaviour OnLeftMouseClick");
        }
    }

    private void OnRightMouseClick()
    {
        if (inputManager.RightMouseClick)
        {
            Debug.Log("PlayerBehaviour OnRightMouseClick");
            spawnController.OnSpawnBlockRandomPosition();
        }
    }


    // grass
    private void OnKey1Up()
    {
        if (inputManager.Key1Up)
        {
            Debug.Log("PlayerBehaviour OnKey1Up");
            spawnController.CurrentBlockType = BlockType.Grass;
            blockSelectUIController.OnBlockSelectChange(BlockType.Grass);
        }
    }

    // sand
    private void OnKey2Up()
    {
        if (inputManager.Key2Up)
        {
            Debug.Log("PlayerBehaviour OnKey2Up");
            spawnController.CurrentBlockType = BlockType.Sand;
            blockSelectUIController.OnBlockSelectChange(BlockType.Sand);
        }
    }

    //stone
    private void OnKey3Up()
    {
        if (inputManager.Key3Up)
        {
            Debug.Log("PlayerBehaviour OnKey3Up");
            spawnController.CurrentBlockType = BlockType.Stone;
            blockSelectUIController.OnBlockSelectChange(BlockType.Stone);
        }
    }

    // random
    private void OnKey4Up()
    {
        if (inputManager.Key4Up)
        {
            Debug.Log("PlayerBehaviour OnKey4Down");
            spawnController.CurrentBlockType = BlockType.None;
            blockSelectUIController.OnBlockSelectChange(BlockType.None);
        }
    }

    public void OnBreakBlock(Block block)
    {
        spawnController.OnBreakBlock(block);
    }
    #endregion //Input Handle
}
