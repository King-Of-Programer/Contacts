using Android.Widget;
using ContactsApp.Interfaces;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(ContactsApp.Droid.Classes.XamarinToast))]
namespace ContactsApp.Droid.Classes
{
    public class XamarinToast : IToast
    {
        public void LongMessage(string message)
        {
            Toast.MakeText(AndroidApp.Context, message, ToastLength.Long).Show();
        }

        public void ShortMessage(string message)
        {
            Toast.MakeText(AndroidApp.Context, message, ToastLength.Short).Show();
        }
    }
}