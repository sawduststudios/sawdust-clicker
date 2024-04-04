using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RandomBoost : MonoBehaviour, IPointerClickHandler
{
    public SORandomBoost Boost;

    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void InitDissapear(float duration)
    {
        StartCoroutine(Dissapear(duration));
    }

    public void Init()
    {
        img.sprite = Boost.Sprite;
    }

    IEnumerator Dissapear(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Boost.ApplyBoost();
        Destroy(gameObject);
    }
}
