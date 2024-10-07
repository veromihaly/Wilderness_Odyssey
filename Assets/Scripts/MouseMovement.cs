using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody; // Reference to the player body

    private float xRotation = 0f;

    void Start()
    {
        // Lock the cursor to the middle of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!InventorySystem.Instance.isOpen && !CraftingSystem.Instance.isOpen &&
            !MenuManager.Instance.isMenuOpen && !DialogSystem.Instance.dialogUIActive &&
            !QuestManager.Instance.isQuestMenuOpen && !StorageManager.Instance.storageUIOpen &&
            !CampfireUIManager.Instance.isUiOpen)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Control rotation around x-axis (look up and down)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Apply vertical rotation to the camera
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Rotate the player's body based on mouseX
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
