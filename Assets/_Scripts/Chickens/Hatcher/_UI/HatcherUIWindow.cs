using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HatcherUIWindow : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            gameObject.transform.Find("HatcherUI").GetComponent<HatcherUI>().closeWindow();
        });
    }
}
