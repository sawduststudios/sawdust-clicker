using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    private TextMeshProUGUI timeText;

    private void Awake()
    {
        // Get the TextMeshProUGUI component from the GameObject
        timeText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the total time from SawdustManager.Instance.TotalTime
        float totalTime = SawdustManager.Instance.TotalTime;

        // Calculate minutes and seconds
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);

        // Format the time into MM:SS format
        string formattedTime = string.Format("{0}:{1:00}", minutes, seconds);

        // Display the formatted time in the TextMeshProUGUI component
        timeText.text = formattedTime;
    }
}
