
using System.Diagnostics;
using Maui.Controls.Sample.Sandbox;

namespace Maui.Controls.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	public List<string>? ImageLocalFilePaths { get; set; }

	public List<string>? ImageURIPaths { get; set; }

	public List<string>? ResourceImageNames { get; set; }

	protected  override void OnAppearing()
	{
		base.OnAppearing();
		//await LoadLocalFilePaths();
		LoadImageURIPaths();
		LoadResourceImageNames();
	}

	private void LoadResourceImageNames()
	{
		ResourceImageNames = new List<string>();
		for (int i = 1; i < 20; i++)
		{
			ResourceImageNames.Add($"dog{i}");
		}
	}

	private async Task LoadLocalFilePaths()
	{
		ImageLocalFilePaths = new List<string>();
		for (int i = 1; i < 20; i++)
		{
			var resourceStream = await FileSystem.OpenAppPackageFileAsync($"dog{i}.png");
			var path = Path.Combine(FileSystem.AppDataDirectory, $"dog{i}.png");

			if (File.Exists(path))
				File.Delete(path);

			using (var fileStream = File.Create(path))
			{
				await resourceStream.CopyToAsync(fileStream);
				ImageLocalFilePaths.Add(path);
			}
		}
	}
	private void LoadImageURIPaths()
	{
		ImageURIPaths = new List<string>();
	}

	private async void OpenFromAppResources_Clicked(object sender, EventArgs e)
	{
		if (ResourceImageNames is not null)
		{
			PerformanceTimer.Stopwatch.Restart();
			await Navigation.PushAsync(new ImagesPage(ResourceImageNames));
		}
	}

	private async void OpenFormLocalStorage_Clicked(object sender, EventArgs e)
	{
		if (ImageLocalFilePaths is not null)
		{
			PerformanceTimer.Stopwatch.Restart();
			await Navigation.PushAsync(new ImagesPage(ImageLocalFilePaths));
		}
	}

	private void OpenFromUrl_Clicked(object sender, EventArgs e)
	{
	}
}

public static class PerformanceTimer
{
	public static Stopwatch Stopwatch { get; set; } = new Stopwatch();
}