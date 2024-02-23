using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUI : MonoBehaviour
{
    [SerializeField] Slider timeSlider;
    [SerializeField] Slider maxTilesSlider;
    [SerializeField] Button generateButton;
    [SerializeField] TMP_Dropdown moduleSelector;
    [SerializeField] Toggle instantToggle;

    public Toggle breakpointToggle;
    [SerializeField] Slider breakpointSlider;

    [SerializeField] TextMeshProUGUI maxTilesText;
    [SerializeField] TextMeshProUGUI dataText;
    [SerializeField] TextMeshProUGUI timeText;

    bool simulating = false;
    float timeSpentSimulating = 0;
    public float TimeSliderValue { get { return timeSlider.value; } }
    public int maxSliderValue {  get {  return (int)maxTilesSlider.value; } }
    public string GetCurrentModule { get { return moduleSelector.options[moduleSelector.value].text; } private set { } }

    public bool isInstant {  get { return instantToggle.isOn; } }

    public float breakPointValue {  get {  return breakpointSlider.value; } }

    public void SetGenerationOptions(List<string> allModules)
    {
        moduleSelector.ClearOptions();

        moduleSelector.AddOptions(allModules);
    }

    public void StartSession()
    {
        simulating = true;
        timeSpentSimulating = 0f;
        generateButton.interactable = false;
    }

    public void StopSession()
    {
        simulating = false;
    }

    public void ClearSession()
    {
        generateButton.interactable = true;
        dataText.text = "Open connectors in world: \n" + 0 + "\n" +
    "Tiles spawned: \n" + 0 + "\n" +
    "Time spent in simulation: " + 0;
    }

    public void WriteToUI(int connectors, int generatedTiles)
    {
        maxTilesText.text = "Max Tiles: " + maxTilesSlider.value;
        timeText.text = "Update Time: " + timeSlider.value;

        if (simulating)
        {
            timeSpentSimulating += Time.deltaTime;

            dataText.text = "Open connectors in world: \n" + connectors + "\n" +
                "Tiles spawned: \n" + generatedTiles + "\n" +
                "Time spent in simulation: " + timeSpentSimulating.ToString("F1");
        }
    }

    public void SetDataText(int connectorCount, int tileCount)
    {
        dataText.text = "Open connectors in world: \n" + connectorCount + "\n" +
    "Tiles spawned: \n" + tileCount + "\n" +
    "Time spent in simulation: " + "Instant Generation";
    }
}
