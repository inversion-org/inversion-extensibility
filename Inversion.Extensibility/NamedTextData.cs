using System;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Inversion
{
    public class NamedTextData : IData
    {
        private JObject _data;

        public string Name { get; }

        /// <summary>
        /// The string value of the text data.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Provides an abstract representation
        /// of the objects data expressed as a JSON object.
        /// </summary>
        /// <remarks>
        /// For this type the json object is only created the once.
        /// </remarks>
        public JObject Data
        {
            get
            {
                return _data ?? (_data = this.ToJsonObject());
            }
        }

        /// <summary>
        /// Instantiates a new `NamedTextData` object with the value
        /// of the text provided.
        /// </summary>
        /// <param name="name">The label to initialise from.</param>
        /// <param name="text">The text to initialise from.</param>

        public NamedTextData(string name, string text)
        {
            Name = name;
            Value = text;
        }

        /// <summary>
        /// Instantiates a new `NamedTextData` object as a copy
        /// of the one provided.
        /// </summary>
        /// <param name="text">The `NamedTextData` to copy.</param>

        public NamedTextData(NamedTextData text)
        {
            Name = text.Name;
            Value = text.Value;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Creates a new instance as a copy
        /// of the original.
        /// </summary>
        /// <returns>
        /// A copy as a `NamedTextData` object.
        /// </returns>

        public NamedTextData Clone()
        {
            return new NamedTextData(this);
        }

        /// <summary>
        /// Produces an xml representation of the text data.
        /// </summary>
        /// <param name="writer">The xml writer the representation should be written to.</param>
        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement(this.Name);
            writer.WriteCData(this.Value);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Produces a json representation of the text data.
        /// </summary>
        /// <param name="writer">The json writer the representation should be written to.</param>
        public void ToJson(JsonWriter writer)
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