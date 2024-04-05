using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _clickAmmountText;
    [SerializeField] private float _velocityDecayRate = 1500f;
    [SerializeField] private float _timeBeforeFade = 0.6f;
    [SerializeField] private float _fadeSpeed = 3f;

    private CanvasGroup _canvasGroup;

    private Vector2 _currVelocity;

    private float _timer = 0f;
    private float _alpha = 1f;


    public static PopUpText Create(double ammount, Vector3 position)
    {
        GameObject popUp = Instantiate(SawdustManager.Instance.SawdustTextPopUp, SawdustManager.Instance.MainCanvas.transform);
        //popUp.transform.position = SawdustManager.Instance.MainCanvas.transform.position;
        popUp.transform.position = position;

        PopUpText popUpComp = popUp.GetComponent<PopUpText>();
        popUpComp.Init(ammount);

        return popUpComp;
    }

    public void Init(double ammount)
    {
        _clickAmmountText.text = "+" + ammount.ToFormattedStr();

        float randomX = Random.Range(-300f, 300f);
        _currVelocity = new Vector2(randomX, SawdustManager.Instance.PopUpVelocity);
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        // move
        _currVelocity.y -= _velocityDecayRate * Time.deltaTime;
        transform.Translate(_currVelocity * Time.deltaTime);

        // fade
        _timer += Time.deltaTime;
        if (_timer >= _timeBeforeFade)
        {
            _alpha -= _fadeSpeed * Time.deltaTime;
            _canvasGroup.alpha = _alpha;

            if (_alpha <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
