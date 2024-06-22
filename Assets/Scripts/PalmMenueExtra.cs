using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmMenueExtra : MonoBehaviour
{
    private bool _ExtraModelEnabled;
    [SerializeField]
    GameObject Manager; // <-- Should be a Manager Script no GO

    [SerializeField]
    private GameObject _rotationEnabledIcon;

    [SerializeField]
    private GameObject _rotationDisabledIcon;

    private void Start()
    {
       ToggleLogic();
    }

    public void ToggleLogic()
    {
        _ExtraModelEnabled = !_ExtraModelEnabled;
        // Update der UI-Icons
        _rotationEnabledIcon.SetActive(_ExtraModelEnabled);
        _rotationDisabledIcon.SetActive(!_ExtraModelEnabled);
    }


    public void ToggleExtraModel()
    {
        _ExtraModelEnabled = !_ExtraModelEnabled;

        if (_ExtraModelEnabled)
        {
            DisableModel();
           // EnableModel(); // Aufruf der Methode, wenn das Extra Model aktiviert wird
        }
        else
        {
            EnableModel(); // Aufruf der Methode, wenn das Extra Model deaktiviert wird
        }

        // Update der UI-Icons
        _rotationEnabledIcon.SetActive(_ExtraModelEnabled);
        _rotationDisabledIcon.SetActive(!_ExtraModelEnabled);
    }

    // Methode wird aufgerufen, wenn die Rotation aktiviert wird
    private void EnableModel()
    {
        //Manager.ActivateAdditionalModel();
        // Hier können Sie Code hinzufügen, der ausgeführt wird, wenn die Rotation aktiviert wird
    }

    // Methode wird aufgerufen, wenn die Rotation deaktiviert wird
    private void DisableModel()
    {
        //miaManager.DeactivateAdditionalModel();
        // Hier können Sie Code hinzufügen, der ausgeführt wird, wenn die Rotation deaktiviert wird
    }

}
