using System;
using System.Reactive.Linq;
using App.ReactiveUI.Authorization.ViewModels;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ReactiveUI.Authorization.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage, IViewFor<LoginViewModel>
    {
        public LoginPage()
        {
            InitializeComponent();

            ViewModel = new LoginViewModel();
            this.Bind(ViewModel, viewModel => viewModel.Password,p=>p.Password.Text);
            this.Bind(ViewModel, viewModel => viewModel.Email, p => p.Username.Text);
            this.BindCommand(ViewModel, viewModel => viewModel.LoginCommand, p => p.LoginEvent);

            /*
             * Finally our View needs to enable and disable the Email textbox,
             * Password textbox and Loading Label based on the IsLoading property. 
             * After the bindings in our LoginPage constructor we add:             
             */

            this.WhenAnyValue(x => x.ViewModel.IsLoading)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(busyState =>
                {
                    Username.IsEnabled = !busyState;
                    Password.IsEnabled = !busyState;
                    LoginEvent.IsEnabled = busyState;
                });
        }

        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            "ViewModel",
            typeof(LoginViewModel), 
            typeof(LoginPage));
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (LoginViewModel) value; }
        }

        public LoginViewModel ViewModel
        {
            get { return (LoginViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty,value); }
        }
    }
}