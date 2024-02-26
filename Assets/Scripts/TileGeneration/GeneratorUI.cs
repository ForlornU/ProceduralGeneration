using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUI : MonoBehaviour
{
    [SerializeField] Button generateButton;

    [SerializeField] TextMeshProUGUI maxTilesText;
    [SerializeField] TextMeshProUGUI dataText;
    [SerializeField] TextMeshProUGUI timeText;

    bool paused = false;
    bool simulating = false;
    float timeSpentSimulating = 0;

    public void PauseSession()
    {
        paused = !paused;

        if (paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
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
