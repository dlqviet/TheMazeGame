using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-1)]
public class MazeGenerator : SingletonPersistent<MazeGenerator>
{
    public NavMeshSurface surface;

    public int mazeRow;
    public int mazeColumn;
    public GameObject destination;

    [HideInInspector]
    public float cellSize = 3f;
    [HideInInspector]
    public Vector3 startLocation;
    [HideInInspector]
    public Vector3 endLocation;

    [SerializeField]
    private FloorPool floorPool;
    [SerializeField]
    private WallPool wallPool;

    private CellAttributes[,] cellArray;
    private int curRow = 0;
    private int curColumn = 0;
    private bool complete = false;

    void Start()
    {
        // create maze grid
        InitializeTheMaze();
        // create maze path with "hunt and kill" algorithm
        BuildTheMaze();
    }

    public void InitializeTheMaze()
    {
        cellArray = new CellAttributes[mazeRow, mazeColumn];

        for (int r = 0; r < mazeRow; r++)
        {
            for (int c = 0; c < mazeColumn; c++)
            {
                cellArray[r, c] = new CellAttributes();
                cellArray[r, c].cellFloor = floorPool.GetFloor();
                cellArray[r, c].cellFloor.transform.position = new Vector3(r * cellSize, - (cellSize / 3), c * cellSize);


                if (c == 0)
                {
                    cellArray[r, c].leftWall = wallPool.GetWall();
                    cellArray[r, c].leftWall.transform.position = new Vector3(r * cellSize, 0, (c * cellSize) - (cellSize / 2));
                }

                cellArray[r, c].rightWall = wallPool.GetWall();
                cellArray[r, c].rightWall.transform.position = new Vector3(r * cellSize, 0, (c * cellSize) + (cellSize / 2));

                if (r == 0)
                {
                    cellArray[r, c].topWall = wallPool.GetWall();
                    cellArray[r, c].topWall.transform.position = new Vector3((r * cellSize) - (cellSize / 2), 0, c * cellSize);
                    cellArray[r, c].topWall.transform.Rotate(Vector3.up, 90f);
                }

                cellArray[r, c].bottomWall = wallPool.GetWall();
                cellArray[r, c].bottomWall.transform.position = new Vector3((r * cellSize) + (cellSize / 2), 0, c * cellSize);
                cellArray[r, c].bottomWall.transform.Rotate(Vector3.up, 90f);
            }
        }
    }

    private void BuildTheMaze()
    {
        cellArray[curRow, curColumn].visited = true;
        DigTheWayOut();
        while (!complete)
        {
            CreateFalseWays();
        }
        surface.BuildNavMesh();

        destination.SetActive(true);

        Debug.Log("START: " + startLocation);
        Debug.Log("END: " + endLocation);
    }

