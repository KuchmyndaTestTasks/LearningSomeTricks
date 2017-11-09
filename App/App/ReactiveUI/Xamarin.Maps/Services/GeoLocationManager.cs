using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace App.ReactiveUI.Xamarin.Maps.Services
{
    sealed class GeoLocationManager
    {
        #region Constructors

        private GeoLocationManager()
        {
            geolocator = CrossGeolocator.Current;
            geolocator.DesiredAccuracy = 50;
        }

        #endregion

        #region Fields

        private readonly IGeolocator geolocator;
        private static readonly GeoLocationManager _locationManager  = new GeoLocationManager();

        #endregion

        #region Properties

        public static GeoLocationManager LocationManager
        {
            get { return _locationManager?? new GeoLocationManager(); }
        }
        //private static Lazy<GeoLocationManager> Lazy { get; } = new Lazy<GeoLocationManager>(() => new GeoLocationManager());

        #endregion

        #region Methods

        public bool IsAvailableService() =>
            CrossGeolocator.IsSupported && IsEnabledService() && geolocator.IsGeolocationAvailable;

        public bool IsEnabledService() => geolocator.IsGeolocationEnabled;

        public async Task RequestGrantingGpsPermission(Action<bool> requestPermissionsWay)
        {
        }

        public async Task<Position> GetCurrentPosition()
        {
            Position foundPosition = null;
            try
            {
                var crossPermission = CrossPermissions.Current;
                var permissionStatus =await crossPermission.CheckPermissionStatusAsync(Permission.Location);
                if (permissionStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        throw new Exception("We need GPS permissions");
                    }
                    var results = await crossPermission.RequestPermissionsAsync(Permission.Location);
                    permissionStatus = results[Permission.Location];
                }
                if (permissionStatus == PermissionStatus.Granted)
                {
                    if (!IsAvailableService())
                        throw new Exception("Device isn't supported GPS-location.");
                    if (!IsEnabledService())
                        throw new Exception("Service isn't enabled. Please, switch on GPS.");
                    foundPosition = await geolocator.GetPositionAsync(new  TimeSpan(10000));
                }
            }
            catch (Exception exception)
            {
                //Todo: need implement alert window
                Debug.WriteLine(exception.Message);
            }

            return foundPosition;
        }

        #endregion
    }
}
