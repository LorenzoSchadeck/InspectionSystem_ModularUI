using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInteract : MonoBehaviour
{
    [Header("Interact")]
    private Transform player;
    private Camera playerCamera;
    public float activationRange = 3f;
    private bool isInteract = false;

    [Header("Modular UI")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private AudioClip interactionSound;
    private AudioSource audioSource;

    [Header("Inspection Mode")]
    [Space(5)]
    [SerializeField] private GameObject interactCamera;
    [SerializeField] private float inspectionDistance;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isInspecting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCamera = Camera.main;
        interactionText.enabled = false;
        audioSource = gameObject.AddComponent<AudioSource>();
        
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (isInteract)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleInteraction();
            }
            if (isInspecting)
            {
                UpdateItemPosition();
                RotateItem();
            }
        }
        else
        {
            if (IsPlayerInRange() && IsPlayerLookingAtObject())
            {
                interactionText.enabled = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ToggleInteraction();
                }
            }
            else
            {
                interactionText.enabled = false;
            }
        }
    }

    private bool IsPlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= activationRange;
    }

    private bool IsPlayerLookingAtObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, activationRange))
        {
            return hit.transform == transform;
        }
        return false;
    }

    private void PlayInteractionSound()
    {
        if (interactionSound != null)
        {
            audioSource.clip = interactionSound;
            audioSource.Play();
        }
    }

    private void UpdateUI()
    {
        if (itemNameText != null && itemDescriptionText != null)
        {
            itemNameText.text = itemName;
            itemDescriptionText.text = itemDescription;
        }
    }

    private void ToggleInteraction()
    {
        isInteract = !isInteract;
        PlayInteractionSound();
        if (isInteract)
        {
            StartInspection();
        }
        else
        {
            ExitInspection();
        }
    }

    private void StartInspection()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        UpdateItemPosition();

        isInspecting = true;
        descriptionPanel.SetActive(true);
        interactCamera.SetActive(true);
        interactionText.enabled = false;
        Player.canMove = false;
        UpdateUI();
    }

    private void ExitInspection()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isInspecting = false;
        descriptionPanel.SetActive(false);
        interactCamera.SetActive(false);
        Player.canMove = true;
        interactionText.enabled = true;
    }

    private void UpdateItemPosition()
    {
        Vector3 inspectionPosition = playerCamera.transform.position + playerCamera.transform.forward * inspectionDistance;
        transform.position = inspectionPosition;
    }

    private void RotateItem()
    {
        float rotationSpeed = 100f;
        float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, -rotationX, Space.World);
        transform.Rotate(Vector3.right, rotationY, Space.World);
    }
}
