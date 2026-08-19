using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardDrawing : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.01f;

    private LineRenderer currentLine;
    private readonly List<Vector3> points = new();

    private void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void Update()
    {
        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            StartLine();
        }

        if (touch.press.isPressed)
        {
            Vector2 touchPosition = touch.position.ReadValue();
            DrawAt(touchPosition);
        }

        if (touch.press.wasReleasedThisFrame)
        {
            EndLine();
        }
    }

    private void StartLine()
    {
        GameObject lineObject = new GameObject("DrawingLine");

        lineObject.transform.SetParent(transform);

        currentLine = lineObject.AddComponent<LineRenderer>();

        currentLine.material = lineMaterial;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.useWorldSpace = true;

        points.Clear();
    }

    private void DrawAt(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject != gameObject)
                return;

            Vector3 point = hit.point;

            if (points.Count > 0)
            {
                if (Vector3.Distance(points[^1], point) < 0.005f)
                    return;
            }

            points.Add(point);

            currentLine.positionCount = points.Count;
            currentLine.SetPositions(points.ToArray());
        }
    }

    private void EndLine()
    {
        currentLine = null;
        points.Clear();
    }
}