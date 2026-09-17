using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

// One instance per rebindable binding - e.g. Jump gets two of these (one for
// its Keyboard&Mouse binding, one for its Gamepad binding); a Move WASD part
// gets exactly one (Keyboard&Mouse only - the gamepad side is a single
// whole-stick binding with its own separate row/instance).
public class RebindBinding : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string actionName;
    [SerializeField] private string bindingGroup = "Keyboard&Mouse";
    [Tooltip("Leave empty for a simple (non-composite) binding, e.g. Jump/Attack. Set to \"up\"/\"down\"/\"left\"/\"right\" for a Move WASD part.")]
    [SerializeField] private string compositePartName;

    [SerializeField] private TextMeshProUGUI bindingText;
    [SerializeField] private Button rebindButton;

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    private void OnEnable() => UpdateBindingText();

    private void OnDisable()
    {
        rebindOperation?.Dispose();
        rebindOperation = null;
    }

    public void StartRebind()
    {
        InputAction action = ResolveAction();
        int bindingIndex = FindBindingIndex(action);

        if (action == null || bindingIndex < 0)
            return;

        action.Disable();

        if (rebindButton != null)
            rebindButton.interactable = false;
        if (bindingText != null)
            bindingText.text = "Press any input...";

        InputActionRebindingExtensions.RebindingOperation operation = action.PerformInteractiveRebinding(bindingIndex)
            .WithBindingGroup(bindingGroup)
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(_ => FinishRebind(action))
            .OnComplete(_ => FinishRebind(action));

        // Restrict which device's input is actually accepted while capturing -
        // WithBindingGroup only picks which binding slot gets overridden, it
        // doesn't stop e.g. a keyboard press from being captured while
        // rebinding a Gamepad row (and vice versa).
        if (bindingGroup == "Gamepad")
        {
            operation = operation.WithControlsHavingToMatchPath("<Gamepad>");
        }
        else if (bindingGroup == "Keyboard&Mouse")
        {
            operation = operation.WithControlsHavingToMatchPath("<Keyboard>")
                .WithControlsHavingToMatchPath("<Mouse>");
        }

        rebindOperation = operation.Start();
    }

    public void ResetToDefault()
    {
        InputAction action = ResolveAction();
        int bindingIndex = FindBindingIndex(action);

        if (bindingIndex >= 0)
            action.RemoveBindingOverride(bindingIndex);

        UpdateBindingText();
        RebindSaveLoad.Save(inputActions);
    }

    private void FinishRebind(InputAction action)
    {
        rebindOperation?.Dispose();
        rebindOperation = null;
        action.Enable();

        if (rebindButton != null)
            rebindButton.interactable = true;

        UpdateBindingText();
        RebindSaveLoad.Save(inputActions);
    }

    private void UpdateBindingText()
    {
        if (bindingText == null)
            return;

        InputAction action = ResolveAction();
        int bindingIndex = FindBindingIndex(action);
        bindingText.text = bindingIndex >= 0 ? action.GetBindingDisplayString(bindingIndex) : "-";
    }

    private InputAction ResolveAction()
    {
        return inputActions != null ? inputActions.FindActionMap(actionMapName)?.FindAction(actionName) : null;
    }

    private int FindBindingIndex(InputAction action)
    {
        if (action == null)
            return -1;

        ReadOnlyArray<InputBinding> bindings = action.bindings;
        for (int i = 0; i < bindings.Count; i++)
        {
            InputBinding binding = bindings[i];

            if (string.IsNullOrEmpty(binding.groups) || !binding.groups.Contains(bindingGroup))
                continue;

            if (!string.IsNullOrEmpty(compositePartName) && !string.Equals(binding.name, compositePartName, StringComparison.OrdinalIgnoreCase))
                continue;

            return i;
        }

        return -1;
    }
}
