namespace dropkick.Configuration.Dsl.Files
{
	public interface UnzipOptions
	{
		UnzipOptions To(string destinationPath);
		void DeleteDestinationBeforeDeploying();
	}
}
