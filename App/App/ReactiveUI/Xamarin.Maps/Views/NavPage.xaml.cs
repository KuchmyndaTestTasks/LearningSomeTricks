using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using App.ReactiveUI.Authorization.ViewModels;
using App.ReactiveUI.Authorization.Views;
using App.ReactiveUI.Xamarin.Maps.Services;
using App.ReactiveUI.Xamarin.Maps.ViewModels;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace App.ReactiveUI.Xamarin.Maps.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavPage : ContentPage, IViewFor<NavPageViewModel>
    {
        #region <Constructors & Destructors>

        public NavPage()
        {
            InitializeComponent();
            ViewModel = new NavPageViewModel();
            //xamarin maps pins
            //this.Bind(ViewModel, vm => vm.RouteTrip, v => v.Route);
            this.WhenAnyValue(x => x.ViewModel.RouteTrip)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(trip =>
                {
                    Map.Pins.Add(new Pin() {Position = trip.from, Type = PinType.Place, Label = "user"});
                    Map.Pins.Add(new Pin() {Position = trip.to, Type = PinType.Place, Label = "your place"});
                    Map.MoveToRegion(MapCalculator.GetCenteredZoom(trip.from, trip.to));
                    //Map.MoveToRegion(MapSpan.FromCenterAndRadius(MapCalculator.CenterOfPoints(trip.from,trip.to),
                    //    Distance.FromKilometers(MapCalculator.CalculateDistance(trip.from, trip.to,
                    //        MapCalculator.DistanceUnit.Kilometer))));
                });
        }

        #endregion

        #region <Fields>

        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            "ViewModel",
            typeof(NavPageViewModel),
            typeof(NavPage));

        private NavPageViewModel.Route _pins;

        #endregion

        #region <Properties>

        public NavPageViewModel.Route Route
        {
            get { return _pins; }
            set
            {
                _pins = value; 
            }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (NavPageViewModel) value; }
        }

        public NavPageViewModel ViewModel
        {
            get { return (NavPageViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

    }
}