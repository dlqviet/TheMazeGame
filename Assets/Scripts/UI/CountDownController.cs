using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownController : MonoBehaviour
{
    public int countDownTime;
    public Text display;

    private MazeRunnerManager mazeRunnerManager;

    private void Start()
    {
        StartCoroutine(StartAfterCountDown());
        mazeRunnerManager = MazeRunnerManager.Instance;
    }

    IEnumerator StartAfterCountDown()
    {
        while (countDownTime > 0)
        {
            display.text = countDownTime.ToString();

            yield return new WaitForSeconds(1f);

            countDownTime--;
        }

        display.text = "GO!";

        yield return new WaitForSeconds(1f);

        display.gameObject.SetActive(false);

        mazeRunnerManager.GameStart();
    }
}
