using TMPro;
using UnityEngine;
using WindowSystem.View;

namespace UI.MatchInfoRoundStartInfo
{
	public class MatchRoundStartInfoWindowView: WindowViewBase
	{
		[SerializeField] private TextMeshProUGUI roundNumberText;

		public TextMeshProUGUI RoundNumberText => roundNumberText;
	}
}