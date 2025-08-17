using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : IItemEffect
{
    private Camera mainCamera;

    private Coroutine coroutine;

    // 되감기 중인 오브젝트 추적용 집합 (중복 방지 및 종료 시 처리)
    private HashSet<Rewinder> activeRewinders = new HashSet<Rewinder>();

    // 씬에 존재하는 모든 Rewinder를 관리 (Rewinder가 직접 등록/해제)
    private static HashSet<Rewinder> existRewinders = new HashSet<Rewinder>();

    // 타임스케일 조절 관련
    public float rewindTimeScale = 0.4f;
    private float originalTimeScale;
    private float originalFixedDeltaTime;
    private bool isRewindActive = false;

    public CameraEffect(Camera mainCamera)
    {
        this.mainCamera = mainCamera;
    }

    public void Use(out bool canUse)
    {
        if (coroutine == null)
        {
            //카메라 UI를 켜주고
            UIHUD.Instance.CamaraViewer(true);
            coroutine = GameManager.Instance.StartCoroutine(CameraControll());
        }
        else
        {
            //카메라 UI를 꺼준다
            UIHUD.Instance.CamaraViewer(false);
            GameManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
            StopAllRewinds();
        }

        canUse = true;
    }

    // 카메라 정면 시야(FOV) 내의 오브젝트들을 계속 탐지하며, R 키 입력 시 되감기 수행
    private IEnumerator CameraControll()
    {
        if (mainCamera == null)
        {
            coroutine = null;
            yield break;
        }

        float maxDistance = 10f; // 최대 탐지 거리
        float maxAngle    = 45f; // 카메라 정면 기준 좌우 45도
        yield return new WaitUntil(() => GameManager.Instance.InputHandler.IsRecoding);
        AudioManager.Instance.PlayRecorder();
        while (GameManager.Instance.InputHandler.IsRecoding)
        {
            {
                // AudioManager.Instance.PlayRecorder();
                // 효과 화면
                RewindEffecter.Instance.ShowEffect();

                // 타임 스케일 조정
                if (!isRewindActive)
                {
                    isRewindActive = true;
                    originalTimeScale = Time.timeScale;
                    originalFixedDeltaTime = Time.fixedDeltaTime;

                    Time.timeScale = rewindTimeScale;
                    Time.fixedDeltaTime = originalFixedDeltaTime * rewindTimeScale;
                }

                // 등록된 Rewinder 오브젝트 검색
                foreach (var rewinder in existRewinders)
                {
                    if (rewinder == null || rewinder.IsRewindComplete)
                        continue;

                    Vector3 dirToTarget = rewinder.transform.position - mainCamera.transform.position;
                    float   distance    = dirToTarget.magnitude;
                    float   angle       = Vector3.Angle(mainCamera.transform.forward, dirToTarget);

                    // 시야각과 거리 조건 충족 시 대상에 추가
                    // 되감기가 완료됐는지도 체크
                    if (angle <= maxAngle && distance <= maxDistance && !rewinder.IsRewindComplete)
                    {
                        if (!activeRewinders.Contains(rewinder))
                        {
                            rewinder.StartRewind();
                            activeRewinders.Add(rewinder);
                        }

                        // 매 프레임 GetComponent하면 성능에 안좋은 영향을 미침
                        // GhostTrail 컴포넌트가 있으면 호출
                        // GhostTrail ghostTrail = rewinder.GetComponent<GhostTrail>();
                        // if (ghostTrail != null)
                        // {
                        //     ghostTrail.Init();
                        // }
                        rewinder?.InitGhostTrail();
                    }
                }
            }
            // else
            // {
            //     Debug.Log("이건 안올텐데?");
            //     // Time.timeScale = originalTimeScale;
            //     // Time.fixedDeltaTime = originalFixedDeltaTime;
            //     // isRewindActive = false;
            //     //
            //     // // 효과 화면 끄기
            //     // RewindEffecter.Instance.CloseEffect();
            //     break;
            // }
            // R키 입력 중일 때
            // if (Input.GetKey(KeyCode.R))
            // {
            //     // 효과 화면
            //     RewindEffecter.Instance.ShowEffect();
            //
            //     // 타임 스케일 조정
            //     if (!isRewindActive)
            //     {
            //         isRewindActive = true;
            //         originalTimeScale = Time.timeScale;
            //         originalFixedDeltaTime = Time.fixedDeltaTime;
            //
            //         Time.timeScale = rewindTimeScale;
            //         Time.fixedDeltaTime = originalFixedDeltaTime * rewindTimeScale;
            //     }
            //
            //     // 등록된 Rewinder 오브젝트 검색
            //     foreach (var rewinder in existRewinders)
            //     {
            //         if (rewinder == null || rewinder.IsRewindComplete)
            //             continue;
            //
            //         Vector3 dirToTarget = rewinder.transform.position - mainCamera.transform.position;
            //         float   distance    = dirToTarget.magnitude;
            //         float   angle       = Vector3.Angle(mainCamera.transform.forward, dirToTarget);
            //
            //         // 시야각과 거리 조건 충족 시 대상에 추가
            //         // 되감기가 완료됐는지도 체크
            //         if (angle <= maxAngle && distance <= maxDistance && !rewinder.IsRewindComplete)
            //         {
            //             if (!activeRewinders.Contains(rewinder))
            //             {
            //                 rewinder.StartRewind();
            //                 activeRewinders.Add(rewinder);
            //             }
            //
            //             // GhostTrail 컴포넌트가 있으면 호출
            //             GhostTrail ghostTrail = rewinder.GetComponent<GhostTrail>();
            //             if (ghostTrail != null)
            //             {
            //                 ghostTrail.Init();
            //             }
            //         }
            //     }
            // }

            // else if (Input.GetKeyUp(KeyCode.R))
            // {
            //     // 타임 스케일 복원
            //     Time.timeScale = originalTimeScale;
            //     Time.fixedDeltaTime = originalFixedDeltaTime;
            //     isRewindActive = false;
            //
            //     // 효과 화면 끄기
            //     RewindEffecter.Instance.CloseEffect();
            // }
            //
            // else if (activeRewinders.Count > 0)
            // {
            //     // R 키를 떼면 모든 되감기 중지
            //     StopAllRewinds();
            // }

            yield return null;
        }

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        isRewindActive = false;

        // 효과 화면 끄기
        RewindEffecter.Instance.CloseEffect();
        StopAllRewinds();
        AudioManager.Instance.StopRecorder();
        coroutine = GameManager.Instance.StartCoroutine(CameraControll());
    }

    // 모든 되감기 대상 오브젝트의 되감기 종료 처리
    private void StopAllRewinds()
    {
        foreach (var rewinder in activeRewinders)
        {
            // 되감기가 안 끝났는지 && 활성화되었는지 여부
            if (!rewinder.IsRewindComplete && rewinder.isActiveAndEnabled)
            {
                rewinder.StopRewind();
            }
        }

        activeRewinders.Clear();
    }

    // Rewinder가 사용 가능할 때 호출
    public static void Subscribe(Rewinder rewinder)
    {
        if (rewinder != null)
            existRewinders.Add(rewinder);
    }

    // Rewinder가 사용 불가능할 때 호출
    public static void UnSubscribe(Rewinder rewinder)
    {
        if (rewinder != null)
            existRewinders.Remove(rewinder);
    }
}