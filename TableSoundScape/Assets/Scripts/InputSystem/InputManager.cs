using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : PersistentSingleton<InputManager>
{
    private InputSystem_Actions actions;

    public ButtonInputHandler submit, cancel, click, rightClick, middleClick, debug;
    public Vector2 navigate { private set; get; }
    public Vector2 point { private set; get; }
    public Vector2 scrollWheel { private set; get; }

    private void OnEnable()
    {
        if (actions == null)
        {
            actions = new InputSystem_Actions();
            actions.UI.Navigate.performed += i => navigate = i.ReadValue<Vector2>();
            actions.UI.Point.performed += i => point = i.ReadValue<Vector2>();
            actions.UI.ScrollWheel.performed += i => scrollWheel = i.ReadValue<Vector2>();
        }

        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }

    private void Start()
    {
        submit = new ButtonInputHandler(actions.UI.Submit);
        cancel = new ButtonInputHandler(actions.UI.Cancel);
        click = new ButtonInputHandler(actions.UI.Click);
        rightClick = new ButtonInputHandler(actions.UI.RightClick);
        middleClick = new ButtonInputHandler(actions.UI.MiddleClick);
        debug = new ButtonInputHandler(actions.UI.Debug);
    }
}

public class ButtonInputHandler
{
    private InputAction action;
    private bool tapUsed;
    private bool releaseUsed;

    public ButtonInputHandler(InputAction action)
    {
        this.action = action;
    }

    public bool HOLD
    {
        get
        {
            return input();
        }
    }

    public bool TAP
    {
        get
        {
            bool usedLastFrame = tapUsed;
            tapUsed = input();
            return usedLastFrame ? false : tapUsed;
        }
    }

    public bool RELEASE
    {
        get
        {
            bool usedLastFrame = releaseUsed;
            releaseUsed = input();
            return usedLastFrame ? !releaseUsed : false;
        }
    }

    private bool input()
    {
        return action.phase == InputActionPhase.Performed;
    }
}
