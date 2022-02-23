using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObsPosSO", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class ObstaclePosSO : ScriptableObject { 
    public bool[] obstaclePos= new bool[100];
}
