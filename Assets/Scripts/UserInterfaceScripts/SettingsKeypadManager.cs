using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsKeypadManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip ("1 means the first Top Left, 9 mean the last Bottom Right.")] 
     List<GameObject> all9SettingBtns = new List<GameObject>();

    [SerializeField]
    OVRPassthroughLayer passthroughLayer;
    [SerializeField]
    GameObject environment;
    bool passthroughActiv;

    [SerializeField]
    OVRManager ovrManager;

    public void SwitchPassthrough()
    {
        if (passthroughActiv)
        {
            environment.SetActive(true);
            passthroughLayer.textureOpacity = 0;
            passthroughLayer.hidden = true;
         //   StartCoroutine(DisableMixedRealityRoutine(false));
            passthroughActiv = false;
        }
        else
        {
            environment.SetActive(false);
            passthroughLayer.textureOpacity = 1.0f;
           passthroughLayer.hidden = false;
          //  StartCoroutine(DisableMixedRealityRoutine(true));
            passthroughActiv = true;
        }
    }

    private IEnumerator DisableMixedRealityRoutine(bool activ)
    {
        yield return new WaitForEndOfFrame();

        ovrManager.isInsightPassthroughEnabled = activ;
    }
}
