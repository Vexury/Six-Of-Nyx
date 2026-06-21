using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private InputReader inputReader;
    [SerializeField] private Rigidbody marble;
    [SerializeField] private Transform marbleStart;
    [SerializeField] private GlassShell glassShell;
    [SerializeField] private FinishZone finishZone;
    [SerializeField] private float failResetDelay = 0.5f;
    [SerializeField] private float winLoadDelay = 1.5f;

    private enum State { Playing, Failed, Won }
    private State _state;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        glassShell.OnMarbleFailed += HandleFail;
        finishZone.OnMarbleFinished += HandleWin;
    }

    private void OnDisable()
    {
        glassShell.OnMarbleFailed -= HandleFail;
        finishZone.OnMarbleFinished -= HandleWin;
    }

    private void Start()
    {
        _state = State.Playing;
        inputReader.EnableGameplayInput();
    }

    private void HandleFail()
    {
        if (_state != State.Playing) return;
        _state = State.Failed;
        marble.linearVelocity = Vector3.zero;
        marble.angularVelocity = Vector3.zero;
        StartCoroutine(FailRoutine());
    }

    private IEnumerator FailRoutine()
    {
        inputReader.DisableAllInput();
        yield return new WaitForSeconds(failResetDelay);
        ResetMarble();
        inputReader.EnableGameplayInput();
        _state = State.Playing;
    }

    private void HandleWin()
    {
        if (_state != State.Playing) return;
        _state = State.Won;
        StartCoroutine(WinRoutine());
    }

    private IEnumerator WinRoutine()
    {
        inputReader.DisableAllInput();
        yield return new WaitForSeconds(winLoadDelay);
        SceneController.Instance.LoadNextScene();
    }

    private void ResetMarble()
    {
        marble.linearVelocity = Vector3.zero;
        marble.angularVelocity = Vector3.zero;
        marble.position = marbleStart.position;
    }
}
