using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISystem : MonoBehaviour, GameSystem
{
    [ReadOnly] public List<UI> Windows = new List<UI>();
    [ReadOnly] public List<Toast> Toasts = new List<Toast>();

    public PoolSystem Pool { get; private set; }
    public EventSystem EventSystem;

    public Canvas Canvas;
    public Camera UICamera;
    public bool BlockRaycast
    {
        set
        {
            m_BlockWindow.SetActive(value);
        }
    }
    public bool HUD
    {
        get
        {
            return Canvas.gameObject.activeSelf;
        }
        set
        {
            Canvas.gameObject.SetActive(value);
        }
    }

    public readonly float SCALE_TWEENING_SPEED = 0.2f;

    public UI CurrentWindow => m_WindowStack.Peek();
    Stack<UI> m_WindowStack = new Stack<UI>();
    [SerializeField] GameObject m_BlockWindow;
    [SerializeField] GameObject m_Pool;

    public Toast CurrentToast { get; set; }
    Queue<Toast> m_Toasts = new Queue<Toast>();

    public void Init()
    {
        m_WindowStack.Clear();
        foreach (var window in Windows)
            window.gameObject.SetActive(false);

        m_BlockWindow.gameObject.SetActive(false);

        DontDestroyOnLoad(Canvas);

        if (UICamera != null)
            DontDestroyOnLoad(UICamera);

        if (EventSystem != null)
            DontDestroyOnLoad(EventSystem);

        m_Toasts.Clear();
        foreach (var toast in Toasts)
            toast.gameObject.SetActive(false);

        Pool = new PoolSystem();
        Pool.Init(m_Pool);
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_WindowStack.Peek().Type != UIType.MainLobby)
                CloseWindow();
            else
            {
                var confirm = OpenWindow<ConfirmUI>(UIType.Confirm);
                confirm.SetData("������ �����Ͻðڽ��ϱ�?", Application.Quit);
            }

        }

        if (m_Toasts.Count > 0)
        {
            var toast = m_Toasts.Peek();
            if (toast.Initalize == true && CurrentToast == null)
            {
                CurrentToast = toast;
                toast.transform.SetAsLastSibling();
                toast.OnOpened();
                m_Toasts.Dequeue();
            }
                
        }
            
    }

    #region Windows
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
        EditorUtility.SetDirty(this);
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
    #endregion

    #region Toasts
    public Toast OpenToast(ToastType type)
    {
        return OpenToast<Toast>(type);
    }

    public T OpenToast<T>(ToastType type) where T : Toast
    {
        var toast = Toasts.Find(toast => toast.Type == type);
        if (toast != null)
        {
            m_Toasts.Enqueue(toast);
            return toast as T;
        }
        return null;
    }

    public void CloseToast(bool forceQuit)
    {
        GameManager.UISystem.CurrentToast.OnClosed();
        if (forceQuit)
        {
            GameManager.UISystem.CurrentToast.gameObject.SetActive(false);
            GameManager.UISystem.CurrentToast = null;
        }
    }

    [ContextMenu("# Get All Toasts")]
    void GetAllToasts()
    {
        EditorUtility.SetDirty(this);
        Toasts.Clear();
        if (Canvas != null)
        {
            // Toasts ������Ʈ ã��
            GameObject root = null;
            for (int i = 0; i < Canvas.transform.childCount; i++)
            {
                var child = Canvas.transform.GetChild(i);
                if (child.name.Equals("Toasts"))
                {
                    root = child.gameObject;
                    break;
                }
            }

            // Toasts �ڽĵ��� UI ���� �͸� Toasts�� ���
            for (int i = 0; i < root.transform.childCount; i++)
            {
                var child = root.transform.GetChild(i);
                var comp = child.GetComponent<Toast>();
                if (comp != null)
                    Toasts.Add(comp);
            }
        }
        AssetDatabase.Refresh();
    }
    #endregion
}