using Grocery.App.ViewModels;
using Grocery.App.Views;

namespace Grocery.App
{
    public partial class App : Application
    {
        public App(LoginViewModel viewModel)
        {
            InitializeComponent();

            MainPage = new AppShell();
            //zoadt Apshell word uitgeschakeld
            MainPage = new LoginView(viewModel);
        }
    }
}
