using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Oculus.Movement.Effects;
using Oculus.Interaction;

public class VariantListUI : MonoBehaviour
{
    [SerializeField]
    LayerTransformPairChanger mirrorTransformManager;
    [SerializeField]
    LayerMaskManager layerMaskManager;

    [Tooltip("Fill only for VariantList!")]
    public List<GameObject> variantGameObjects = new List<GameObject>();
    public List<InteractableUnityEventWrapper> interactables = new List<InteractableUnityEventWrapper>();
    public Transform interactablesParent;

    void Start()
    {
        LoadInteractablesFromParent();
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
        if (variantGameObjects.Count != 0)
        {
            SetUpInteractablesForVariants();
        }
    }

    void SetUpInteractablesForVariants()
    {
        int numItems = Mathf.Min(variantGameObjects.Count, interactables.Count);
        if (variantGameObjects.Count > interactables.Count)
        {
            Debug.LogWarning("Nicht genügend Interactables vorhanden, um alle Varianten anzuzeigen.");
        }

        for (int i = 0; i < interactables.Count; i++)
        {
            if (i < numItems)
            {
                if (variantGameObjects[i] != null && interactables[i] != null)
                {
                    TextMeshPro tmpText = interactables[i].transform.GetChild(1).transform.GetComponentInChildren<TextMeshPro>();
                    if (tmpText != null)
                    {
                        tmpText.text = variantGameObjects[i].name;
                    }
                    int index = i;
                    // Entferne vorherige Listener
                    interactables[i].WhenSelect.RemoveAllListeners();
                    interactables[i].WhenSelect.AddListener(() => {
                        ToggleVariant(index);
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

    void ToggleVariant(int index)
    {
        layerMaskManager.ToggleEverything();
        if (index >= 0 && variantGameObjects.Count != 0)
        {
            foreach (var variant in variantGameObjects)
            {
                if (variant.activeSelf) { variant.gameObject.SetActive(false); }
            }
            variantGameObjects[index].SetActive(true);
            variantGameObjects[index].transform.parent.transform.parent.gameObject.GetComponent<AVR_Related>().activeMirrored = variantGameObjects[index];

            mirrorTransformManager._lateMirroredObject = variantGameObjects[index].GetComponentInChildren<LateMirroredObject>();
            mirrorTransformManager.ChangeMirrorTransformerModel();
            mirrorTransformManager.SetToAllPairs();
            AVRGameObjectRecorder.Instance.ActivateOtherVariant(variantGameObjects[index].name);
            StudyManager.Instance.SwitchVariantTask();
        }
    }

    public void UpdateVaraintList()
    {
        variantGameObjects.Clear();
        variantGameObjects = AVRGameObjectRecorder.Instance.GetChildrenWithAnimator();
        LoadInteractablesFromParent();
    }
}
