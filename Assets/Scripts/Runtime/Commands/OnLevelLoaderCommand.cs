using Runtime.Interfaces;
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
        // Object.Instantiate(Resources.Load<GameObject>($"Prefabs/LevelPrefabs/level {levelIndex}"), _levelHolder,
        //     true);
        
        Debug.Log($"Level {levelIndex + 1} loaded.");
        
        //CoreGameSignals.OnGridInitialize.Invoke(CoreGameSignals.OnGetLevelData.Invoke(levelIndex).cells, _levelHolder);
    }
}