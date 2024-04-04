using UnityEngine;
using System.Collections.Generic;

public class SpriteProvider : MonoBehaviour
{
    public static SpriteProvider Instance;

    // Dictionary to store string-sprite pairs
    public Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

    // Structure to hold string-sprite pairs in the inspector
    [System.Serializable]
    public struct StringSpritePair
    {
        public string key;
        public Sprite value;
    }

    // List of string-sprite pairs to be assigned in the inspector
    public List<StringSpritePair> stringSpritePairs = new List<StringSpritePair>();
    public Sprite defaultSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize the sprite dictionary with values from the inspector
    private void InitializeDictionary()
    {
        foreach (StringSpritePair pair in stringSpritePairs)
        {
            spriteDictionary[pair.key] = pair.value;
        }
    }

    // Method to get a sprite by key
    public Sprite GetSprite(string key)
    {
        if (spriteDictionary.ContainsKey(key))
        {
            return spriteDictionary[key];
        }
        else
        {
            Debug.LogWarning("Sprite with key '" + key + "' not found.");
            return null;
        }
    }
}
