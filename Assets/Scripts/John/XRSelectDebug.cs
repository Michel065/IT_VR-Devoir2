using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSelectDebug : MonoBehaviour
{
    [SerializeField] UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;

    void Reset()
    {
        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
    }

    void OnEnable()
    {
        if (!interactor) interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
        if (!interactor)
        {
            Debug.LogError($"[{name}] XRSelectDebug: Aucun XRBaseInteractor trouv� sur cet objet.");
            return;
        }

        interactor.selectEntered.AddListener(OnSelectEntered);
        interactor.selectExited.AddListener(OnSelectExited);
        interactor.hoverEntered.AddListener(OnHoverEntered);
        interactor.hoverExited.AddListener(OnHoverExited);

        Debug.Log($"[{name}] XRSelectDebug: pr�t (Interactor = {interactor.GetType().Name}).");
    }

    void OnDisable()
    {
        if (!interactor) return;
        interactor.selectEntered.RemoveListener(OnSelectEntered);
        interactor.selectExited.RemoveListener(OnSelectExited);
        interactor.hoverEntered.RemoveListener(OnHoverEntered);
        interactor.hoverExited.RemoveListener(OnHoverExited);
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log($"[{name}] HOVER -> {args.interactableObject.transform.name}");
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        Debug.Log($"[{name}] HOVER EXIT -> {args.interactableObject.transform.name}");
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"[{name}] SELECT (GRAB) -> {args.interactableObject.transform.name}");
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log($"[{name}] SELECT EXIT -> {args.interactableObject.transform.name}");
    }
}
