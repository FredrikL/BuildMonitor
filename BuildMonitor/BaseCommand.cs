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

namespace Hardware.I2C.LED.BlinkM.Command
{
    #region Enums

    public enum ScriptId
    {
        /// <summary>
        /// default startup, can be reprogrammed
        /// </summary>
        Eeprom_Script = (byte)0,
        /// <summary>
        /// red -> green -> blue
        /// </summary>
        Rgb = (byte)1,
        /// <summary>
        /// while -> off
        /// </summary>
        While_Flash = (byte)2,
        /// <summary>
        /// red -> off
        /// </summary>
        Red_Flash = (byte)3,
        /// <summary>
        /// green -> off
        /// </summary>
        Green_Flash = (byte)4,
        /// <summary>
        /// blue -> off
        /// </summary>
        Blue_Flash = (byte)5,
        /// <summary>
        /// cyan -> off
        /// </summary>
        Cyan_Flash = (byte)6,
        /// <summary>
        /// magenta -> off
        /// </summary>
        Magenta_Flash = (byte)7,
        /// <summary>
        /// yellow -> off
        /// </summary>
        Yellow_Flash = (byte)8,
        /// <summary>
        /// off
        /// </summary>
        Black = (byte)9,
        /// <summary>
        /// red -> yellow -> green -> cyan -> blue -> purple
        /// </summary>
        Hue_Cycle = (byte)10,
        /// <summary>
        /// random hue -> random hue
        /// </summary>
        Mood_Light = (byte)11,
        /// <summary>
        /// random yellows
        /// </summary>
        Virtual_Candle = (byte)12,
        /// <summary>
        /// random blues
        /// </summary>
        Water_Reflections = (byte)13,
        /// <summary>
        /// random orangeish reds
        /// </summary>
        Old_Neon = (byte)14,
        /// <summary>
        /// spring colors -> summer -> fall -> winter
        /// </summary>
        The_Seasons = (byte)15,
        /// <summary>
        /// random blues and purples -> while flashes
        /// </summary>
        Thunderstorm = (byte)16,
        /// <summary>
        /// red -> green -> yellow
        /// </summary>
        Stop_Light = (byte)17,
        /// <summary>
        /// S.O.S in white
        /// </summary>
        Morse_Code = (byte)18,
    }

    #endregion

    /// <summary>
    /// This base class is any command of the BlinkM.
    /// </summary>
    public class BaseCommand
    {
        #region Enums

        public enum CommandHeader
        {
            Go_to_RGB_Color_Now = (byte)'n',
            Fade_to_RGB_Color = (byte)'c',
            Fade_to_HSB_Color = (byte)'h',
            Fade_to_Random_RGB_Color = (byte)'C',
            Fade_to_Random_HSB_Color = (byte)'H',
            Play_Light_Script = (byte)'p',
            Stop_Script = (byte)'o',
            Set_Fade_Speed = (byte)'f',
            Set_Time_Adjust = (byte)'t',
            Get_Current_RGB_Color = (byte)'g',
            Write_Script_Line = (byte)'W',
            Read_Script_Line = (byte)'R',
            Set_Script_Length_and_Repeats = (byte)'L',
            Set_BlinkM_Address = (byte)'A',
            Get_BlinkM_Address = (byte)'a',
            Get_BlinkM_Firmware_Version = (byte)'Z',
            Set_Startup_Parameters = (byte)'B',
        }

        #endregion

        #region Property Members

        /// <summary>
        /// The required whole number of milliseconds for wait.
        /// </summary>
        public int WaitMillis { get; protected set; }

        /// <summary>
        /// Sending command's header.
        /// </summary>
        public CommandHeader Header { get; protected set; }

        /// <summary>
        /// Sending byte arrays.
        /// </summary>
        public byte[] SendData { get; protected set; }

