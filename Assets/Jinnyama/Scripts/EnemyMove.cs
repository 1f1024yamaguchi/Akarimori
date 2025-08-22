using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private FirstPersonController _playerController;
    private NavMeshAgent _agent;
    [Header("速度上昇設定")]
    [SerializeField] private float speedUpInterval = 10f; //速度が上がる間隔
    [SerializeField] private float speedIncrement = 0.5f; //一度に増幅する速度
    private float timer; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>(); //navmeshagentを保持しておく
        timer = 0f;//タイマー初期化
        
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        if(timer >speedUpInterval)
        {
            _agent.speed += speedIncrement;
            Debug.Log("速度上昇" + _agent.speed);
            timer = 0f; //タイマーリセット
        }
        _agent.destination = _playerController.transform.position;
        //プレイヤーを目指して進む
        
    }
}
