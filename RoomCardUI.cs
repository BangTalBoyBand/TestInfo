using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RoomListScripts.Test
{
    [System.Serializable]
    public class RoomCardUI : MonoBehaviour
    {
        [Header("UI References")]
        public Image thumbnailImage;
        public TextMeshProUGUI playerCountText;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI creatorText;
        public TextMeshProUGUI likeRateText;
        public TextMeshProUGUI playCountText;
        public TextMeshProUGUI ratingText;
        public Transform tagsContainer;
        public GameObject tagPrefab;
        public Button cardButton;
        
        [Header("Colors")]
        [SerializeField] private Color easyColor = new Color32(27, 94, 32, 255);
        [SerializeField] private Color mediumColor = new Color32(230, 81, 0, 255);
        [SerializeField] private Color hardColor = new Color32(183, 28, 28, 255);
        [SerializeField] private Color defaultTagColor = new Color32(57, 59, 61, 255);
        
        public void SetData(RoomData data)
        {
            // 텍스트 업데이트
            if (playerCountText) 
            {
                playerCountText.text = $"{data.playerCount}명 플레이 중";
            }
            
            if (titleText) 
            {
                titleText.text = data.title;
            }
            
            if (creatorText) 
            {
                creatorText.text = $"by {data.creator}";
            }
            
            // 이모지를 텍스트 기반으로 변경
            if (likeRateText) 
            {
                likeRateText.text = $"[좋아요] {data.likeRate}";
            }
            
            if (playCountText) 
            {
                playCountText.text = $"[플레이] {data.playCount}";
            }
            
            if (ratingText) 
            {
                ratingText.text = $"[평점] {data.rating:F1}";
            }
            
            // 썸네일 이미지 설정 (data에 썸네일 URL이나 Sprite가 있다면)
            if (data.thumbnailSprite != null && thumbnailImage)
            {
                thumbnailImage.sprite = data.thumbnailSprite;
            }
            else if (thumbnailImage)
            {
                thumbnailImage.color = data.thumbnailColor;
            }
            
            // 태그 생성
            CreateTags(data);
            
            // 버튼 클릭 이벤트
            if (cardButton)
            {
                cardButton.onClick.RemoveAllListeners();
                cardButton.onClick.AddListener(() => OnCardClick(data));
            }
        }
        
        void CreateTags(RoomData data)
        {
            // 기존 태그 제거
            foreach (Transform child in tagsContainer)
            {
                Destroy(child.gameObject);
            }
            
            // 난이도 태그
            CreateTag(GetDifficultyText(data.difficulty), GetDifficultyColor(data.difficulty));
            
            // 기타 태그들
            foreach (string tag in data.tags)
            {
                CreateTag(tag, defaultTagColor);
            }
        }
        
        void CreateTag(string text, Color bgColor)
        {
            if (tagPrefab == null) return;
            
            GameObject tag = Instantiate(tagPrefab, tagsContainer);
            tag.SetActive(true);
            
            // 배경색 설정
            Image bg = tag.GetComponent<Image>();
            if (bg) bg.color = bgColor;
            
            // 텍스트 설정
            TextMeshProUGUI tagText = tag.GetComponentInChildren<TextMeshProUGUI>();
            if (tagText)
            {
                tagText.text = text;
                tagText.color = GetTagTextColor(bgColor);
                
                // 태그 프리팹에서 이미 폰트 크기와 설정이 되어있으므로
                // 여기서는 텍스트와 색상만 변경
            }
            
            // 크기 조정
            RectTransform tagRect = tag.GetComponent<RectTransform>();
            if (tagRect)
            {
                float textWidth = Mathf.Max(40, 30 + (text.Length * 7)); // 최소 너비 보장
                tagRect.sizeDelta = new Vector2(textWidth, 20);
            }
            
            LayoutElement layout = tag.GetComponent<LayoutElement>();
            if (layout)
            {
                layout.preferredWidth = Mathf.Max(40, 30 + (text.Length * 7));
                layout.minWidth = 40;
                layout.flexibleWidth = 0;
            }
        }
        
        string GetDifficultyText(Difficulty diff)
        {
            switch (diff)
            {
                case Difficulty.Easy: return "쉬움";
                case Difficulty.Medium: return "보통";
                case Difficulty.Hard: return "어려움";
                default: return "";
            }
        }
        
        Color GetDifficultyColor(Difficulty diff)
        {
            switch (diff)
            {
                case Difficulty.Easy: return easyColor;
                case Difficulty.Medium: return mediumColor;
                case Difficulty.Hard: return hardColor;
                default: return defaultTagColor;
            }
        }
        
        Color GetTagTextColor(Color bgColor)
        {
            float brightness = (bgColor.r * 0.299f + bgColor.g * 0.587f + bgColor.b * 0.114f);
            return brightness > 0.5f ? Color.black : Color.white;
        }
        
        void OnCardClick(RoomData data)
        {
            Debug.Log($"방 카드 클릭: {data.title}");
            // 이벤트 시스템으로 전달하거나 직접 처리
            ERoomListUIManager manager = GetComponentInParent<ERoomListUIManager>();
            if (manager != null)
            {
                manager.OnRoomCardClick(data);
            }
        }
    }
}
