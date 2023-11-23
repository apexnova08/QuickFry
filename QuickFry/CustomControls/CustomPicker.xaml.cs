namespace QuickFry.CustomControls;

public partial class CustomPicker : Frame
{
	public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(CustomPicker), propertyChanged: (bindable, oldVal, newVal) =>
	{
		var control = (CustomPicker)bindable;
        control.cPicker.Title = newVal as string;
	});
	public CustomPicker()
	{
		InitializeComponent();
	}

	public string Title
	{
		get => GetValue(TitleProperty) as string;
		set => SetValue(TitleProperty, value);
	}
}