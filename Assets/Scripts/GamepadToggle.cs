using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadToggle : MonoBehaviour
{

    private void Start()
    {
        PlayerController.Instance.GetComponent<PlayerInput>().controlsChangedEvent.AddListener(Check);
        Check(PlayerController.Instance.GetComponent<PlayerInput>());
    }
    private void Check(PlayerInput playerInput)
    {
        if (playerInput.currentControlScheme.Equals("Gamepad"))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
