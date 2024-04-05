using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioClip _clip;
    public void OnPointerClick(PointerEventData eventData)
    {
        SawdustManager.Instance.OnTrunkClicked(eventData);
        AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, 0.8f);
    }
}
