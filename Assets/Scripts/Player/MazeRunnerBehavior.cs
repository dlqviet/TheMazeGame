using UnityEngine;
using UnityEngine.AI;

public class MazeRunnerBehavior : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject spawnFX;

    private MazeGenerator mazeGenerator;
    private RestartScript restartScript;

    private void Start()
    {
        mazeGenerator = MazeGenerator.Instance;
        restartScript = RestartScript.Instance;

        Instantiate(spawnFX);
    }

    private void FixedUpdate()
    {
        agent.SetDestination(mazeGenerator.endLocation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Destination")
        {
            restartScript.ResetGame();
        }
    }
}
