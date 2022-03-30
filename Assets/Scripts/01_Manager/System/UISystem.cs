using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : GameSystem
{
    [ReadOnly]
    public List<UI> Windows = new List<UI>();
    Stack<UI> m_windowStack = new Stack<UI>();

    public UI NoneCloseableWindow;
    public Canvas Canvas;
    public UI CurrentWindow => m_windowStack.Peek();

    public override void Init()
    {
        m_windowStack.Clear();
        foreach (var window in Windows)
            window.gameObject.SetActive(false);

        DontDestroyOnLoad(Canvas);
    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_windowStack.Peek().Type != UIType.MainMenu)
                CloseWindow();
            else
                OpenWindow<AskForQuitUI>(UIType.AskForQuit);
        }
    }

    public void OpenWindow(UIType type)
    {
        foreach (var window in Windows)
        {
            if (window.Type == type)
            {
                window.gameObject.SetActive(true);
                window.transform.SetAsFirstSibling();

                window.OnOpened();

                m_windowStack.Push(window);
            }
        }
    }

    public T OpenWindow<T>(UIType type) where T : UI
    {
        T result = null;
        foreach (var window in Windows)
        {
            if (window.Type == type)
            {
                window.gameObject.SetActive(true);
                window.transform.SetAsFirstSibling();

                window.OnOpened();

                result = window as T;

                m_windowStack.Push(window);
            }
        }
        return result;
    }

    public void CloseWindow()
    {
        var window = m_windowStack.Peek();

        if (window.Equals(NoneCloseableWindow))
            return;

        window = m_windowStack.Pop();
        window.gameObject.SetActive(false);
        window.OnClosed();
    }

    [ContextMenu("# Get All Windows")]
    void GetAllWindows()
    {
        Windows.Clear();
        var canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
        {
            for (int i = 0; i < canvasObj.transform.childCount; i++)
            {
                var child = canvasObj.transform.GetChild(i);
                var comp = child.GetComponent<UI>();
                if (comp != null)
                    Windows.Add(comp);
            }
        }
    }
}
