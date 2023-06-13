using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.OrderItem
{
    public class OrderItemView : BaseMonoBehaviour
    {
        [field: SerializeField] public Button CancelButton { get; private set; }
    }
}