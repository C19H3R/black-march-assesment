using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;
public class PlayerController : MonoBehaviour
{
    //player Enum
    public enum PlayerState { WaitingForInput, PlayerMoving, WaitingForEnemy, CalculatingMove };

    #region InspectorVars
    //get enemy Position 
    [SerializeField]
    Transform _enemyPosition;

    //get obstacle Data 
    [SerializeField]
    ObstaclePosSO _gridObstacleData;

    [SerializeField]
    public PlayerState currentPlayerState = PlayerState.WaitingForInput;
    #endregion

    bool[,] gridInfo = new bool[10,10];
    List<Vector2> _pathToTarget=new List<Vector2>();

    #region MonoBehaviourCallbacks
    void Start()
    {
        UpdateGridObstacleData();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentPlayerState)
        {
            case PlayerState.WaitingForInput:
                {
                    GetInput();
                    break;
                }
            case PlayerState.PlayerMoving:
                {
                    break;
                }
            case PlayerState.WaitingForEnemy:
                {

                    break;
                }
            case PlayerState.CalculatingMove:
                {
                    break;
                }
            default:
                break;
        }
    }

    #endregion

    #region Coroutines
    
    IEnumerator MovePlayer()
    {
        
        for (int i = 0; i < _pathToTarget.Count; i++)
        {
            Vector3 currentPos = transform.position;
            Vector3 Gotoposition = new Vector3(_pathToTarget[i].x,transform.position.y,_pathToTarget[i].y);
            float elapsedTime = 0;
            float waitTime = 0.2f;
            while (elapsedTime < waitTime)
            {
                transform.position = Vector3.Lerp(currentPos, Gotoposition, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = Gotoposition;
        }
        currentPlayerState = PlayerState.WaitingForEnemy;

        yield return null;
    }
    #endregion

    #region Functions

    void UpdateGridObstacleData()
    {
        for (int i = 0; i < gridInfo.GetLength(0); i++)
        {


            for (int j = 0; j < gridInfo.GetLength(1); j++)
            {

                if (_gridObstacleData.obstaclePos[i * 10 + j])
                {
                    gridInfo[i, j] = true;
                }
                else
                {
                    gridInfo[i, j] = false;
                }
            }
        }

    }
    void GetInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100) && Input.GetMouseButtonDown(0))
        {
            GameObject currentTile = hit.transform.gameObject;
            Vector2 endCoords = new Vector2(currentTile.transform.position.x, currentTile.transform.position.z);
            if (currentTile.tag == "Grid Tile" && !gridInfo[(int)endCoords.x, (int)endCoords.y])
            {
                currentPlayerState = PlayerState.CalculatingMove;
                CalculateMove(endCoords);
            }
        }
    }
    void CalculateMove(Vector2 endCoords)
    {

        bool isFound = false;

        List<List<Node>> PathFindingGrid = new List<List<Node>>();

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
            currentPlayerState = PlayerState.WaitingForInput;
        }
        else
        {
            _pathToTarget = new List<Vector2>();
            while (shortestPath.Count != 0)
            {

                _pathToTarget.Add(shortestPath.Pop().Position);
            }
            currentPlayerState = PlayerState.PlayerMoving;
            StartCoroutine(MovePlayer());
        }

    }

    #endregion
}
