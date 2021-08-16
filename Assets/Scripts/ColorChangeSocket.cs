using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ColorChangeSocket : XRSocketInteractor
{
    public Material originalMaterial;
    public Material highlightMaterial;

    protected override void OnHoverEnter(XRBaseInteractable interactable)
    {
        base.OnHoverEnter(interactable);
        this.GetComponent<MeshRenderer>().material = highlightMaterial;
    }

    protected override void OnHoverExit(XRBaseInteractable interactable)
    {
        base.OnHoverExit(interactable);
        this.GetComponent<MeshRenderer>().material = originalMaterial;
    }
}
