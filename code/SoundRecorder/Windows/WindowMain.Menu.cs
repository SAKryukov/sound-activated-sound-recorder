/*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Windows {
    using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
    using System.IO;
    using ModelTop = Application.ModelTop;
    using System.Windows.Input;

    public partial class WindowMain {

        partial class MenuDefinitionSet {
            internal const string LoadPreferencesDialogTitle = "Load Preferences";
            internal const string SavePreferencesDialogTitle = "Save Preferences";
            internal const string DataFileExt = "SoundRecorder.Preferences.xml";
            internal const string DataDialogFilter = "Preferences files|*." + DataFileExt;
            internal const string DefaultDataFileName = DataFileExt;
        } //class MenuDefinitionSet

        void SetupMenu() {
            loadPreferencesDialog.Title = MenuDefinitionSet.LoadPreferencesDialogTitle;
            savePreferencesDialog.Title = MenuDefinitionSet.SavePreferencesDialogTitle;
            savePreferencesDialog.DefaultExt = MenuDefinitionSet.DataFileExt;
            loadPreferencesDialog.Filter = MenuDefinitionSet.DataDialogFilter;
            savePreferencesDialog.Filter = loadPreferencesDialog.Filter;
            menuItemExit.Click += (sender, eventArgs) => { Close(); };
            menuItemLoad.Click += (sender, eventArgs) => LoadPreferences();
            menuItemSave.Click += (sender, eventArgs) => SavePreferences();
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, new ExecutedRoutedEventHandler((sender, eventArgs) => {
                help.ShowHelp(this);
            }), new CanExecuteRoutedEventHandler((sender, eventArgs) => {
                eventArgs.CanExecute = true;
            }))); //Open

        } //SetupMenu

        void SavePreferences() {
            if (savePreferencesDialog.ShowDialog() == true)
                UiToData().Store(savePreferencesDialog.FileName);
        } //SavePreferences
        void LoadPreferences(string fileName) {
            ModelTop top = ModelTop.Load(fileName);
            PopulateUi(top);
        } //LoadPreferences
        void LoadPreferences() {
            if (loadPreferencesDialog.ShowDialog() == true)
                LoadPreferences(loadPreferencesDialog.FileName);
        } //LoadPreferences

        string FindDefaultPreferencesFile() {
            string[] commandLine = System.Environment.GetCommandLineArgs();
            if (commandLine.Length == 2) {
                string first = commandLine[1];
                if (File.Exists(first))
                    return first;
            } //if
            string fileName = MenuDefinitionSet.DefaultDataFileName;
            if (File.Exists(fileName))
                return fileName;
            string location = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            fileName = Path.Combine(location, fileName);
            if (File.Exists(fileName))
                return fileName;
            return null;
        } //FindDefaultPreferencesFile

        void LoadDefaultPreferences() {
            string fileName = FindDefaultPreferencesFile();
            if (fileName != null)
                LoadPreferences(fileName);
        } //SA???

        ModelTop UiToData() {
            ModelTop data = new() { UseSoundActivation = checkBoxUseSoundActivation.IsChecked == true };
            if (!int.TryParse(textBoxDelay.Text, out int delay))
                delay = 0;
            data.DelayBeforeActivationMs = delay;
            data.ActivationThreshold = volumeIndicator.Threshold;
            data.AutoRestartOnSave = checkBoxAutoRestart.IsChecked == true;
            data.BaseFileName = textBoxBaseFileName.Text;
            data.NumberOfDigitsInFileNumber = (int)comboBoxWidth.SelectedItem;
            data.MaximumIndicatorValue = volumeIndicator.Maximum;
            return data;
        } //UiToData

        void PopulateUi(ModelTop data) {
            checkBoxUseSoundActivation.IsChecked = data.UseSoundActivation;
            textBoxDelay.Text = data.DelayBeforeActivationMs.ToString();
            volumeIndicator.Threshold = data.ActivationThreshold;
            checkBoxAutoRestart.IsChecked = data.AutoRestartOnSave;
            textBoxBaseFileName.Text = data.BaseFileName;
            comboBoxWidth.SelectedItem = data.NumberOfDigitsInFileNumber;
            volumeIndicator.Maximum = data.MaximumIndicatorValue;
        } //PopulateUi

        readonly OpenFileDialog loadPreferencesDialog = new();
        readonly SaveFileDialog savePreferencesDialog = new();

    } //class WindowMain

} //namespace SoundRecorder.Windows
