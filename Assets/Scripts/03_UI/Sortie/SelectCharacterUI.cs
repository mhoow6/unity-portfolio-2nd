using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class SelectCharacterUI : Display, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform BackgroundRectTransform => rectTransform;
    public bool PortraitVisible
    {
        get
        {
            return m_CharacterPortrait.gameObject.activeSelf && CharacterLevel.gameObject.activeSelf;
        }
        set
        {
            m_CharacterPortrait.gameObject.SetActive(value);
            CharacterLevel.gameObject.SetActive(value);
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
    public Text CharacterLevel;
    CanvasGroup m_CanvasGroup;

    SelectCharacterUI m_Copied;
    SelectCharacterUI m_Moved;

    CustomRect Rect;
    string m_OnPointerDownColorCode = "#00FFFF";
    string m_OnPointerUpColorCode = "#FFFFFF";
    const float DISPLAY_SWAP_ALPHA = 0.8f;

    public void SetData(ObjectCode characterCode, int characterLevel = -1)
    {
        DisplayedCharacter = characterCode;

        var record = GameManager.PlayerData.CharacterDatas.Find(c => c.Code == characterCode);
        var row = TableManager.Instance.CharacterTable.Find(c => c.Code == characterCode);

        if (record == null)
        {
            Debug.LogWarning($"{characterCode}�� �ش��ϴ� ĳ���Ͱ� �÷��̾� �����Ϳ� �����ϴ�.");
            PortraitVisible = false;
            return;
        }

        if (characterLevel == -1)
            CharacterLevel.text = $"Lv. {record.Level}";
        else
            CharacterLevel.text = $"Lv. {characterLevel}";

        m_CharacterPortrait.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{row.PortraitName}");
        PortraitVisible = true;
    }

    protected override void OnAwake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        // ������ Ʈ���� ���� ���� ��� ���� �߻� -> Scale�� ����
        BackgroundRectTransform.localScale = Vector3.one;

        // ĳ���� ���ҵ��� ���� ��� ó��
        if (m_Copied != null)
            Destroy(m_Copied.gameObject);
        if (m_Moved != null)
            Destroy(m_Moved.gameObject);
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

            var characterUI = GameManager.UISystem.OpenWindow<CharacterUI>(UIType.Character);
            characterUI.SetData(DisplayedCharacter, sortie.WorldIdx, sortie.StageIdx, leaderSlot.Equals(this));
        }
        
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
            instanitate.IsLeaderSlot = IsLeaderSlot;

            // ��ġ ���߱�
            var rt = instanitate.BackgroundRectTransform;
            rt.anchoredPosition = eventData.position;
            
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
                        warning.SetData("���ڸ��� ��ü�� �� �����ϴ�.");
                        break;
                    }

                    // �ڱ� �ڽ� �����Ͽ�
                    var tryload = Resources.Load<SelectCharacterUI>($"{GameManager.GameDevelopSettings.UIResourcePath}/SelectCharacterUI");
                    if (tryload)
                    {
                        // �ڸ� �ű�� ������ ������ ���� �ν��Ͻ�
                        // LayoutGroup�� ���� ��ġ���� ������ ���� ����
                        sortie.SelectCharacterDisplayLayoutGroup.enabled = false;
                        var instanitate = Instantiate(m_Copied, sortie.SelectCharacterDisplayLayoutGroup.transform);
                        instanitate.rectTransform.anchorMin = new Vector2(0, 1);
                        instanitate.rectTransform.anchorMax = new Vector2(0, 1);

                        m_Moved = instanitate;

                        ObjectCode fromCharacter = m_Copied.DisplayedCharacter;
                        ObjectCode toCharacter = display.DisplayedCharacter;

                        // ������ ����
                        instanitate.SetData(fromCharacter);
                        instanitate.IsLeaderSlot = false;

                        Vector3 from = Vector3.zero;
                        Vector3 to = Vector3.zero;

                        // ��ġ ���߱�
                        foreach (var obj in sortie.SelectCharacterDisplays)
                        {
                            if (obj.DisplayedCharacter == fromCharacter)
                            {
                                from = obj.rectTransform.anchoredPosition;
                            }
                            if (obj.DisplayedCharacter == toCharacter)
                            {
                                to = obj.rectTransform.anchoredPosition;
                            }
                        }
                        var rt = instanitate.BackgroundRectTransform;

                        // ������ ������ ������Ʈ�� ù ��ġ
                        rt.anchoredPosition = from;

                        // ���İ� ����
                        instanitate.Alpha = DISPLAY_SWAP_ALPHA;

                        // �ش� �������� X ��ǥ �̵�
                        instanitate.BackgroundRectTransform.DOAnchorPosX(to.x, 0.5f, false)
                            .SetEase(Ease.Linear)
                            .OnPlay(() =>
                            {
                                instanitate.Raycastable = false;
                            })
                            .OnComplete(() =>
                            {
                                if (instanitate.IsNullOrDestroyed() == false)
                                {
                                    // �̵� �Ϸ�� ���� �ڸ��� �ִ� ĳ���� ����
                                    ObjectCode displayCharacter = display.DisplayedCharacter;
                                    display.SetData(DisplayedCharacter);
                                    this.SetData(displayCharacter);

                                    // ���� ��Ƽ ������ ������ ����
                                    sortie.UpdatePartyPreset();

                                    sortie.SelectCharacterDisplayLayoutGroup.enabled = true;

                                    // �̵��뵵�� ���� �� �����ֱ�
                                    Destroy(instanitate.gameObject);
                                }
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
