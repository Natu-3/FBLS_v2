using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blockstructure : MonoBehaviour
{
    public Dictionary<int,List<int>> dictionary = new Dictionary<int,List<int>>();
    
    public Blockstructure()
    {
                
    }

    public void getElement(int keys,List<int> values)
    {
        dictionary.Add(keys, values);
    }

    /*public List<int> searchListToKey(int key)
    {
       
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
