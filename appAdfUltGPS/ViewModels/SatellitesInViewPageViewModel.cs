using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using lib_NMEA_GPS;
using System.Collections.ObjectModel;

namespace appAdfUltGPS.ViewModels
{
    public class SatellitesInViewPageViewModel : INotifyPropertyChanged, ICleanUpViewModel
    {
        private NMEA_GPS gpsDevice;
        private SatelliteDataIDComparer satelliteIDEqC = new SatelliteDataIDComparer();
        // ** Not Used ** private SatelliteDataPropertyComparer satellitePropEqC = new SatelliteDataPropertyComparer();

        #region BindingProperties
        private string _messageCount;
        public string MessageCount
        {
            get { return _messageCount; }
            set { Set(ref _messageCount, value); }
        }
        private string _satelliteCount;
        public string SatelliteCount
        {
            get { return _satelliteCount; }
            set { Set(ref _satelliteCount, value); }
        }
        public ObservableCollection<SatelliteData> SatelliteData = new ObservableCollection<SatelliteData>();
        #endregion

        public SatellitesInViewPageViewModel()
        {
            gpsDevice = (App.Current as App).GPSdevice;
            // Subscribe to the SatellitesInViewDataReceivedEvent 
            if (gpsDevice != null)
                gpsDevice.SatellitesInViewDataReceivedEvent += SatellitesInViewDataListener;
        }

        public void CleanUpViewModel()
        {   // Un-Subscribe from the SatellitesInViewDataReceivedEvent and release the reference to the gpsDevice object
            // so that the garbage collector can re-claim the memory allocated for instances of this class.
            if (gpsDevice != null)
                gpsDevice.SatellitesInViewDataReceivedEvent -= SatellitesInViewDataListener;
            gpsDevice = null;
        }

        private void SatellitesInViewDataListener(object sender, SatellitesInViewDataEventArgs e)
        {   // Update our property values from the ActiveSatellitesDataEventArgs.  The view will be notified of the updates.
            try
            {
                MessageCount = e.SatellitesInViewData.MessageCount.ToString();
                SatelliteCount = e.SatellitesInViewData.SatellitesInView.ToString();
                // If the satellite count is zero, just clear the list, don't bother trying to determine if there are items to add, delete or update.
                if (e.SatellitesInViewData.SatellitesInView <= 0)
                {
                    SatelliteData.Clear();
                }
                else  // Check to see if satellites need to be added to, deleted from or updated within the observable collection based upon the new satellite data received. 
                {
                    // From the list of SatelliteDataObj received, identify each item whose ID is  not already in the
                    // SatelliteData observable collection.  These items we will have to add to the observable collection.
                    var AddIDs = e.SatellitesInViewData.SatelliteData.Except(SatelliteData, satelliteIDEqC);

                    // From the SatelliteData observable collection, identify each item whose ID is not in the
                    // list of SatelliteDataObj received.  These items we will have to delete from the observable collection.
                    var DeleteIDs = SatelliteData.Except(e.SatellitesInViewData.SatelliteData, satelliteIDEqC);

                    // TODO: Consider refactoring this code to 'replace' an item in the Delete list with an item from the Add list, then add or delete any where add<or>delete.
                    // Delete items
                    foreach (SatelliteData satDataObj in DeleteIDs.ToList())
                    {
                        for (int i = 0; i < SatelliteData.Count; i++)
                        {
                            if (SatelliteData[i].SatelliteID == satDataObj.SatelliteID)
                                SatelliteData.RemoveAt(i);
                        }
                    }
                    // Add Items
                    foreach (SatelliteData satDataObj in AddIDs)
                    {
                        SatelliteData.Add(new SatelliteData(satDataObj));
                    }
                    // Update Items - Azimuth,Elevation and SNR
                    foreach (SatelliteData satDataObj in e.SatellitesInViewData.SatelliteData)
                    {   // Find the item within the observable collection whose ID matches the current satDataObj.
                        SatelliteData itemToUpdate = SatelliteData.First(x => x.SatelliteID == satDataObj.SatelliteID);
                        // Update this satellite's properties if any different from the properties of the corresponding satellite within the new data received.
                        if ( !(itemToUpdate.Azimuth.Equals(satDataObj.Azimuth)) || !(itemToUpdate.Elevation.Equals(satDataObj.Elevation)) || !(itemToUpdate.SNR.Equals(satDataObj.SNR)))
                        {
                            itemToUpdate.Azimuth = satDataObj.Azimuth;
                            itemToUpdate.Elevation = satDataObj.Elevation;
                            itemToUpdate.SNR = satDataObj.SNR;
                        }
                    }
                }
            }
            catch(Exception ex)
            {    
                // TODO: do something useful if an exception is caught.
                 string message = ex.Message.ToString();
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
    /// <summary>
    /// Helper class used when searching two List<SatelliteData> objects for satellites with the same SatelliteID.
    /// </summary>
    class SatelliteDataIDComparer : IEqualityComparer<SatelliteData>
    {
        public bool Equals(SatelliteData x, SatelliteData y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether the SatelliteID properties are equal. 
            return x != null && y != null && x.SatelliteID.Equals(y.SatelliteID);
        }
        public int GetHashCode(SatelliteData obj)
        {
            //Get hash code for the SatelliteID property. 
            return obj.SatelliteID.GetHashCode();
        }
    }
    /// <summary>
    /// Helper class used for determining if the properties of two satellites within two List<SatelliteData> objects are equal.
    /// ** This class is not used within this code and can be omitted **
    /// </summary>
    class SatelliteDataPropertyComparer : IEqualityComparer<SatelliteData>
    {
        public bool Equals(SatelliteData x, SatelliteData y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether the SatelliteID properties are equal. 
            return x != null && y != null && x.Azimuth.Equals(y.Azimuth) && x.Elevation.Equals(y.Elevation) && x.SNR.Equals(y.SNR);
        }
        public int GetHashCode(SatelliteData obj)
        {
            //Get hash code for the Azimuth property. 
            int hashAzimuth = obj.Azimuth.GetHashCode();
            //Get hash code for the Elevation property. 
            int hashElevation = obj.Elevation.GetHashCode();    
            //Get hash code for the SNR property. 
            int hashSNR = obj.SNR.GetHashCode();
            //Calculate the hash code for the SateliteData object.
            return hashAzimuth ^ hashElevation ^ hashSNR;
        }
    }
}
