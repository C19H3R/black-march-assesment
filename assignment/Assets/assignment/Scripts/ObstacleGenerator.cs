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
        for (int i = 0;  i < _obstaclePosData.obstaclePos.Length;  i++)
        {

            int x = i / 10;
            int z = i%10;
            if (_obstaclePosData.obstaclePos[i])
            {
                GameObject newObstacle = Instantiate(_sphereObject, new Vector3(x, 1, z), Quaternion.identity, transform);
            }
        }
    }


    void Update()
    {
        
    }
}
