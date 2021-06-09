using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPool : MonoBehaviour
{
    [SerializeField]
    private GameObject floorPrefab;
    [SerializeField]
    private Queue<GameObject> floorPool = new Queue<GameObject>();
    [SerializeField]
    private int poolSize = 0;

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject floorPiece = Instantiate(floorPrefab);
            floorPool.Enqueue(floorPiece);
            floorPiece.SetActive(false);
        }
    }

    //take object out of pool
    public GameObject GetFloor()
    {
        if (floorPool.Count > 0)
        {
            GameObject floorPiece = floorPool.Dequeue();
            floorPiece.SetActive(true);
            return floorPiece;
        }
        else
        {
            GameObject floorPiece = Instantiate(floorPrefab);
            return floorPiece;
        }
    }

    //return object to pool
    public void ReturnFloor(GameObject floorPiece)
    {
        floorPool.Enqueue(floorPiece);
        floorPiece.SetActive(false);
    }
}
