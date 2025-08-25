using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public abstract class SaveGameManager : MonoBehaviour
{
    // Override this in derived classes to provide custom data to save
    public abstract object GetSaveData();

    // Override this in derived classes to load custom data
    public abstract void LoadSaveData(object data);

    // Save data to JSON file (updates if file exists)
    public virtual void SaveToJson(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        object saveData = GetSaveData();
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);

        File.WriteAllText(path, json);
    }

    // Load data from JSON file
    public virtual void LoadFromJson(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            object loadedData = JsonConvert.DeserializeObject(json, GetSaveData().GetType());
            LoadSaveData(loadedData);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + path);
        }
    }
}

public class GameManager : MonoBehaviour
{

    // public static GameManager instance; // Singleton instance of the GameManager

    public delegate void OnDamageTaken(float damage); // Delegate for damage taken event
    public event OnDamageTaken onDamageTaken; // Event to notify when damage is taken

    public delegate void OnHeal(float amount); // Delegate for healing event
    public event OnHeal onHeal; // Event to notify when healing is done

    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    Debug.LogWarning("Created a new GameManager instance because one wasn't found in the scene.");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            //Optional: DontDestroyOnLoad(this.gameObject);
        }
    }

    public string fileName = "gamestate";

    int nextLevelExperience, skillPoints; // Tracks the current experience points
    float currentStamina, currentHealth; // Tracks the current stamina and health

    void Start()
    {
        // Load player stats from JSON if file exists
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerStats loadedData = JsonConvert.DeserializeObject<PlayerStats>(json);
        }
    }

    [System.Obsolete]
    public void NewGame()
    {
        // Get PlayerStats and WeaponData components
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        WeaponData weaponData = FindObjectOfType<WeaponData>();

        // Create a dictionary to hold all data
        var saveData = new Dictionary<string, object>();

        // Serialize PlayerStats if available
        if (playerStats != null)
        {
            // Use JsonConvert to serialize the PlayerStats object
            string playerStatsJson = JsonConvert.SerializeObject(playerStats, Formatting.Indented);
            // Deserialize back to a dictionary for merging
            var playerStatsDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(playerStatsJson);
            saveData["PlayerStats"] = playerStatsDict;
        }

        // Serialize WeaponData if available
        if (weaponData != null)
        {
            string weaponDataJson = JsonConvert.SerializeObject(weaponData, Formatting.Indented);
            var weaponDataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(weaponDataJson);
            saveData["WeaponData"] = weaponDataDict;
        }

        // Serialize the combined data to JSON
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, GameManager.instance.fileName);

        File.WriteAllText(path, json);
        Debug.Log("Game state saved to: " + path);
        Debug.Log("Button clicked: Starting game...");
    }

    public void DealDamage(float damage) => onDamageTaken?.Invoke(damage); // Notify subscribers about the damage taken
    public void Heal(float amount) => onHeal?.Invoke(-amount); // Notify subscribers about the healing done
}
