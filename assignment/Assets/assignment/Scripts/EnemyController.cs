using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

public interface IMoveableAI<T>
{
    public List<T> CalculateAIMove(T endCoords);
}


public class EnemyController : MonoBehaviour, IMoveableAI<Vector2>
{

    enum EnemyState { WaitingForPlayer, PlayerMoving, EnemyMoving, CalculatingMove };

    #region InspectorVars
    //get player info
    [SerializeField]
    PlayerController _playerController;
    //getobstacle data
    [SerializeField]
    ObstaclePosSO _gridObstacleData;
    //display state in inspector
    [SerializeField]
    EnemyState _currentEnemyState = EnemyState.WaitingForPlayer;

    #endregion


    bool[,] gridInfo = new bool[10, 10];
    List<List<Vector2>> _pathToTargetList = new List<List<Vector2>>();

    List<Vector2> _pathToTarget = new List<Vector2>();



    #region Monoobehaviourcallbacks
    void Start()
    {
        UpdateGridObstacleData();
    }

    void Update()
    {
        switch (_currentEnemyState)
        {
            case EnemyState.WaitingForPlayer:
                GetPlayerState();
                break;
            case EnemyState.PlayerMoving:
                break;
            case EnemyState.EnemyMoving:
                break;
            case EnemyState.CalculatingMove:
                break;
            default:
                break;
        }
    }

    #endregion

    #region coroutines

    IEnumerator MoveEnemy()
    {
        for (int i = 0; i < _pathToTarget.Count; i++)
        {
            Vector3 currentPos = transform.position;
            Vector3 Gotoposition = new Vector3(_pathToTarget[i].x, transform.position.y, _pathToTarget[i].y);
            float elapsedTime = 0;
            float waitTime = 0.2f;
            while (elapsedTime < waitTime)
            {
                transform.position = Vector3.Lerp(currentPos, Gotoposition, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        _pathToTargetList =new List<List<Vector2>>();
        _currentEnemyState = EnemyState.WaitingForPlayer;
       _playerController.currentPlayerState = PlayerController.PlayerState.WaitingForInput;
        yield return null;
    }
    #endregion

    #region MyRegion

    void UpdateGridObstacleData()
    {
        for (int i = 0; i < gridInfo.GetLength(0); i++)
        {

            for (int j = 0; j < gridInfo.GetLength(1); j++)
            {
                if (_gridObstacleData.obstaclePos[i * 10 + j])
                {
                    //obstacle
                    gridInfo[i, j] = true;
                }
                else
                {
                    //walkable
                    gridInfo[i, j] = false;
                }
            }
        }
    }

    void GetPlayerState()
    {
        Vector2 playerUpTile = new Vector2(_playerController.transform.position.x-1, _playerController.transform.position.z); 
        Vector2 playerDownTile =new Vector2(_playerController.transform.position.x + 1, _playerController.transform.position.z);
        Vector2 playerRightTile =new Vector2(_playerController.transform.position.x , _playerController.transform.position.z+1);
        Vector2 playerLeftTile= new Vector2(_playerController.transform.position.x , _playerController.transform.position.z-1);
        bool IsCoordInGrid(Vector2 coords)
        {
            bool xRange = coords.x >= 0 && coords.x < 10;
            bool zRange = coords.y >= 0 && coords.y < 10;
            
            return xRange &&zRange;
        }
        bool playerUpWalkable = IsCoordInGrid(playerUpTile) && !gridInfo[(int)playerUpTile.x, (int)playerUpTile.y];
        bool playerDownWalkable = IsCoordInGrid(playerDownTile) && !gridInfo[(int)playerDownTile.x, (int)playerDownTile.y];
        bool playerRightWalkable = IsCoordInGrid(playerRightTile) && !gridInfo[(int)playerRightTile.x, (int)playerRightTile.y];
        bool playerleftWalkable = IsCoordInGrid(playerLeftTile) && !gridInfo[(int)playerLeftTile.x, (int)playerLeftTile.y];



        if (_playerController.currentPlayerState == PlayerController.PlayerState.WaitingForEnemy)
        {
            //check for up
            if (playerUpWalkable)
            {
                List<Vector2> path = CalculateAIMove(playerUpTile);
                if (path != null)
                {
                    _pathToTargetList.Add(path);
                }
            }
            //check for down
            if (playerDownWalkable)
            {
                List<Vector2> path = CalculateAIMove(playerDownTile);
                if (path != null)
                {
                    _pathToTargetList.Add(path);
                }
            }
            //check for left 
            if (playerleftWalkable)
            {
                List<Vector2> path = CalculateAIMove(playerLeftTile);
                if (path != null)
                {
                    _pathToTargetList.Add(path);
                }
            }
            //check for right
            if (playerRightWalkable)
            {
                List<Vector2> path = CalculateAIMove(playerLeftTile);
                if (path != null)
                {
                    _pathToTargetList.Add(path);
                }
            }

            if (_pathToTargetList.Count == 0)
            {
                _currentEnemyState = EnemyState.WaitingForPlayer;
                _playerController.currentPlayerState = PlayerController.PlayerState.WaitingForInput;
                return;
            }
            int pickRandomPath = Random.Range(0, _pathToTargetList.Count - 1);
            _pathToTarget = _pathToTargetList[pickRandomPath];
            if (_pathToTarget != null)
            {
                _currentEnemyState = EnemyState.EnemyMoving;
                StartCoroutine(MoveEnemy());
            }
            else
            {
                _currentEnemyState = EnemyState.WaitingForPlayer;
            }
        }

    }
    public List<Vector2> CalculateAIMove(Vector2 endCoords)
    {

        List<List<Node>> PathFindingGrid = new List<List<Node>>();
        List<Vector2> pathToTarget = new List<Vector2>();

        for (int i = 0; i < gridInfo.GetLength(0); i++)
        {
            List<Node> row = new List<Node>();
            for (int j = 0; j < gridInfo.GetLength(1); j++)
            {
                if (gridInfo[i, j])
                {

                    row.Add(new Node(new Vector2(i, j), false));
                }
                else
                {
                    row.Add(new Node(new Vector2(i, j), true));
                }
            }
            PathFindingGrid.Add(row);
        }
        Astar pathFinder = new Astar(PathFindingGrid);

        Node startNode = new Node(new Vector2(transform.position.x, transform.position.z), true);
        Node endNode = new Node(endCoords, true);

        Stack<Node> shortestPath = pathFinder.FindPath(startNode, endNode);

        if (shortestPath == null)
        {
            return null;
        }
        else
        {
            pathToTarget = new List<Vector2>();
            while (shortestPath.Count != 0)
            {
                pathToTarget.Add(shortestPath.Pop().Position);
            }
        }

        return pathToTarget;
    }
    #endregion

}
