using TMPro;
using UI.MatchInfoRoundStartInfo;
using UI.OverlayWindow;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem.View;

namespace UI.MatchInfoWindow
{
	public class MatchInfoWindowView : WindowViewBase
	{
		[SerializeField] private TextMeshProUGUI currentRoundWithTimer;
		[SerializeField] private TextMeshProUGUI nextRoundStartWithTimer;
		[SerializeField] private Image enemyCastleHealthBar;
		[SerializeField] private TextMeshProUGUI enemyCastleHealthText;
		[SerializeField] private Image ourCastleHealthBar;
		[SerializeField] private TextMeshProUGUI ourCastleHealthText;
		[SerializeField] private Text ourGoldCoinsCountText;
		[SerializeField] private Text ourGoldCoinsIncomeText;
		[SerializeField] private Text ourCrystalsCountText;

		[Space] 
		[SerializeField] private RectTransform middlePanelRect;

		[Space] 
		[SerializeField] private MatchRoundStartInfoWindowView matchRoundStartInfoWindowView;
		[SerializeField] private EnemyFieldViewWindowView enemyFieldViewWindowView;

		public TextMeshProUGUI CurrentRoundWithTimer => currentRoundWithTimer;
		public TextMeshProUGUI NextRoundStartWithTimer => nextRoundStartWithTimer;
		public Image EnemyCastleHealthBar => enemyCastleHealthBar;
		public TextMeshProUGUI EnemyCastleHealthText => enemyCastleHealthText;
		public Image OurCastleHealthBar => ourCastleHealthBar;
		public TextMeshProUGUI OurCastleHealthText => ourCastleHealthText;
		public Text OurGoldCoinsCountText => ourGoldCoinsCountText;
		public Text OurGoldCoinsIncomeText => ourGoldCoinsIncomeText;
		public Text OurCrystalsCountText => ourCrystalsCountText;

		public RectTransform MiddlePanelRect => middlePanelRect;

		public MatchRoundStartInfoWindowView MatchRoundStartInfoWindowView => matchRoundStartInfoWindowView;
		public EnemyFieldViewWindowView EnemyFieldViewWindowView => enemyFieldViewWindowView;
	}
}