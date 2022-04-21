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
    [SerializeField] Camera m_UICamera;

    public readonly float ScaleTweeningSpeed = 0.2f;

    public void Init()
    {
        m_WindowStack.Clear();
        foreach (var window in Windows)
            window.gameObject.SetActive(false);

        m_BlockWindow.gameObject.SetActive(false);

        DontDestroyOnLoad(Canvas);
        DontDestroyOnLoad(m_UICamera);
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
                confirm.SetData("������ �����Ͻðڽ��ϱ�?", Application.Quit);
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

        // ���� â�� ����
        if (m_WindowStack.Count > 0)
        {
            var current = m_WindowStack.Peek();
            if (m_WindowStack.Peek() != null)
            {
                current.gameObject.SetActive(false);
                current.OnClosed();
            }
        }

        // ���Ӱ� ����
        foreach (var window in Windows)
        {
            if (window.Type == type)
            {
                // ���� ���� ���� â�� ����.
                window.gameObject.SetActive(true);
                window.transform.SetAsLastSibling();
                result = window as T;
                m_WindowStack.Push(window);

                window.OnOpened();
                return result;
            }
        }

        if (!result)
            Debug.LogError($"{type}�� �ش��ϴ� â�� �ý��ۿ� ����� �� �Ǿ� �ֽ��ϴ�.");

        return result;
    }

    public void CloseAllWindow()
    {
        while (m_WindowStack.Count != 1)
        {
            CloseWindow(false);
        }
    }

    public void CloseWindow(bool openPreviousWindow = true)
    {
        // ���� â �ݱ�
        var window = m_WindowStack.Pop();
        window.gameObject.SetActive(false);
        window.OnClosed();

        // ���� â ����
        if (m_WindowStack.Count > 0 && openPreviousWindow)
        {
            var prev = m_WindowStack.Peek();
            prev.gameObject.SetActive(true);
            prev.transform.SetAsLastSibling();
            prev.OnOpened();
        }
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