        /// <summary>
        /// Receiving byte arrays.
        /// </summary>
        public byte[] ReceiveData { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="header">Sending command's header.</param>
        /// <param name="sendData">Sendging byte arrays.</param>
        public BaseCommand(CommandHeader header, params byte[] sendData)
            : this(0, header, sendData) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="header">Sending command's header.</param>
        /// <param name="sendData">Sendging byte arrays.</param>
        public BaseCommand(int waitMillis, CommandHeader header, params byte[] sendData)
        {
            this.WaitMillis = waitMillis;
            this.Header = header;
            this.SendData = sendData;
        }

        #endregion

        #region Methods
        #region Public Methods

        public byte[] GetSendBytes()
        {
            return this.CreateCommandBuffer(this.Header, this.SendData);
        }

        public byte[] GetReceiveBytes()
        {
            return this.ReceiveData;
        }

        #endregion

        #region Private Methods

        private byte[] CreateCommandBuffer(CommandHeader cmd, params byte[] data)
        {
            byte[] bufs = new byte[data.Length + 1];
            bufs[0] = (byte)cmd;
            data.CopyTo(bufs, 1);

            return bufs;
        }

        #endregion
        #endregion
    }

    /// <summary>
    /// This base class is any color command of the BlinkM.
    /// </summary>
    public class ColorBaseCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="header">Sending command's header.</param>
        /// <param name="color">Color code. (ex. #FFFFFF)</param>
        public ColorBaseCommand(CommandHeader header, uint color)
            : this(0, header, color) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="header">Sending command's header.</param>
        /// <param name="color">Color code. (ex. #FFFFFF)</param>
        public ColorBaseCommand(int waitMillis, CommandHeader header, uint color)
            : base(waitMillis, header)
        {
            this.SendData = this.CreateData(color);
        }

        #endregion

        #region Methods
        #region Protected Methods

        protected byte[] CreateData(uint color)
        {
            byte[] opts = new byte[] { 
                (byte)(color >> 16 & 0xff)
                , (byte)(color >> 8 & 0xff)
                , (byte)(color & 0xff)
            };

            return opts;
        }

        #endregion
        #endregion
    }

    /// <summary>
    /// This class is the 'Go to RGB Color Now' command of the BlinkM.
    /// </summary>
    public class GotoRGBColorNowCommand : ColorBaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="color">Color code. (ex. #FFFFFF)</param>
        public GotoRGBColorNowCommand(uint color)
            : this(0, color) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="color">Color code. (ex. #FFFFFF)</param>
        public GotoRGBColorNowCommand(int waitMillis, uint color)
            : base(waitMillis, CommandHeader.Go_to_RGB_Color_Now, color) { }

        #endregion
    }

    /// <summary>
    /// This class is the 'Fade to RGB Color' command of the BlinkM.
    /// </summary>
    public class FadeToRGBColorCommand : ColorBaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="color">Color code. (ex. #FFFFFF)</param>
        public FadeToRGBColorCommand(uint color)
            : this(0, color) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="color">Color code. (ex. #FFFFFF)</param>
        public FadeToRGBColorCommand(int waitMillis, uint color)
            : base(waitMillis, CommandHeader.Fade_to_RGB_Color, color) { }

        #endregion
    }

    /// <summary>
    /// This class is the 'Fade to Random RGB Color' command of the BlinkM.
    /// </summary>
    public class FadeToRandomRGBColorCommand : ColorBaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="color">Color code. (ex. #FFFFFF)</param>
        public FadeToRandomRGBColorCommand(uint color)
            : this(0, color) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="color">Color code. (ex. #FFFFFF)</param>
        public FadeToRandomRGBColorCommand(int waitMillis, uint color)
            : base(waitMillis, CommandHeader.Fade_to_Random_RGB_Color, color) { }

        #endregion
    }

    /// <summary>
    /// This class is the 'Play Light Script' command of the BlinkM.
    /// </summary>
    public class PlayLightScriptCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="scriptId">The script id of the script to play forever.</param>
        public PlayLightScriptCommand(ScriptId scriptId)
            : this(0, scriptId, 0) { }

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="scriptId">The script id of the script to play.</param>
        /// <param name="repeatCount">The number of repeats to play the script. 0 means play the script forever.</param>
        public PlayLightScriptCommand(ScriptId scriptId, byte repeatCount)
            : this(0, scriptId, repeatCount) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="scriptId">The script id of the script to play.</param>
        /// <param name="repeatCount">The number of repeats to play the script. 0 means play the script forever.</param>
        public PlayLightScriptCommand(int waitMillis, ScriptId scriptId, byte repeatCount)
            : base(waitMillis, CommandHeader.Play_Light_Script, (byte)scriptId, repeatCount, 0x00) { }

        #endregion
    }

    /// <summary>
    /// This class is the 'Stop Script' command of the BlinkM.
    /// </summary>
    public class StopScriptCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        public StopScriptCommand()
            : this(0) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        public StopScriptCommand(int waitMillis)
            : base(waitMillis, CommandHeader.Stop_Script) { }

        #endregion
    }

    /// <summary>
    /// This class is the 'Set Fade Speed' command of the BlinkM.
    /// </summary>
    public class SetFadeSpeedCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="fadeSpeed">Fade speed, between 0 and 255.</param>
        public SetFadeSpeedCommand(byte fadeSpeed)
            : this(0, fadeSpeed) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="fadeSpeed">Fade speed, between 0 and 255.</param>
        public SetFadeSpeedCommand(int waitMillis, byte fadeSpeed)
            : base(waitMillis, CommandHeader.Set_Fade_Speed, fadeSpeed) { }

        #endregion
    }

