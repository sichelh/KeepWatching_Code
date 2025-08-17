using UnityEngine;

public class TriggerInteractableObject : MonoBehaviour, IInteractable, IPuzzleTrigger
{
    [SerializeField] private int objectIndex;
    [SerializeField] private Transform triggerPosition;

    public Outline     Outline       { get; private set; }
    public IVisibility VisibilityObj { get; private set; }
    public int         ObjectIndex   => objectIndex;

    private PlayerController playerController;

    private void Awake()
    {
        Outline = GetComponent<Outline>();
        Outline.enabled = false;
        playerController = PlayerController.Instance;
        VisibilityObj = GetComponent<IVisibility>();
    }

    public void TriggerEnter()
    {
        // 트리거에 들어갔을 때 오브젝트 표시
        this.transform.parent = triggerPosition;
        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = Vector3.zero;
        this.GetComponent<Rigidbody>().isKinematic = true;
        Outline.enabled = false;
    }

    public void TriggerExit()
    {
    }

    public void Execute()
    {
        if (VisibilityObj != null && !VisibilityObj.IsVisible())
        {
            return;
        }

        if (playerController.GrabObjectController.IsGrab)
        {
            Exit();
            return;
        }

        playerController.GrabObjectController.GrabObject(gameObject);
    }

    public void Exit()
    {
        playerController.GrabObjectController.UnGrabObject();
        playerController.InteractionController.SetNull();
    }
}