using System;
using PlayerStates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(StatManager))]
[RequireComponent(typeof(FlashlightController))]
public class PlayerController : SceneOnlySingleton<PlayerController>
{
    [Header("Look")]
    [SerializeField] Transform cameraContainer;

    [SerializeField] private float minXLook;        // 최소 시야각
    [SerializeField] private float maxXLook;        // 최대 시야각
    [SerializeField] private float lookSensitivity; // 카메라 민감도
    private float camCurXRot;

    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private float estimatedSpeed;

    public Animator                  Animator                  { get; private set; }
    public FlashlightController      FlashlightController      { get; private set; }
    public LightControlledVisibility LightControlledVisibility { get; private set; }
    public GrabObjectController      GrabObjectController      { get; private set; }
    public InteractionController     InteractionController     { get; private set; }

    private Rigidbody rigid;
    private IState<PlayerController, PlayerState>[] states;
    private StateMachine<PlayerController, PlayerState> stateMachine;
    private PlayerState currentState;
    private GameManager gameManager;


    public StatManager PlayerStat { get; private set; }

    private void Awake()
    {
        FlashlightController = GetComponent<FlashlightController>();
        LightControlledVisibility = GetComponent<LightControlledVisibility>();
        GrabObjectController = GetComponent<GrabObjectController>();
        InteractionController = GetComponent<InteractionController>();

        stateMachine = new StateMachine<PlayerController, PlayerState>();
        PlayerStat = GetComponent<StatManager>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        SetupState();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        TryStateTransition();
        stateMachine?.Update();

        UpdateHeartRate();
    }

    private void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    private void SetupState()
    {
        states = new IState<PlayerController, PlayerState>[Enum.GetValues(typeof(PlayerState)).Length];
        for (int i = 0; i < states.Length; i++)
        {
            states[i] = GetState((PlayerState)i);
        }

        stateMachine = new StateMachine<PlayerController, PlayerState>();
        stateMachine.Setup(this, states[(int)PlayerState.Idle]);
    }

    private IState<PlayerController, PlayerState> GetState(PlayerState state)
    {
        return state switch
        {
            PlayerState.Idle => new IdleState(),
            PlayerState.Move => new MoveState(),
            _                => null
        };
    }


    private void ChangeState(PlayerState newState)
    {
        stateMachine.ChangeState(states[(int)newState]);
        currentState = newState;
    }

    private void TryStateTransition()
    {
        PlayerState? next = states[(int)currentState].CheckTransition(this);
        if (next.HasValue && next.Value != currentState)
        {
            ChangeState(next.Value);
        }
    }

    public void Movement(bool isSprinting)
    {
        Vector2 moveInput = gameManager.InputHandler.MovementInput;
        Vector3 moveDir   = transform.forward * moveInput.y + transform.right * moveInput.x;
        moveDir.y = 0f; // y축 고정

        float   moveSpd       = PlayerStat.GetValue(StatType.MoveSpeed) * (isSprinting ? 1.5f : 1f);
        Vector3 deltaPosition = moveDir.normalized * moveSpd * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + deltaPosition);
    }

    private void CameraLook()
    {
        // 수직 회전은 카메라에 적용
        if (UIManager.Instance.IsOpenUI)
            return;
        Vector2 mouseDelta = gameManager.InputHandler.MouseDelta;
        camCurXRot -= mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(camCurXRot, 0f, 0f);

        // 수평 회전은 캐릭터에 적용
        transform.Rotate(Vector3.up * mouseDelta.x * lookSensitivity);
    }

    private float GetNearestEnemyDistance()
    {
        float      nearest = Mathf.Infinity;
        Collider[] hits    = Physics.OverlapSphere(transform.position, 25f, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            float d = Vector3.Distance(transform.position, hit.transform.position);
            if (d < nearest)
                nearest = d;
        }

        return nearest;
    }

    /* 이동속도 기반 보정 제거
    private void EstimateSpeed()
    {
        currentPosition = transform.position;
        Vector3 delta = currentPosition - lastPosition;
        estimatedSpeed = delta.magnitude / Time.deltaTime;
        lastPosition = currentPosition;
    }
    */

    private void UpdateHeartRate()
    {
        /* 이동속도 기반 보정 제거
        Vector3 horizontalVelocity = rigid.velocity;
        horizontalVelocity.y = 0f;
        float currentSpeed = horizontalVelocity.magnitude;

        // 이동속도 기반 보정
        float speedBonusRatio = Mathf.Clamp01(estimatedSpeed / 7.5f);
        float speedBonus = speedBonusRatio * 100f;
        */

        // 적 거리 기반 보정
        float nearestEnemyDist = GetNearestEnemyDistance();
        float proximityRatio   = Mathf.Clamp01(1f - (nearestEnemyDist / 25f));
        float proximityBonus   = proximityRatio * 150f;

        int heartRate = Mathf.RoundToInt(50f + /*speedBonus +*/ proximityBonus);
        UIHUD.Instance.SetHeartRate(heartRate);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}