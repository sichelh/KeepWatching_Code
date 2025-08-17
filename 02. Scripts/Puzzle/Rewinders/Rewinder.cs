using System.Collections.Generic;
using UnityEngine;

public class Rewinder : MonoBehaviour
{
    // 되감기 완료 여부
    private bool rewindComplete = false;

    // 읽기 전용
    public bool IsRewindComplete => rewindComplete;

    // 위치와 회전을 함께 저장할 구조체
    private struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformData(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    // 시간순으로 저장되는 위치/회전 기록용 스택
    private Stack<TransformData> transformHistory = new Stack<TransformData>();

    // 위치 기록 간격 설정(초)
    [SerializeField] private float recordInterval = 0.05f;

    // 최대 기록 횟수
    [SerializeField] private int maxRecordCount = 300;

    // 기록 주기를 제어할 시간 누적 변수
    private float recordTimer = 0f;

    // 직전 저장된 위치/회전
    private TransformData lastRecorded;

    // 최초 위치/회전
    private TransformData initialTransform;


    // 되감기 상태 플래그
    private bool isRewinding = false;

    // 되감기 속도 계수 (1은 실시간)
    [SerializeField] private float rewindSpeed = 1.5f;

    // 현재 되감기 진행 중인 구간
    private TransformData? from, to;

    // 현재 보간 진행률 (0~1)
    private float rewindProgress = 0f;


    // 불투명도 설정
    private float currentAlpha = 0f;

    private Renderer render;
    private Material[] materials;

    private ParticleSystem ps;


    private Rigidbody rb;
    private GhostTrail ghostTrail;

    // 초기화
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        render = GetComponent<Renderer>();

        ps = GetComponent<ParticleSystem>();

        ghostTrail = GetComponent<GhostTrail>();

        if (render == null)
        {
            // 자식 중에 렌더러가 있는지 탐색
            render = GetComponentInChildren<Renderer>();
        }

        if (render != null)
        {
            // 머테리얼 복사
            Material[] originalMaterials = render.materials;
            materials = new Material[originalMaterials.Length];

            for (int i = 0; i < originalMaterials.Length; i++)
            {
                materials[i] = new Material(originalMaterials[i]);
            }

            // 새로 만든 복사본들을 렌더러에 적용
            render.materials = materials;
        }

        initialTransform = new TransformData(transform.position, transform.rotation);
        lastRecorded = initialTransform;

        // 최초 위치/회전을 스택에 미리 기록
        transformHistory.Push(initialTransform);

        // 두 번째 초기 기록 강제 추가 (처음과 같은 위치/회전)
        transformHistory.Push(initialTransform);
    }

    // CameraEffect에서 사용가능하게끔 등록
    private void OnEnable()
    {
        CameraEffect.Subscribe(this);
    }

    private void OnDisable()
    {
        CameraEffect.UnSubscribe(this);
    }

    private void FixedUpdate()
    {
        // 되감기 상태일 때
        if (isRewinding)
        {
            RewindStep();
        }

        else
        {
            // 기록 간격만큼 시간을 누적
            recordTimer += Time.deltaTime;

            if (recordTimer >= recordInterval)
            {
                recordTimer = 0f;
                RecordIfMoved();
            }
        }
    }

    // 현재 위치가 이전 기록 위치와 다를 경우에만 저장
    private void RecordIfMoved()
    {
        Vector3    currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        // 일정 거리 이상 이동, 회전했을 때만 기록
        if (Vector3.Distance(currentPosition, lastRecorded.position) > 0.01f ||
            Quaternion.Angle(currentRotation, lastRecorded.rotation) > 0.01f)
        {
            TransformData newData = new TransformData(currentPosition, currentRotation);
            transformHistory.Push(newData);
            lastRecorded = newData;

            // 스택이 너무 커졌을 때 오래된 위치를 제거 (초기 위치는 보존)
            if (transformHistory.Count > maxRecordCount)
            {
                // 스택을 큐로 변환(스택은 오래된 항목 제거 안됨)
                Queue<TransformData> tempQueue   = new Queue<TransformData>(transformHistory);
                List<TransformData>  cleanedList = new List<TransformData>();

                // 최초 위치 보존하고, 나머지는 뒤에서부터 자르기
                cleanedList.Add(initialTransform);

                while (tempQueue.Count > maxRecordCount - 1)
                {
                    // 오래된 기록 제거
                    tempQueue.Dequeue();
                }

                // 나머지 최신 항목 추가
                cleanedList.AddRange(tempQueue);

                // 다시 스택 변환
                transformHistory = new Stack<TransformData>(cleanedList);
            }
        }
    }

    // 되감기 시작
    public void StartRewind()
    {
        isRewinding = true;
        rewindComplete = false;
        rewindProgress = 0f;

        if (rb != null)
        {
            // rb.velocity = Vector3.zero;
            // rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 초기 from/to 설정
        if (transformHistory.Count >= 2)
        {
            to = transformHistory.Pop();
            from = transformHistory.Peek();
        }
    }

    // 되감기 과정
    private void RewindStep()
    {
        if (rewindComplete || from == null || to == null) return;

        // 감기 속도 조절
        float effectiveRewindSpeed = rewindSpeed;

        // 불투명도 조절일 때는 느리게
        if (GetComponent<AlphaCrasher>() != null)
        {
            effectiveRewindSpeed *= 0.002f; // 감기 속도를 느리게
        }

        rewindProgress += effectiveRewindSpeed * Time.deltaTime / recordInterval;

        if (rewindProgress >= 1f)
        {
            // 다음 단계로 이동
            rewindProgress = 0f;

            if (transformHistory.Count > 1)
            {
                to = transformHistory.Pop();
                from = transformHistory.Peek();
            }

            else
            {
                // 마지막까지 되감았을 경우
                transform.position = from.Value.position;
                transform.rotation = from.Value.rotation;

                // 되감기 완료
                rewindComplete = true;
                StopRewind(isComplete: true);

                return;
            }
        }

        // 위치/회전 보간
        transform.position = Vector3.Lerp(to.Value.position, from.Value.position, rewindProgress);
        transform.rotation = Quaternion.Slerp(to.Value.rotation, from.Value.rotation, rewindProgress);

        // 알파도 rewindProgress에 맞춰 보간 (0 → 1)
        if (GetComponent<AlphaCrasher>() != null)
        {
            render.enabled = true;

            currentAlpha = Mathf.Lerp(currentAlpha, 1f, rewindProgress);

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].HasProperty("_Color"))
                {
                    Color color = materials[i].color;
                    color.a = currentAlpha;
                    materials[i].color = color;
                }
            }

            if (currentAlpha >= 0.99f && !rewindComplete)
            {
                rewindComplete = true;
                StopRewind(true);
            }

            // 파티클 있으면 재생
            if (ps != null)
            {
                var main = ps.main;
                main.useUnscaledTime = true;
                ps.Play();
            }
        }
    }

    // 되감기 종료
    public void StopRewind(bool isComplete = false)
    {
        isRewinding = false;
        from = null;
        to = null;

        // isKinematic 고정
        if (rb != null)
        {
            // 끝까지 전부 감았으면 true, 중간에 멈춘거면 false
            rb.isKinematic = isComplete;
            rb.useGravity = true;
        }
    }

    public void InitGhostTrail()
    {
        ghostTrail?.Init();
    }
}