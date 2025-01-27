using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamepadImage : MonoBehaviour
{
    private Image _Image;

    private void Awake()
    {
        _Image = GetComponent<Image>();
        PlayerController.Instance.GetComponent<PlayerInput>().controlsChangedEvent.AddListener(Check);
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
