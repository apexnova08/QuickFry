using QuickFry.ViewModels;

namespace QuickFry;

public partial class CTestPage : ContentPage
{
	CustomerViewModel CVM = new CustomerViewModel();

	public CTestPage()
	{
		InitializeComponent();
	}

	void Order(object sender, EventArgs e)
	{
		//CVM.SubmitOrder();
	}
}