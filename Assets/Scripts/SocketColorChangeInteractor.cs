using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketColorChangeInteractor : XRSocketInteractor
{
    public Material hoverMaterial;
    public Material originalMaterial;

    protected override void OnHoverEnter(XRBaseInteractable interactable)
    {
        base.OnHoverEnter(interactable);
        this.GetComponent<MeshRenderer>().material = hoverMaterial;
    }

    protected override void OnHoverExit(XRBaseInteractable interactable)
    {
        base.OnHoverExit(interactable);
        this.GetComponent<MeshRenderer>().material = originalMaterial;
    }
}
