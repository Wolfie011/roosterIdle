using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BreederUIWindow : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            gameObject.transform.Find("BreederUI").GetComponent<BreederUI>().closeWindow();
        });
    }
}
