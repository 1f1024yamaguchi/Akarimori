using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private FirstPersonController _playerController;
    private NavMeshAgent _agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>(); //navmeshagentを保持しておく
        
    }

    // Update is called once per frame
    void Update()
    {
        _agent.destination = _playerController.transform.position;
        //プレイヤーを目指して進む
        
    }
}
