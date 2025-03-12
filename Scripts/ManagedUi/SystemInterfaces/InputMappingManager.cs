using ManagedUi.SystemInterfaces.InputMapping;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ManagedUi.SystemInterfaces
{
[CreateAssetMenu(fileName = "InputMapping", menuName = "UiManager/InputMappingManager")]
public class InputMappingManager : ScriptableObject
{

    [SerializeField] private List<InputMappingDeviceType> deviceInputMappings;

    public Color GetDeviceColor(PlayerInput playerInput)
    {

        string currentDeviceRawPath = playerInput.devices[0].ToString();

        Color newDisplayColor = Color.white;

        foreach (var mappingDevice in deviceInputMappings)
        {
            if (mappingDevice.DeviceName == currentDeviceRawPath)
            {
                newDisplayColor = mappingDevice.DeviceColor;
            }
        }
        return newDisplayColor;
    }

    public Sprite GetDeviceBindingIcon(InputControl control, InputBinding binding)
    {
        var currentDevice = GetDeviceFromInputControl(control.device);
        Sprite displaySpriteIcon = null;

        for (int i = 0; i < deviceInputMappings.Count; i++)
        {
            if (deviceInputMappings[i].DeviceSchema == currentDevice)
            {
                displaySpriteIcon = deviceInputMappings[i].GetInputVisualization(binding).isolatedSprite;
            }
        }
        return displaySpriteIcon;
    }
    
    
    public enum ControlSchema
    {
        Keyboard,
        Gamepad,
        Xbox,
        PS5,
        Switch
    }
    
    private static ControlSchema GetDeviceFromInputControl(InputControl control)
    {
        if (control.device is Gamepad gamepad)
        {
            string deviceName = gamepad.name.ToLower();
            string manufacturer = gamepad.description.manufacturer.ToLower();
            return GetSchemaFromString(deviceName, manufacturer);
        }
        return ControlSchema.Keyboard;
    }

    private static ControlSchema GetSchemaFromString(string deviceName, string manufacturer)
    {
        if (deviceName.Contains("xbox") || manufacturer.Contains("microsoft"))
        {
            return ControlSchema.Xbox;
        }
        else if (deviceName.Contains("dualshock") || deviceName.Contains("ps4"))
        {
            return ControlSchema.PS5;
        }
        else if (deviceName.Contains("dual") || deviceName.Contains("ps5"))
        {
            return ControlSchema.PS5;
        }
        else
        {
            return ControlSchema.Gamepad;
        }
    }


}


}