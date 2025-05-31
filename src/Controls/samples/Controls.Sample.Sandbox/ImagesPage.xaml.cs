using System.Collections.ObjectModel;

namespace Maui.Controls.Sample.Sandbox;

public partial class ImagesPage : ContentPage
{
	private ObservableCollection<string>? _images;
	public ObservableCollection<string>? Images
	{
		get { return _images; }
		set
		{
			_images = value;
			OnPropertyChanged();
		}
	}

	public ImagesPage(List<string> imageslist)
	{
		InitializeComponent();
		Images = new ObservableCollection<string>(imageslist);
		BindingContext = this;
	}

	private void CollectionView_SizeChanged(object sender, EventArgs e)
	{
		if (PerformanceTimer.Stopwatch.IsRunning)
		{
			PerformanceTimer.Stopwatch.Stop();
			System.Diagnostics.Debug.WriteLine($"CollectionView load time: {PerformanceTimer.Stopwatch.ElapsedMilliseconds} ms");
			timeLabel.Text = $"{PerformanceTimer.Stopwatch.ElapsedMilliseconds} ms";
		}
	}
}