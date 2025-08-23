using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartBotton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Main_Scene");
        });
        
    }

    
}
