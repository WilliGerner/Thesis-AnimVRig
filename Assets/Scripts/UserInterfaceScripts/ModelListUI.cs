using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Oculus.Movement.Effects;
using Oculus.Interaction;

public class ModelListUI : MonoBehaviour
{
    [SerializeField]
    LayerTransformPairChanger mirrorTransformManager;

    [Tooltip("Fill only for ModelList!")]
    public List<GameObject> modelGameObjects = new List<GameObject>();
    public List<InteractableUnityEventWrapper> interactables = new List<InteractableUnityEventWrapper>();
    public Transform interactablesParent;

    [SerializeField] VariantListUI variantList;

    void Start()
    {
        LoadInteractablesFromParent();
        if (modelGameObjects.Count != 0)
        {
            SetUpInteractablesForModels();
        }
    }

    void LoadInteractablesFromParent()
    {
        interactables.Clear();
        foreach (Transform child in interactablesParent)
        {
            InteractableUnityEventWrapper interactable = child.GetComponent<InteractableUnityEventWrapper>();
            if (interactable != null)
            {
                interactables.Add(interactable);
            }
        }
    }

    void SetUpInteractablesForModels()
    {
        int numItems = Mathf.Min(modelGameObjects.Count, interactables.Count);
        if (modelGameObjects.Count > interactables.Count)
        {
            Debug.LogWarning("Nicht genügend Interactables vorhanden, um alle Modelle anzuzeigen.");
        }

        for (int i = 0; i < interactables.Count; i++)
        {
            if (i < numItems)
            {
                if (modelGameObjects[i] != null && interactables[i] != null)
                {
                    TextMeshPro tmpText = interactables[i].transform.GetChild(1).transform.GetComponentInChildren<TextMeshPro>();
                    if (tmpText != null)
                    {
                        tmpText.text = modelGameObjects[i].name;
                    }
                    int index = i;
                    // Entferne vorherige Listener
                    interactables[i].WhenSelect.RemoveAllListeners();
                    // Füge neuen Listener hinzu
                    interactables[i].WhenSelect.AddListener(() => {
                        ToggleModel(index);
                    });
                    interactables[i].gameObject.SetActive(true);
                }
            }
            else
            {
                interactables[i].gameObject.SetActive(false);
            }
        }
    }

    void ToggleModel(int index)
    {
        if (index >= 0 && modelGameObjects.Count != 0)
        {
            AVRGameObjectRecorder.Instance.ActivateOtherModel(modelGameObjects[index].name);
            mirrorTransformManager._lateMirroredObject = modelGameObjects[index].GetComponentInChildren<LateMirroredObject>();
            mirrorTransformManager.ChangeMirrorTransformerModel();
            variantList.UpdateVaraintList();
            mirrorTransformManager.SetToAllPairs(true);
        }
    }
}
