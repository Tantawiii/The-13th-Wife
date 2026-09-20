using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public const string SaveFileName = "Unnamed.json";

    public static SaveManager Instance { get; private set; }

    [SerializeField] private bool encryptData = true;

    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> saveables;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, SaveFileName, encryptData);
        saveables = FindSaveables();

        yield return null;

        LoadGame();
    }

    public void SaveGame()
    {
        foreach (ISaveable saveable in saveables)
            saveable.SaveData(ref gameData);

        dataHandler.SaveData(gameData);
    }

    public void LoadGame()
    {
        saveables ??= FindSaveables();

        gameData = dataHandler.LoadData();

        if (gameData == null)
            gameData = new GameData();

        foreach (ISaveable saveable in saveables)
            saveable.LoadData(gameData);
    }

    [ContextMenu("Delete Save Data")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, SaveFileName, encryptData);
        dataHandler.DeleteData();
        LoadGame();
    }

    private List<ISaveable> FindSaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISaveable>().ToList();
    }
}
