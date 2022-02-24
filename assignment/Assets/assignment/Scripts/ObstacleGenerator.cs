using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField]
    private ObstaclePosSO _obstaclePosData;

    [SerializeField]
    private GameObject _sphereObject;

    
    void Start()
    {
        ReadDataFromScriptableObjects();
    }

    void ReadDataFromScriptableObjects()
    {
        for (int i = 0; i < _obstaclePosData.obstaclePos.Length; i++)
        {

            int x = i / 10;
            int z = i % 10;
            if (_obstaclePosData.obstaclePos[i])
            {
                GameObject newObstacle = Instantiate(_sphereObject, new Vector3(x, -0.25f, z), Quaternion.identity, transform);
            }
        }
    }
}
