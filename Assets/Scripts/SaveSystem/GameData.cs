using System;
using UnityEngine;

[Serializable]
public class GameData
{
    public SerializableDictionary<string, bool> unlockedCheckpoints;
    public bool hasCheckpoint;
    public Vector3 lastCheckpointPosition;

    public GameData()
    {
        unlockedCheckpoints = new SerializableDictionary<string, bool>();
        hasCheckpoint = false;
        lastCheckpointPosition = Vector3.zero;
    }
}
