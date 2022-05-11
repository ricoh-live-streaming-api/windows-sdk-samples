using UnityEngine;
using UnityEngine.InputSystem;

public class UIControler : MonoBehaviour
{
    private bool isUIEnabled = true;

    /// <summary>
    /// Canvas 上の UI の表示・非表示を切り替える
    /// </summary>
    /// <param name="context"></param>
    public void OnSetComponents(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Performed)
        {
            isUIEnabled = !isUIEnabled;
            gameObject.SetActive(isUIEnabled);
        }
    }
}