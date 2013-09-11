#region Copyright & License
//
// Copyright c 2011 A.PLUC, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Threading;
using Microsoft.SPOT.Hardware;
using Utilities.Helpers;

namespace Hardware.I2C.LED.BlinkM
{
    /// <summary>
    /// This class interfaces with the BlinkM - I2C full-color RGB LED w/24-bit color control
    /// 
    /// 4 Pin Configuration
    /// Pin -: GND - Ground
    /// Pin +: VCC - Supply voltage 3-5VDC
    /// Pin d: SDA - I2C data input/output
    /// Pin c: SCL - I2C clock input/output
    /// 
    /// </summary>
    public class BlinkMController : IDisposable
    {
        private const ushort I2CAddress = 0x09;
        private const int I2CClockRateKhz = 100;
        private const int I2CTimeout = 100;

        private static BlinkMController _instance = null;

        public static BlinkMController GetInstance()
        {
            if (_instance == null)
            {
                _instance = new BlinkMController();
            }
            return _instance;
        }

        private I2CDevice.Configuration _I2CConfig
            = new I2CDevice.Configuration(I2CAddress, I2CClockRateKhz);

        #region Constructors.

        /// <summary>
        /// Default constructor.
        /// </summary>
        private BlinkMController() { }

        #endregion

        /// <summary>
        /// Read operation from the BlinkM.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public int Read(Command.BaseCommand command)
        {
            I2CBus bus = I2CBus.GetInstance();
            int retValue = bus.Read(this._I2CConfig, command.GetReceiveBytes(), I2CTimeout);
            if (command.WaitMillis > 0)
            {
                Thread.Sleep(command.WaitMillis);
            }

            return retValue;
        }

        /// <summary>
        /// Write operation to the BlinkM.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public int Write(Command.BaseCommand command)
        {
            I2CBus bus = I2CBus.GetInstance();
            int retValue = bus.Write(this._I2CConfig, command.GetSendBytes(), I2CTimeout);
            if (command.WaitMillis > 0)
            {
                Thread.Sleep(command.WaitMillis);
            }

            return retValue;
        }

        /// <summary>
        /// Write operation to the BlinkM and Read operation from the BlinkM.
        /// </summary>
        /// <param name="command"></param>
        public void WriteRead(Command.BaseCommand command)
        {
            this.Write(command);
            this.Read(command);
        }

        public void SequentialRead(params Command.BaseCommand[] commands)
        {
            foreach (Command.BaseCommand command in commands)
            {
                this.Read(command);
            }
        }

        public void SequentialWrite(params Command.BaseCommand[] commands)
        {
            foreach (Command.BaseCommand command in commands)
            {
                this.Write(command);
            }
        }

        public void SequentialWriteRead(params Command.BaseCommand[] commands)
        {
            foreach (Command.BaseCommand command in commands)
            {
                this.WriteRead(command);
            }
        }

        #region IDisposable Members
        // The skeleton for this implementaion of IDisposable is taken directly from MSDN.
        // I have left the MSDN comments in place for reference.

        // Track whether Dispose has been called.
        private bool disposed = false;

        // Implement IDisposable.
        public void Dispose()
        {
            _instance = null;
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    //if (clock != null)
                    //    clock.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                /* Empty */

                // Note disposing has been done.
                disposed = true;

            }
        }

        #endregion
    }
}