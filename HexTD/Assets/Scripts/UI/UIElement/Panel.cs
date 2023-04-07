namespace UI.UIElement
{
	public class Panel : RootUIElement
	{
		protected virtual bool SuppressReleaseAfterDisappear => false;

		protected override void OnDisappeared()
		{
			if (!SuppressReleaseAfterDisappear)
			{
				Destroy(gameObject);
			}

			base.OnDisappeared();
		}
	}
}