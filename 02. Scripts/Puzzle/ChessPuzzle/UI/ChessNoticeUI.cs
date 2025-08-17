using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ChessNoticeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private TMP_Text alarmText;

    [SerializeField] private float noticeDuration = 2f;

    private void Awake()
    {
        hintText.gameObject.SetActive(false);
        alarmText.gameObject.SetActive(false);
    }


    public void ShowHint()
    {
        StartCoroutine(HintRoutine());
    }

    public void ShowWrongMoveAlarm()
    {
        StartCoroutine(alarmRoutine());
    }

    private IEnumerator HintRoutine()
    {
        hintText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        hintText.gameObject.SetActive(false);
    }

    private IEnumerator alarmRoutine()
    {
        alarmText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        alarmText.gameObject.SetActive(false);
    }

}
