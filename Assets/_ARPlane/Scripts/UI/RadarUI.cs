using UnityEngine;
using UniversoAumentado.ARCraft.AR;

public class RadarUI : MonoBehaviour
{
    [SerializeField] private ARCraft arCraft;
    [SerializeField] private RectTransform indicator;

    private Vector2 screenCenter;
    private Rect screenRect;

    void Awake()
    {
        // todo: what if screen orientation changes ?
        screenCenter = new Vector2(Screen.width, Screen.height) / 2;
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
    }

    void Update()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(arCraft.transform.position);
        if (screenRect.Contains(screenPoint))
        {
            // Ship is currently on screen, no need to show the indicator
            indicator.gameObject.SetActive(false);
        }
        else
        {
            //Ship is off screen
            indicator.gameObject.SetActive(true);

            screenPoint -= screenCenter;
            screenPoint.Normalize();
            var diameter = Mathf.Min(Screen.width, Screen.height);
            screenPoint *= diameter / 2 - indicator.sizeDelta.x / 2; //we assume portrait mode
            indicator.anchoredPosition = screenPoint;

            float angle = Mathf.Atan2(screenPoint.y, screenPoint.x) * Mathf.Rad2Deg;
            indicator.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
