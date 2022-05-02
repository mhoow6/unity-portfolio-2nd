using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UISystem : MonoBehaviour, GameSystem
{
    [ReadOnly] public List<UI> Windows = new List<UI>();
    public Canvas Canvas;
    public UI CurrentWindow => m_WindowStack.Peek();
    public bool BlockRaycast
    {
        set
        {
            m_BlockWindow.SetActive(value);
        }
    }
    public Camera UICamera;
    public PoolSystem Pool { get; private set; }

    public readonly float ScaleTweeningSpeed = 0.2f;

    Stack<UI> m_WindowStack = new Stack<UI>();
    [SerializeField] GameObject m_BlockWindow;
    [SerializeField] GameObject m_Pool;

    public void Init()
    {
        m_WindowStack.Clear();
        foreach (var window in Windows)
            window.gameObject.SetActive(false);

        m_BlockWindow.gameObject.SetActive(false);

        DontDestroyOnLoad(Canvas);

        if (UICamera != null)
            DontDestroyOnLoad(UICamera);

        Pool = new PoolSystem();
        Pool.Init(m_Pool);
        DontDestroyOnLoad(m_Pool);
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

    public void OpenWindow(UIType type, bool closeCurrentWindow = true)
    {
        OpenWindow<UI>(type, closeCurrentWindow);
    }

    public T OpenWindow<T>(UIType type, bool closeCurrentWindow = true) where T : UI
    {
        T result = null;

        // ���� â�� ����
        if (m_WindowStack.Count > 0 && closeCurrentWindow)
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
        if (Canvas != null)
        {
            // Windows ������Ʈ ã��
            GameObject root = null;
            for (int i = 0; i < Canvas.transform.childCount; i++)
            {
                var child = Canvas.transform.GetChild(i);
                if (child.name.Equals("Windows"))
                {
                    root = child.gameObject;
                    break;
                }
            }

            // Window�� �ڽĵ��� UI ���� �͸� Windows�� ���
            for (int i = 0; i < root.transform.childCount; i++)
            {
                var child = root.transform.GetChild(i);
                var comp = child.GetComponent<UI>();
                if (comp != null)
                    Windows.Add(comp);
            } 
        }
        AssetDatabase.Refresh();
    }
}