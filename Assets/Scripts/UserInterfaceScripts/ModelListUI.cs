using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModelListUI : MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();
    public List<Toggle> toggles = new List<Toggle>();
    public Transform togglesParent; // Parent-Objekt für Toggles

    void Start()
    {
        LoadTogglesFromParent();
        SetUpTogglesWithGameObjects();
    }

    void LoadTogglesFromParent()
    {
        // Leere die bestehende Liste zuerst
        toggles.Clear();

        // Gehe durch alle Kinder des Parent-Objekts und füge sie zur Liste hinzu, wenn sie ein Toggle-Komponente haben
        foreach (Transform child in togglesParent)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggles.Add(toggle);
            }
        }
    }

    void SetUpTogglesWithGameObjects()
    {
        int numItems = Mathf.Min(gameObjects.Count, toggles.Count);
        for (int i = 0; i < numItems; i++)
        {
            if (gameObjects[i] != null && toggles[i] != null)
            {
                TextMeshProUGUI tmpText = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = gameObjects[i].name;
                }
                toggles[i].isOn = gameObjects[i].activeSelf;
                int index = i;  // Lokale Kopie von i für den Lambda-Ausdruck
                toggles[i].onValueChanged.AddListener((isOn) => {
                    ToggleGameObject(index, isOn);
                    // Informiere den ModelKeypadManager über das aktivierte GameObject
                    if (isOn) // Wenn das GameObject aktiviert wird, setze es als Spiegelobjekt
                    {
                        ModelKeypadManager.Instance.SetMirroredObject(gameObjects[index]);
                    }
                });
            }
        }
    }

    void ToggleGameObject(int index, bool isActive)
    {
        if (index >= 0 && index < gameObjects.Count)
        {
            // Setze die Aktivität des GameObjects basierend auf dem Toggle-Zustand
            gameObjects[index].SetActive(isActive);
        }
    }
}
