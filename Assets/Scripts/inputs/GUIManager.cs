using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GUIManager : MonoBehaviour
{

    [SerializeField]
    GameObject panelGui, panelMenu;

    [SerializeField]
    UnityEngine.UI.Toggle toggleGlobalInputs;

    [SerializeField]
    TextMeshProUGUI textTimer, textEndScore;

    gameManager smanager;

    private void Start()
    {
        toggleGlobalInputs.isOn = GuiParameters.useGlobalPOVInputs;
        smanager = this.GetComponent<gameManager>();
        guiStart();
    }

    public void guiStart()
    {
        panelMenu.SetActive(false);
    }

    public void guiEnd(string endText)
    {
        panelMenu.SetActive(true);
        textEndScore.SetText(endText);
    }

    public void updateTextTimer(string s)
    {
        textTimer.text = s;
    }

    public void isGlobalInputsToogled(bool state)
    {
        GuiParameters.useGlobalPOVInputs = state;
    }

    void onToggleGui(bool state)
    {
        panelGui.SetActive(state);
    }

    private void Update()
    {
        //updateTimer();
    }

   /* void updateTimer()
    {
        if(SceneManager.Instance.timer == null)
        {
            return;
        }

        updateTextTimer(SceneManager.Instance.timer.toString());
    }*/
}
