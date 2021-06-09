using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class MazeRunnerManager : SingletonPersistent<MazeRunnerManager>
{
    public GameObject mazeRunnerPrefab;

    private MazeGenerator mazeGenerator;

    private void Start()
    {
        mazeGenerator = MazeGenerator.Instance;
        mazeRunnerPrefab.transform.position = mazeGenerator.startLocation;
    }

    public void GameStart()
    {
        mazeRunnerPrefab.SetActive(true);
    }
}
