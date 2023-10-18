using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class Helpers
{
    public static TextMesh CreateWorldText(
        Transform parent,
        string text,
        Vector3 localPosition,
        int fontSize,
        Color color,
        TextAnchor textAnchor,
        TextAlignment textAlignment,
        int sortingOrder)
    {
        /*
         * Create a piece of text on a given GameObject 'parent'
         */

        // Create a gameObject and make it a child of 'parent'
        GameObject gameObject = new GameObject("World Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;

        // Set Text 
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.characterSize = 0.03f;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

        return textMesh;
    }

    public static Vector2 GetMouseWorldPosition()
    {
        Camera worldCamera = Camera.main;
        Vector3 mousePosition = worldCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mousePosition.x, mousePosition.y);
    }
}
