using System.Collections.ObjectModel;

namespace Maui.Controls.Sample;

public partial class MainPage : ContentPage
{
	private ObservableCollection<string>? items;
	public ObservableCollection<string>? Items
	{
		get => items;
		set
		{
			items = value;
			OnPropertyChanged();
		}
	}

	public MainPage()
	{
		InitializeComponent();
		BindingContext = this;
	}
	
	private void Button_Clicked2(object sender, EventArgs e)
	{
		Items = new ObservableCollection<string>();
		for (int i = 1; i <= 40; i++)
		{
			Items.Add($"Item {i}");
		}
		MyCollectionView.SelectedItem = "Item 39";
		Items = new ObservableCollection<string>();
		for (int i = 1; i <= 10; i++)
		{
			Items.Add($"Item {i}");
		}
	}
}