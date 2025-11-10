/*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Wave {
    using System;
    using System.Runtime.InteropServices;
    using StringBuilder = System.Text.StringBuilder;

    internal static class Mci {

        static class DefinitionSet {
            internal const string DllName = "winmm.dll";
            internal const string LevelMeterDeviceId = "soundLevelMeterDevice";
            internal const string SoundRecordDeviceId = "soundRecordDevice";
            internal const string OpenCommandFormat = "open new type waveaudio alias {0}";
            internal const string StatusLevelCommandFormat = "status {0} level";
            internal const string RecordCommandFormat = "record {0}";
            internal const string PauseCommandFormat = "pause {0}";
            internal const string StopCommandFormat = "stop {0}";
            internal const string CloseCommandFormat = "close {0}";
            internal const string SaveCommandFormatFormat = @"save {0} ""{{0}}""";
            internal static readonly string OpenLevelMeterCommand = string.Format(OpenCommandFormat, LevelMeterDeviceId);
            internal static readonly string OpenRecorderCommand = string.Format(OpenCommandFormat, SoundRecordDeviceId);
            internal static readonly string StatusLevelCommand = string.Format(StatusLevelCommandFormat, LevelMeterDeviceId);
            internal static readonly string RecordCommand = string.Format(RecordCommandFormat, SoundRecordDeviceId);
            internal static readonly string PauseCommand = string.Format(PauseCommandFormat, SoundRecordDeviceId);
            internal static readonly string StopCommand = string.Format(StopCommandFormat, SoundRecordDeviceId);
            internal static readonly string CloseRecorderCommand = string.Format(CloseCommandFormat, SoundRecordDeviceId);
            internal static readonly string CloseLevelMeterCommand = string.Format(CloseCommandFormat, LevelMeterDeviceId);
            internal static readonly string SaveCommandFormat = string.Format(SaveCommandFormatFormat, SoundRecordDeviceId);
            internal const int ReturnNumDigits = 0x10;
        } //DefinitionSet

        internal class MciException : ApplicationException {
            internal MciException(long mciErrorCode) : base(string.Format("MCI error {0}", mciErrorCode)) { this.MciErrorCode = mciErrorCode; }
            internal long MciErrorCode { get; private set; }
        } //class MciException

        [DllImport(DefinitionSet.DllName, CharSet = CharSet.Unicode, SetLastError = false)]
        static extern long mciSendString(
            string strCommand,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder strReturn,
            uint iReturnLength,
            IntPtr oCallback = 0);

        internal static void StartLevelMeter() {
            mciSendString(DefinitionSet.OpenLevelMeterCommand, null, 0, IntPtr.Zero);
        } //StartLevelMeter

        static readonly StringBuilder stringBuilderByReference = new();
        internal static (double average, double maximum)  GetLevel(int count, int delayMs) {
            double result = 0;
            double maxLevel = double.NegativeInfinity;
            for (int index = 0; index < count; ++index) {
                stringBuilderByReference.Clear();
                mciSendString(DefinitionSet.StatusLevelCommand, stringBuilderByReference, DefinitionSet.ReturnNumDigits, IntPtr.Zero);
                if (!double.TryParse(stringBuilderByReference.ToString(), out double value))
                    return (0, 0);
                result += value;
                if (value > maxLevel) maxLevel = value;
                System.Threading.Thread.Sleep(delayMs);
            } //loop
            return (result / count, maxLevel);
        } //GetLevel
        internal static void CloseLevelMeter() {
            mciSendString(DefinitionSet.CloseLevelMeterCommand, null, 0, IntPtr.Zero);
        } //CloseLevelMeter

        internal static void Open() {
            mciSendString(DefinitionSet.OpenRecorderCommand, null, 0, IntPtr.Zero);
        } //Open

        internal static void Record() {
            mciSendString(DefinitionSet.RecordCommand, null, 0, IntPtr.Zero);
        } //Record

        internal static void Pause() {
            mciSendString(DefinitionSet.PauseCommand, null, 0, IntPtr.Zero);
        } //Pause

        internal static void Stop() {
            mciSendString(DefinitionSet.StopCommand, null, 0, IntPtr.Zero);
        } //Stop

        internal static void Close() {
            mciSendString(DefinitionSet.CloseRecorderCommand, null, 0, IntPtr.Zero);
        } //Close

        internal static void SaveRecording(string fileName) {
            mciSendString(string.Format(DefinitionSet.SaveCommandFormat, fileName), null, 0, IntPtr.Zero);
        } //SaveRecording

    } //class Mci

} //namespace SoundRecorder.Wave
