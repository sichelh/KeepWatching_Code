using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private float checkRate;
    [SerializeField] private float maxDistance;
    [SerializeField] Camera playerCamera;

    [SerializeField] private ChessBoardManager boardMgr;
    [SerializeField] private GameObject interactionBtnImage;
    [SerializeField] GameObject curInteractableObject;

    private LayerMask interactabeLayerMask;
    private LayerMask clickLayerMask;
    private float lastCheckTime;

    private IInteractable curInteractable;
    private IInteractable prevInteractable;
    private GrabObjectController grabObjectController;

    private InputHandler inputHandler;

    private void Start()
    {
        grabObjectController = GetComponent<GrabObjectController>();
        interactabeLayerMask = LayerMask.GetMask("Interactable", "Chess");
        clickLayerMask = LayerMask.GetMask("Tile");

        inputHandler = GameManager.Instance?.InputHandler;
        if (inputHandler != null)
            inputHandler.OnLeftClick += HandleLeftClick;
    }

    private void OnDestroy()
    {
        if (inputHandler != null)
            inputHandler.OnLeftClick -= HandleLeftClick;
    }

    private void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactabeLayerMask))
            {
                GameObject hitObj = hit.collider.gameObject;

                if (hitObj.TryGetComponent<IInteractable>(out var interactable))
                {
                    curInteractableObject = hitObj;

                    curInteractable = interactable;
                    if (interactable.VisibilityObj != null && !interactable.VisibilityObj.IsVisible())
                        return;

                    interactionBtnImage.SetActive(true);

                    if (curInteractable.Outline != null)
                        curInteractable.Outline.enabled = true;

                    if (prevInteractable != null && prevInteractable != curInteractable)
                    {
                        if (prevInteractable.Outline != null)
                            prevInteractable.Outline.enabled = false;
                        interactionBtnImage.SetActive(false);
                    }

                    prevInteractable = curInteractable;
                }
                else
                {
                    DisableOutline();
                }
            }
            else
            {
                interactionBtnImage.SetActive(false);
                DisableOutline();
                SetNull();
            }
        }

        if (GameManager.Instance.InputHandler.IsInteract)
        {
            curInteractable?.Execute();
            GameManager.Instance.InputHandler.ResetInteract();
        }
    }

    public void HandleInteract()
    {
        if (curInteractableObject == null) return;

        ChessPiece chessPiece = curInteractableObject.GetComponentInParent<ChessPiece>();
        if (chessPiece != null)
        {
            if (boardMgr.SelectedPiece == chessPiece)
                boardMgr.DeselectPiece();
            else
                boardMgr.SelectPiece(chessPiece);
            return;
        }

        var chessTile = curInteractableObject.GetComponentInParent<ChessTile>();
        if (chessTile != null)
        {
            boardMgr.TryMoveSelectedPieceTo(chessTile);
            return;
        }

        if (curInteractableObject.TryGetComponent<GetableItem>(out var getable))
        {
            getable.Execute();
            SetNull();
            return;
        }

        if (grabObjectController.IsGrab)
        {
            grabObjectController.UnGrabObject();
            SetNull();
            return;
        }

        //GrabObject에 이미 Rigidbody를 체크하는 부분이 있어서 불필요한 GetComponent인것 같습니다.
        if (curInteractableObject.GetComponent<Rigidbody>() != null)
        {
            grabObjectController.GrabObject(curInteractableObject);
        }
    }

    public void HandleLeftClick()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 10f, clickLayerMask))
            return;

        if (!boardMgr.HasSelectedPiece())
            return;

        if (boardMgr.HasSelectedPiece())
        {
            var clickedTile = hit.collider.GetComponentInParent<ChessTile>();
            if (clickedTile != null)
            {
                boardMgr.TryMoveSelectedPieceTo(clickedTile);
            }
        }
    }

    public void SetNull()
    {
        curInteractable = null;
        prevInteractable = null;
        curInteractableObject = null;
    }

    private void DisableOutline()
    {
        if (prevInteractable?.Outline != null)
            prevInteractable.Outline.enabled = false;
        if (curInteractable?.Outline != null)
            curInteractable.Outline.enabled = false;
    }
}