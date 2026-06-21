using System;
using UnityEngine;

public class FinishZone : MonoBehaviour
{
    public event Action OnMarbleFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Marble"))
            OnMarbleFinished?.Invoke();
    }
}
