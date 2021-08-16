using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EditorControllerSimulator : MonoBehaviour
{
#if UNITY_EDITOR
  public float controllerDefaultDistance = 1;
  public float scrollWheelToDistance = 0.1f;
  public KeyCode selectKey = KeyCode.Mouse1;
  public KeyCode activateKey = KeyCode.Return;
  public KeyCode triggerKey = KeyCode.Return;
  public KeyCode gripKey = KeyCode.Mouse1;
  public KeyCode switchControllerKey = KeyCode.BackQuote;

  private XRController m_xrController;
  private float m_distance = 0;

  private Type GetNestedType(object obj, string typeName)
  {
    foreach (var type in m_xrController.GetType().GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public))
    {
      if (type.Name == typeName)
      {
        return type;
      }
    }
    return null;
  }

  private Dictionary<string, object> GetEnumValues(Type enumType)
  {
    Debug.Assert(enumType.IsEnum == true);
    Dictionary<string, object> enumValues = new Dictionary<string, object>();
    foreach (object value in Enum.GetValues(enumType))
    {
      enumValues[Enum.GetName(enumType, value)] = value;
    }
    return enumValues;
  }

  private void UpdateXRControllerState(string interaction, KeyCode inputKey)
  {
    // Update interaction state
    bool state = Input.GetKey(inputKey);
    Type interactionTypes = GetNestedType(m_xrController, "InteractionTypes");
    Dictionary<string, object> interactionTypesEnum = GetEnumValues(interactionTypes);
    MethodInfo updateInteractionType = m_xrController.GetType().GetMethod("UpdateInteractionType", BindingFlags.NonPublic | BindingFlags.Instance);
    updateInteractionType.Invoke(m_xrController, new object[] { interactionTypesEnum[interaction], (object)state });
  }

  private bool GetButtonPressed(InputHelpers.Button button)
  {
    switch (button)
    {
      case InputHelpers.Button.Trigger:
        return Input.GetKey(triggerKey);
      case InputHelpers.Button.Grip:
        return Input.GetKey(gripKey);
      default:
        return false;
    }
  }

  private void LateUpdate()
  {
    // Switch to a different controller?
    if (Input.GetKeyDown(switchControllerKey))
    {
      XRController[] controllers = FindObjectsOfType<XRController>();
      int currentIdx = Array.IndexOf(controllers, m_xrController);
      if (currentIdx >= 0)
      {
        int nextIdx = (currentIdx + 1) % controllers.Length;
        m_xrController = controllers[nextIdx];
        Debug.LogFormat("Switched controller: {0}", m_xrController.name);
      }
    }

    // Ensure we are overriding ControllerInput as well, which is our wrapper for direct button press detection
    ControllerInput controllerInput = m_xrController.GetComponent<ControllerInput>();
    if (controllerInput)
    {
      controllerInput.SetButtonPressProvider(GetButtonPressed);
    }
    
    // Controller movement
    float scroll = Input.mouseScrollDelta.y;
    if (Input.GetMouseButton(0) || scroll != 0)
    {
      // Scroll wheel controls depth
      m_distance += scroll * scrollWheelToDistance;
      float depthOffset = controllerDefaultDistance + m_distance;

      // Mouse position sets position in XY plane at current depth
      Vector3 screenPos = Input.mousePosition;
      Ray ray = Camera.main.ScreenPointToRay(screenPos);
      Vector3 position = ray.origin + ray.direction * depthOffset;
      m_xrController.transform.position = position;
    }

    // Interaction states
    UpdateXRControllerState("select", selectKey);
    UpdateXRControllerState("activate", activateKey);
  }

  private void Awake()
  {
    m_xrController = GetComponent<XRController>();
    if (m_xrController == null)
    {
      m_xrController = FindObjectOfType<XRController>();
    }
  }
#endif
}
