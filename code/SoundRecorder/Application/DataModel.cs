/*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Application {
    using System.Runtime.Serialization;
    using System.Xml;

    [DataContract(Name="SoundRecorder", Namespace=@"http://www.SAKryukov.org/Schema/SoundRecorder")]
    class ModelTop {

        [DataMember(Order = 1)]
        internal bool UseSoundActivation { get; set; }
        [DataMember(Order = 2)]
        internal int DelayBeforeActivationMs { get; set; }
        [DataMember(Order = 3)]
        internal double ActivationThreshold { get; set; }
        [DataMember(Order = 4)]
        internal bool AutoRestartOnSave { get; set; }
        [DataMember(Order = 5)]
        internal string BaseFileName { get; set; }
        [DataMember(Order = 6)]
        internal int NumberOfDigitsInFileNumber { get; set; }
        [DataMember(Order = 7)]
        internal double MaximumIndicatorValue { get; set; }

        internal void Store(string fileName) {
            XmlWriterSettings settings = new() {
                NewLineHandling = NewLineHandling.Entitize,
                Indent = true,
                IndentChars = "\t"
            };
            using XmlWriter writer = XmlWriter.Create(fileName, settings);
            serializer.WriteObject(writer, this);
        } //Store

        internal static ModelTop Load(string fileName) {
            using XmlReader reader = XmlReader.Create(fileName);
            return (ModelTop)serializer.ReadObject(reader);
        } //Load

        static readonly DataContractSerializer serializer = new(typeof(ModelTop));

    } //class Top

} //SoundRecorder.Application
