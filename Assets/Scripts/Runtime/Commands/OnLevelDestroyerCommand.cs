using Runtime.Interfaces;
using Runtime.Signals;
using UnityEngine;

public class OnLevelDestroyerCommand : ICommand
{
    private readonly Transform _levelHolder;

    internal OnLevelDestroyerCommand(Transform levelHolder)
    {
        _levelHolder = levelHolder;
    }

    public void Execute()
    {
        // TODO: Implement not destroying the level prefab, just delete the data.
    }
}