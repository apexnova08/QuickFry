using Firebase.Database;
using QuickFry.Models;
using System.Collections.ObjectModel;

namespace QuickFry;

public partial class ATestPage : ContentPage
{
	public ObservableCollection<Test> Items { get; set; } = new ObservableCollection<Test>();
	FirebaseClient client = new FirebaseClient("https://quickfry20-default-rtdb.asia-southeast1.firebasedatabase.app/");

	public ATestPage()
	{
		InitializeComponent();

		BindingContext = this;

		var collection = client.Child("Products").AsObservable<Test>().Subscribe((dbevent) =>
		{
			if (dbevent != null)
			{
				Items.Add(dbevent.Object);
			}
		});
	}
}