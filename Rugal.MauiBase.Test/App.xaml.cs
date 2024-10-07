using Rugal.MauiBase.Core.ApplicationBase;

namespace Rugal.MauiBase.Test
{
    public partial class App : ProviderApp
    {
        public App(IServiceProvider Provider) : base(Provider) 
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }
    }
}
