using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class RewinderManager : MonoBehaviour
//{
//    private static RewinderManager instance;

//    // 모든 오브젝트에 대해 Record, Rewind 호출
//    internal static Action Recorder;
//    internal static Action<float, bool> Rewinder;

//    internal static List<Rewinder> RewinderList { get; set; } = new List<Rewinder>();
//    internal static Dictionary<GameObject, Rewinder> RewinderDictionary { get; set; } = new Dictionary<GameObject, Rewinder>();

//    [Tooltip("간격은 반드시 1보다 커야 합니다.")]
//    // 프레임 몇 개마다 한 번 기록할지 결정
//    public int recordPeriod = 1;
//    // 현재 프레임 카운터
//    private int period = 0;

//    private void Awake()
//    {
//        instance = this;
//        if(recordPeriod <= 0) recordPeriod = 1;
//    }

//    // 시작할 때 물리 힘을 가해 흩어놓기
//    private void Start()
//    {
//        foreach(var rewinder in RewinderList)
//        {
//            Rigidbody rb = rewinder.GetComponent<Rigidbody>();

//            if(rb != null)
//            {
//                // 랜덤한 방향과 세기
//                Vector3 randomDir = UnityEngine.Random.onUnitSphere;
//                float forceMagnitude = UnityEngine.Random.Range(5f, 10f);
//                rb.AddForce(randomDir * forceMagnitude, ForceMode.Impulse);
//            }
//        }
//    }

//    // R 키 입력 시 되감기 발동
//    private void Update()
//    {
//        if(Input.GetKeyDown(KeyCode.R))
//        {
//            Debug.Log("R키 입력됨");
//            RewinderManager.RewindAll();
//        }
//    }

//    // FixedUpdate마다 기록 또는 되감기 실행
//    private void FixedUpdate()
//    {
//        if(!(recordPeriod <= 0) && period <= 0)
//        {
//            // 기록
//            Recorder?.Invoke();
//            period = recordPeriod;
//        }

//        // 부드럽게 보간
//        float lerpTime = (float)(recordPeriod - (period - 1)) / recordPeriod;
//        // 되감기
//        Rewinder?.Invoke(lerpTime, period == 1);
//        period -= 1;
//    }

//    // 모든 오브젝트 되감기 시작
//    public static void RewindAll()
//    {
//        foreach (var rewinder in RewinderList)
//        {
//            rewinder.RewindEnable();
//        }
//    }

//    // 특정 GameObject 되감기 시작
//    public static void Rewind(GameObject gameObject)
//    {
//        RewinderDictionary[gameObject].RewindEnable();
//    }

//    public static void Rewind(Rewinder rewinder)
//    {
//        rewinder.RewindEnable();
//    }

//    // 모든 Rewinder의 기록 제한 갱신
//    public static void SetRecordLimitAll(int limit)
//    {
//        foreach(var rewinder in RewinderList)
//        {
//            rewinder.recordLimit = limit;
//        }
//    }

//    public static int GetRecordPeriod()
//    {
//        return instance.recordPeriod;
//    }

//    public static void SetRecordPeriod(int period)
//    {
//        instance.recordPeriod = period;
//    }

//    public static Rewinder GetRewinder(GameObject gameObject)
//    {
//        return RewinderDictionary[gameObject];
//    }

//    public static Rewinder[] GetRewinderAll()
//    {
//        return RewinderList.ToArray();
//    }
//}
