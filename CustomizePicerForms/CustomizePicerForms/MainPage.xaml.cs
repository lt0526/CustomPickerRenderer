using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace CustomizePicerForms
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            MyPicker picker = new MyPicker
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 73,
                HeightRequest = 25,
                //BackgroundColor = Color.Black
                TextColor = Color.Black
            };

            List<string> myList = new List<string>();
            myList.Add("OPTIONS 1");
            myList.Add("OPTIONS 2");
            myList.Add("OPTIONS 3");
            myList.Add("OPTIONS 4");
            myList.Add("OPTIONS 5");

            foreach (string str in myList)
            {
                picker.Items.Add(str);
            }

            this.Content = picker;


            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;

            picker.Focused += Picker_Focused;
            picker.Unfocused += Picker_Unfocused;
        }

        private void Picker_Unfocused(object sender, FocusEventArgs e)
        {

        }

        private void Picker_Focused(object sender, FocusEventArgs e)
        {

        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
