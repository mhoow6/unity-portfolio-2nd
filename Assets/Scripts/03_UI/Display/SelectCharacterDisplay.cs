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
            Debug.Log($"{characterCode}에 해당하는 캐릭터가 플레이어 데이터에 없습니다.");
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
        // 스케일 트위닝 도중 닫힐 경우 문제 발생 -> Scale값 복원
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
            // 자기 자신 복사
            var display = Resources.Load<SelectCharacterDisplay>($"{GameManager.Config.UIResourcePath}/Display/SelectCharacterDisplay");
            if (display)
            {
                // 인스턴싱
                var sortie = ui as SortieUI;
                var instanitate = Instantiate(display, sortie.transform);
                m_Copied = instanitate;

                // 데이터 세팅
                instanitate.SetData(m_DisplayedCharacter);

                // 위치 맞추기
                // 인스턴싱된건 layoutGroup의 자식이 아님. 따라서 layoutGroup의 x좌표만큼의 더해줘야함
                var rt = instanitate.RectTransform;
                rt.anchoredPosition = new Vector3
                    (RectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x,
                    sortie.SelectCharacterGroupTransform.anchoredPosition.y,
                    0);

                // 색 변화
                if (ColorUtility.TryParseHtmlString(m_OnPointerDownColorCode, out Color color))
                    instanitate.m_Background.color = color;

                // layoutGroup안에 있는 display들은 awake나 start단계에서 정렬이 되지 않는다.
                // 드래그를 시작할때 해당 display들의 rect를 초기화시키자.
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
        // 복사본을 드래그한다.
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie)
        {
            var sortie = ui as SortieUI;
            // rectTransform 범위에 스크린 좌표가 있으면 해당하는 좌표를 rectTransform 축의 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(sortie.StageBackgroundTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
            m_Copied.RectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} 드래그 끝");

        // 드래그한 위치에 자신을 제외한 Display가 있으면 스왑
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie)
        {
            var sortie = ui as SortieUI;
            foreach (var display in sortie.SelectCharacterDisplays)
            {
                if (display.Equals(this))
                    continue;

                // 해당 Display에서 드래그를 멈췄다면
                if (display.Rect.IsScreenPointInRect(eventData.position))
                {
                    // 자기 자신 복사하여
                    var tryload = Resources.Load<SelectCharacterDisplay>($"{GameManager.Config.UIResourcePath}/Display/SelectCharacterDisplay");
                    if (tryload)
                    {
                        // 인스턴싱
                        var instanitate = Instantiate(display, sortie.transform);

                        // 데이터 세팅
                        instanitate.SetData(m_DisplayedCharacter);

                        // 위치 맞추기
                        var rt = instanitate.RectTransform;
                        rt.anchorMin = new Vector2(0, 1);
                        rt.anchorMax = new Vector2(0, 1);
                        rt.anchoredPosition = new Vector3
                            (RectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x,
                            sortie.SelectCharacterGroupTransform.anchoredPosition.y,
                            0);

                        // 알파값 변경
                        instanitate.Alpha = DISPLAY_SWAP_ALPHA;

                        // 해당 슬롯으로 X 좌표 이동
                        instanitate.RectTransform.DOAnchorPosX(display.RectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x, 1f, false)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                // 이동 완료시 데이터 스왑
                                ObjectCode displayCharacter = display.m_DisplayedCharacter;
                                display.SetData(m_DisplayedCharacter);
                                this.SetData(displayCharacter);
                            });
                    }
                    break;
                }
            }
        }


        // 복사본 파괴
        Destroy(m_Copied.gameObject);
        m_Copied = null;

        // 색 변화
        if (ColorUtility.TryParseHtmlString(m_OnPointerUpColorCode, out Color color))
            m_Background.color = color;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ColorUtility.TryParseHtmlString(m_OnPointerUpColorCode, out Color color))
            m_Background.color = color;
    }
}
