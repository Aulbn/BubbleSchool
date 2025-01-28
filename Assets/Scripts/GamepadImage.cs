using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamepadImage : MonoBehaviour
{
    private Image _Image;

    // private PlayerController _Player;

    public enum GamepadInputButton
    {
        None,
        West,
        South,
        North,
        East,
    }

    public GamepadInputButton InputButton;
    public Button _Button;

    private void Awake()
    {
        _Image = GetComponent<Image>();
        // _Player = PlayerController.Instance;
    }

    private void Start()
    {
        PlayerController.Instance.GetComponent<PlayerInput>().controlsChangedEvent.AddListener(Check);
        Check(PlayerController.Instance.GetComponent<PlayerInput>());
    }

    private void Update()
    {
        switch (InputButton)
        {
            case GamepadInputButton.West:
                if (PlayerController.MeleeThisFrame)
                    _Button.onClick.Invoke();
                break;
            case GamepadInputButton.South:
                if (PlayerController.ThrowDownThisFrame)
                    _Button.onClick.Invoke();
                break;
            case GamepadInputButton.North:
                if (PlayerController.GamepadNorthThisFrame)
                    _Button.onClick.Invoke();
                break;
            case GamepadInputButton.East:
                if (PlayerController.GamepadEastThisFrame)
                    _Button.onClick.Invoke();
                break;
        }
    }

    private void Check(PlayerInput playerInput)
    {
        if (playerInput.currentControlScheme.Equals("Gamepad"))
        {
            _Image.enabled = true;
        }
        else
        {
            _Image.enabled = false;
        }
    }


}
