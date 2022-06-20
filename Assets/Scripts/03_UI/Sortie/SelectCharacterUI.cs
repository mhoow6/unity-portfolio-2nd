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
            Debug.LogError($"{characterCode}에 해당하는 캐릭터가 플레이어 데이터에 없습니다.");
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
        // 스케일 트위닝 도중 닫힐 경우 문제 발생 -> Scale값 복원
        BackgroundRectTransform.localScale = Vector3.one;

        // 캐릭터 스왑도중 나갈 경우 처리
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

    #region 이벤트시스템 메소드
    public void OnPointerClick(PointerEventData eventData)
    {
        // 리더가 선택이 안 되어있으면 리더부터 고르게 할 것
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie)
        {
            var sortie = ui as SortieUI;
            var leaderSlot = sortie.SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader];
            if (!leaderSlot.Equals(this) && leaderSlot.DisplayedCharacter == ObjectCode.NONE)
            {
                var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                warning.SetData("파티의 리더부터 골라주세요.");
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
            // 인스턴싱
            var sortie = ui as SortieUI;
            var instanitate = Instantiate(sortie.SelectCharacterUIPrefab, sortie.transform);
            m_Copied = instanitate;

            // 데이터 세팅
            instanitate.SetData(DisplayedCharacter);

            // 위치 맞추기
            // 인스턴싱된건 layoutGroup의 자식이 아님. 따라서 layoutGroup의 x좌표만큼의 더해줘야함
            var rt = instanitate.BackgroundRectTransform;
            rt.anchoredPosition = new Vector3
                (BackgroundRectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x,
                sortie.SelectCharacterGroupTransform.anchoredPosition.y,
                0);

            // 색 변화
            if (ColorUtility.TryParseHtmlString(m_OnPointerDownColorCode, out Color color))
                instanitate.m_Background.color = color;

            // layoutGroup안에 있는 display들은 awake나 start단계에서 정렬이 되지 않는다.
            // 드래그를 시작할때 해당 display들의 rect를 초기화시키자.
            foreach (var dis in sortie.SelectCharacterDisplays)
            {
                // 스크린좌표를 활용하기 위해 뷰포트 변환
                var convert = GameManager.UISystem.UICamera.WorldToViewportPoint(dis.BackgroundRectTransform.position);
                Vector3 resolutionMultipled = new Vector3(convert.x * Screen.width, convert.y * Screen.height, 0);
                dis.Rect = new CustomRect(resolutionMultipled, dis.BackgroundRectTransform.rect.width, dis.BackgroundRectTransform.rect.height);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 복사본을 드래그한다.
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie && DisplayedCharacter != ObjectCode.NONE)
        {
            var sortie = ui as SortieUI;
            // rectTransform 범위에 스크린 좌표가 있으면 해당하는 좌표를 rectTransform 축의 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(sortie.StageBackgroundTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
            m_Copied.BackgroundRectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그한 위치에 자신을 제외한 Display가 있으면 스왑
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie && DisplayedCharacter != ObjectCode.NONE)
        {
            var sortie = ui as SortieUI;
            foreach (var display in sortie.SelectCharacterDisplays)
            {
                if (display.Equals(this))
                    continue;

                // 해당 Display에서 드래그를 멈췄다면
                if (display.Rect.IsScreenPointInRect(eventData.position))
                {
                    // 리더가 빈자리랑 스왑할 경우엔 하지 못하게 해야한다.
                    if (display.DisplayedCharacter == ObjectCode.NONE)
                    {
                        var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
                        warning.SetData("파티의 리더는 빈자리와 교체할 수 없습니다.");
                        break;
                    }

                    // 자기 자신 복사하여
                    var tryload = Resources.Load<SelectCharacterUI>($"{GameManager.GameDevelopSettings.UIResourcePath}/Display/SelectCharacterDisplay");
                    if (tryload)
                    {
                        // 자리 옮기는 연출을 보여줄 슬롯 인스턴싱
                        var instanitate = Instantiate(display, sortie.transform);
                        m_Moved = instanitate;

                        // 데이터 세팅
                        instanitate.SetData(DisplayedCharacter);

                        // 위치 맞추기
                        var rt = instanitate.BackgroundRectTransform;
                        rt.anchorMin = new Vector2(0, 1);
                        rt.anchorMax = new Vector2(0, 1);
                        rt.anchoredPosition = new Vector3
                            (BackgroundRectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x,
                            sortie.SelectCharacterGroupTransform.anchoredPosition.y,
                            0);

                        // 알파값 변경
                        instanitate.Alpha = DISPLAY_SWAP_ALPHA;

                        // 해당 슬롯으로 X 좌표 이동
                        instanitate.BackgroundRectTransform.DOAnchorPosX(display.BackgroundRectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x, 1f, false)
                            .SetEase(Ease.Linear)
                            .OnPlay(() =>
                            {
                                instanitate.Raycastable = false;
                            })
                            .OnComplete(() =>
                            {
                                // 이동 완료시 실제 자리에 있는 캐릭터 스왑
                                ObjectCode displayCharacter = display.DisplayedCharacter;
                                display.SetData(DisplayedCharacter);
                                this.SetData(displayCharacter);

                                // 바로 없어지면 OnComplete가 실행안됨. 2초 뒤에 사라지게함
                                instanitate.LateDestroy(2f);

                                // 실제 파티 프리셋 데이터 갱신
                                sortie.UpdatePartyPreset();
                            });
                    }
                    break;
                }
            }

            // 사용했던 복사본 파괴
            Destroy(m_Copied.gameObject);
            m_Copied = null;

            // 색 변화
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
