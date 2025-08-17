using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SceneOnlySingleton<GameManager>
{
    [SerializeField] private DeadScene deadScene;

    public InputHandler InputHandler { get; private set; }
    public event Action OnGameOver;


    public bool      CanSceneChange { get; private set; }
    public DeadScene DeadScene      => deadScene;

    protected override void Awake()
    {
        base.Awake();
        InputHandler = GetComponent<InputHandler>();
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void GameOver()
    {
        CanSceneChange = false;
        OnGameOver?.Invoke();
        StartCoroutine(OnGameOverCoroutine());
    }

    private IEnumerator OnGameOverCoroutine()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MainScene");
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}