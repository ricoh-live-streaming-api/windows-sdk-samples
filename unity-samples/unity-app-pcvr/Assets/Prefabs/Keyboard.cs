using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    /// <summary>
    /// Game object to which this script is attached
    /// </summary>
    public GameObject keyboard;
    /// <summary>
    /// InputField object that the script will modify
    /// </summary>
    public InputField inputField;
    /// <summary>
    /// Timer for input delay
    /// </summary>
    private float timer = -1;
    /// <summary>
    /// User specified float for seconds to delay before accepting more input
    /// </summary>
    public float timeDelay = 0.25f;

    private bool isShift = true;

    public void AddText(Button buttonObject)
    {
        if (Time.time - timer > timeDelay)
        {
            Text addText = buttonObject.GetComponentInChildren<Text>();
            inputField.text += addText.text;
        }
    }

    public void BackSpace()
    {
        if (Time.time - timer > timeDelay)
        {
            if (0 < inputField.text.Length)
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            }
        }
    }

    public void Enter()
    {
        keyboard.SetActive(false);
    }

    public void Shift()
    {
        if (Time.time - timer > timeDelay)
        {
            isShift = !isShift;
            foreach (Button shiftButton in keyboard.GetComponentsInChildren<Button>())
            {
                if ((string.Compare(shiftButton.name, "BackSpace") == 0) ||
                    (string.Compare(shiftButton.name, "Enter") == 0))
                {
                    continue;
                }
                Text shiftText = shiftButton.GetComponentInChildren<Text>();
                shiftText.text = (isShift ? shiftText.text.ToUpper() : shiftText.text.ToLower());
            }
        }
    }
}
