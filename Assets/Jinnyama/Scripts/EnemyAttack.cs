using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyAttack : MonoBehaviour
{
    //[SerializeField] private GameObject gameOverUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //gameOverUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene("GameOver_Scene");
        }
    }
}
