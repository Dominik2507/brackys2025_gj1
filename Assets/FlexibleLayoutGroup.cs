using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class FlexibleLayoutGroup : MonoBehaviour
{

    [SerializeField] float line_height = 50f;
    [SerializeField] float horizontal_spacing = 10f;

    private void OnEnable()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = Vector2.up;
        rect.anchorMax = Vector2.up;
        
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
    }

    private void OnHierarchyChanged()
    {
        UpdateElementsLayout();
    }

    private void UpdateElementsLayout()
    {
        float line_width = 0;
        float max_line_width = GetComponent<RectTransform>().rect.width;

        float current_row = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject childObject = transform.GetChild(i).gameObject;
            if(childObject.TryGetComponent<ContentSizeFitter>(out ContentSizeFitter childFitter))
            {
                childFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            else
            {
                childObject.AddComponent<ContentSizeFitter>();
                ContentSizeFitter fitter = childObject.GetComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            RectTransform childRect = childObject.GetComponent<RectTransform>();

            float child_width = childRect.sizeDelta.x;
            float child_height = childRect.sizeDelta.y;

            if(line_width + child_width > max_line_width)
            {
                current_row += 1;
                line_width = 0;
            }
            childRect.anchorMin = Vector2.up;
            childRect.anchorMax = Vector2.up;
            childRect.pivot = new Vector2(0.5f, 1);

            childRect.anchoredPosition = new Vector2(line_width + child_width / 2, -current_row * line_height);

            line_width += child_width + horizontal_spacing;

        }
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, line_height * (current_row + 1));
    }

}
