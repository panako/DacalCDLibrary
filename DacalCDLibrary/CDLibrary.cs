using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DacalCDLibrary
{
    //TODO enums with error codes and errors

    /// <summary>
    /// Class to manage Dacal's CD libraries connected to PC (Dacal DC-300).
    /// </summary>
    /// <remarks>
    /// API and DLLs (USBCDDLL.dll) used to be available at Dacal's website: <see href="http://www.dacal.com.tw/api.html"/> but they are no longer there.
    /// <para>
    /// Base on following post <see href="https://sourceforge.net/p/jnative/discussion/525506/thread/095564a3/"/>
    /// </para>
    /// </remarks>
    public class CDLibrary : IDisposable
    {

        #region import USBCDDLL.dll -> Dacal's API to CD Library

        #region Functions to init USB CD Library

        /// <summary>
        /// Initialization of the library. Must be called at start of an application.
        /// </summary>
        /// <returns></returns>
        [DllImport("USBCDDLL.dll")]
        private static extern int InitUSBCDLibrary();

        /// <summary>
        /// Should be called when application terminates.
        /// </summary>
        /// <returns></returns>
        [DllImport("USBCDDLL.dll")]
        private static extern int CloseUSBCDLibrary();

        /// <summary>
        /// Get the number of device on the system. USB can support up to 127 devices on each system.
        /// </summary>
        /// <returns>Will return the device number now exist on the system.</returns>
        [DllImport("USBCDDLL.dll")]
        private static extern int GetDeviceNumber();

        [DllImport("USBCDDLL.dll")]
        private static extern int EnumDevice(int gI);

        #endregion  //Functions to init USB CD Library

        #region Functions to operate USB CD Library

        [DllImport("USBCDDLL.dll")]
        private static extern int USBCDGetStatus(int deviceID);

        [DllImport("USBCDDLL.dll")]
        private static extern int USBCDReset(int deviceID);

        [DllImport("USBCDDLL.dll")]
        private static extern int USBCDMoveto(int deviceID, int index);

        [DllImport("USBCDDLL.dll")]
        private static extern int USBCDGetCDUp(int deviceID);

        [DllImport("USBCDDLL.dll")]
        private static extern int USBCDGetCDDown(int deviceID);

        [DllImport("USBCDDLL.dll")]
        private static extern int USBCDLEDOFF(int deviceID);

        [DllImport("USBCDDLL.dll")]
        private static extern int USBCDLEDON(int deviceID);

        [DllImport("USBCDDLL.dll")]
        private static extern void SetCDCallbackProc(CDDeviceChangePROC lpProc);

        /// <summary>
        /// Delegate passed to callback function
        /// </summary>
        /// <param name="i"></param>
        public delegate void CDDeviceChangePROC(int i);

        #endregion  //Functions to operate USB CD Library

        #endregion  //import USBCDDLL.dll -> Dacal's API to CD Library

        /// <summary>
        /// Delegate function that is called when device change is detected.
        /// </summary>
        /// <param name="deviceNumber"></param>
        public delegate void CDLibOnDeviceChangeHandler(int deviceNumber);
        /// <summary>
        /// Event is raised when device change is detected by Dacal's DLL.
        /// </summary>
        public event CDLibOnDeviceChangeHandler CDLibOnDeviceChange;

        /// <summary>
        /// Default constructor: Dacal's CD Library manager. Initialize Library USBCDDLL.dll.
        /// </summary>
        /// <exception cref="CDLibraryException">Error</exception>
        public CDLibrary()
        {
            int errorCode = InitUSBCDLibrary();

            if (errorCode > 0)
            {
                string msg = "ERROR: InitUSBCDLibrary returns " + errorCode.ToString();
                throw new CDLibraryException(msg);
            }

            SetCDCallbackProc(DeviceChangeCallback);    //set callback that informs about changes in connected devices
        }

        /// <summary>
        /// This callback function is called everytime device change is detected.
        /// </summary>
        /// <param name="device"></param>
        private void DeviceChangeCallback(int device)
        {
            //raise an evant about HW change
            if (CDLibOnDeviceChange != null)
                CDLibOnDeviceChange(device);
        }

        /// <summary>
        /// Get number of connected devices. USB can support up to 127 devices on each system.
        /// </summary>
        /// <returns>Device number now exist on the system.</returns>
        public long CDLibGetConnectedDeviceNumber()
        {
            return GetDeviceNumber();
        }

        /// <summary>
        /// Get unique device ID connected to PC.
        /// Each has a fixed ID, so you can control different device via device ID.
        /// </summary>
        /// <param name="deviceNumber">Index of device number exist on the system (get by <see cref="CDLibGetConnectedDeviceNumber"/>). First device is '0'</param>
        /// <returns>Device ID</returns>
        public int CDLibGetDeviceID(int deviceNumber)
        {
            return EnumDevice(deviceNumber);
        }

        /// <summary>
        /// Reset device
        /// </summary>
        /// <param name="deviceID">Device ID</param>
        /// <returns>Error code</returns>
        public int CDLibResetDevice(int deviceID)
        {
            return USBCDReset(deviceID);
        }

        /// <summary>
        /// Move CD library to position 'CDSlot' and get CD.
        /// If CD slot is out of range CD Library will do nothing.
        /// </summary>
        /// <param name="deviceID">Device (CD Library) number returned by <see cref="CDLibGetDeviceID"/></param>
        /// <param name="CDSlot">CD slot number (1..150)</param>
        /// <returns>Error code</returns>
        public int CDLibGetCD(int deviceID, int CDSlot)
        {
            return USBCDMoveto(deviceID, CDSlot);
        }

        /// <summary>
        /// GetDeviceStatus:
        /// <para>0 - DEVICE_COMMANDOK : Command success.</para>
        /// <para>1 - DEVICE_IDERROR : Device ID not exist.</para>
        /// <para>2 - DEVICE_BUSY : Device is now busy, please try later.</para>
        /// <para>3 - DEVICE_UNKNOWERROR : unknow error.</para>
        /// </summary>
        /// <param name="deviceID">Device ID</param>
        /// <returns>Error code:
        /// <para>0 - DEVICE_COMMANDOK : Command success.</para>
        /// <para>1 - DEVICE_IDERROR : Device ID not exist.</para>
        /// <para>2 - DEVICE_BUSY : Device is now busy, please try later.</para>
        /// <para>3 - DEVICE_UNKNOWERROR : unknow error.</para>
        /// </returns>
        public int CDLibGetDeviceStatus(int deviceID) 
        {
            return USBCDGetStatus(deviceID);
        }

        /// <summary>
        /// Get position's CD Up.
        /// </summary>
        /// <param name="deviceID">Device ID</param>
        /// <returns>Error code</returns>
        public int CDLibGetCDUP(int deviceID)
        {
            return USBCDGetCDUp(deviceID);
        }

        /// <summary>
        /// Get position's CD Down.
        /// </summary>
        /// <param name="deviceID">Device ID</param>
        /// <returns>Error code</returns>
        public int CDLibGetCDDown(int deviceID)
        {
            return USBCDGetCDDown(deviceID);
        }

        /// <summary>
        /// Set LED display on device ON, to show CD's position.
        /// </summary>
        /// <param name="deviceID">Device ID</param>
        /// <returns>Error code</returns>
        public int CDLibSetLEDDisplayOn(int deviceID)
        {
            return USBCDLEDON(deviceID);
        }

        /// <summary>
        /// Set LED display on device to OFF.
        /// </summary>
        /// <param name="deviceID">Device ID</param>
        /// <returns>Error code</returns>
        public int CDLibSetLEDDisplayOff(int deviceID)
        {
            return USBCDLEDOFF(deviceID);
        }

        #region Dispose section
        private bool disposed = false;

        /// <summary>
        /// Use C# destructor syntax for finalization code. Necessary for Dispose. 
        /// </summary>
        ~CDLibrary()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the private Dispose(bool) helper and indicate 
            // that we are explicitly disposing
            this.Dispose(true);

            // Tell the garbage collector that the object doesn't require any
            // cleanup when collected since Dispose was called explicitly.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //add resource here
                    CloseUSBCDLibrary();
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                disposed = true;
            }
        }

        #endregion
    }
}
