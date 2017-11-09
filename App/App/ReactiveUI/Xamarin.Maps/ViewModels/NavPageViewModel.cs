using App.ReactiveUI.Xamarin.Maps.Services;
using ReactiveUI;
using Xamarin.Forms.Maps;

namespace App.ReactiveUI.Xamarin.Maps.ViewModels
{
    public class NavPageViewModel:ReactiveObject
    {
        public struct Route
        {
            public global::Xamarin.Forms.Maps.Position from { get; set; }
            public global::Xamarin.Forms.Maps.Position to { get; set; }
        }
       
        #region <Constructors>

        public NavPageViewModel()
        {
            var found = GeoLocationManager.LocationManager.GetCurrentPosition().GetAwaiter().GetResult();
            UserLocation = new Position(found.Latitude,found.Longitude);
            PortPoint = new Position(50.753600f, -1.652888f);

            _route=this.WhenAnyValue(model => model.UserLocation, model => model.PortPoint, (position, position1) =>
                new Route
                {
                    from = position,
                    to = position1
                }
            ).ToProperty(this, model => model.RouteTrip, out _route);
        }

        #endregion

        #region <Fields>

        private Position _portPoint;
        private Position _userLocation;
        public readonly ObservableAsPropertyHelper<Route> _route;

        #endregion

        #region <Properties>

        public Position PortPoint
        {
            get { return _portPoint; }
            set { this.RaiseAndSetIfChanged(ref _portPoint,value); }
        }

        public Position UserLocation
        {
            get { return _userLocation; }
            set { this.RaiseAndSetIfChanged(ref _userLocation, value); }
        }

        public Route RouteTrip
        {
            get => _route.Value;
        }

        #endregion
    }
}
