using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    // 잔상을 렌더링할 때 사용할 머티리얼 (반투명 쉐이더 권장)
    [SerializeField] private Material material;

    // 잔상이 지속되는 시간 (초)
    public float lifeTime = 0.5f;

    // 잔상이 생성되는 간격 (초)
    public float spawnInterval = 0.05f;

    private LineRenderer lineRenderer;

    // 잔상의 위치와 각 위치가 생성된 시간
    private List<Vector3> positions = new List<Vector3>();
    private List<float> spawnTimes = new List<float>();

    // 다음 잔상 생성 시간을 결정하기 위한 타이머
    private float nextSpawnTime = 0f;

    // 마지막으로 기록된 위치 (보간 시 사용)
    private Vector3? lastPosition = null;

    private void Awake()
    {
        // LineRenderer 컴포넌트가 없다면 추가
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 라인 렌더러 기본 설정
        lineRenderer.positionCount = 0;
        lineRenderer.material = material; // 지정된 머티리얼 적용
        lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, 0.3f); // 일정한 너비 유지
        lineRenderer.numCapVertices = 5; // 끝 모서리 둥글게
        lineRenderer.useWorldSpace = true; // 월드 좌표계 사용
        lineRenderer.loop = false; // 라인 루프 X

        // 초기 색상 (알파 0) 설정
        Color c = Color.white;
        c.a = 0;
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;
    }

    // 외부에서 잔상을 생성할 때 호출
    public void Init()
    {
        // 아직 다음 생성 시점이 안 되면 무시
        if (Time.time < nextSpawnTime)
            return;

        // 다음 생성 시점 갱신
        nextSpawnTime = Time.time + spawnInterval;

        Vector3 currentPos = transform.position;

        // 이전 위치가 있다면 중간 위치도 추가하여 라인이 끊기지 않게 함
        if (lastPosition.HasValue)
        {
            Vector3 midPoint = Vector3.Lerp(lastPosition.Value, currentPos, 0.5f);
            positions.Add(midPoint);
            spawnTimes.Add(Time.time - (spawnInterval / 2)); // 약간 앞선 시간으로 등록
        }

        // 현재 위치 추가
        positions.Add(currentPos);
        spawnTimes.Add(Time.time);

        // 현재 위치를 다음 기준점으로 저장
        lastPosition = currentPos;

        // 라인 렌더러에 위치 설정
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    private void Update()
    {
        float now = Time.time;

        // lifeTime이 지난 잔상 삭제
        int removeCount = 0;
        for (int i = 0; i < spawnTimes.Count; i++)
        {
            if (now - spawnTimes[i] > lifeTime)
                removeCount++;
            else
                break; // 시간순으로 정렬되어 있으므로 이후는 남겨야 함
        }

        if (removeCount > 0)
        {
            positions.RemoveRange(0, removeCount);
            spawnTimes.RemoveRange(0, removeCount);
        }

        // 위치가 없으면 처리 중단
        if (positions.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // 라인 렌더러 위치 갱신
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());

        // Gradient 설정 (색상 + 알파)
        Gradient gradient = new Gradient();

        // 색상은 흰색에서 검은색으로 변화 (필요시 수정 가능)
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(Color.white, 0f);
        colorKeys[1] = new GradientColorKey(Color.black, 1f);

        // 최대 8개의 알파 포인트만 지원
        int maxKeys = 8;
        int actualKeys = Mathf.Min(positions.Count, maxKeys);
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[actualKeys];

        // 포인트별로 알파 계산 (오래된 것은 투명, 최근 것은 불투명)
        for (int i = 0; i < actualKeys; i++)
        {
            // 현재 키의 위치 비율
            float t = (float)i / (actualKeys - 1);

            // 해당 비율에 해당하는 위치 인덱스 계산
            int index = Mathf.RoundToInt(t * (positions.Count - 1));

            // 인덱스가 배열 범위를 벗어나지 않게 보정
            index = Mathf.Clamp(index, 0, spawnTimes.Count - 1);

            // 현재 시점과 해당 위치가 생성된 시간의 차이
            float age = now - spawnTimes[index];

            // 잔상이 오래될수록 더 투명하게
            float alpha = Mathf.Clamp01(1 - (age / lifeTime));

            alphaKeys[i] = new GradientAlphaKey(alpha, t);
        }

        // 그라데이션 적용
        gradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = gradient;
    }
}