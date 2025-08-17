using System;
using System.Collections;
using System.Collections.Generic;
using EnemyStates;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private List<Transform> secondFloor = new List<Transform>();
    [SerializeField] private List<Transform> firstFloor = new List<Transform>();
    public float ViewDistance = 10f;
    public float ViewAngle = 120f;


    private IState<EnemyController, EnemyState>[] states;
    private StateMachine<EnemyController, EnemyState> stateMachine;
    private EnemyState currentState;

    public NavMeshAgent    Agent           { get; private set; }
    public Transform       PlayerTransform { get; private set; }
    public Animator        Animator        { get; private set; }
    public List<Transform> SecondFloor     => secondFloor;
    public List<Transform> FirstFloor      => firstFloor;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        stateMachine = new StateMachine<EnemyController, EnemyState>();
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        PlayerTransform = PlayerController.Instance.transform;
        SetupState();
    }

    private void Update()
    {
        TryStateTransition();
        stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
    }

    private void SetupState()
    {
        states = new IState<EnemyController, EnemyState>[Enum.GetValues(typeof(EnemyState)).Length];
        for (int i = 0; i < states.Length; i++)
        {
            states[i] = GetState((EnemyState)i);
        }

        stateMachine = new StateMachine<EnemyController, EnemyState>();
        stateMachine.Setup(this, states[(int)PlayerState.Idle]);
    }

    private IState<EnemyController, EnemyState> GetState(EnemyState state)
    {
        return state switch
        {
            EnemyState.Idle   => new IdleState(),
            EnemyState.Patrol => new PatrolState(),
            EnemyState.Chase  => new ChaseState(),
            _                 => null
        };
    }

    private void ChangeState(EnemyState newState)
    {
        stateMachine.ChangeState(states[(int)newState]);
        currentState = newState;
    }

    private void TryStateTransition()
    {
        EnemyState? next = states[(int)currentState].CheckTransition(this);
        if (next.HasValue && next.Value != currentState)
        {
            ChangeState(next.Value);
        }
    }

    public bool IsPlayerInSight(out bool isPlayerLookingAtMe)
    {
        Vector3 toPlayer = PlayerTransform.position - transform.position;
        float   dist     = toPlayer.magnitude;
        float   angle    = Vector3.Angle(transform.forward, toPlayer.normalized);
        isPlayerLookingAtMe = Vector3.Angle(PlayerTransform.forward, -toPlayer.normalized) < 60f;

        if (dist < ViewDistance && angle < ViewAngle * 0.5f)
        {
            return true;
        }

        return false;
    }
}