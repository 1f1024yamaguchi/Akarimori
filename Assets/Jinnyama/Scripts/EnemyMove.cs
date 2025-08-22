using UnityEngine;
using UnityEngine.AI;
using System.Collections;
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private FirstPersonController _playerController;
    private NavMeshAgent _agent;
    [Header("速度上昇設定")]
    [SerializeField] private float speedUpInterval = 10f; //速度が上がる間隔
    [SerializeField] private float speedIncrement = 0.5f; //一度に増幅する速度
    private float speedUpTimer; 

    private bool _isStunned = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>(); //navmeshagentを保持しておく
        speedUpTimer = 0f;//タイマー初期化
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isStunned)
        {
            HandleSpeedUp();
            HandleChasing();
        }
    }

    void HandleSpeedUp()
    {
        speedUpTimer += Time.deltaTime;
        if(speedUpTimer > speedUpInterval)
        {
            _agent.speed += speedIncrement;
            Debug.Log("速度上昇" + _agent.speed);
            speedUpTimer = 0f; //タイマーリセット
        }
    }

    void HandleChasing()
    {
        _agent.destination = _playerController.transform.position;
        //プレイヤーを目指して進む
    }

    //以下ひるみ機能
    public void Stun(float duration)
    {
        if(!_isStunned)
        {
            StartCoroutine(StunCoroutine(duration));
        }
    }

    private IEnumerator StunCoroutine(float duration)
    {
        _isStunned = true; //ひるみ状態にする
        _agent.isStopped = true; //NavMeshAgentの動きを止める
        Debug.Log("敵がひるんだ");

        yield return new WaitForSeconds(duration);

        //怯み状態から戻る
        _agent.isStopped = false; //NavMeshAgentの動きを再開
        _isStunned = false;
        Debug.Log("我に返った");
    }

        
        
        
    
}
