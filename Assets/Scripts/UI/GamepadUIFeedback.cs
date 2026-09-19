using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Makes gamepad UI navigation visible without touching how mouse/keyboard
// interaction already looks: while a gamepad was the last device used, the
// EventSystem's current selection gets its background pushed to a light grey
// and its highlight tint pushed to match, so it clearly reads as "this is
// where you are" without a visible cursor. The moment the mouse or keyboard
// is used, the override is dropped and the button reverts to whatever it was
// already authored to look like. Applies scene-wide (Options, Pause, Lose,
// wherever the current selection happens to be) since it just follows
// whatever EventSystem.current.currentSelectedGameObject is.
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

    // wasUpdatedThisFrame fires on the tiniest state change - including
    // sub-deadzone stick drift and phantom zero-delta mouse "still here"
    // events some OSes send every frame - which was flipping gamepadActive
    // back off almost immediately and making the highlight a one-frame
    // glimpse. Requiring an actually meaningful input keeps it stable.
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
