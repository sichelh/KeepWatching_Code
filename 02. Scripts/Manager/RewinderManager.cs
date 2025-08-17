using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class RewinderManager : MonoBehaviour
//{
//    private static RewinderManager instance;

//    // ��� ������Ʈ�� ���� Record, Rewind ȣ��
//    internal static Action Recorder;
//    internal static Action<float, bool> Rewinder;

//    internal static List<Rewinder> RewinderList { get; set; } = new List<Rewinder>();
//    internal static Dictionary<GameObject, Rewinder> RewinderDictionary { get; set; } = new Dictionary<GameObject, Rewinder>();

//    [Tooltip("������ �ݵ�� 1���� Ŀ�� �մϴ�.")]
//    // ������ �� ������ �� �� ������� ����
//    public int recordPeriod = 1;
//    // ���� ������ ī����
//    private int period = 0;

//    private void Awake()
//    {
//        instance = this;
//        if(recordPeriod <= 0) recordPeriod = 1;
//    }

//    // ������ �� ���� ���� ���� ������
//    private void Start()
//    {
//        foreach(var rewinder in RewinderList)
//        {
//            Rigidbody rb = rewinder.GetComponent<Rigidbody>();

//            if(rb != null)
//            {
//                // ������ ����� ����
//                Vector3 randomDir = UnityEngine.Random.onUnitSphere;
//                float forceMagnitude = UnityEngine.Random.Range(5f, 10f);
//                rb.AddForce(randomDir * forceMagnitude, ForceMode.Impulse);
//            }
//        }
//    }

//    // R Ű �Է� �� �ǰ��� �ߵ�
//    private void Update()
//    {
//        if(Input.GetKeyDown(KeyCode.R))
//        {
//            Debug.Log("RŰ �Էµ�");
//            RewinderManager.RewindAll();
//        }
//    }

//    // FixedUpdate���� ��� �Ǵ� �ǰ��� ����
//    private void FixedUpdate()
//    {
//        if(!(recordPeriod <= 0) && period <= 0)
//        {
//            // ���
//            Recorder?.Invoke();
//            period = recordPeriod;
//        }

//        // �ε巴�� ����
//        float lerpTime = (float)(recordPeriod - (period - 1)) / recordPeriod;
//        // �ǰ���
//        Rewinder?.Invoke(lerpTime, period == 1);
//        period -= 1;
//    }

//    // ��� ������Ʈ �ǰ��� ����
//    public static void RewindAll()
//    {
//        foreach (var rewinder in RewinderList)
//        {
//            rewinder.RewindEnable();
//        }
//    }

//    // Ư�� GameObject �ǰ��� ����
//    public static void Rewind(GameObject gameObject)
//    {
//        RewinderDictionary[gameObject].RewindEnable();
//    }

//    public static void Rewind(Rewinder rewinder)
//    {
//        rewinder.RewindEnable();
//    }

//    // ��� Rewinder�� ��� ���� ����
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
