using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SelectCharacterUI : Display, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    [HideInInspector] public RectTransform BackgroundRectTransform;
    public bool PortraitVisible
    {
        get
        {
            return m_CharacterPortrait.gameObject.activeSelf && m_CharacterLevel.gameObject.activeSelf;
        }
        set
        {
            m_CharacterPortrait.gameObject.SetActive(value);
            m_CharacterLevel.gameObject.SetActive(value);
        }
    }
    public float Alpha
    {
        get
        {
            return m_CanvasGroup.alpha;
        }
        set
        {
            m_CanvasGroup.alpha = value;
        }

    }
    public bool Raycastable
    {
        get
        {
            return m_CanvasGroup.blocksRaycasts;
        }
        set
        {
            m_CanvasGroup.blocksRaycasts = value;
        }
    }
    public bool IsLeaderSlot
    {
        get
        {
            return m_LeaderIcon.gameObject.activeSelf;
        }
        set
        {
            m_LeaderIcon.gameObject.SetActive(value);
        }
    }
    public ObjectCode DisplayedCharacter { get; private set; } = ObjectCode.NONE;

    [SerializeField] Image m_LeaderIcon;
    [SerializeField] Image m_CharacterPortrait;
    [SerializeField] Image m_Background;
    [SerializeField] Text m_CharacterLevel;
    CanvasGroup m_CanvasGroup;

    SelectCharacterUI m_Copied;
    SelectCharacterUI m_Moved;

    CustomRect Rect;
    string m_OnPointerDownColorCode = "#00FFFF";
    string m_OnPointerUpColorCode = "#FFFFFF";
    const float DISPLAY_SWAP_ALPHA = 0.8f;

    public void SetData(ObjectCode characterCode)
    {
        DisplayedCharacter = characterCode;

        var record = GameManager.PlayerData.CharacterDatas.Find(c => c.Code == characterCode);
        var row = TableManager.Instance.CharacterTable.Find(c => c.Code == characterCode);

        if (record == null)
        {
            Debug.LogError($"{characterCode}�� �ش��ϴ� ĳ���Ͱ� �÷��̾� �����Ϳ� �����ϴ�.");
            PortraitVisible = false;
            return;
        }

        m_CharacterLevel.text = $"Lv. {record.Level}";
        m_CharacterPortrait.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{row.PortraitName}");
        PortraitVisible = true;
    }

    public void LateDestroy(float duration)
    {
        StartCoroutine(DestroyCoroutine(duration));
    }

    private void Awake()
    {
        BackgroundRectTransform = GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        // ������ Ʈ���� ���� ���� ��� ���� �߻� -> Scale�� ����
        BackgroundRectTransform.localScale = Vector3.one;

        // ĳ���� ���ҵ��� ���� ��� ó��
        if (m_Copied != null)
            Destroy(m_Copied);
        if (m_Moved != null)
            Destroy(m_Moved);
    }

    IEnumerator DestroyCoroutine(float duration)
    {
        float timer = 0f;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    #region �̺�Ʈ�ý��� �޼ҵ�
    public void OnPointerClick(PointerEventData eventData)
    {
        // ������ ������ �� �Ǿ������� �������� ���� �� ��
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie)
        {
            var sortie = ui as SortieUI;
            var leaderSlot = sortie.SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader];
            if (!leaderSlot.Equals(this) && leaderSlot.DisplayedCharacter == ObjectCode.NONE)
            {
                var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                warning.SetData("��Ƽ�� �������� ����ּ���.");
                return;
            }
        }
        
        //var ui = GameManager.UISystem.OpenWindow<CharacterDetailUI>(UIType.CharacterDetail);
        //ui.SetData(m_DisplayedCharacter);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie && DisplayedCharacter != ObjectCode.NONE && m_Moved == null)
        {
            // �ν��Ͻ�
            var sortie = ui as SortieUI;
            var instanitate = Instantiate(sortie.SelectCharacterUIPrefab, sortie.transform);
            m_Copied = instanitate;

            // ������ ����
            instanitate.SetData(DisplayedCharacter);

            // ��ġ ���߱�
            // �ν��Ͻ̵Ȱ� layoutGroup�� �ڽ��� �ƴ�. ���� layoutGroup�� x��ǥ��ŭ�� ���������
            var rt = instanitate.BackgroundRectTransform;
            rt.anchoredPosition = new Vector3
                (BackgroundRectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x,
                sortie.SelectCharacterGroupTransform.anchoredPosition.y,
                0);

            // �� ��ȭ
            if (ColorUtility.TryParseHtmlString(m_OnPointerDownColorCode, out Color color))
                instanitate.m_Background.color = color;

            // layoutGroup�ȿ� �ִ� display���� awake�� start�ܰ迡�� ������ ���� �ʴ´�.
            // �巡�׸� �����Ҷ� �ش� display���� rect�� �ʱ�ȭ��Ű��.
            foreach (var dis in sortie.SelectCharacterDisplays)
            {
                // ��ũ����ǥ�� Ȱ���ϱ� ���� ����Ʈ ��ȯ
                var convert = GameManager.UISystem.UICamera.WorldToViewportPoint(dis.BackgroundRectTransform.position);
                Vector3 resolutionMultipled = new Vector3(convert.x * Screen.width, convert.y * Screen.height, 0);
                dis.Rect = new CustomRect(resolutionMultipled, dis.BackgroundRectTransform.rect.width, dis.BackgroundRectTransform.rect.height);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ���纻�� �巡���Ѵ�.
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie && DisplayedCharacter != ObjectCode.NONE)
        {
            var sortie = ui as SortieUI;
            // rectTransform ������ ��ũ�� ��ǥ�� ������ �ش��ϴ� ��ǥ�� rectTransform ���� ��ǥ�� ��ȯ
            RectTransformUtility.ScreenPointToLocalPointInRectangle(sortie.StageBackgroundTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
            m_Copied.BackgroundRectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡���� ��ġ�� �ڽ��� ������ Display�� ������ ����
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie && DisplayedCharacter != ObjectCode.NONE)
        {
            var sortie = ui as SortieUI;
            foreach (var display in sortie.SelectCharacterDisplays)
            {
                if (display.Equals(this))
                    continue;

                // �ش� Display���� �巡�׸� ����ٸ�
                if (display.Rect.IsScreenPointInRect(eventData.position))
                {
                    // ������ ���ڸ��� ������ ��쿣 ���� ���ϰ� �ؾ��Ѵ�.
                    if (display.DisplayedCharacter == ObjectCode.NONE)
                    {
                        var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                        warning.SetData("��Ƽ�� ������ ���ڸ��� ��ü�� �� �����ϴ�.");
                        break;
                    }

                    // �ڱ� �ڽ� �����Ͽ�
                    var tryload = Resources.Load<SelectCharacterUI>($"{GameManager.GameDevelopSettings.UIResourcePath}/Display/SelectCharacterDisplay");
                    if (tryload)
                    {
                        // �ڸ� �ű�� ������ ������ ���� �ν��Ͻ�
                        var instanitate = Instantiate(display, sortie.transform);
                        m_Moved = instanitate;

                        // ������ ����
                        instanitate.SetData(DisplayedCharacter);

                        // ��ġ ���߱�
                        var rt = instanitate.BackgroundRectTransform;
                        rt.anchorMin = new Vector2(0, 1);
                        rt.anchorMax = new Vector2(0, 1);
                        rt.anchoredPosition = new Vector3
                            (BackgroundRectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x,
                            sortie.SelectCharacterGroupTransform.anchoredPosition.y,
                            0);

                        // ���İ� ����
                        instanitate.Alpha = DISPLAY_SWAP_ALPHA;

                        // �ش� �������� X ��ǥ �̵�
                        instanitate.BackgroundRectTransform.DOAnchorPosX(display.BackgroundRectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x, 1f, false)
                            .SetEase(Ease.Linear)
                            .OnPlay(() =>
                            {
                                instanitate.Raycastable = false;
                            })
                            .OnComplete(() =>
                            {
                                // �̵� �Ϸ�� ���� �ڸ��� �ִ� ĳ���� ����
                                ObjectCode displayCharacter = display.DisplayedCharacter;
                                display.SetData(DisplayedCharacter);
                                this.SetData(displayCharacter);

                                // �ٷ� �������� OnComplete�� ����ȵ�. 2�� �ڿ� ���������
                                instanitate.LateDestroy(2f);

                                // ���� ��Ƽ ������ ������ ����
                                sortie.UpdatePartyPreset();
                            });
                    }
                    break;
                }
            }

            // ����ߴ� ���纻 �ı�
            Destroy(m_Copied.gameObject);
            m_Copied = null;

            // �� ��ȭ
            if (ColorUtility.TryParseHtmlString(m_OnPointerUpColorCode, out Color color))
                m_Background.color = color;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ColorUtility.TryParseHtmlString(m_OnPointerUpColorCode, out Color color))
            m_Background.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ColorUtility.TryParseHtmlString(m_OnPointerDownColorCode, out Color color))
            m_Background.color = color;
    }
    #endregion
}
