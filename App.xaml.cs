using MathMaster.View;
using MathMaster.Services;
using Microsoft.Maui.Controls;

namespace MathMaster
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
            //return new Window(new NavigationPage(new WelcomePage()));
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            BackgroundMusicService.Instance.Stop();
        }

        protected override async void OnResume()
        {
            base.OnResume();
            await BackgroundMusicService.Instance.ResumeLastMusicIfAny();
        }
    }
}