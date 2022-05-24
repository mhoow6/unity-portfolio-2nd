using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using UnityEngine.EventSystems;

public class SelectCharacterDisplay : Display, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
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

    [SerializeField] Image m_CharacterPortrait;
    [SerializeField] Image m_Background;
    [SerializeField] Text m_CharacterLevel;

    ObjectCode m_DisplayedCharacter = ObjectCode.NONE;
    string m_OnPointerDownColorCode = "#00FFFF";
    string m_OnPointerUpColorCode = "#FFFFFF";

    RectTransform m_RectTransform;
    CustomRect m_Rect;
    SelectCharacterDisplay m_Copied;

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

    protected override void OnInit()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Rect = new CustomRect(m_RectTransform.rect);
    }

    protected override void OnClosed()
    {
        // 스케일 트위닝 도중 닫힐 경우 문제 발생 -> Scale값 복원
        m_RectTransform.localScale = Vector3.one;
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
                var rt = instanitate.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector3
                    (m_RectTransform.anchoredPosition.x + sortie.SelectCharacterGroupTransform.anchoredPosition.x,
                    sortie.SelectCharacterGroupTransform.anchoredPosition.y,
                    0);

                // 색 변화
                if (ColorUtility.TryParseHtmlString(m_OnPointerDownColorCode, out Color color))
                    instanitate.m_Background.color = color;
            }
        }

        Debug.Log($"{gameObject.name}가 드래그 하기 시작");
    }

    public void OnDrag(PointerEventData eventData)
    {
        var ui = GameManager.UISystem.CurrentWindow;
        if (ui.Type == UIType.Sortie)
        {
            var sortie = ui as SortieUI;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(sortie.StageBackgroundTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
            m_Copied.m_RectTransform.anchoredPosition = localPoint;
        }
        
        //Debug.Log($"{m_Copied.m_RectTransform.anchoredPosition}");

        Debug.Log($"{gameObject.name}가 드래그 중..");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} 드래그 끝");
        Destroy(m_Copied.gameObject);
        m_Copied = null;

        if (ColorUtility.TryParseHtmlString(m_OnPointerUpColorCode, out Color color))
            m_Background.color = color;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ColorUtility.TryParseHtmlString(m_OnPointerUpColorCode, out Color color))
            m_Background.color = color;
    }
}
