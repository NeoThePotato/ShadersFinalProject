using UnityEngine;
using UnityEngine.UI;

public class CursorFollower : MonoBehaviour
{
    RectTransform rectTransform;
    [SerializeField] public Vector2 offset = new Vector2(10, 10);

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isActiveAndEnabled)
        {
            Vector2 mousePosition = Input.mousePosition;
            rectTransform.position = mousePosition + offset;
        }
    }
}