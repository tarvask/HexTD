using Tools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI.Tools.SimpleToggle
{
	public class SimpleToggleGroup : BaseMonoBehaviour
	{
		public ToggleGroup ToggleGroup;
		public SimpleToggleItem ToggleItemPrefab;

		public SimpleToggleItem AddToggle(string label)
		{
			var newToggleItem = Object.Instantiate(ToggleItemPrefab, ToggleGroup.transform);
			ToggleGroup.RegisterToggle(newToggleItem.Toggle);
			newToggleItem.Toggle.group = ToggleGroup;

			newToggleItem.Label.text = label;

			return newToggleItem;
		}

		public void EnableOneToggle(Toggle toggle, bool isSendCallback = true)
		{
			ToggleGroup.SetAllTogglesOff(isSendCallback);

			if (isSendCallback)
			{
				toggle.isOn = true;
			}
			else
			{
				toggle.SetIsOnWithoutNotify(true);
			}

			ToggleGroup.NotifyToggleOn(toggle, isSendCallback);
		}
	}
}