using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    public int clickThreshold = 3; // Number of clicks needed before resetting the game
    private int clickCount = 0; // Current count of clicks

    private void Start()
    {
        // Ensure the button has an onClick listener attached
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogError("Button component not found on GameObject.");
        }

        StartCoroutine(RefreshClickCount());
    }

    // Coroutine to refresh the click count after a delay
    private IEnumerator RefreshClickCount()
    {
        while (true) 
        {
            // Wait for 1 second
            yield return new WaitForSeconds(8f);
            // Reset click count
            clickCount = 0;
        }
    }

    private void OnClick()
    {
        // Increment click count
        clickCount++;

        // Check if click threshold is reached
        if (clickCount >= clickThreshold)
        {
            // Reset click count
            clickCount = 0;

            // Call the ResetGame method on SawdustManager.Instance
            SawdustManager.Instance.ResetGame();
        }
    }
}
