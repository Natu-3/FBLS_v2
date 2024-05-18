using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rowNode : MonoBehaviour
{
    private void Awake()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
        void Start()
    {
       
    }
    void Update()
    {
        
    }

  
}
