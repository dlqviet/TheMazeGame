using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject mainCamera;

    private MazeGenerator mazeGenerator;
    private Vector3 average = new Vector3(0, 0, 0);
    private float cameraHeight = 0;

    private void Start()
    {
        mazeGenerator = MazeGenerator.Instance;

        GameObject[] floorList = new GameObject[mazeGenerator.mazeRow * mazeGenerator.mazeColumn];

        floorList = GameObject.FindGameObjectsWithTag("Floor");

        foreach (GameObject f in floorList)
        {
            average += f.transform.position;
        }

        average /= (mazeGenerator.mazeRow * mazeGenerator.mazeColumn);

        if (mazeGenerator.mazeRow >= mazeGenerator.mazeColumn)
        {
            cameraHeight = mazeGenerator.mazeRow * mazeGenerator.cellSize * 5 / 4;
        }
        else
        {
            cameraHeight = mazeGenerator.mazeColumn * mazeGenerator.cellSize * 7 / 12;
        }

        average.y = cameraHeight;

        mainCamera.transform.position = average;
    }
}
