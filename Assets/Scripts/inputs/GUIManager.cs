using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{

    [SerializeField]
    GameObject panelGui;

    [SerializeField]
    Toggle toggleGlobalInputs;

    private void Start()
    {
        toggleGlobalInputs.isOn = GuiParameters.useGlobalPOVInputs;
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
