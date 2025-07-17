using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Có thể cần nếu sử dụng LINQ sau này

public class Instantiator : MonoBehaviour
{
    public GameObject cardPrefab;
    private SwipeEffect _currentActiveSwipeEffect; // Tham chiếu đến SwipeEffect đang hoạt động

    void OnEnable() // Sử dụng OnEnable để đăng ký sự kiện khi script được kích hoạt
    {
        SecondCard.OnSecondCardBecomesActiveSwipeEffect += HandleNewActiveSwipeEffect;
    }

    void OnDisable() // Sử dụng OnDisable để hủy đăng ký sự kiện khi script bị vô hiệu hóa/hủy
    {
        SecondCard.OnSecondCardBecomesActiveSwipeEffect -= HandleNewActiveSwipeEffect;
        // Đảm bảo hủy đăng ký sự kiện của thẻ cũ nếu nó vẫn còn tham chiếu khi Instantiator bị tắt
        if (_currentActiveSwipeEffect != null)
        {
            _currentActiveSwipeEffect.cardDestroyed -= OnCardDestroyed;
        }
    }

    void Start()
    {
        // 1. Sinh ra thẻ đầu tiên (thẻ trên cùng) - Thẻ này sẽ là thẻ hoạt động chính, có SwipeEffect.
        GameObject firstCardObject = Instantiate(cardPrefab, transform, false);
        // Mặc định Instantiate sẽ thêm vào cuối danh sách con, tức là ở trên cùng.
        // Cần đảm bảo rằng thẻ này có SwipeEffect và không có SecondCard
        SwipeEffect initialSwipeEffect = firstCardObject.GetComponent<SwipeEffect>();
        if (initialSwipeEffect == null)
        {
            initialSwipeEffect = firstCardObject.AddComponent<SwipeEffect>();
        }
        SecondCard firstCardSecondCard = firstCardObject.GetComponent<SecondCard>();
        if (firstCardSecondCard != null)
        {
            Destroy(firstCardSecondCard);
        }

        // 2. Sinh ra thẻ thứ hai (thẻ phía sau) - Thẻ này sẽ có SecondCard.
        // Đặt nó xuống dưới thẻ đầu tiên.
        GameObject secondCardObject = Instantiate(cardPrefab, transform, false);
        secondCardObject.transform.SetAsFirstSibling(); // Đặt thẻ này xuống dưới cùng (dưới thẻ firstCardObject)
        // Cần đảm bảo rằng thẻ này có SecondCard và không có SwipeEffect
        SecondCard secondCardComponent = secondCardObject.GetComponent<SecondCard>();
        if (secondCardComponent == null)
        {
            secondCardComponent = secondCardObject.AddComponent<SecondCard>();
        }
        SwipeEffect secondCardSwipeEffect = secondCardObject.GetComponent<SwipeEffect>();
        if (secondCardSwipeEffect != null)
        {
            Destroy(secondCardSwipeEffect);
        }

        // 3. Thiết lập thẻ thứ hai để quan sát thẻ đầu tiên
        if (secondCardComponent != null)
        {
            secondCardComponent.SetFirstCardToObserve(firstCardObject, initialSwipeEffect);
            // Debug.Log("Instantiator: Set SecondCard to observe FirstCard: " + firstCardObject.name);
        }
        else
        {
            Debug.LogError("Instantiator: Second card prefab is missing SecondCard component or failed to add!", this);
        }

        // 4. Xử lý thẻ SwipeEffect mới này (đăng ký sự kiện và đặt làm thẻ hiện tại)
        HandleNewActiveSwipeEffect(initialSwipeEffect);
    }

    // KHÔNG CẦN PHƯƠNG THỨC NÀY NỮA VÌ CÁC TRƯỜNG HỢP ĐẶC BIỆT ĐƯỢC XỬ LÝ TRONG START VÀ INSTANTIATENEWCARD_FORNEXTSECOND
    // void InstantiateNewCard() // Giữ lại cho tương thích nếu có nơi khác gọi mà không có tham số
    // {
    //     InstantiateNewCard(false);
    // }

    // ĐỔI TÊN RÕ RÀNG HƠN CHO MỤC ĐÍCH SINH THẺ THỨ HAI MỚI
    private GameObject InstantiateNewCard_ForNextSecond()
    {
        GameObject newCard = Instantiate(cardPrefab, transform, false);
        newCard.transform.SetAsFirstSibling(); // Đặt thẻ mới xuống dưới cùng của stack (làm SecondCard mới)

        // Đảm bảo thẻ mới sinh ra có SecondCard component và không có SwipeEffect
        SecondCard sc = newCard.GetComponent<SecondCard>();
        if (sc == null)
        {
            sc = newCard.AddComponent<SecondCard>();
        }
        SwipeEffect se = newCard.GetComponent<SwipeEffect>();
        if (se != null)
        {
            Destroy(se);
        }
        return newCard;
    }


    private void HandleNewActiveSwipeEffect(SwipeEffect newActiveSwipeEffect)
    {
        if (_currentActiveSwipeEffect != null)
        {
            _currentActiveSwipeEffect.cardDestroyed -= OnCardDestroyed;
        }

        _currentActiveSwipeEffect = newActiveSwipeEffect;
        if (_currentActiveSwipeEffect != null)
        {
            _currentActiveSwipeEffect.cardDestroyed += OnCardDestroyed;
            // Debug.Log("Instantiator: New active SwipeEffect set and subscribed: " + newActiveSwipeEffect.name);

            // Cập nhật thẻ SecondCard đang tồn tại để nó quan sát SwipeEffect mới này.
            // Thẻ SecondCard luôn là thẻ ở vị trí đầu tiên (index 0) trong danh sách con của CardHolder
            if (transform.childCount >= 2) // Cần ít nhất 2 thẻ: active và second
            {
                GameObject secondCardGameObject = transform.GetChild(0).gameObject; // Lấy thẻ con đầu tiên
                SecondCard secondCardComponent = secondCardGameObject.GetComponent<SecondCard>();

                if (secondCardComponent != null)
                {
                    // Truyền tham chiếu thẻ SwipeEffect mới này cho SecondCard
                    secondCardComponent.SetFirstCardToObserve(newActiveSwipeEffect.gameObject, newActiveSwipeEffect);
                    // Debug.Log("Instantiator: Updated SecondCard (at index 0) to observe new active SwipeEffect.");
                }
                else
                {
                    Debug.LogWarning("Instantiator: No SecondCard component found on the expected second card (at index 0).", this);
                }
            }
        }
        else
        {
            Debug.LogError("Instantiator: Attempted to set null SwipeEffect as active.", this);
        }
    }

    void OnCardDestroyed()
    {
        // Một thẻ đã bị hủy, sinh ra thẻ mới làm SecondCard tiếp theo
        // Debug.Log("Instantiator: Card destroyed. Instantiating new SecondCard.");
        InstantiateNewCard_ForNextSecond(); // Gọi phương thức rõ ràng hơn để sinh thẻ SecondCard mới
        // SecondCard sẽ tự động kích hoạt event OnSecondCardBecomesActiveSwipeEffect khi nó được tạo ra
        // và trở thành active SwipeEffect. Instantiator sẽ lắng nghe event đó qua HandleNewActiveSwipeEffect.
    }
}