using System;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(ObjectManipulator))]
[RequireComponent(typeof(NearInteractionGrabbable))]
public class PieceManager : MonoBehaviour
{
    public BoardManager Board;
    public GameObject Arrow;

    private void OnDestroy()
    {
        Destroy(Arrow);
    }

    public void OnManipulationStarted()
    {
        transform.SetParent(null);
        Arrow.SetActive(false);

        if (TryGetComponent(out Rigidbody rb))
            Destroy(rb);
    }

    public void OnManipulationEnded()
    {
        if (Board.TryToSnap(transform))
            return;

        Arrow.SetActive(true);
        gameObject.AddComponent<Rigidbody>();
    }

    private void OnValidate()
    {
        if (Board == null)
            Board = FindFirstObjectByType<BoardManager>();
    }
}
