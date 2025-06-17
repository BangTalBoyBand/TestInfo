using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace RoomListScripts.Test
{
    public class ERoomListUIManager : MonoBehaviour
    {
        [Header("Prefab References")]
        public GameObject roomCardPrefab;
        
        [Header("UI References")]
        public Transform roomsGridContainer;
        public TMP_InputField searchInput;
        public TextMeshProUGUI pageTitle;
        public TextMeshProUGUI pageSubtitle;
        public TextMeshProUGUI creditText;
        public TMP_Dropdown sortDropdown;
        public List<Button> filterButtons;
        public List<Button> sidebarButtons;
        public List<TextMeshProUGUI> navigationTexts;
        
        [Header("Room Data")]
        public List<RoomData> allRooms = new List<RoomData>();
        private string currentFilter = "전체";
        private string searchQuery = "";
        
        [Header("UI Text Settings")]
        public string currentPageTitle = "방탈출 게임";
        public string currentPageSubtitle = "스릴 넘치는 방탈출 어드벤처를 경험해보세요";
        public int playerCredits = 1250;
        
        void Start()
        {
            // UI 텍스트 초기화
            UpdateUITexts();
            
            // 샘플 데이터 로드 또는 서버에서 가져오기
            LoadRoomData();
            
            // 필터 버튼 이벤트 연결
            SetupFilterButtons();
            
            // 검색 기능 설정
            if (searchInput != null)
            {
                searchInput.onValueChanged.AddListener(OnSearchValueChanged);
            }
            
            // 정렬 드롭다운 설정
            if (sortDropdown != null)
            {
                sortDropdown.onValueChanged.AddListener(OnSortChanged);
            }
        }
        
        void UpdateUITexts()
        {
            if (pageTitle) pageTitle.text = currentPageTitle;
            if (pageSubtitle) pageSubtitle.text = currentPageSubtitle;
            if (creditText) creditText.text = $"₵ {playerCredits:N0}";
        }
        
        public void UpdateCredits(int newCredits)
        {
            playerCredits = newCredits;
            if (creditText) creditText.text = $"₵ {playerCredits:N0}";
        }
        
        public void ChangePageTitle(string title, string subtitle)
        {
            currentPageTitle = title;
            currentPageSubtitle = subtitle;
            UpdateUITexts();
        }
        
        void LoadRoomData()
        {
            // 서버에서 데이터를 가져오거나 로컬 데이터 로드
            // 예시 데이터:
            allRooms.Clear();
            
            allRooms.Add(new RoomData(
                "Escape the Haunted Mansion", 
                "MasterBuilder_2023", 
                127, "94%", "2.1M", 4.8f, 
                Difficulty.Hard, 
                new string[]{"호러", "협동"}
            ));
            
            allRooms.Add(new RoomData(
                "Prison Break Adventure", 
                "EscapeGuru", 
                45, "87%", "850K", 4.5f, 
                Difficulty.Medium, 
                new string[]{"액션", "전략"}
            ));
            
            allRooms.Add(new RoomData(
                "Mystery Laboratory", 
                "ScienceFan", 
                92, "91%", "1.3M", 4.7f,
                Difficulty.Easy,
                new string[]{"퍼즐", "교육"}
            ));
            
            allRooms.Add(new RoomData(
                "Underwater Temple Escape",
                "AquaAdventurer",
                201, "89%", "970K", 4.6f,
                Difficulty.Medium,
                new string[]{"어드벤처", "협동"}
            ));
            
            // 방 카드 생성
            RefreshRoomDisplay();
        }
        
        public void AddRoom(RoomData roomData)
        {
            allRooms.Add(roomData);
            RefreshRoomDisplay();
        }
        
        public void AddRooms(List<RoomData> rooms)
        {
            allRooms.AddRange(rooms);
            RefreshRoomDisplay();
        }
        
        public void ClearAllRooms()
        {
            allRooms.Clear();
            RefreshRoomDisplay();
        }
        
        void RefreshRoomDisplay()
        {
            // 기존 카드 제거
            foreach (Transform child in roomsGridContainer)
            {
                Destroy(child.gameObject);
            }
            
            // 필터링 및 정렬된 방 표시
            var filteredRooms = FilterRooms(allRooms, currentFilter, searchQuery);
            var sortedRooms = SortRooms(filteredRooms, sortDropdown ? sortDropdown.value : 0);
            
            foreach (var room in sortedRooms)
            {
                CreateRoomCard(room);
            }
        }
        
        void CreateRoomCard(RoomData data)
        {
            if (roomCardPrefab == null) return;
            
            GameObject card = Instantiate(roomCardPrefab, roomsGridContainer);
            RoomCardUI roomCardUI = card.GetComponent<RoomCardUI>();
            
            if (roomCardUI != null)
            {
                roomCardUI.SetData(data);
            }
        }
        
        void SetupFilterButtons()
        {
            // 필터 칩 버튼들
            foreach (var button in filterButtons)
            {
                button.onClick.AddListener(() => OnFilterClick(button));
            }
            
            // 사이드바 버튼들
            foreach (var button in sidebarButtons)
            {
                button.onClick.AddListener(() => OnSidebarClick(button));
            }
        }
        
        void OnFilterClick(Button clickedButton)
        {
            // 모든 필터 버튼 비활성화 상태로
            foreach (var button in filterButtons)
            {
                Image bg = button.GetComponent<Image>();
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
                if (bg) bg.color = new Color32(74, 76, 79, 255); // buttonSecondary
                if (text) text.color = new Color32(189, 190, 190, 255); // textSecondary
            }
            
            // 클릭된 버튼 활성화
            Image clickedBg = clickedButton.GetComponent<Image>();
            TextMeshProUGUI clickedText = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
            if (clickedBg) clickedBg.color = new Color32(0, 178, 255, 255); // accentColor
            if (clickedText)
            {
                clickedText.color = Color.white;
                currentFilter = clickedText.text;
            }
            
            RefreshRoomDisplay();
        }
        
        void OnSidebarClick(Button clickedButton)
        {
            // 사이드바 카테고리나 필터 클릭 처리
            TextMeshProUGUI text = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
            if (text)
            {
                string category = text.text;
                // 카테고리에 따른 처리
                Debug.Log($"사이드바 선택: {category}");
                
                // 페이지 제목 변경 예시
                switch (category)
                {
                    case "방탈출":
                        ChangePageTitle("방탈출 게임", "스릴 넘치는 방탈출 어드벤처를 경험해보세요");
                        break;
                    case "어드벤처":
                        ChangePageTitle("어드벤처 게임", "흥미진진한 모험을 떠나보세요");
                        break;
                    case "RPG":
                        ChangePageTitle("RPG 게임", "나만의 캐릭터로 세계를 탐험하세요");
                        break;
                }
                
                RefreshRoomDisplay();
            }
        }
        
        void OnSearchValueChanged(string searchText)
        {
            searchQuery = searchText;
            RefreshRoomDisplay();
        }
        
        void OnSortChanged(int sortIndex)
        {
            RefreshRoomDisplay();
        }
        
        List<RoomData> FilterRooms(List<RoomData> rooms, string filter, string search)
        {
            var filtered = rooms;
            
            // 필터 적용
            if (filter != "전체")
            {
                // 난이도별 필터링
                if (filter == "쉬움") filtered = filtered.FindAll(r => r.difficulty == Difficulty.Easy);
                else if (filter == "보통") filtered = filtered.FindAll(r => r.difficulty == Difficulty.Medium);
                else if (filter == "어려움") filtered = filtered.FindAll(r => r.difficulty == Difficulty.Hard);
                else
                {
                    // 태그별 필터링
                    filtered = filtered.FindAll(r => System.Array.Exists(r.tags, tag => tag == filter));
                }
            }
            
            // 검색어 적용
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                filtered = filtered.FindAll(r => 
                    r.title.ToLower().Contains(search) || 
                    r.creator.ToLower().Contains(search) ||
                    System.Array.Exists(r.tags, tag => tag.ToLower().Contains(search))
                );
            }
            
            return filtered;
        }
        
        List<RoomData> SortRooms(List<RoomData> rooms, int sortIndex)
        {
            var sorted = new List<RoomData>(rooms);
            
            switch (sortIndex)
            {
                case 0: // 인기순
                    sorted.Sort((a, b) => b.GetLikePercentage().CompareTo(a.GetLikePercentage()));
                    break;
                case 1: // 최신순
                    // 실제로는 생성 날짜로 정렬해야 함
                    sorted.Reverse();
                    break;
                case 2: // 평점순
                    sorted.Sort((a, b) => b.rating.CompareTo(a.rating));
                    break;
                case 3: // 플레이어순
                    sorted.Sort((a, b) => b.playerCount.CompareTo(a.playerCount));
                    break;
            }
            
            return sorted;
        }
        
        public void OnRoomCardClick(RoomData data)
        {
            Debug.Log($"방 입장: {data.title}");
            // 실제 방 입장 로직 구현
            // 예: SceneManager.LoadScene("GameRoom");
            // 예: NetworkManager.JoinRoom(data.roomId);
        }
        
        public void OnCreateGameClick()
        {
            Debug.Log("게임 만들기 클릭");
            // 게임 생성 UI 열기
        }
        
        public void OnFavoritesClick()
        {
            Debug.Log("즐겨찾기 클릭");
            // 즐겨찾기 목록 표시
        }
        
        public void OnLoadMoreClick()
        {
            Debug.Log("더 많은 게임 보기");
            // 추가 게임 로드
            // 예: LoadMoreRoomsFromServer();
        }
    }
}
