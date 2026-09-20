using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamepadUIFeedback : MonoBehaviour
{
    [SerializeField] private Color selectedBackgroundColor = new Color(0.75f, 0.75f, 0.75f, 1f);
    [SerializeField] private Color selectedHighlightColor = new Color(0.75f, 0.75f, 0.75f, 1f);

    private bool gamepadActive;

    private Image highlightedImage;
    private Color originalColor;
    private Selectable highlightedSelectable;
    private Color originalHighlightedColor;
    private Color originalSelectedColor;

    private void Update()
    {
        UpdateActiveDevice();

        GameObject current = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        bool shouldHighlight = gamepadActive && current != null;

        if (highlightedImage != null && (!shouldHighlight || highlightedImage.gameObject != current))
            ClearHighlight();

        if (shouldHighlight && highlightedImage == null)
            ApplyHighlight(current);
    }

    private const float StickActivationThreshold = 0.5f;
    private const float MouseMoveThreshold = 0.1f;

    private void UpdateActiveDevice()
    {
        if (IsGamepadActive())
            gamepadActive = true;
        else if (IsMouseOrKeyboardActive())
            gamepadActive = false;
    }

    private bool IsGamepadActive()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null)
            return false;

        if (gamepad.leftStick.ReadValue().sqrMagnitude > StickActivationThreshold * StickActivationThreshold)
            return true;
        if (gamepad.dpad.ReadValue().sqrMagnitude > 0.01f)
            return true;

        return gamepad.buttonSouth.wasPressedThisFrame || gamepad.buttonNorth.wasPressedThisFrame ||
               gamepad.buttonEast.wasPressedThisFrame || gamepad.buttonWest.wasPressedThisFrame ||
               gamepad.startButton.wasPressedThisFrame || gamepad.selectButton.wasPressedThisFrame;
    }

    private bool IsMouseOrKeyboardActive()
    {
        Mouse mouse = Mouse.current;
        if (mouse != null &&
            (mouse.delta.ReadValue().sqrMagnitude > MouseMoveThreshold * MouseMoveThreshold ||
             mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame ||
             mouse.middleButton.wasPressedThisFrame || Mathf.Abs(mouse.scroll.ReadValue().y) > 0.01f))
            return true;

        return Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
    }

    private void ApplyHighlight(GameObject target)
    {
        Image image = target.GetComponent<Image>();
        if (image == null)
            return;

        highlightedImage = image;
        originalColor = image.color;
        image.color = selectedBackgroundColor;

        highlightedSelectable = target.GetComponent<Selectable>();
        if (highlightedSelectable != null)
        {
            ColorBlock colors = highlightedSelectable.colors;
            originalHighlightedColor = colors.highlightedColor;
            originalSelectedColor = colors.selectedColor;
            colors.highlightedColor = selectedHighlightColor;
            colors.selectedColor = selectedHighlightColor;
            highlightedSelectable.colors = colors;
        }
    }

    private void ClearHighlight()
    {
        if (highlightedImage != null)
            highlightedImage.color = originalColor;

        if (highlightedSelectable != null)
        {
            ColorBlock colors = highlightedSelectable.colors;
            colors.highlightedColor = originalHighlightedColor;
            colors.selectedColor = originalSelectedColor;
            highlightedSelectable.colors = colors;
        }

        highlightedImage = null;
        highlightedSelectable = null;
    }
}
