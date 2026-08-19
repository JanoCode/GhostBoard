using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BoardPlacement : MonoBehaviour
{
    [SerializeField] private GameObject boardPrefab;
    [SerializeField] private GameObject boardPreviewPrefab;

    private ARRaycastManager raycastManager;

    private static readonly List<ARRaycastHit> hits = new();

    private GameObject previewBoard;
    private GameObject spawnedBoard;

    private bool hasValidPlacement;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        Debug.Log("BoardPlacement iniciado");
    }

    private void Start()
    {
        previewBoard = Instantiate(boardPreviewPrefab);
        previewBoard.SetActive(false);
    }

    private void Update()
    {
        if (spawnedBoard != null)
            return;

        UpdatePreview();

        if (Touchscreen.current == null)
            return;

        if (!Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return;

        if (!hasValidPlacement)
            return;

        PlaceBoard();
    }

    private void UpdatePreview()
    {
        Vector2 screenCenter = new Vector2(
            Screen.width / 2f,
            Screen.height / 2f
        );

        if (raycastManager.Raycast(
            screenCenter,
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            Vector3 directionToCamera =
                Camera.main.transform.position - hitPose.position;

            directionToCamera.y = 0f;

            Quaternion boardRotation =
                Quaternion.LookRotation(-directionToCamera);

            previewBoard.transform.SetPositionAndRotation(
                hitPose.position,
                boardRotation
            );

            previewBoard.SetActive(true);

            hasValidPlacement = true;
        }
        else
        {
            previewBoard.SetActive(false);

            hasValidPlacement = false;
        }
    }

    private void PlaceBoard()
    {
        spawnedBoard = Instantiate(
            boardPrefab,
            previewBoard.transform.position,
            previewBoard.transform.rotation
        );

        previewBoard.SetActive(false);

        Debug.Log("GhostBoard creada");
    }
}