using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UISystem : MonoBehaviour, GameSystem
{
    [ReadOnly]
    public List<UI> Windows = new List<UI>();
    Stack<UI> m_WindowStack = new Stack<UI>();

    public UI NoneCloseableWindow;
    public Canvas Canvas;
    public UI CurrentWindow => m_WindowStack.Peek();

    public bool BlockRaycast
    {
        set
        {
            m_BlockWindow.SetActive(value);
        }
    }

    [SerializeField] GameObject m_BlockWindow;

    public readonly float ScaleTweeningSpeed = 0.2f;

    public void Init()
    {
        m_WindowStack.Clear();
        foreach (var window in Windows)
            window.gameObject.SetActive(false);

        m_BlockWindow.gameObject.SetActive(false);

        DontDestroyOnLoad(Canvas);
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_WindowStack.Peek().Type != UIType.MainMenu)
                CloseWindow();
            else
            {
                var confirm = OpenWindow<ConfirmUI>(UIType.Confirm);
                confirm.SetData("게임을 종료하시겠습니까?", Application.Quit);
            }
                
        }
    }

    public void OpenWindow(UIType type)
    {
        OpenWindow<UI>(type);
    }

    public T OpenWindow<T>(UIType type) where T : UI
    {
        T result = null;

        // 기존에 열렸던 창인지 검사
        foreach (var window in m_WindowStack)
        {
            if (window.Type == type)
            {
                // 현재 열고 싶은 창을 연다.
                window.gameObject.SetActive(true);
                window.transform.SetAsLastSibling();
                result = window as T;

                window.OnOpened();
                return result;
            }
        }

        // 새롭게 열기
        foreach (var window in Windows)
        {
            if (window.Type == type)
            {
                // 현재 열고 싶은 창을 연다.
                window.gameObject.SetActive(true);
                window.transform.SetAsLastSibling();
                result = window as T;
                m_WindowStack.Push(window);

                window.OnOpened();
                return result;
            }
        }

        if (!result)
            Debug.LogError($"{type}에 해당하는 창이 시스템에 등록이 안 되어 있습니다.");

        return result;
    }

    public void CloseAllWindow()
    {
        while (m_WindowStack.Count != 1)
        {
            CloseWindow();
        }
    }

    public void CloseWindow()
    {
        var window = m_WindowStack.Peek();

        if (window.Equals(NoneCloseableWindow))
            return;

        window = m_WindowStack.Pop();
        window.gameObject.SetActive(false);
        window.OnClosed();
    }

    [ContextMenu("# Get All Windows")]
    void GetAllWindows()
    {
        Windows.Clear();
        var canvasObj = GameObject.Find("Canvas_Windows");
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
        AssetDatabase.Refresh();
    }
}
