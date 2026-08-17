using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationUI : MonoBehaviour
{
    public PathFinder pathFinder;
    public Arrowspawner arrowSpawner;
    public GraphLoader graphLoader;

    public Dropdown startDropdown;
    public Dropdown endDropdown;
    public Button navigateButton;
    public Text navigateButtonText;
    public Image panelBackground;

    private List<string> locationNames = new List<string>()
    {
        "MainGate", "Admin", "Canteen", "Library", "ECE", "MechCivil", "Nursing"
    };

    void Start()
    {
        startDropdown.ClearOptions();
        endDropdown.ClearOptions();
        startDropdown.AddOptions(locationNames);
        endDropdown.AddOptions(locationNames);

        startDropdown.value = 0;
        endDropdown.value = 3;

        if (navigateButtonText != null)
            navigateButtonText.text = "Navigate!";

        if (panelBackground != null)
            panelBackground.color = new Color(0f, 0f, 0f, 0.6f);

        // Listen for dropdown changes
        startDropdown.onValueChanged.AddListener(OnStartChanged);
        endDropdown.onValueChanged.AddListener(OnEndChanged);

        navigateButton.onClick.AddListener(OnNavigateClicked);

        // Initialize filtered options
        UpdateEndOptions();
    }

    void OnStartChanged(int index)
    {
        UpdateEndOptions();
    }

    void OnEndChanged(int index)
    {
        UpdateStartOptions();
    }

    void UpdateEndOptions()
    {
        // Get current start selection
        string selectedStart = startDropdown.options.Count > 0
            ? startDropdown.options[startDropdown.value].text
            : "";

        // Save current end selection
        string previousEndName = endDropdown.options.Count > 0
            ? endDropdown.options[endDropdown.value].text
            : "";

        // Build end options excluding selected start
        List<string> endOptions = new List<string>();
        foreach (string loc in locationNames)
        {
            if (loc != selectedStart)
                endOptions.Add(loc);
        }

        endDropdown.onValueChanged.RemoveListener(OnEndChanged);
        endDropdown.ClearOptions();
        endDropdown.AddOptions(endOptions);

        // Restore previous selection if possible
        int newIndex = endOptions.IndexOf(previousEndName);
        endDropdown.value = newIndex >= 0 ? newIndex : 0;
        endDropdown.onValueChanged.AddListener(OnEndChanged);
    }

    void UpdateStartOptions()
    {
        // Get current end selection
        string selectedEnd = endDropdown.options.Count > 0
            ? endDropdown.options[endDropdown.value].text
            : "";

        // Save current start selection
        string previousStartName = startDropdown.options.Count > 0
            ? startDropdown.options[startDropdown.value].text
            : "";

        // Build start options excluding selected end
        List<string> startOptions = new List<string>();
        foreach (string loc in locationNames)
        {
            if (loc != selectedEnd)
                startOptions.Add(loc);
        }

        startDropdown.onValueChanged.RemoveListener(OnStartChanged);
        startDropdown.ClearOptions();
        startDropdown.AddOptions(startOptions);

        // Restore previous selection if possible
        int newIndex = startOptions.IndexOf(previousStartName);
        startDropdown.value = newIndex >= 0 ? newIndex : 0;
        startDropdown.onValueChanged.AddListener(OnStartChanged);
    }

    void OnNavigateClicked()
    {
        if (graphLoader == null || graphLoader.nodeMap.Count == 0)
        {
            Debug.LogWarning("Graph not loaded yet!");
            return;
        }

        string start = startDropdown.options[startDropdown.value].text;
        string end = endDropdown.options[endDropdown.value].text;

        Debug.Log("Navigating from " + start + " to " + end);

        pathFinder.startNodeId = start;
        pathFinder.endNodeId = end;

        StartCoroutine(RespawnWithDelay());
    }

    IEnumerator RespawnWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        arrowSpawner.RespawnPath();
    }
}



// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class NavigationUI : MonoBehaviour
// {
//     public PathFinder pathFinder;
//     public Arrowspawner arrowSpawner;
//     public GraphLoader graphLoader;
//     public Dropdown startDropdown;
//     public Dropdown endDropdown;
//     public Button navigateButton;
//     public Text navigateButtonText;
//     public Image panelBackground;

//     private List<string> locationNames = new List<string>()
//     {
//         "MainGate", "Admin", "Canteen", "Library", "ECE", "MechCivil", "Nursing"
//     };

//     void Start()
//     {
//         startDropdown.ClearOptions();
//         endDropdown.ClearOptions();
//         startDropdown.AddOptions(locationNames);
//         endDropdown.AddOptions(locationNames);

//         startDropdown.value = 0;
//         endDropdown.value = 3;

