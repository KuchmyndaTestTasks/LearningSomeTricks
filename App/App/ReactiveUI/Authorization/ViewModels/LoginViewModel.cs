using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Legacy;
using ReactiveCommand = ReactiveUI.Legacy.ReactiveCommand;

namespace App.ReactiveUI.Authorization.ViewModels
{
    /*
     *  Our ViewModel needs to inherit from ReactiveObject.
        We'll add a ReactiveCommand (which is an implementation of the ICommand used in MVVM) for our button Login.
        We'll add two properties for Email and Password. For the INotifyPropertyChanged implementation ReactiveUI 
        gives us the RaiseAndSetIfChanged.
        We'll add another property that will give us an idea when the login action is happening and when the action 
        is done. This is not a property we want to set from our View. It's a "read-only" calculated property that can 
        only be set by the LoginViewModel.
     */
    public class LoginViewModel:ReactiveObject
    {
        #region <Constructors & Destructors>

        public LoginViewModel()
        {
            var canLogin = this.WhenAnyValue(x => Email, x => Password, (email, password) =>
             !String.IsNullOrWhiteSpace(email) &&
                Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)) &&
                // check if password not empty
                !String.IsNullOrWhiteSpace(password));

            /*
                Now our ReactiveCommand for the Login button can observe the canLogin to 
                enable/disable the button. When the button is clicked we'll execute an 
                async Task. So in the constructor of our ViewModel we add:
            */
            LoginCommand = ReactiveCommand.CreateAsyncTask(canLogin,async o =>
            {
                await Task.Delay(4000).ConfigureAwait(false);
            });

            //Next we hook our ObservableAsPropertyHelper IsLoading to the ReactiveCommand 
            //so the property changes when the Login action is executing.In the constructor 
            //of our ViewModel we add:
            LoginCommand.IsExecuting.ToProperty(this, x => x.IsLoading, out _isLoading);
        }
        /*
         *  Now you can start the app and look if you have errors. The app however still doesn't react to anything. First we'll add an observable so we can verify if we can click on the Login button or not. 
            So we need an observable that changes when
            Email contains a valid email address
            Password is not empty 
            In the constructor of the LoginViewModel we add:
         */
        #endregion
        #region <Fields>
        private string _email;
        private string _password;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        #endregion

        #region <Properties>

        public bool IsLoading => _isLoading.Value;
        public ReactiveCommand<Unit> LoginCommand { get; private set; }

        public string Email
        {
            get { return _email??""; }
            set { this.RaiseAndSetIfChanged(ref _email, value); }
        }

        public string Password
        {
            get { return _password??""; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        #endregion

    }
}
