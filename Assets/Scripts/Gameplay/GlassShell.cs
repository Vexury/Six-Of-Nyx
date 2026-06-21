using System;
using UnityEngine;

public class GlassShell : MonoBehaviour
{
    public event Action OnMarbleFailed;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Marble"))
            OnMarbleFailed?.Invoke();
    }
}
