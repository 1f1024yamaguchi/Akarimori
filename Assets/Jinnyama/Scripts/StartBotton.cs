using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button), typeof(AudioSource))]
public class StartBotton : MonoBehaviour
{
    [Tooltip("クリック時に再生するサウンド")]
    public AudioClip clickSound;
    private AudioSource audioSource; //音を再生するためのコンポーネント
    private Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 1f;
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();
        button.interactable = true;

        button.onClick.AddListener(HandleClick);
        
        
    }

    private void HandleClick()
    {
        StartCoroutine(LoadSceneAfterSound());
    }

    private IEnumerator LoadSceneAfterSound()
    {
        
        button.interactable = false;

        audioSource.PlayOneShot(clickSound);

        //一秒待つ
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 1f;

        SceneManager.LoadScene("Main_Scene");
    }

    
}
