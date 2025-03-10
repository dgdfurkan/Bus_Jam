﻿using Runtime.Interfaces;
using Runtime.Signals;
using UnityEngine;

public class OnLevelLoaderCommand : ICommand
{
    private readonly Transform _levelHolder;

    internal OnLevelLoaderCommand(Transform levelHolder)
    {
        _levelHolder = levelHolder;
    }

    public void Execute(int levelIndex)
    {
        // TODO: Implement including level data to the level prefab, so no need to instantiate prefab. Just load the data.
        
        Debug.Log($"Level {levelIndex + 1} loaded.");
        CoreGameSignals.Instance.OnLoadLevelInitialize.Invoke(levelIndex);
        
    }
}