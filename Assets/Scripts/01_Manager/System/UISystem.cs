using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour, GameSystem
{
    [ReadOnly]
    public List<UI> Windows = new List<UI>();
    Stack<UI> m_windowStack = new Stack<UI>();

    public UI NoneCloseableWindow;
    public Canvas Canvas;
    public UI CurrentWindow => m_windowStack.Peek();

    public void Init()
    {
        m_windowStack.Clear();
        foreach (var window in Windows)
            window.gameObject.SetActive(false);

        DontDestroyOnLoad(Canvas);
    }

    public void Tick()
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
        OpenWindow<UI>(type);
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
                result = window as T;
                m_windowStack.Push(window);

                window.OnOpened();                
            }
        }

        if (!result)
            Debug.LogError($"{type}에 해당하는 창이 시스템에 등록이 안 되어 있습니다.");

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
