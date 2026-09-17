using UnityEngine;
using UnityEngine.InputSystem;

public static class RebindSaveLoad
{
    private const string PrefsKey = "InputBindingOverrides";

    public static void Save(InputActionAsset actions)
    {
        if (actions == null)
            return;

        PlayerPrefs.SetString(PrefsKey, actions.SaveBindingOverridesAsJson());
    }

    public static void Load(InputActionAsset actions)
    {
        if (actions == null)
            return;

        string json = PlayerPrefs.GetString(PrefsKey, string.Empty);

        if (!string.IsNullOrEmpty(json))
            actions.LoadBindingOverridesFromJson(json);
    }
}
