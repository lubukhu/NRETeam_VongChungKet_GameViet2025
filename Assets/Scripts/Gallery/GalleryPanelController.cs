// [tooltips] Điều khiển toàn bộ logic và hiển thị cho Gallery Panel.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GalleryPanelController : MonoBehaviour
{
    [Header("Tab Content Panels")]
    [SerializeField] private GameObject characterContentPanel;
    [SerializeField] private GameObject backgroundContentPanel;
    [SerializeField] private GameObject endingContentPanel;

    [Header("Databases (Source of Truth)")]
    [Tooltip("Kéo asset CharacterDatabase vào đây.")]
    [SerializeField] private CharacterDatabase characterDB;
    [Tooltip("Kéo asset BackgroundDatabase vào đây.")]
    [SerializeField] private BackgroundDatabase backgroundDB;
    [Tooltip("Kéo asset EndingDatabase vào đây.")]
    [SerializeField] private EndingDatabase endingDB; 

    [Header("UI Setup")]
    [SerializeField] private GameObject galleryItemPrefab;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Transform charactersContentParent;
    [SerializeField] private Transform backgroundsContentParent;
    [SerializeField] private Transform endingsContentParent;

    private bool _isPopulated = false;

    private void OnEnable()
    {
        if (!_isPopulated)
        {
            PopulateAllGalleries();
            _isPopulated = true;
        }
        ShowCharacterTab();
    }

    private void PopulateAllGalleries()
    {
        foreach (Transform child in charactersContentParent) Destroy(child.gameObject);
        foreach (Transform child in backgroundsContentParent) Destroy(child.gameObject);
        foreach (Transform child in endingsContentParent) Destroy(child.gameObject);

        PopulateCharacterGallery();
        PopulateBackgroundGallery();
        PopulateEndingGallery();
    }
    
    private void PopulateCharacterGallery()
    {
        if (characterDB == null || GalleryDataManager.Instance == null) return;
        List<CharacterName> unlockedCharacters = GalleryDataManager.Instance.UnlockedCharacters;

        foreach (var charInfo in characterDB.allCharacters)
        {
            GameObject itemGO = Instantiate(galleryItemPrefab, charactersContentParent);
            GalleryItemUI itemUI = itemGO.GetComponent<GalleryItemUI>();

            if (unlockedCharacters.Contains(charInfo.characterEnum))
            {
                itemUI.Setup(charInfo.characterSprite, charInfo.displayName);
            }
            else
            {
                itemUI.SetLockedState(lockedSprite, "Chưa gặp gỡ");
            }
        }
    }

    private void PopulateBackgroundGallery()
    {
        if (backgroundDB == null || GalleryDataManager.Instance == null) return;
        List<string> unlockedBackgroundNames = GalleryDataManager.Instance.UnlockedBackgroundNames;

        foreach (var bgInfo in backgroundDB.allBackgrounds)
        {
            GameObject itemGO = Instantiate(galleryItemPrefab, backgroundsContentParent);
            GalleryItemUI itemUI = itemGO.GetComponent<GalleryItemUI>();

            if (unlockedBackgroundNames.Contains(bgInfo.backgroundName))
            {
                itemUI.Setup(bgInfo.backgroundSprite, bgInfo.backgroundName);
            }
            else
            {
                itemUI.SetLockedState(lockedSprite);
            }
        }
    }

    // HÀM NÀY ĐÃ ĐƯỢC CẬP NHẬT ĐỂ DÙNG ENDINGDATABASE
    private void PopulateEndingGallery()
    {
        if (endingDB == null || GalleryDataManager.Instance == null) return;
        List<string> unlockedEndingIDs = GalleryDataManager.Instance.UnlockedEndingIDs;

        // Duyệt qua Ending Database để biết tất cả các ending có thể có
        foreach (var endingInfo in endingDB.allEndings)
        {
            GameObject itemGO = Instantiate(galleryItemPrefab, endingsContentParent);
            GalleryItemUI itemUI = itemGO.GetComponent<GalleryItemUI>();

            // Kiểm tra xem ending này đã được mở khóa chưa
            if (unlockedEndingIDs.Contains(endingInfo.endingId))
            {
                itemUI.Setup(endingInfo.endingSprite, endingInfo.endingDescription);
            }
            else
            {
                itemUI.SetLockedState(lockedSprite, "Kết cục chưa biết");
            }
        }
    }

    // --- CÁC HÀM ĐIỀU KHIỂN TAB ---
    public void ShowCharacterTab()
    {
        characterContentPanel.SetActive(true);
        backgroundContentPanel.SetActive(false);
        endingContentPanel.SetActive(false);
    }

    public void ShowBackgroundTab()
    {
        characterContentPanel.SetActive(false);
        backgroundContentPanel.SetActive(true);
        endingContentPanel.SetActive(false);
    }

    public void ShowEndingTab()
    {
        characterContentPanel.SetActive(false);
        backgroundContentPanel.SetActive(false);
        endingContentPanel.SetActive(true);
    }
}