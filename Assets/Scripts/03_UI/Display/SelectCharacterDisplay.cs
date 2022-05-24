using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SelectCharacterDisplay : Display, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    [HideInInspector] public RectTransform RectTransform;
    public CustomRect Rect;
    [SerializeField] Image m_CharacterPortrait;
    [SerializeField] Image m_Background;
    [SerializeField] Text m_CharacterLevel;

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

    CanvasGroup m_CanvasGroup;
    SelectCharacterDisplay m_Copied;

    ObjectCode m_DisplayedCharacter = ObjectCode.NONE;
    string m_OnPointerDownColorCode = "#00FFFF";
    string m_OnPointerUpColorCode = "#FFFFFF";
    const float DISPLAY_SWAP_ALPHA = 0.8f;

    public void SetData(ObjectCode characterCode)
    {
        m_DisplayedCharacter = characterCode;

        var record = GameManager.PlayerData.CharacterDatas.Find(c => c.Code == characterCode);
        var row = TableManager.Instance.CharacterTable.Find(c => c.Code == characterCode);

        if (record != null)
        {
            m_CharacterLevel.text = $"Lv. {record.Level}";
            m_CharacterPortrait.sprite = Resources.Load<Sprite>($"{GameManager.Config.TextureResourcePath}/{row.PortraitName}");
        }
            
        else
        {
            Debug.Log($"{characterCode}�� �ش��ϴ� ĳ���Ͱ� �÷��̾� �����Ϳ� �����ϴ�.");
            PortraitVisible = false;
        }
    }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void OnClosed()
    {
        // ������ Ʈ���� ���� ���� ��� ���� �߻� -> Scale�� ����
        RectTransform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //var ui = GameManager.UISystem.OpenWindow<CharacterDetailUI>(UIType.CharacterDetail);
        //ui.SetData(m_DisplayedCharacter);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie)
        {
            // �ڱ� �ڽ� ����
            var display = Resources.Load<SelectCharacterDisplay>($"{GameManager.Config.UIResourcePath}/Display/SelectCharacterDisplay");
            if (display)
            {
                // �ν��Ͻ�
                var sortie = ui as SortieUI;
                var instanitate = Instantiate(display, sortie.transform);
                m_Copied = instanitate;

                // ������ ����
                instanitate.SetData(m_DisplayedCharacter);

                // ��ġ ���߱�
                // �ν��Ͻ̵Ȱ� layoutGroup�� �ڽ��� �ƴ�. ���� layoutGroup�� x��ǥ��ŭ�� ���������
                var rt = instanitate.RectTransform;
                rt.anchoredPosition = new Vector3
                    (RectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x,
                    sortie.SelectCharacterGroupTransform.anchoredPosition.y,
                    0);

                // �� ��ȭ
                if (ColorUtility.TryParseHtmlString(m_OnPointerDownColorCode, out Color color))
                    instanitate.m_Background.color = color;

                // layoutGroup�ȿ� �ִ� display���� awake�� start�ܰ迡�� ������ ���� �ʴ´�.
                // �巡�׸� �����Ҷ� �ش� display���� rect�� �ʱ�ȭ��Ű��.
                foreach (var dis in sortie.SelectCharacterDisplays)
                {
                    var convert = GameManager.UISystem.UICamera.WorldToViewportPoint(dis.RectTransform.position);
                    Vector3 resolutionMultipled = new Vector3(convert.x * Screen.width, convert.y * Screen.height, 0);
                    dis.Rect = new CustomRect(resolutionMultipled, dis.RectTransform.rect.width, dis.RectTransform.rect.height);
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ���纻�� �巡���Ѵ�.
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie)
        {
            var sortie = ui as SortieUI;
            // rectTransform ������ ��ũ�� ��ǥ�� ������ �ش��ϴ� ��ǥ�� rectTransform ���� ��ǥ�� ��ȯ
            RectTransformUtility.ScreenPointToLocalPointInRectangle(sortie.StageBackgroundTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
            m_Copied.RectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} �巡�� ��");

        // �巡���� ��ġ�� �ڽ��� ������ Display�� ������ ����
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie)
        {
            var sortie = ui as SortieUI;
            foreach (var display in sortie.SelectCharacterDisplays)
            {
                if (display.Equals(this))
                    continue;

                // �ش� Display���� �巡�׸� ����ٸ�
                if (display.Rect.IsScreenPointInRect(eventData.position))
                {
                    // �ڱ� �ڽ� �����Ͽ�
                    var tryload = Resources.Load<SelectCharacterDisplay>($"{GameManager.Config.UIResourcePath}/Display/SelectCharacterDisplay");
                    if (tryload)
                    {
                        // �ν��Ͻ�
                        var instanitate = Instantiate(display, sortie.transform);

                        // ������ ����
                        instanitate.SetData(m_DisplayedCharacter);

                        // ��ġ ���߱�
                        var rt = instanitate.RectTransform;
                        rt.anchorMin = new Vector2(0, 1);
                        rt.anchorMax = new Vector2(0, 1);
                        rt.anchoredPosition = new Vector3
                            (RectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x,
                            sortie.SelectCharacterGroupTransform.anchoredPosition.y,
                            0);

                        // ���İ� ����
                        instanitate.Alpha = DISPLAY_SWAP_ALPHA;

                        // �ش� �������� X ��ǥ �̵�
                        instanitate.RectTransform.DOAnchorPosX(display.RectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x, 1f, false)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                // �̵� �Ϸ�� ������ ����
                                ObjectCode displayCharacter = display.m_DisplayedCharacter;
                                display.SetData(m_DisplayedCharacter);
                                this.SetData(displayCharacter);
                            });
                    }
                    break;
                }
            }
        }


        // ���纻 �ı�
        Destroy(m_Copied.gameObject);
        m_Copied = null;

        // �� ��ȭ
        if (ColorUtility.TryParseHtmlString(m_OnPointerUpColorCode, out Color color))
            m_Background.color = color;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ColorUtility.TryParseHtmlString(m_OnPointerUpColorCode, out Color color))
            m_Background.color = color;
    }
}
