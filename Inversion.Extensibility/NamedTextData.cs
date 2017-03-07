using System;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Inversion
{
    public class NamedTextData : TextData
    {
        private JObject _data;

        public string Name { get; }

        /// <summary>
        /// The string value of the text data.
        /// </summary>
        public new string Value { get; }

        public static implicit operator string(NamedTextData text)
        {
            return text.Value;
        }

        /// <summary>
        /// Instantiates a new `NamedTextData` object with the value
        /// of the text provided.
        /// </summary>
        /// <param name="name">The label to initialise from.</param>
        /// <param name="text">The text to initialise from.</param>

        public NamedTextData(string name, string text) : base(text)
        {
            Name = name;
            Value = text;
        }

        /// <summary>
        /// Instantiates a new `NamedTextData` object as a copy
        /// of the one provided.
        /// </summary>
        /// <param name="text">The `NamedTextData` to copy.</param>

        public NamedTextData(NamedTextData text) : base(text.Value)
        {
            Name = text.Name;
            Value = text.Value;
        }

        /// <summary>
        /// Creates a new instance as a copy
        /// of the original.
        /// </summary>
        /// <returns>
        /// A copy as a `NamedTextData` object.
        /// </returns>

        public new NamedTextData Clone()
        {
            return new NamedTextData(this);
        }

        /// <summary>
        /// Produces an xml representation of the text data.
        /// </summary>
        /// <param name="writer">The xml writer the representation should be written to.</param>
        public override void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement(this.Name);
            writer.WriteCData(this.Value);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Produces a json representation of the text data.
        /// </summary>
        /// <param name="writer">The json writer the representation should be written to.</param>
        public override void ToJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(this.Name);
            writer.WriteValue(this.Value);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Value ?? "[no value]";
        }
    }
}