//         if (navigateButtonText != null)
//             navigateButtonText.text = "Navigate!";

//         if (panelBackground != null)
//             panelBackground.color = new Color(0f, 0f, 0f, 0.6f);

//         // Listen for dropdown changes
//         startDropdown.onValueChanged.AddListener(OnStartChanged);
//         endDropdown.onValueChanged.AddListener(OnEndChanged);

//         navigateButton.onClick.AddListener(OnNavigateClicked);

//         // Initialize filtered options
//         UpdateEndOptions();
//         UpdateStartOptions();
//     }

//     void OnStartChanged(int index)
//     {
//         UpdateEndOptions();
//     }

//     void OnEndChanged(int index)
//     {
//         UpdateStartOptions();
//     }

//     void UpdateEndOptions()
//     {
//         string selectedStart = locationNames[startDropdown.value];

//         // Build end options excluding selected start
//         List<string> endOptions = new List<string>();
//         foreach (string loc in locationNames)
//         {
//             if (loc != selectedStart)
//                 endOptions.Add(loc);
//         }

//         int previousEndValue = endDropdown.value;
//         string previousEndName = endDropdown.options.Count > 0
//             ? endDropdown.options[previousEndValue].text
//             : "";

//         endDropdown.ClearOptions();
//         endDropdown.AddOptions(endOptions);

//         // Try to restore previous selection
//         int newIndex = endOptions.IndexOf(previousEndName);
//         endDropdown.value = newIndex >= 0 ? newIndex : 0;
//     }

//     void UpdateStartOptions()
//     {
//         string selectedEnd = locationNames[endDropdown.value];

//         // Build end options — need actual selected end name
//         string actualEndName = endDropdown.options.Count > 0
//             ? endDropdown.options[endDropdown.value].text
//             : "";

//         List<string> startOptions = new List<string>();
//         foreach (string loc in locationNames)
//         {
//             if (loc != actualEndName)
//                 startOptions.Add(loc);
//         }

//         int previousStartValue = startDropdown.value;
//         string previousStartName = startDropdown.options.Count > 0
//             ? startDropdown.options[previousStartValue].text
//             : "";

//         startDropdown.ClearOptions();
//         startDropdown.AddOptions(startOptions);

//         int newIndex = startOptions.IndexOf(previousStartName);
//         startDropdown.value = newIndex >= 0 ? newIndex : 0;
//     }

//     void OnNavigateClicked()
//     {
//         if (graphLoader.nodeMap.Count == 0)
//         {
//             Debug.LogWarning("Graph not loaded yet!");
//             return;
//         }
//         string start = startDropdown.options[startDropdown.value].text;
//         string end = endDropdown.options[endDropdown.value].text;

//         pathFinder.startNodeId = start;
//         pathFinder.endNodeId = end;

//         StartCoroutine(RespawnWithDelay());
//     }

//     System.Collections.IEnumerator RespawnWithDelay()
//     {
//         yield return new WaitForSeconds(0.5f);
//         arrowSpawner.RespawnPath();
//     }
// }



// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class NavigationUI : MonoBehaviour
// {
//     public PathFinder pathFinder;
//     public Arrowspawner arrowSpawner;

//     public Dropdown startDropdown;
//     public Dropdown endDropdown;
//     public Button navigateButton;
//     public Text navigateButtonText;
//     public Image panelBackground;

//     private List<string> locationNames = new List<string>()
//     {
//         "MainGate", "Admin", "Canteen", "Library", "ECE", "MechCivil", "Nursing"
//     };

//     void Start()
//     {
//         // Populate dropdowns
//         startDropdown.ClearOptions();
//         endDropdown.ClearOptions();
//         startDropdown.AddOptions(locationNames);
//         endDropdown.AddOptions(locationNames);

//         startDropdown.value = 0;
//         endDropdown.value = 3;

//         // Style button text
//         if (navigateButtonText != null)
//             navigateButtonText.text = "Navigate";

//         // Style panel
//         if (panelBackground != null)
//             panelBackground.color = new Color(0f, 0f, 0f, 0.6f);

//         navigateButton.onClick.AddListener(OnNavigateClicked);
//     }

//     void OnNavigateClicked()
//     {
//         string start = locationNames[startDropdown.value];
//         string end = locationNames[endDropdown.value];

//         if (start == end)
//         {
//             Debug.LogWarning("Start and End are the same!");
//             return;
//         }

//         pathFinder.startNodeId = start;
//         pathFinder.endNodeId = end;

//         StartCoroutine(RespawnWithDelay());
//     }

//     System.Collections.IEnumerator RespawnWithDelay()
//     {
//         yield return new WaitForSeconds(0.5f);
//         arrowSpawner.RespawnPath();
//     }
// }

