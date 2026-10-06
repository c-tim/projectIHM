using UnityEngine;

public class GUIManager : MonoBehaviour
{

    [SerializeField]
    GameObject panelGui;


    void onToggleGui(bool state)
    {
        panelGui.SetActive(state);
    }
}
