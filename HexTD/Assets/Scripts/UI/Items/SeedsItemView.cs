using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SeedItem
{
    public class SeedsItemView : BaseMonoBehaviour
    {
        [field:SerializeField] public Button BuyButton { get; private set; }
        [field:SerializeField] public Button InfoButton { get; private set; }
    }
}