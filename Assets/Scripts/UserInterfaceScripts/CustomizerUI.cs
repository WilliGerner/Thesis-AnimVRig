using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomizerUI : MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();
    public List<Toggle> toggles = new List<Toggle>();
    public Transform togglesParent; // Parent-Objekt f�r Toggles

    void Start()
    {
        LoadTogglesFromParent();
        SetUpTogglesWithGameObjects();
    }

    void LoadTogglesFromParent()
    {
        // Leere die bestehende Liste zuerst
        toggles.Clear();

        // Gehe durch alle Kinder des Parent-Objekts und f�ge sie zur Liste hinzu, wenn sie ein Toggle-Komponente haben
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
        // �berpr�fe, ob die Listen gleich lang sind und f�ge Listener hinzu
        int numItems = Mathf.Min(gameObjects.Count, toggles.Count);
        for (int i = 0; i < numItems; i++)
        {
            if (gameObjects[i] != null && toggles[i] != null)
            {
                // Setze den Text des TextMeshProUGUI, das als Kind des Toggles angenommen wird
                TextMeshProUGUI tmpText = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = gameObjects[i].name;
                }

                // Aktueller Zustand des GameObjects setzen
                toggles[i].isOn = gameObjects[i].activeSelf;

                // F�ge einen Listener hinzu, der die Aktivit�t des GameObjects steuert
                int index = i;  // Lokale Kopie von i f�r den Lambda-Ausdruck
                toggles[i].onValueChanged.AddListener((isOn) => {
                   // ToggleGameObject(index, isOn);
                });
            }
        }
    }

    void ToggleGameObject(int index, bool isActive)
    {
        if (index >= 0 && index < gameObjects.Count)
        {
            // Setze die Aktivit�t des GameObjects basierend auf dem Toggle-Zustand
            gameObjects[index].SetActive(isActive);
        }
    }
}
