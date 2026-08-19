using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BoardPlacement : MonoBehaviour
{
    [SerializeField] private GameObject boardPrefab;

    private ARRaycastManager raycastManager;
    private static readonly List<ARRaycastHit> hits = new();

    private GameObject spawnedBoard;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        Debug.Log("BoardPlacement iniciado");
    }

    private void Update()
    {
        if (spawnedBoard != null)
            return;

        if (Touchscreen.current == null)
            return;

        if (!Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return;

        Vector2 touchPosition =
            Touchscreen.current.primaryTouch.position.ReadValue();

        Debug.Log("Pantalla tocada");

        if (raycastManager.Raycast(
            touchPosition,
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("Raycast encontró una pared");

            Pose hitPose = hits[0].pose;

            Vector3 directionToCamera = Camera.main.transform.position - hitPose.position;
            directionToCamera.y = 0f;

            Quaternion boardRotation = Quaternion.LookRotation(-directionToCamera);

            spawnedBoard = Instantiate(
                boardPrefab,
                hitPose.position,
                boardRotation
            );

            Debug.Log("GhostBoard creada");
        }
    }
}