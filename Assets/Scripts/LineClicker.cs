using UnityEngine;
using UnityEngine.EventSystems;

public class LineClicker : MonoBehaviour
{
    // ProductionLine (YENÝ ÝSÝM)
    private ProductionLine myLine;

    void Start()
    {
        myLine = GetComponent<ProductionLine>();
    }

    void OnMouseDown()
    {
        // UIManager (YENÝ ÝSÝM) ve IsUIOpen (YENÝ ÖZELLÝK)
        if (UIManager.Instance != null && !UIManager.Instance.IsUIOpen && !IsPointerOverUI())
        {
            if (myLine != null)
            {
                UIManager.Instance.OpenUpgradePanel(myLine);
            }
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
        return false;
    }
}