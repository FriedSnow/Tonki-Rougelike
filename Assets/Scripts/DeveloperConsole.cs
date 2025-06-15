using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public class DeveloperConsole : StandardItem
{
    private static DeveloperConsole instance;
    private bool isActive = false;
    private string inputBuffer = "";
    void Awake()
    {
        GetPlayer();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (textSlide == null)
        {
            textSlide = FindObjectOfType<TextSlide>();
            if (textSlide == null)
            {
                Debug.LogError("TextSlide not found!");
            }
        }
    }
    private void Start()
    {
        if (textSlide == null)
        {
            textSlide = FindObjectOfType<TextSlide>();
            if (textSlide == null)
            {
                Debug.LogError("TextSlide not found!");
            }
        }
        GetPlayer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) // Change to F1 key for toggling console
        {
            ToggleConsole();
        }

        if (isActive && Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteCommand(inputBuffer);
            inputBuffer = "";
        }
        else if (isActive && Input.GetKeyDown(KeyCode.Backspace))
        {
            if (inputBuffer.Length > 0)
                inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 1);
        }
    }

    void OnGUI()
    {
        if (!isActive) return;

        GUI.Box(new Rect(0, 0, Screen.width, 40), "");
        inputBuffer = GUI.TextField(new Rect(10, 10, Screen.width - 20, 20), inputBuffer);
    }

    void ToggleConsole()
    {
        isActive = !isActive;
        Time.timeScale = isActive ? 0f : 1f; // Pause the game when console is active
    }

    void ExecuteCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return;

        string[] parts = command.Split(' ');
        string commandName = parts[0];
        string[] args = parts.Length > 1 ? parts[1..] : new string[0];

        switch (commandName.ToLower())
        {
            case "spawn":
                SpawnPrefab(args);
                break;
            case "debug":
                player.coins += 10;
                player.Heal(10);
                player.AddArmor(10);
                break;
            case "unlock":
                PlayerPrefs.SetInt("Unlockable3", 1);
                textSlide.ShowItemName("Достижение получено!", Color.cyan, "...");
                break;
            default:
                Debug.Log($"Unknown command: {commandName}");
                break;
        }
    }

    void SpawnPrefab(string[] args)
    {
        if (args.Length < 2)
        {
            Debug.LogError("Not enough arguments for spawn command.");
            return;
        }

        string qualityShortcut = args[0]; // The shortcut for quality level
        string prefabName = args[1]; // The name of the prefab you want to spawn

        string fullPath = GetFullPathFromShortcut(qualityShortcut, prefabName);

        if (string.IsNullOrEmpty(fullPath))
        {
            Debug.LogError($"No full path found for shortcut: {qualityShortcut} and prefab name: {prefabName}");
            return;
        }

        string label = GetLabelFromPath(fullPath);

        if (string.IsNullOrEmpty(label))
        {
            Debug.LogError($"No label found for path: {fullPath}");
            return;
        }

        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(label, null);
        handle.Completed += (h) =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                var assets = h.Result;
                var filteredAssets = assets.Where(asset => asset.name == prefabName).ToList();

                if (filteredAssets.Count > 0)
                {
                    GameObject player = FindPlayer();
                    if (player != null)
                    {
                        Instantiate(filteredAssets[0], player.transform.position + player.transform.forward + new Vector3(0, 0, 15), Quaternion.identity);
                    }
                    else
                    {
                        Debug.LogError("Player object not found in scene.");
                    }
                }
                else
                {
                    Debug.LogError($"Prefab not found with name: {prefabName} in label: {label}");
                }
            }
            else
            {
                Debug.LogError($"Failed to load assets with label: {label}");
            }

            // Release the handle after use
            Addressables.Release(h);
        };
    }

    string GetFullPathFromShortcut(string qualityShortcut, string prefabName)
    {
        string basePath = "";

        switch (qualityShortcut.ToLower())
        {
            case "q0":
                basePath = "Items/ItemsByQuality/0Quality/";
                break;
            case "q1":
                basePath = "Items/ItemsByQuality/1Quality/";
                break;
            case "q2":
                basePath = "Items/ItemsByQuality/2Quality/";
                break;
            case "q3":
                basePath = "Items/ItemsByQuality/3Quality/";
                break;
            case "p":
                basePath = "Items/Pickups/";
                break;
            case "e":
                basePath = "Enemies/";
                break;
            default:
                Debug.LogError($"Unknown quality shortcut: {qualityShortcut}");
                return null;
        }

        return basePath + prefabName;
    }

    string GetLabelFromPath(string path)
    {
        if (path.StartsWith("Items/ItemsByQuality/0Quality/"))
        {
            return "0QualityItems";
        }
        else if (path.StartsWith("Items/ItemsByQuality/1Quality/"))
        {
            return "1QualityItems";
        }
        else if (path.StartsWith("Items/ItemsByQuality/2Quality/"))
        {
            return "2QualityItems";
        }
        else if (path.StartsWith("Items/ItemsByQuality/3Quality/"))
        {
            return "3QualityItems";
        }
        else if (path.StartsWith("Items/Pickups/"))
        {
            return "Pickups";
        }
        else if (path.StartsWith("Enemies/"))
        {
            return "Enemies";
        }

        Debug.LogError($"Unknown path: {path}");
        return null;
    }

    GameObject FindPlayer()
    {
        TankController tankController = FindObjectOfType<TankController>();
        if (tankController != null)
        {
            return tankController.gameObject;
        }
        else
        {
            Debug.LogWarning("No TankController found in scene.");
            return null;
        }
    }

    void CustiomCommand(string command)
    {

    }
    protected override void ApplyEffect() { }
}