    private void DigTheWayOut()
    {
        if (curRow == 0 && curColumn == 0)
        {
            startLocation = cellArray[curRow, curColumn].cellFloor.transform.position;
            startLocation.y = 0f;
        }
        while (CheckNextUnvisitedCell())
        {
            // randomly dig to a direction
            int digDirection = Random.Range(0, 4);

            // 0 = dig up
            if (digDirection == 0)
            {
                if (CheckThisCell(curRow - 1, curColumn))
                {
                    if (cellArray[curRow, curColumn].topWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].topWall);
                    }

                    curRow--;
                    cellArray[curRow, curColumn].visited = true;

                    if (cellArray[curRow, curColumn].bottomWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].bottomWall);
                    }
                }
            }

            // 1 = dig down
            else if (digDirection == 1)
            {
                if (CheckThisCell(curRow + 1, curColumn))
                {
                    if (cellArray[curRow, curColumn].bottomWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].bottomWall);
                    }

                    curRow++;
                    cellArray[curRow, curColumn].visited = true;

                    if (cellArray[curRow, curColumn].topWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].topWall);
                    }
                }
            }

            // 2 = dig left
            else if (digDirection == 2)
            {
                if (CheckThisCell(curRow, curColumn - 1))
                {
                    if (cellArray[curRow, curColumn].leftWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].leftWall);
                    }

                    curColumn--;
                    cellArray[curRow, curColumn].visited = true;

                    if (cellArray[curRow, curColumn].rightWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].rightWall);
                    }
                }
            }

            // 3 = dig right
            else if (digDirection == 3)
            {
                if (CheckThisCell(curRow, curColumn + 1))
                {
                    if (cellArray[curRow, curColumn].rightWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].rightWall);
                    }

                    curColumn++;
                    cellArray[curRow, curColumn].visited = true;

                    if (cellArray[curRow, curColumn].leftWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].leftWall);
                    }
                }
            }

            if (!CheckNextUnvisitedCell())
            {
                endLocation = cellArray[curRow, curColumn].cellFloor.transform.position;
                endLocation.y = 0.1f;

                destination.transform.position = endLocation;
            }
        }
    }

    private void CreateFalseWays()
    {
        complete = true;

        for (int r = 0; r < mazeRow; r++)
        {
            for (int c = 0; c < mazeColumn; c++)
            {
                if (!cellArray[r, c].visited && CheckNextVisitedCell(r, c))
                {
                    complete = false;
                    curRow = r;
                    curColumn = c;
                    cellArray[curRow, curColumn].visited = true;
                    BreakARandomWall();
                    return;
                }
            }
        }
    }


    /* SUPPORT FUNCTIONS */
    // check if the adjacent cell is unvisited or not
    bool CheckNextUnvisitedCell()
    {
        // check the top cell
        if (CheckThisCell(curRow - 1, curColumn))
        {
            return true;
        }

        // check the bottom cell
        if (CheckThisCell(curRow + 1, curColumn))
        {
            return true;
        }

        // check the left cell
        if (CheckThisCell(curRow, curColumn - 1))
        {
            return true;
        }

        // check the right cell
        if (CheckThisCell(curRow, curColumn + 1))
        {
            return true;
        }

        return false;
    }

    // check if the adjacent cell is visited or not
    bool CheckNextVisitedCell(int thisRow, int thisColumn)
    {
        if (thisRow > 0 && cellArray[thisRow - 1, thisColumn].visited)
        {
            return true;
        }
        if (thisRow < mazeRow - 1 && cellArray[thisRow + 1, thisColumn].visited)
        {
            return true;
        }
        if (thisColumn > 0 && cellArray[thisRow, thisColumn - 1].visited)
        {
            return true;
        }
        if (thisColumn < mazeColumn - 1 && cellArray[thisRow, thisColumn + 1].visited)
        {
            return true;
        }
        return false;
    }

    // check if a cell is visited or out of the maze map
    bool CheckThisCell(int thisRow, int thisColumn)
    {
        if (thisRow >= 0 && thisRow < mazeRow
            && thisColumn >= 0 && thisColumn < mazeColumn
            && !cellArray[thisRow, thisColumn].visited)
        {
            return true;
        }

        return false;
    }

    // break walls to dig false paths
    void BreakARandomWall()
    {
        bool broken = false;

        while (!broken)
        {
            int wallDirection = Random.Range(0, 4);

            // 0 = break top wall
            if (wallDirection == 0)
            {
                if (curRow > 0 && cellArray[curRow - 1, curColumn].visited)
                {
                    if (cellArray[curRow, curColumn].topWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].topWall);
                    }
                    if (cellArray[curRow - 1, curColumn].bottomWall)
                    {
                        PutIntoObjectPool(cellArray[curRow - 1, curColumn].bottomWall);
                    }

                    broken = true;
                }
            }

            // 1 = break bottom wall
            else if (wallDirection == 1)
            {
                if (curRow < mazeRow - 1 && cellArray[curRow + 1, curColumn].visited)
                {
                    if (cellArray[curRow, curColumn].bottomWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].bottomWall);
                    }
                    if (cellArray[curRow + 1, curColumn].topWall)
                    {
                        PutIntoObjectPool(cellArray[curRow + 1, curColumn].topWall);
                    }
                    broken = true;
                }
            }

            // 2 = break left wall
            else if (wallDirection == 2)
            {
                if (curColumn > 0 && cellArray[curRow, curColumn - 1].visited)
                {
                    if (cellArray[curRow, curColumn].leftWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].leftWall);
                    }
                    if (cellArray[curRow, curColumn - 1].rightWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn - 1].rightWall);
                    }
                    broken = true;
                }
            }

            // 3 = break right wall
            else if (wallDirection == 3)
            {
                if (curColumn < mazeColumn - 1 && cellArray[curRow, curColumn + 1].visited)
                {
                    if (cellArray[curRow, curColumn].rightWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn].rightWall);
                    }
                    if (cellArray[curRow, curColumn + 1].leftWall)
                    {
                        PutIntoObjectPool(cellArray[curRow, curColumn + 1].leftWall);
                    }
                    broken = true;
                }
            }

        }
    }

    // put game object into object pool if not being used
    private void PutIntoObjectPool(GameObject thisGameObject)
    {
        thisGameObject.SetActive(false);
        if (wallPool != null)
        {
            wallPool.ReturnWall(thisGameObject);
        }
    }
}
