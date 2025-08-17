using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrashManager : MonoBehaviour
{
    private List<ICrashable> crashables = new List<ICrashable>();

    private void Awake()
    {
        crashables = FindObjectsOfType<MonoBehaviour>()     // 모든 MonoBehaviour 검색
            .OfType<ICrashable>()                           // 그 중 ICrashable 인터페이스를 구현한 컴포넌트만 필터링
            .ToList();                                      // List<ICrashable>로 변환 후 저장
    }

    private void Start()
    {
        AllCrash();
    }

    // 모든 Crashable에 Crash 호출
    public void AllCrash()
    {
        foreach (var crash in crashables)
        {
            crash.Crash();
        }
    }
}
