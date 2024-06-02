//Modified attributed code under Creative Commons Attribution-ShareAlike 4.0 International (using provided DLL in Assets/Plugins)
//https://sdumetz.github.io/2017/07/01/handle-unix-signals-unity.html

using System.Runtime.InteropServices;
using UnityEngine;

public class HandleTerm : MonoBehaviour
{
    //This is the important part
    public delegate void SigTermDelegate();

    [DllImport("sighandler")]
    private static extern void OnTerm(SigTermDelegate handler);

    void ExitHandler()
    {
        //We must call Application.Quit() manually :
        // The default built-in handler is cancelled.
        Application.Quit(0);
    }

    void Start()
    {
#if !UNITY_EDITOR
        //Register handler on initialization
        OnTerm(new SigTermDelegate(this.ExitHandler));
#endif
    }

    void OnApplicationQuit()
    {
        //This doesn't get called under normal circumstances when app get a SIGTERM.
        Debug.Log("Application Quit");
    }
}
