namespace Tools.Addressables.Download
{
	public interface IDownloadProgress
	{
		void Report(long downloadedBytes, long totalBytes);
	}
}