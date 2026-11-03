using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{

    [SerializeField]
    GameObject panelGui;

    [SerializeField]
    Toggle toggleGlobalInputs;

    SceneManager smanager;

    private void Start()
    {
        toggleGlobalInputs.isOn = GuiParameters.useGlobalPOVInputs;
        smanager = this.GetComponent<SceneManager>();
    }

    public void isGlobalInputsToogled(bool state)
    {
        GuiParameters.useGlobalPOVInputs = state;
    }

    void onToggleGui(bool state)
    {
        panelGui.SetActive(state);
    }
}
