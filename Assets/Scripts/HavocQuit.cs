using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HavocQuit : MonoBehaviour
{
    [SerializeField] Button m_quit = null;

    void Start()
    {
        FindObjectOfType<EventSystem>().SetSelectedGameObject(m_quit.gameObject);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Quit();
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
