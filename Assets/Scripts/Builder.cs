using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Builder : Singleton<Builder>
{
    [SerializeField] Tilemap map;
    [SerializeField] MapEditor mapEditor;
    [SerializeField] GameObject panel;
    public Cell cell;
    PlayerInput playerInput;
    Vector2 mousePos; 
    Vector3Int currentGridPosition;
    Camera _camera;

    public MapEditor MapEditor {
        get => mapEditor;
    }

    public GameObject Panel {
        get => panel;
    }

    public Vector3Int CurrentGridPosition {
        get => currentGridPosition;
    }

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
        _camera = Camera.main;
        panel.SetActive(false);
    }

    public void OnEnable() {
        playerInput.Enable();

        playerInput.MapEditing.MousePosition.performed += OnMouseMove;
        playerInput.MapEditing.MouseLeftClick.performed += OnLeftClick;
        playerInput.MapEditing.MouseRightClick.performed += OnRightClick;
        playerInput.MapEditing.OnEnter.performed += OnEnter;
        playerInput.MapEditing.OnExit.performed += OnExit;
    }

    public void OnDisable() {
        playerInput.Disable();

        playerInput.MapEditing.MousePosition.performed -= OnMouseMove;
        playerInput.MapEditing.MouseLeftClick.performed -= OnLeftClick;
        playerInput.MapEditing.MouseRightClick.performed -= OnRightClick;
        playerInput.MapEditing.OnEnter.performed -= OnEnter;
        playerInput.MapEditing.OnExit.performed -= OnExit;
    }

    private void OnMouseMove(InputAction.CallbackContext ctx) {
        mousePos = ctx.ReadValue<Vector2> ();
    }

    private void OnLeftClick (InputAction.CallbackContext ctx) {
        mapEditor.NutrientsUp(currentGridPosition);
    }

    private void OnRightClick (InputAction.CallbackContext ctx) {
        cell = mapEditor.GetCell(currentGridPosition);
        
        if(panel.activeSelf) {
            panel.SetActive(false);
        } else {
            panel.SetActive(true);
            OnDisable();
        }
    }

    private void OnEnter(InputAction.CallbackContext ctx) {
        mapEditor.SaveGridToJson();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnExit(InputAction.CallbackContext ctx) {
        Destroy (gameObject);
        SceneManager.LoadScene(0);
    }
    void Update()
    {
        if(_camera != null) {
            Vector3 pos = _camera.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = map.WorldToCell(pos);

            if (gridPos != currentGridPosition) {
                currentGridPosition = gridPos;
            }
        }
    }

}
