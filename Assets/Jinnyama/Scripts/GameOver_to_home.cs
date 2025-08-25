using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOver_to_home : MonoBehaviour
{

    public float delayInSeconds = 5.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        //指定された時間だけ待つ
        yield return new WaitForSeconds(delayInSeconds);

        SceneManager.LoadScene("Start_Scene");

    }
}
