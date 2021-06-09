using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPool : MonoBehaviour
{
    [SerializeField]
    private GameObject wallPrefab;
    [SerializeField]
    private Queue<GameObject> wallPool = new Queue<GameObject>();
    [SerializeField]
    private int poolSize = 0;

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject aWall = Instantiate(wallPrefab);
            wallPool.Enqueue(aWall);
            aWall.SetActive(false);
        }
    }

    //take object out of pool
    public GameObject GetWall()
    {
        if (wallPool.Count > 0)
        {
            GameObject aWall = wallPool.Dequeue();
            aWall.SetActive(true);
            return aWall;
        }
        else
        {
            GameObject aWall = Instantiate(wallPrefab);
            return aWall;
        }
    }

    //return object to pool
    public void ReturnWall(GameObject aWall)
    {
        wallPool.Enqueue(aWall);
        aWall.SetActive(false);
    }
}
