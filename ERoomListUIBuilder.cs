using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RoomListScripts.Test
{
    [ExecuteInEditMode]
    public class ERoomListUIBuilder : MonoBehaviour
    {
        [Header("Build Settings")]
        [SerializeField] private bool buildOnStart = false;
        [SerializeField] private bool createSampleCards = true;
        
        [Header("Font Settings")]
        [SerializeField] private TMP_FontAsset mainFont;
        [SerializeField] private TMP_FontAsset boldFont;
        [SerializeField] private Material fontMaterial;
        
        [Header("UI Text Configuration")]
        [SerializeField] private string logoText = "EROOM";
        [SerializeField] private string[] navigationItems = { "홈", "게임", "카탈로그", "개발", "그룹" };
        [SerializeField] private string searchPlaceholder = "게임, 아이템, 사용자 검색...";
        [SerializeField] private string creditAmount = "₵ 1,250";
        [SerializeField] private string pageTitleText = "방탈출 게임";
        [SerializeField] private string pageSubtitleText = "스릴 넘치는 방탈출 어드벤처를 경험해보세요";
        [SerializeField] private string[] sortOptions = { "인기순", "최신순", "평점순", "플레이어순" };
        [SerializeField] private string favoritesButtonText = "즐겨찾기";
        [SerializeField] private string createGameButtonText = "게임 만들기";
        [SerializeField] private string loadMoreButtonText = "더 많은 게임 보기";
        
        [Header("Sidebar Configuration")]
        [SerializeField] private string categorySectionTitle = "카테고리";
        [SerializeField] private string[] categoryItems = { "방탈출", "어드벤처", "RPG", "시뮬레이션", "전략" };
        [SerializeField] private string filterSectionTitle = "필터";
        [SerializeField] private string[] filterItems = { "인기순", "최신순", "평점순", "플레이어순" };
        
        [Header("Filter Chips")]
        [SerializeField] private string[] filterChipTexts = { "전체", "쉬움", "보통", "어려움", "협동", "경쟁", "퍼즐", "호러" };
        
        [Header("Color Scheme")]
        private readonly Color backgroundColor = new Color32(57, 59, 61, 255);       // #393B3D
        private readonly Color topBarColor = new Color32(35, 37, 39, 255);          // #232527
        private readonly Color sidebarColor = new Color32(44, 47, 49, 255);         // #2C2F31
        private readonly Color cardColor = new Color32(44, 47, 49, 255);            // #2C2F31
        private readonly Color accentColor = new Color32(0, 178, 255, 255);         // #00B2FF
        private readonly Color successColor = new Color32(0, 166, 81, 255);         // #00A651
        private readonly Color textPrimary = Color.white;                           // #FFFFFF
        private readonly Color textSecondary = new Color32(189, 190, 190, 255);     // #BDBEBE
        private readonly Color buttonSecondary = new Color32(74, 76, 79, 255);      // #4A4C4F
        
        [Header("Generated References (자동 생성됨)")]
        public Canvas mainCanvas;
        public GameObject roomCardPrefab;
        public Transform roomsGridContainer;
        public TMP_InputField searchInput;
        public TextMeshProUGUI pageTitle;
        public TextMeshProUGUI pageSubtitle;
        public TextMeshProUGUI creditText;
        public TMP_Dropdown sortDropdown;
        public Transform filterContainer;
        public List<Button> sidebarButtons = new List<Button>();
        public List<Button> filterChips = new List<Button>();
        public List<TextMeshProUGUI> navigationTexts = new List<TextMeshProUGUI>();
        
        private RectTransform topBar;
        private RectTransform sidebar;
        private RectTransform contentArea;

        [ContextMenu("Build UI (UI 생성하기)")]
        public void BuildUI()
        {
            Debug.Log("EROOM UI 생성 시작...");
            
            // 기존 UI 제거
            ClearExistingUI();
            
            // UI 생성
            CreateUI();
            
            if (createSampleCards)
            {
                CreateSampleRoomCard();
            }
            
            Debug.Log("UI 생성 완료! 다음 단계:");
            Debug.Log("1. Hierarchy에서 Canvas를 선택");
            Debug.Log("2. 우클릭 > Prefab > Save as Prefab");
            Debug.Log("3. ERoomListUIBuilder 스크립트는 제거하거나 비활성화");
            Debug.Log("4. ERoomListUIManager 스크립트를 추가하여 런타임 관리");
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
            #endif
        }
        
        [ContextMenu("Clear UI (UI 제거하기)")]
        public void ClearExistingUI()
        {
            // Canvas 찾기
            Canvas existingCanvas = GetComponentInChildren<Canvas>();
            if (existingCanvas != null)
            {
                if (Application.isPlaying)
                    Destroy(existingCanvas.gameObject);
                else
                    DestroyImmediate(existingCanvas.gameObject);
            }
            
            // References 초기화
            mainCanvas = null;
            roomCardPrefab = null;
            roomsGridContainer = null;
            searchInput = null;
            pageTitle = null;
            pageSubtitle = null;
            creditText = null;
            sortDropdown = null;
            filterContainer = null;
            sidebarButtons.Clear();
            filterChips.Clear();
            navigationTexts.Clear();
        }
        
        private void Start()
        {
            if (buildOnStart && !Application.isPlaying)
            {
                BuildUI();
            }
        }
        
        private void CreateUI()
        {
            // Setup Canvas
            GameObject canvasGO = new GameObject("EROOM_Canvas");
            canvasGO.transform.SetParent(transform, false);
            mainCanvas = canvasGO.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Event System 추가 (Unity 6 Input System)
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                // InputSystemUIInputModule 추가
                InputSystemUIInputModule inputModule = eventSystem.AddComponent<InputSystemUIInputModule>();
            }
            
            // Background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(mainCanvas.transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = backgroundColor;
            
            // Create Top Bar
            CreateTopBar();
            
            // Create Sidebar
            CreateSidebar();
            
            // Create Content Area
            CreateContentArea();
        }
        
        private void CreateTopBar()
        {
            GameObject topBarGO = new GameObject("TopBar");
            topBarGO.transform.SetParent(mainCanvas.transform, false);
            topBar = topBarGO.AddComponent<RectTransform>();
            
            // Position at top
            topBar.anchorMin = new Vector2(0, 1);
            topBar.anchorMax = new Vector2(1, 1);
            topBar.pivot = new Vector2(0.5f, 1);
            topBar.sizeDelta = new Vector2(0, 50);
            topBar.anchoredPosition = Vector2.zero;
            
            Image topBarBg = topBarGO.AddComponent<Image>();
            topBarBg.color = topBarColor;
            
            // Add shadow
            Shadow shadow = topBarGO.AddComponent<Shadow>();
            shadow.effectDistance = new Vector2(0, -2);
            shadow.effectColor = new Color(0, 0, 0, 0.3f);
            
            // Logo
            GameObject logo = CreateText(logoText, topBarGO.transform, 
                new Vector2(20, -25), new Vector2(100, 50), 24, FontStyle.Bold); // 폰트 크기 증가
            logo.GetComponent<TextMeshProUGUI>().color = accentColor;
            logo.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
            logo.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
            logo.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            logo.name = "Logo";
            
            // Navigation Container
            GameObject navContainer = new GameObject("NavigationContainer");
            navContainer.transform.SetParent(topBarGO.transform, false);
            RectTransform navRect = navContainer.AddComponent<RectTransform>();
            navRect.anchorMin = new Vector2(0, 0);
            navRect.anchorMax = new Vector2(1, 1);
            navRect.anchoredPosition = new Vector2(150, 0);
            navRect.sizeDelta = new Vector2(-550, 0);
            
            HorizontalLayoutGroup navLayout = navContainer.AddComponent<HorizontalLayoutGroup>();
            navLayout.spacing = 25;
            navLayout.childForceExpandHeight = true;
            navLayout.childForceExpandWidth = false;
            navLayout.childControlHeight = true;
            navLayout.childControlWidth = false;
            navLayout.childAlignment = TextAnchor.MiddleLeft;
            
            // Navigation Items
            for (int i = 0; i < navigationItems.Length; i++)
            {
                GameObject navItem = CreateNavItem(navigationItems[i], navContainer.transform, i == 1);
                var navText = navItem.GetComponentInChildren<TextMeshProUGUI>();
                if (navText) 
                {
                    navigationTexts.Add(navText);
                    // 네비게이션 텍스트가 잘리지 않도록 크기 조정
                    navText.enableWordWrapping = false;
                    navText.overflowMode = TextOverflowModes.Overflow;
                }
            }
            
            // User Section (Right side)
            CreateUserSection(topBarGO.transform);
        }
        
        private GameObject CreateNavItem(string text, Transform parent, bool isActive)
        {
            GameObject navItem = new GameObject($"NavItem_{text}");
            navItem.transform.SetParent(parent, false);
            
            RectTransform rect = navItem.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 40); // 너비 증가
            
            Button navButton = navItem.AddComponent<Button>();
            navButton.transition = Selectable.Transition.None;
            
            GameObject textObj = CreateText(text, navItem.transform,
                Vector2.zero, new Vector2(100, 40), 16); // 폰트 크기 증가
            TextMeshProUGUI tmpText = textObj.GetComponent<TextMeshProUGUI>();
            tmpText.color = isActive ? textPrimary : textSecondary;
            tmpText.alignment = TextAlignmentOptions.Center;
            
            // 텍스트가 잘 보이도록 설정
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            // Bottom border for active state
            if (isActive)
            {
                GameObject border = new GameObject("ActiveBorder");
                border.transform.SetParent(navItem.transform, false);
                RectTransform borderRect = border.AddComponent<RectTransform>();
                borderRect.anchorMin = new Vector2(0, 0);
                borderRect.anchorMax = new Vector2(1, 0);
                borderRect.sizeDelta = new Vector2(0, 2);
                borderRect.anchoredPosition = new Vector2(0, 0);
                Image borderImg = border.AddComponent<Image>();
                borderImg.color = accentColor;
            }
            
            return navItem;
        }
        
        private void CreateUserSection(Transform parent)
        {
            GameObject userSection = new GameObject("UserSection");
            userSection.transform.SetParent(parent, false);
            RectTransform userRect = userSection.AddComponent<RectTransform>();
            userRect.anchorMin = new Vector2(1, 0.5f);
            userRect.anchorMax = new Vector2(1, 0.5f);
            userRect.pivot = new Vector2(1, 0.5f);
            userRect.sizeDelta = new Vector2(400, 50);
            userRect.anchoredPosition = new Vector2(-20, 0);
            
            HorizontalLayoutGroup userLayout = userSection.AddComponent<HorizontalLayoutGroup>();
            userLayout.spacing = 15;
            userLayout.childForceExpandHeight = false;
            userLayout.childForceExpandWidth = false;
            userLayout.childControlHeight = false;
            userLayout.childControlWidth = false;
            userLayout.childAlignment = TextAnchor.MiddleRight;
            
            // Search Bar
            GameObject searchBar = CreateInputField(searchPlaceholder, 
                userSection.transform, Vector2.zero, new Vector2(250, 35));
            searchInput = searchBar.GetComponent<TMP_InputField>();
            
            // Credit Display
            GameObject creditDisplay = new GameObject("CreditDisplay");
            creditDisplay.transform.SetParent(userSection.transform, false);
            RectTransform creditRect = creditDisplay.AddComponent<RectTransform>();
            creditRect.sizeDelta = new Vector2(80, 30);
            
            Image creditBg = creditDisplay.AddComponent<Image>();
            creditBg.color = successColor;
            
            GameObject creditTextObj = CreateText(creditAmount, creditDisplay.transform,
                Vector2.zero, new Vector2(80, 30), 15, FontStyle.Bold); // 폰트 크기 증가
            creditTextObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            creditTextObj.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            creditTextObj.GetComponent<RectTransform>().anchorMax = Vector2.one;
            creditTextObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            creditText = creditTextObj.GetComponent<TextMeshProUGUI>();
            
            // User Avatar
            GameObject avatar = new GameObject("Avatar");
            avatar.transform.SetParent(userSection.transform, false);
            RectTransform avatarRect = avatar.AddComponent<RectTransform>();
            avatarRect.sizeDelta = new Vector2(32, 32);
            
            Image avatarImg = avatar.AddComponent<Image>();
            avatarImg.color = accentColor;
        }
        
        void CreateSidebar()
        {
            GameObject sidebarGO = new GameObject("Sidebar");
            sidebarGO.transform.SetParent(mainCanvas.transform, false);
            sidebar = sidebarGO.AddComponent<RectTransform>();
            
            sidebar.anchorMin = new Vector2(0, 0);
            sidebar.anchorMax = new Vector2(0, 1);
            sidebar.pivot = new Vector2(0, 0.5f);
            sidebar.sizeDelta = new Vector2(220, -50); // 너비를 220으로 증가
            sidebar.anchoredPosition = new Vector2(0, -25);
            
            Image sidebarBg = sidebarGO.AddComponent<Image>();
            sidebarBg.color = sidebarColor;
            
            // Sidebar Content Container
            GameObject sidebarContent = new GameObject("SidebarContent");
            sidebarContent.transform.SetParent(sidebarGO.transform, false);
            RectTransform contentRect = sidebarContent.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;
            
            VerticalLayoutGroup contentLayout = sidebarContent.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 25;
            contentLayout.padding = new RectOffset(0, 0, 20, 20);
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            
            // Categories Section
            CreateSidebarSection(categorySectionTitle, categoryItems, sidebarContent.transform, true);
            
            // Filters Section
            CreateSidebarSection(filterSectionTitle, filterItems, sidebarContent.transform, false);
        }
        
        private void CreateSidebarSection(string title, string[] items, Transform parent, bool isFirstItemActive)
        {
            GameObject section = new GameObject($"Section_{title}");
            section.transform.SetParent(parent, false);
            
            VerticalLayoutGroup sectionLayout = section.AddComponent<VerticalLayoutGroup>();
            sectionLayout.spacing = 0;
            sectionLayout.childForceExpandWidth = true;
            sectionLayout.childForceExpandHeight = false;
            
            // Section Title
            GameObject titleObj = new GameObject($"Title_{title}");
            titleObj.transform.SetParent(section.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(220, 30);
            
            GameObject titleText = CreateText(title, titleObj.transform, 
                Vector2.zero, new Vector2(220, 30), 13, FontStyle.Bold); // 폰트 크기 줄임
            TextMeshProUGUI titleTMP = titleText.GetComponent<TextMeshProUGUI>();
            titleTMP.color = textSecondary;
            titleTMP.characterSpacing = 2;
            titleTMP.alignment = TextAlignmentOptions.MidlineLeft;
            titleTMP.margin = new Vector4(20, 0, 20, 0);
            
            RectTransform titleTextRect = titleText.GetComponent<RectTransform>();
            titleTextRect.anchorMin = Vector2.zero;
            titleTextRect.anchorMax = Vector2.one;
            titleTextRect.sizeDelta = Vector2.zero;
            titleTextRect.anchoredPosition = Vector2.zero;
            
            // Section Items
            for (int i = 0; i < items.Length; i++)
            {
                GameObject item = CreateSidebarItem(items[i], section.transform, 
                    i == 0 && isFirstItemActive);
                Button itemButton = item.GetComponent<Button>();
                if (itemButton != null)
                    sidebarButtons.Add(itemButton);
            }
        }
        
        GameObject CreateSidebarItem(string text, Transform parent, bool isActive = false)
        {
            GameObject item = new GameObject($"SidebarItem_{text}");
            item.transform.SetParent(parent, false);
            RectTransform rect = item.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(220, 35); // 전체 너비 사용
            
            Image bgImage = item.AddComponent<Image>();
            bgImage.color = isActive ? backgroundColor : new Color(0, 0, 0, 0);
            
            Button btn = item.AddComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;
            btn.targetGraphic = bgImage;
            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color32(57, 59, 61, 100);
            colors.pressedColor = backgroundColor;
            btn.colors = colors;
            
            // 텍스트를 전체 영역에 맞춤
            GameObject textObj = CreateText(text, item.transform, 
                Vector2.zero, new Vector2(220, 35), 14); // 폰트 크기를 14로 줄임
            TextMeshProUGUI tmpText = textObj.GetComponent<TextMeshProUGUI>();
            tmpText.color = isActive ? textPrimary : textSecondary;
            tmpText.alignment = TextAlignmentOptions.MidlineLeft;
            tmpText.margin = new Vector4(20, 0, 20, 0); // 좌우 마진 설정
            tmpText.overflowMode = TextOverflowModes.Ellipsis; // 텍스트가 넘치면 ... 표시
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            return item;
        }
        
        private void CreateContentArea()
        {
            GameObject contentGO = new GameObject("ContentArea");
            contentGO.transform.SetParent(mainCanvas.transform, false);
            contentArea = contentGO.AddComponent<RectTransform>();
            
            contentArea.anchorMin = new Vector2(0, 0);
            contentArea.anchorMax = new Vector2(1, 1);
            contentArea.sizeDelta = new Vector2(-220, -50); // 사이드바 너비에 맞게 조정
            contentArea.anchoredPosition = new Vector2(110, -25); // 사이드바 너비의 절반만큼 이동
            
            // Page Header
            CreatePageHeader(contentArea);
            
            // Filters Bar
            CreateFiltersBar(contentArea);
            
            // Rooms Grid
            CreateRoomsGrid(contentArea);
            
            // Load More Button (스크롤뷰 밖에 위치)
            CreateLoadMoreButton(contentArea);
        }
        
        void CreatePageHeader(Transform parent)
        {
            GameObject header = new GameObject("PageHeader");
            header.transform.SetParent(parent, false);
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.sizeDelta = new Vector2(-60, 80);
            headerRect.anchoredPosition = new Vector2(0, -25);
            
            // Title Container
            GameObject titleContainer = new GameObject("TitleContainer");
            titleContainer.transform.SetParent(header.transform, false);
            RectTransform titleContainerRect = titleContainer.AddComponent<RectTransform>();
            titleContainerRect.anchorMin = new Vector2(0, 0);
            titleContainerRect.anchorMax = new Vector2(0.5f, 1);
            titleContainerRect.sizeDelta = Vector2.zero;
            
            // Title
            GameObject title = CreateText(pageTitleText, titleContainer.transform,
                new Vector2(0, -20), new Vector2(400, 30), 28, FontStyle.Bold); // 폰트 크기 증가
            title.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            title.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            title.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            pageTitle = title.GetComponent<TextMeshProUGUI>();
            pageTitle.enableWordWrapping = false;
            
            // Subtitle
            GameObject subtitle = CreateText(pageSubtitleText, 
                titleContainer.transform, new Vector2(0, -50), new Vector2(500, 20), 16); // 폰트 크기 증가
            subtitle.GetComponent<TextMeshProUGUI>().color = textSecondary;
            subtitle.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            subtitle.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            subtitle.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            pageSubtitle = subtitle.GetComponent<TextMeshProUGUI>();
            pageSubtitle.enableWordWrapping = false;
            
            // Header Actions (right side)
            CreateHeaderActions(header.transform);
        }
        
        void CreateHeaderActions(Transform parent)
        {
            GameObject actions = new GameObject("HeaderActions");
            actions.transform.SetParent(parent, false);
            RectTransform actionsRect = actions.AddComponent<RectTransform>();
            actionsRect.anchorMin = new Vector2(1, 0.5f);
            actionsRect.anchorMax = new Vector2(1, 0.5f);
            actionsRect.pivot = new Vector2(1, 0.5f);
            actionsRect.sizeDelta = new Vector2(400, 40);
            actionsRect.anchoredPosition = Vector2.zero;
            
            HorizontalLayoutGroup actionsLayout = actions.AddComponent<HorizontalLayoutGroup>();
            actionsLayout.spacing = 12;
            actionsLayout.childForceExpandHeight = true;
            actionsLayout.childForceExpandWidth = false;
            actionsLayout.childAlignment = TextAnchor.MiddleRight;
            
            // Sort Dropdown
            GameObject dropdown = CreateDropdown(sortOptions,
                actions.transform, Vector2.zero, new Vector2(140, 35)); // 드롭다운 너비 증가
            sortDropdown = dropdown.GetComponent<TMP_Dropdown>();
            
            // Favorites Button
            CreateButton(favoritesButtonText, actions.transform, Vector2.zero, 
                new Vector2(120, 35), buttonSecondary); // 버튼 너비 증가
            
            // Create Game Button
            CreateButton(createGameButtonText, actions.transform, Vector2.zero, 
                new Vector2(120, 35), accentColor); // 버튼 너비 증가
        }
        
        void CreateFiltersBar(Transform parent)
        {
            GameObject filtersBar = new GameObject("FiltersBar");
            filtersBar.transform.SetParent(parent, false);
            RectTransform filtersRect = filtersBar.AddComponent<RectTransform>();
            filtersRect.anchorMin = new Vector2(0, 1);
            filtersRect.anchorMax = new Vector2(1, 1);
            filtersRect.pivot = new Vector2(0.5f, 1);
            filtersRect.sizeDelta = new Vector2(-60, 50);
            filtersRect.anchoredPosition = new Vector2(0, -120);
            
            Image filtersBg = filtersBar.AddComponent<Image>();
            filtersBg.color = sidebarColor;
            
            filterContainer = filtersBar.transform;
            
            HorizontalLayoutGroup filtersLayout = filtersBar.AddComponent<HorizontalLayoutGroup>();
            filtersLayout.spacing = 8;
            filtersLayout.padding = new RectOffset(20, 20, 0, 0);
            filtersLayout.childForceExpandHeight = true;
            filtersLayout.childForceExpandWidth = false;
            filtersLayout.childAlignment = TextAnchor.MiddleLeft;
            
            // Filter chips
            for (int i = 0; i < filterChipTexts.Length; i++)
            {
                GameObject chip = CreateFilterChip(filterChipTexts[i], filtersBar.transform, i == 0);
                Button chipButton = chip.GetComponent<Button>();
                if (chipButton != null)
                    filterChips.Add(chipButton);
            }
        }
        
        GameObject CreateFilterChip(string text, Transform parent, bool isActive)
        {
            GameObject chip = new GameObject($"FilterChip_{text}");
            chip.transform.SetParent(parent, false);
            RectTransform rect = chip.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80, 30); // 너비 증가
            
            Image bg = chip.AddComponent<Image>();
            bg.color = isActive ? accentColor : buttonSecondary;
            
            Button btn = chip.AddComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;
            btn.targetGraphic = bg;
            
            GameObject textObj = CreateText(text, chip.transform,
                Vector2.zero, new Vector2(80, 30), 14); // 폰트 크기 증가
            TextMeshProUGUI tmpText = textObj.GetComponent<TextMeshProUGUI>();
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = isActive ? textPrimary : textSecondary;
            
            // 텍스트 RectTransform 설정
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            // 필터 칩 크기를 텍스트에 맞게 조정
            LayoutElement layoutElement = chip.AddComponent<LayoutElement>();
            layoutElement.minWidth = 80;
            layoutElement.preferredWidth = 80;
            layoutElement.flexibleWidth = 0;
            
            return chip;
        }
        
        void CreateRoomsGrid(Transform parent)
        {
            GameObject scrollView = CreateScrollView(parent);
            scrollView.name = "RoomsScrollView";
            RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0);
            scrollRect.anchorMax = new Vector2(1, 1);
            scrollRect.sizeDelta = new Vector2(-60, -240);
            scrollRect.anchoredPosition = new Vector2(0, -100);
            
            // Get content and add Grid Layout
            roomsGridContainer = scrollView.transform.Find("Viewport/Content");
            GridLayoutGroup grid = roomsGridContainer.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(280, 280); // 카드 높이를 줄임
            grid.spacing = new Vector2(16, 16);
            grid.padding = new RectOffset(0, 0, 0, 0);
            grid.childAlignment = TextAnchor.UpperLeft;
            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            
            // Content Size Fitter
            ContentSizeFitter fitter = roomsGridContainer.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        
        void CreateLoadMoreButton(Transform parent)
        {
            GameObject loadMore = new GameObject("LoadMoreContainer");
            loadMore.transform.SetParent(parent, false);
            RectTransform loadMoreRect = loadMore.AddComponent<RectTransform>();
            loadMoreRect.anchorMin = new Vector2(0, 0);
            loadMoreRect.anchorMax = new Vector2(1, 0);
            loadMoreRect.pivot = new Vector2(0.5f, 0);
            loadMoreRect.sizeDelta = new Vector2(-60, 60);
            loadMoreRect.anchoredPosition = new Vector2(0, 30);
            
            GameObject btn = CreateButton(loadMoreButtonText, loadMore.transform,
                Vector2.zero, new Vector2(180, 40), buttonSecondary); // 버튼 너비 증가
            RectTransform btnRect = btn.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = Vector2.zero;
        }
        
        void CreateSampleRoomCard()
        {
            if (roomCardPrefab != null) return;
            
            GameObject card = new GameObject("RoomCard_Prefab");
            card.transform.SetParent(roomsGridContainer, false);
            
            // Card RectTransform 설정
            RectTransform cardRect = card.GetComponent<RectTransform>();
            if (cardRect == null) cardRect = card.AddComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(280, 280);
            
            // RoomCard Component 추가
            RoomCardUI roomCardUI = card.AddComponent<RoomCardUI>();
            
            Image cardBg = card.AddComponent<Image>();
            cardBg.color = cardColor;
            
            // Add outline
            Outline outline = card.AddComponent<Outline>();
            outline.effectColor = backgroundColor;
            outline.effectDistance = new Vector2(1, 1);
            
            // Thumbnail
            GameObject thumbnail = new GameObject("Thumbnail");
            thumbnail.transform.SetParent(card.transform, false);
            RectTransform thumbRect = thumbnail.AddComponent<RectTransform>();
            thumbRect.anchorMin = new Vector2(0, 1);
            thumbRect.anchorMax = new Vector2(1, 1);
            thumbRect.pivot = new Vector2(0.5f, 1);
            thumbRect.sizeDelta = new Vector2(0, 120);
            thumbRect.anchoredPosition = Vector2.zero;
            
            Image thumbImg = thumbnail.AddComponent<Image>();
            thumbImg.color = new Color32(102, 126, 234, 255);
            roomCardUI.thumbnailImage = thumbImg;
            
            // Player count
            GameObject playerCount = new GameObject("PlayerCount");
            playerCount.transform.SetParent(thumbnail.transform, false);
            RectTransform playerRect = playerCount.AddComponent<RectTransform>();
            playerRect.anchorMin = new Vector2(1, 1);
            playerRect.anchorMax = new Vector2(1, 1);
            playerRect.pivot = new Vector2(1, 1);
            playerRect.sizeDelta = new Vector2(100, 24);
            playerRect.anchoredPosition = new Vector2(-8, -8);
            
            Image playerBg = playerCount.AddComponent<Image>();
            playerBg.color = new Color(0, 0, 0, 0.7f);
            
            GameObject playerText = CreateText("0명 플레이 중", playerCount.transform,
                Vector2.zero, new Vector2(100, 24), 12);
            playerText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            RectTransform playerTextRect = playerText.GetComponent<RectTransform>();
            playerTextRect.anchorMin = Vector2.zero;
            playerTextRect.anchorMax = Vector2.one;
            playerTextRect.sizeDelta = Vector2.zero;
            playerTextRect.anchoredPosition = Vector2.zero;
            roomCardUI.playerCountText = playerText.GetComponent<TextMeshProUGUI>();
            
            // Room Info Container
            GameObject info = new GameObject("RoomInfo");
            info.transform.SetParent(card.transform, false);
            RectTransform infoRect = info.AddComponent<RectTransform>();
            // 썸네일 아래부터 카드 끝까지
            infoRect.anchorMin = new Vector2(0, 0);
            infoRect.anchorMax = new Vector2(1, 0);
            infoRect.pivot = new Vector2(0.5f, 0);
            infoRect.sizeDelta = new Vector2(-20, 140);
            infoRect.anchoredPosition = new Vector2(0, 10);
            
            VerticalLayoutGroup infoLayout = info.AddComponent<VerticalLayoutGroup>();
            infoLayout.spacing = 4;
            infoLayout.padding = new RectOffset(0, 0, 0, 0);
            infoLayout.childForceExpandWidth = true; // 중요: 자식 요소가 전체 너비를 차지하도록
            infoLayout.childForceExpandHeight = false;
            infoLayout.childAlignment = TextAnchor.UpperLeft;
            
            // Title
            GameObject title = new GameObject("Title");
            title.transform.SetParent(info.transform, false);
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(260, 36);
            
            TextMeshProUGUI titleTMP = title.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "Room Title";
            titleTMP.fontSize = 16;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = textPrimary;
            titleTMP.alignment = TextAlignmentOptions.Left;
            titleTMP.overflowMode = TextOverflowModes.Ellipsis;
            
            // 폰트 설정
            if (boldFont != null)
            {
                titleTMP.font = boldFont;
            }
            else if (mainFont != null)
            {
                titleTMP.font = mainFont;
            }
            
            // 폰트 머티리얼 설정
            if (fontMaterial != null)
            {
                titleTMP.fontSharedMaterial = fontMaterial;
            }
            
            LayoutElement titleLayout = title.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 36;
            titleLayout.minHeight = 36;
            
            roomCardUI.titleText = titleTMP;
            
            // Creator
            GameObject creator = new GameObject("Creator");
            creator.transform.SetParent(info.transform, false);
            RectTransform creatorRect = creator.AddComponent<RectTransform>();
            creatorRect.sizeDelta = new Vector2(260, 20);
            
            TextMeshProUGUI creatorTMP = creator.AddComponent<TextMeshProUGUI>();
            creatorTMP.text = "by Creator";
            creatorTMP.fontSize = 13;
            creatorTMP.color = textSecondary;
            creatorTMP.alignment = TextAlignmentOptions.Left;
            
            // 폰트 설정
            if (mainFont != null)
            {
                creatorTMP.font = mainFont;
            }
            
            // 폰트 머티리얼 설정
            if (fontMaterial != null)
            {
                creatorTMP.fontSharedMaterial = fontMaterial;
            }
            
            LayoutElement creatorLayout = creator.AddComponent<LayoutElement>();
            creatorLayout.preferredHeight = 20;
            creatorLayout.minHeight = 20;
            
            roomCardUI.creatorText = creatorTMP;
            
            // Stats Container
            GameObject stats = new GameObject("Stats");
            stats.transform.SetParent(info.transform, false);
            RectTransform statsRect = stats.AddComponent<RectTransform>();
            statsRect.sizeDelta = new Vector2(260, 20);
            
            LayoutElement statsLayout = stats.AddComponent<LayoutElement>();
            statsLayout.preferredHeight = 20;
            statsLayout.minHeight = 20;
            
            HorizontalLayoutGroup statsHLayout = stats.AddComponent<HorizontalLayoutGroup>();
            statsHLayout.spacing = 10;
            statsHLayout.childForceExpandWidth = false;
            statsHLayout.childForceExpandHeight = true;
            statsHLayout.childAlignment = TextAnchor.MiddleLeft;
            
            // Like Stat
            GameObject likeStat = CreateStatElementSimple("[좋아요] 0%", stats.transform);
            roomCardUI.likeRateText = likeStat.GetComponent<TextMeshProUGUI>();
            
            // Play Count Stat
            GameObject playStat = CreateStatElementSimple("[플레이] 0", stats.transform);
            roomCardUI.playCountText = playStat.GetComponent<TextMeshProUGUI>();
            
            // Rating Stat
            GameObject ratingStat = CreateStatElementSimple("[평점] 0.0", stats.transform);
            roomCardUI.ratingText = ratingStat.GetComponent<TextMeshProUGUI>();
            
            // Tags Container
            GameObject tags = new GameObject("Tags");
            tags.transform.SetParent(info.transform, false);
            RectTransform tagsRect = tags.AddComponent<RectTransform>();
            tagsRect.sizeDelta = new Vector2(260, 24);
            
            LayoutElement tagsLayoutElement = tags.AddComponent<LayoutElement>();
            tagsLayoutElement.preferredHeight = 24;
            tagsLayoutElement.minHeight = 24;
            
            roomCardUI.tagsContainer = tags.transform;
            
            HorizontalLayoutGroup tagsLayout = tags.AddComponent<HorizontalLayoutGroup>();
            tagsLayout.spacing = 4;
            tagsLayout.childForceExpandWidth = false;
            tagsLayout.childForceExpandHeight = true;
            tagsLayout.childAlignment = TextAnchor.MiddleLeft;
            
            // Tag Prefab
            GameObject tagPrefab = CreateTagPrefab();
            roomCardUI.tagPrefab = tagPrefab;
            
            // Add button component
            Button cardButton = card.AddComponent<Button>();
            cardButton.transition = Selectable.Transition.None;
            roomCardUI.cardButton = cardButton;
            
            // Save as prefab reference
            roomCardPrefab = card;
            
            #if UNITY_EDITOR
            // Create actual prefab
            string prefabPath = "Assets/RoomCardPrefab.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(card, prefabPath);
            roomCardPrefab = prefab;
            Debug.Log($"Room Card Prefab이 생성되었습니다: {prefabPath}");
            #endif
        }
        
        GameObject CreateStatElementSimple(string text, Transform parent)
        {
            GameObject stat = new GameObject("Stat");
            stat.transform.SetParent(parent, false);
            
            RectTransform statRect = stat.AddComponent<RectTransform>();
            statRect.sizeDelta = new Vector2(60, 20);
            
            TextMeshProUGUI statText = stat.AddComponent<TextMeshProUGUI>();
            statText.text = text;
            statText.fontSize = 12;
            statText.color = textSecondary;
            statText.alignment = TextAlignmentOptions.Left;
            statText.overflowMode = TextOverflowModes.Ellipsis;
            
            // 폰트 설정
            if (mainFont != null)
            {
                statText.font = mainFont;
            }
            
            // 폰트 머티리얼 설정
            if (fontMaterial != null)
            {
                statText.fontSharedMaterial = fontMaterial;
            }
            
            LayoutElement layout = stat.AddComponent<LayoutElement>();
            layout.preferredWidth = 60;
            layout.minWidth = 50;
            layout.flexibleWidth = 0;
            
            return stat;
        }
        
        GameObject CreateTagPrefab()
        {
            GameObject tag = new GameObject("Tag_Prefab");
            tag.SetActive(false);
            
            RectTransform rect = tag.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(50, 20);
            
            Image bg = tag.AddComponent<Image>();
            bg.color = backgroundColor;
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(tag.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI textTMP = textObj.AddComponent<TextMeshProUGUI>();
            textTMP.text = "Tag";
            textTMP.fontSize = 11;
            textTMP.alignment = TextAlignmentOptions.Center;
            textTMP.color = textSecondary;
            textTMP.overflowMode = TextOverflowModes.Ellipsis;
            
            // 폰트 설정
            if (mainFont != null)
            {
                textTMP.font = mainFont;
            }
            
            // 폰트 머티리얼 설정
            if (fontMaterial != null)
            {
                textTMP.fontSharedMaterial = fontMaterial;
            }
            
            LayoutElement layout = tag.AddComponent<LayoutElement>();
            layout.preferredWidth = 50;
            layout.minWidth = 40;
            layout.flexibleWidth = 0;
            
            return tag;
        }
        
        // Helper Methods - 폰트 문제 해결
        GameObject CreateText(string text, Transform parent, Vector2 position, Vector2 size, int fontSize, FontStyle fontStyle = FontStyle.Normal)
        {
            GameObject textGO = new GameObject($"Text");
            textGO.transform.SetParent(parent, false);
            
            RectTransform rect = textGO.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            
            TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = fontStyle == FontStyle.Bold ? FontStyles.Bold : FontStyles.Normal;
            tmp.color = textPrimary;
            
            // 텍스트 선명도 개선 설정
            tmp.overrideColorTags = false;
            tmp.raycastTarget = false;
            tmp.richText = true;
            tmp.parseCtrlCharacters = true;
            tmp.extraPadding = true;
            tmp.enableKerning = true;
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;
            
            // 폰트 머티리얼 설정 (네모 문제 해결)
            if (fontMaterial != null)
            {
                tmp.fontSharedMaterial = fontMaterial;
            }
            
            // 폰트 설정
            if (fontStyle == FontStyle.Bold && boldFont != null)
            {
                tmp.font = boldFont;
            }
            else if (mainFont != null)
            {
                tmp.font = mainFont;
            }
            
            // 폰트 렌더링 모드 설정
            tmp.fontStyle = FontStyles.Normal;
            tmp.characterSpacing = 0;
            tmp.wordSpacing = 0;
            tmp.lineSpacing = 0;
            tmp.paragraphSpacing = 0;
            tmp.fontSizeMin = fontSize * 0.5f;
            tmp.fontSizeMax = fontSize * 1.5f;
            tmp.enableAutoSizing = false;
            
            return textGO;
        }
        
        GameObject CreateButton(string text, Transform parent, Vector2 position, Vector2 size, Color bgColor)
        {
            GameObject buttonGO = new GameObject($"Button_{text}");
            buttonGO.transform.SetParent(parent, false);
            
            RectTransform rect = buttonGO.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            
            Image bg = buttonGO.AddComponent<Image>();
            bg.color = bgColor;
            
            Button button = buttonGO.AddComponent<Button>();
            button.targetGraphic = bg;
            
            GameObject textObj = CreateText(text, buttonGO.transform,
                Vector2.zero, size, 16, FontStyle.Bold); // 폰트 크기 증가
            textObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            textObj.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            textObj.GetComponent<RectTransform>().anchorMax = Vector2.one;
            textObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            
            return buttonGO;
        }
        
        GameObject CreateInputField(string placeholder, Transform parent, Vector2 position, Vector2 size)
        {
            GameObject inputGO = new GameObject("SearchInput");
            inputGO.transform.SetParent(parent, false);
            
            RectTransform rect = inputGO.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            
            Image bg = inputGO.AddComponent<Image>();
            bg.color = buttonSecondary;
            
            TMP_InputField input = inputGO.AddComponent<TMP_InputField>();
            
            // Text area
            GameObject textArea = new GameObject("Text Area");
            textArea.transform.SetParent(inputGO.transform, false);
            RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.sizeDelta = new Vector2(-20, 0);
            textAreaRect.anchoredPosition = Vector2.zero;
            
            // Placeholder
            GameObject placeholderGO = CreateText(placeholder, textArea.transform,
                Vector2.zero, size, 16); // 폰트 크기 증가
            placeholderGO.name = "Placeholder";
            placeholderGO.GetComponent<TextMeshProUGUI>().color = textSecondary;
            placeholderGO.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            placeholderGO.GetComponent<RectTransform>().anchorMax = Vector2.one;
            placeholderGO.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            input.placeholder = placeholderGO.GetComponent<TMP_Text>();
            
            // Text
            GameObject textGO = CreateText("", textArea.transform,
                Vector2.zero, size, 16); // 폰트 크기 증가
            textGO.name = "Text";
            textGO.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            textGO.GetComponent<RectTransform>().anchorMax = Vector2.one;
            textGO.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            input.textComponent = textGO.GetComponent<TMP_Text>();
            input.textViewport = textAreaRect;
            
            return inputGO;
        }
        
        GameObject CreateDropdown(string[] options, Transform parent, Vector2 position, Vector2 size)
        {
            GameObject dropdownGO = new GameObject("SortDropdown");
            dropdownGO.transform.SetParent(parent, false);
            
            RectTransform rect = dropdownGO.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            
            Image bg = dropdownGO.AddComponent<Image>();
            bg.color = buttonSecondary;
            
            TMP_Dropdown dropdown = dropdownGO.AddComponent<TMP_Dropdown>();
            dropdown.targetGraphic = bg;
            
            // Template (필수)
            GameObject template = new GameObject("Template");
            template.transform.SetParent(dropdownGO.transform, false);
            template.SetActive(false);
            
            RectTransform templateRect = template.AddComponent<RectTransform>();
            templateRect.anchorMin = new Vector2(0, 0);
            templateRect.anchorMax = new Vector2(1, 0);
            templateRect.pivot = new Vector2(0.5f, 1);
            templateRect.anchoredPosition = new Vector2(0, 2);
            templateRect.sizeDelta = new Vector2(0, 150);
            
            Image templateBg = template.AddComponent<Image>();
            templateBg.color = sidebarColor;
            
            ScrollRect templateScroll = template.AddComponent<ScrollRect>();
            
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(template.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = new Vector2(-18, 0);
            viewportRect.anchoredPosition = Vector2.zero;
            viewport.AddComponent<Mask>();
            viewport.AddComponent<Image>().color = new Color(1, 1, 1, 0.01f);
            
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 28);
            
            GameObject item = new GameObject("Item");
            item.transform.SetParent(content.transform, false);
            RectTransform itemRect = item.AddComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0, 0.5f);
            itemRect.anchorMax = new Vector2(1, 0.5f);
            itemRect.sizeDelta = new Vector2(0, 20);
            
            Toggle itemToggle = item.AddComponent<Toggle>();
            
            GameObject itemBg = new GameObject("Item Background");
            itemBg.transform.SetParent(item.transform, false);
            RectTransform itemBgRect = itemBg.AddComponent<RectTransform>();
            itemBgRect.anchorMin = Vector2.zero;
            itemBgRect.anchorMax = Vector2.one;
            itemBgRect.sizeDelta = Vector2.zero;
            Image itemBgImage = itemBg.AddComponent<Image>();
            itemBgImage.color = new Color(1, 1, 1, 0);
            
            GameObject itemCheckmark = new GameObject("Item Checkmark");
            itemCheckmark.transform.SetParent(item.transform, false);
            RectTransform checkRect = itemCheckmark.AddComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0, 0.5f);
            checkRect.anchorMax = new Vector2(0, 0.5f);
            checkRect.sizeDelta = new Vector2(20, 20);
            checkRect.anchoredPosition = new Vector2(10, 0);
            Image checkImage = itemCheckmark.AddComponent<Image>();
            checkImage.color = accentColor;
            
            GameObject itemLabel = new GameObject("Item Label");
            itemLabel.transform.SetParent(item.transform, false);
            RectTransform labelRect = itemLabel.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(20, 1);
            labelRect.offsetMax = new Vector2(-10, -2);
            TextMeshProUGUI labelText = itemLabel.AddComponent<TextMeshProUGUI>();
            labelText.color = textPrimary;
            labelText.fontSize = 16; // 폰트 크기 증가
            
            itemToggle.targetGraphic = itemBgImage;
            itemToggle.graphic = checkImage;
            itemToggle.isOn = true;
            
            templateScroll.content = contentRect;
            templateScroll.viewport = viewportRect;
            
            dropdown.template = templateRect;
            dropdown.itemText = labelText;
            
            // Label
            GameObject label = CreateText(options[0], dropdownGO.transform,
                new Vector2(-10, 0), size, 16); // 폰트 크기 증가
            label.name = "Label";
            label.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            label.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            label.GetComponent<RectTransform>().anchorMax = Vector2.one;
            label.GetComponent<RectTransform>().sizeDelta = new Vector2(-35, 0);
            dropdown.captionText = label.GetComponent<TMP_Text>();
            
            // Arrow
            GameObject arrow = new GameObject("Arrow");
            arrow.transform.SetParent(dropdownGO.transform, false);
            RectTransform arrowRect = arrow.AddComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1, 0.5f);
            arrowRect.anchorMax = new Vector2(1, 0.5f);
            arrowRect.sizeDelta = new Vector2(20, 20);
            arrowRect.anchoredPosition = new Vector2(-15, 0);
            TextMeshProUGUI arrowText = arrow.AddComponent<TextMeshProUGUI>();
            arrowText.text = "▼";
            arrowText.fontSize = 14; // 폰트 크기 증가
            arrowText.color = textSecondary;
            arrowText.alignment = TextAlignmentOptions.Center;
            
            // Add options
            dropdown.options.Clear();
            foreach (string option in options)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }
            
            return dropdownGO;
        }
        
        GameObject CreateScrollView(Transform parent)
        {
            GameObject scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(parent, false);
            
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            
            // Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            viewportRect.anchoredPosition = Vector2.zero;
            
            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(1, 1, 1, 0.01f);
            Mask viewportMask = viewport.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            
            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = new Vector2(0, 800);
            contentRect.anchoredPosition = Vector2.zero;
            
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            
            // Scrollbar
            GameObject scrollbarGO = new GameObject("Scrollbar Vertical");
            scrollbarGO.transform.SetParent(scrollView.transform, false);
            RectTransform scrollbarRect = scrollbarGO.AddComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1, 0);
            scrollbarRect.anchorMax = new Vector2(1, 1);
            scrollbarRect.pivot = new Vector2(1, 0.5f);
            scrollbarRect.sizeDelta = new Vector2(8, 0);
            scrollbarRect.anchoredPosition = Vector2.zero;
            
            Image scrollbarBg = scrollbarGO.AddComponent<Image>();
            scrollbarBg.color = sidebarColor;
            
            Scrollbar scrollbar = scrollbarGO.AddComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            
            // Handle
            GameObject handle = new GameObject("Sliding Area");
            handle.transform.SetParent(scrollbarGO.transform, false);
            RectTransform slideArea = handle.AddComponent<RectTransform>();
            slideArea.anchorMin = Vector2.zero;
            slideArea.anchorMax = Vector2.one;
            slideArea.sizeDelta = Vector2.zero;
            
            GameObject handleGO = new GameObject("Handle");
            handleGO.transform.SetParent(handle.transform, false);
            RectTransform handleRect = handleGO.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(8, 8);
            
            Image handleImage = handleGO.AddComponent<Image>();
            handleImage.color = buttonSecondary;
            
            scrollbar.targetGraphic = handleImage;
            scrollbar.handleRect = handleRect;
            
            scroll.verticalScrollbar = scrollbar;
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            
            return scrollView;
        }
        
        Color GetTagTextColor(Color bgColor)
        {
            float brightness = (bgColor.r * 0.299f + bgColor.g * 0.587f + bgColor.b * 0.114f);
            return brightness > 0.5f ? Color.black : Color.white;
        }
    }
}
