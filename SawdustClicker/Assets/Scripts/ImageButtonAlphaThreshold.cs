using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageButtonAlphaThreshold : MonoBehaviour
{
    private Image _image;
    private void Awake()
    {
        _image = GetComponent<Image>();

        _image.alphaHitTestMinimumThreshold = 0.01f;
    }
}