    /// <summary>
    /// This class is the 'Set Time Adjust' command of the BlinkM.
    /// </summary>
    public class SetTimeAdjustCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="adjustSpeed">Playback speed, between -128 and 127. 0 resets the playback spped to the default.</param>
        public SetTimeAdjustCommand(sbyte adjustSpeed)
            : this(0, adjustSpeed) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="adjustSpeed">Playback speed, between -128 and 127. 0 resets the playback spped to the default.</param>
        public SetTimeAdjustCommand(int waitMillis, sbyte adjustSpeed)
            : base(waitMillis, CommandHeader.Set_Time_Adjust, adjustSpeed > -1 ? (byte)adjustSpeed : (byte)(((adjustSpeed << 1) >> 1) | 0x80)) { }

        #endregion
    }

    /// <summary>
    /// This class is the 'Get Current RGB Color' command of the BlinkM.
    /// </summary>
    public class GetCurrentRGBColorCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        public GetCurrentRGBColorCommand()
            : this(0) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        public GetCurrentRGBColorCommand(int waitMillis)
            : base(waitMillis, CommandHeader.Get_Current_RGB_Color)
        {
            this.ReceiveData = new byte[3];
        }

        #endregion
    }

    /// <summary>
    /// This class is the 'Set Script Length and Repeats' command of the BlinkM.
    /// </summary>
    public class SetScriptLengthAndRepeatsCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="scriptLength">The length of the script.</param>
        /// <param name="repeatCount">The number of repeats to play the script. 0 means play the script forever.</param>
        public SetScriptLengthAndRepeatsCommand(byte scriptLength, byte repeatCount)
            : this(15, scriptLength, repeatCount) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="scriptLength">The length of the script.</param>
        /// <param name="repeatCount">The number of repeats to play the script. 0 means play the script forever.</param>
        public SetScriptLengthAndRepeatsCommand(int waitMillis, byte scriptLength, byte repeatCount)
            : base(waitMillis, CommandHeader.Set_Script_Length_and_Repeats, (byte)ScriptId.Eeprom_Script, scriptLength, repeatCount) { }

        #endregion
    }

    /// <summary>
    /// This class is the 'Set BlinkM Address' command of the BlinkM.
    /// </summary>
    public class SetBlinkMAddress : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="address">New I2C addresso of a BlinkM.</param>
        public SetBlinkMAddress(ushort address)
            : this(15, address) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="address">New I2C addresso of a BlinkM.</param>
        public SetBlinkMAddress(int waitMillis, ushort address)
            : base(waitMillis, CommandHeader.Set_BlinkM_Address, (byte)address, 0xD0, 0x0D, (byte)address) { }

        #endregion
    }

    /// <summary>
    /// This class is the 'Get BlinkM Address' command of the BlinkM.
    /// </summary>
    public class GetBlinkMAddress : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        public GetBlinkMAddress()
            : this(0) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        public GetBlinkMAddress(int waitMillis)
            : base(waitMillis, CommandHeader.Get_BlinkM_Address)
        {
            this.ReceiveData = new byte[1];
        }

        #endregion
    }

    /// <summary>
    /// This class is the 'Get BlinkM Firmware Version' command of the BlinkM.
    /// </summary>
    public class GetBlinkMFirmwareVersion : BaseCommand
    {
        #region Enums

        public enum Version
        {
            BlinkM_Original = (byte)'a' << 8 + (byte)'a'
            ,
            MaxM = (byte)'a' << 8 + (byte)'b'
                ,
            MinM_Original = (byte)'a' << 8 + (byte)'c'
                ,
            BlinkM_MinM_Updated = (byte)'a' << 8 + (byte)'d'
                , CtrlM = (byte)'b' << 8 + (byte)'a'
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        public GetBlinkMFirmwareVersion()
            : this(0) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        public GetBlinkMFirmwareVersion(int waitMillis)
            : base(waitMillis, CommandHeader.Get_BlinkM_Firmware_Version)
        {
            this.ReceiveData = new byte[2];
        }

        #endregion

        #region Methods
        #region Public Methods

        public Version GetVersion()
        {
            switch (this.ReceiveData[0])
            {
                case (byte)'a':
                    switch (this.ReceiveData[1])
                    {
                        case (byte)'a':
                            return Version.BlinkM_Original;
                        case (byte)'b':
                            return Version.MaxM;
                        case (byte)'c':
                            return Version.MinM_Original;
                        case (byte)'d':
                        default:
                            return Version.BlinkM_MinM_Updated;
                    }
                default:
                    return Version.CtrlM;
            }
        }

        #endregion
        #endregion
    }

    /// <summary>
    /// This class is the 'Set Startup Parameters' command of the BlinkM.
    /// </summary>
    public class SetStartupParametersCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// No specified wait milliseconds.
        /// </summary>
        /// <param name="playScript">TRUE means play a startup script.</param>
        /// <param name="scriptId">The script id of the script to play.</param>
        /// <param name="repeatCount">The number of repeats to play the script. 0 means play the script forever.</param>
        /// <param name="fadeSpeed">Fade speed, between 0 and 255.</param>
        /// <param name="adjustSpeed">Playback speed, between -128 and 127. 0 resets the playback spped to the default.</param>
        public SetStartupParametersCommand(bool playScript, ScriptId scriptId, byte repeatCount, byte fadeSpeed, sbyte adjustSpeed)
            : this(20, playScript, scriptId, repeatCount, fadeSpeed, adjustSpeed) { }

        /// <summary>
        /// Constructor.
        /// Set specified wait milliseconds.
        /// </summary>
        /// <param name="waitMillis">The required whole number of milliseconds for wait.</param>
        /// <param name="playScript">TRUE means play a startup script.</param>
        /// <param name="scriptId">The script id of the script to play.</param>
        /// <param name="repeatCount">The number of repeats to play the script. 0 means play the script forever.</param>
        /// <param name="fadeSpeed">Fade speed, between 0 and 255.</param>
        /// <param name="adjustSpeed">Playback speed, between -128 and 127. 0 resets the playback spped to the default.</param>
        public SetStartupParametersCommand(int waitMillis, bool playScript, ScriptId scriptId, byte repeatCount, byte fadeSpeed, sbyte adjustSpeed)
            : base(waitMillis, CommandHeader.Set_Startup_Parameters, playScript ? (byte)0x01 : (byte)0x00, (byte)scriptId, repeatCount, fadeSpeed, adjustSpeed > -1 ? (byte)adjustSpeed : (byte)(((adjustSpeed << 1) >> 1) | 0x80)) { }

        #endregion
    }
}