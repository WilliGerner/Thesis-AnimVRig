using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Oculus.Movement.Effects;

public class ModelListUI : MonoBehaviour
{
    [SerializeField]
    MirroredTransformManager mirrorTransformManager;

    [Tooltip("Fill only for ModelList!")]
    public List<GameObject> modelGameObjects = new List<GameObject>(); // Should Update on isFOrModels =false if the true version is called. Should change the Varaints to the new current Varaint
    [Tooltip("Fill only for VaraintList!")]
    public List<GameObject> variantGameObjects = new List<GameObject>();
    public List<Toggle> toggles = new List<Toggle>();
    public Transform togglesParent; // Parent-Objekt für Toggles
    void Start()
    {
        LoadTogglesFromParent();
       if(modelGameObjects.Count !=0) SetUpTogglesForModels();
        else if (modelGameObjects.Count == 0) SetUpTogglesForVariants();
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

    void SetUpTogglesForModels()
    {
        int numItems = Mathf.Min(modelGameObjects.Count, toggles.Count);
        for (int i = 0; i < numItems; i++)
        {
            if (modelGameObjects[i] != null && toggles[i] != null)
            {
                TextMeshProUGUI tmpText = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = modelGameObjects[i].name;
                }
                toggles[i].isOn = modelGameObjects[i].activeSelf;
                int index = i;  // Lokale Kopie von i für den Lambda-Ausdruck
                toggles[i].onValueChanged.AddListener((isOn) => {
                    ToggleGameObject(index);
                    // Informiere den ModelKeypadManager über das aktivierte GameObject
                    if (isOn) // Wenn das GameObject aktiviert wird, setze es als Spiegelobjekt
                    {
                        //ModelKeypadManager.Instance.SetMirroredObject(modelGameObjects[index]);
                    }
                });
            }
        }
    }

    void SetUpTogglesForVariants()
    {
        int numItems = Mathf.Min(variantGameObjects.Count, toggles.Count);
        for (int i = 0; i < numItems; i++)
        {
            if (variantGameObjects[i] != null && toggles[i] != null)
            {
                TextMeshProUGUI tmpText = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = variantGameObjects[i].name;
                }
                toggles[i].isOn = variantGameObjects[i].activeSelf;
                int index = i;  // Lokale Kopie von i für den Lambda-Ausdruck
                toggles[i].onValueChanged.AddListener((isOn) => {
                    ToggleGameObject(index);
                    // Informiere den ModelKeypadManager über das aktivierte GameObject
                    if (isOn) // Wenn das GameObject aktiviert wird, setze es als Spiegelobjekt
                    {
                        ModelKeypadManager.Instance.SetMirroredObject(variantGameObjects[index]);
                    }
                });
            }
        }
    }

    void ToggleGameObject(int index)
    {
      

        if (index >= 0 && modelGameObjects.Count ==0) 
        {
            foreach (var variant in variantGameObjects)
            {
                if (variant.activeSelf) { variant.gameObject.SetActive(false); }
            }
            variantGameObjects[index].SetActive(true);
            mirrorTransformManager._lateMirroredObject = variantGameObjects[index].GetComponentInChildren<LateMirroredObject>(); // Set new LateMirror after Variant change.
            mirrorTransformManager.ToggleEverything(true);
            mirrorTransformManager.ChangeMirrorTransformerModel(variantGameObjects[index]);
            variantGameObjects[index].transform.parent.transform.parent.gameObject.GetComponent<AVR_Related>().activeMirrored = variantGameObjects[index];
        }

        if (modelGameObjects.Count != 0) // Only for the Model Version
        {
            AVRGameObjectRecorder.Instance.ActivateOtherModel(modelGameObjects[index].name);
            mirrorTransformManager._lateMirroredObject = modelGameObjects[index].GetComponentInChildren<LateMirroredObject>();// Set new LateMirror after Model change.
            mirrorTransformManager.ToggleEverything(true);
            UpdateVaraintList();
            mirrorTransformManager.ChangeMirrorTransformerModel(modelGameObjects[index]);
        }
    }

    void UpdateVaraintList()
    {
        variantGameObjects.Clear();
        variantGameObjects = AVRGameObjectRecorder.Instance.GetChildrenWithAnimator();
        SetUpTogglesForVariants();
    }
}
