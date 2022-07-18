using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UISystem : MonoBehaviour, IGameSystem
{
    [ReadOnly] public List<UI> Windows = new List<UI>();
    [ReadOnly] public List<Toast> Toasts = new List<Toast>();

    public PoolSystem Pool { get; private set; }

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

    public UI CurrentWindow
    {
        get
        {
            if (m_WindowStack.Count > 0)
                return m_WindowStack.Peek();
            else
                return null;
        }
        
    }
    
    [SerializeField, ReadOnly] UI m_CurrentWindow;

    Stack<UI> m_WindowStack = new Stack<UI>();
    [SerializeField] GameObject m_BlockWindow;
    [SerializeField] GameObject m_Pool;

    [ReadOnly] public Toast CurrentToast;
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

        m_Toasts.Clear();
        foreach (var toast in Toasts)
            toast.gameObject.SetActive(false);

        Pool = new PoolSystem();
        Pool.Init(m_Pool);

        SceneManager.sceneUnloaded += (Scene arg0) =>
        {
            CloseAllWindow();
            m_WindowStack.Clear();
        };
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_WindowStack.Count > 0)
            {
                if (m_WindowStack.Count > 1)
                {
                    if (m_WindowStack.Peek().Type == UIType.Confirm)
                        CloseWindow(false);
                    else
                        CloseWindow(true);
                }
                else
                {
                    var confirm = OpenWindow<ConfirmUI>(UIType.Confirm, false);
                    confirm.SetData("������ �����Ͻðڽ��ϱ�?", Application.Quit, () => { CloseWindow(false); });
                }
            }
        }

        if (m_Toasts.Count > 0)
        {
            var toast = m_Toasts.Peek();
            if (toast.Initalize == true && CurrentToast == null)
            {
                toast.gameObject.SetActive(true);
                toast.transform.SetAsLastSibling();

                toast.OnOpened();

                CurrentToast = toast;
                m_Toasts.Dequeue();
            }
                
        }
            
    }

    #region Windows
    public UI OpenWindow(UIType type, bool closeCurrentWindow = true)
    {
        return OpenWindow<UI>(type, closeCurrentWindow);
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

                // ������
                m_CurrentWindow = window;

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
        while (m_WindowStack.Count != 0)
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

            // ������
            m_CurrentWindow = prev;
        }
    }

#if UNITY_EDITOR
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
#endif
    #endregion

    #region Toasts
    public Toast PushToast(ToastType type)
    {
        return PushToast<Toast>(type);
    }

    public T PushToast<T>(ToastType type) where T : Toast
    {
        var toast = Toasts.Find(toast => toast.Type == type);
        if (toast != null)
        {
            toast.Initalize = false;
            toast.OnPushed();
            m_Toasts.Enqueue(toast);
            return toast as T;
        }
        return null;
    }

    public void CloseToast(bool forceQuit)
    {
        if (CurrentToast == null)
            return;

        CurrentToast.OnClosed();
        if (forceQuit)
        {
            GameManager.UISystem.CurrentToast.gameObject.SetActive(false);
            GameManager.UISystem.CurrentToast = null;
        }
    }
#if UNITY_EDITOR
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
#endif
    #endregion
}