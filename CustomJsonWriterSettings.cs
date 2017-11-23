using System;
using System.IO;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace WZ.Enterprise.InfoPerson.DataAccess.MongoDb
{
    public static class CustomJsonWriterSettings
    {
        /// <summary>
        /// Serializes an object to a JSON string.
        /// </summary>
        /// <typeparam name="TNominalType">The nominal type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="writerSettings">The JsonWriter settings.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configurator">The serializastion context configurator.</param>
        /// <param name="args">The serialization args.</param>
        /// <returns>
        /// A JSON string.
        /// </returns>
        public static string ToMyJson<TNominalType>(
            this TNominalType obj,
            JsonWriterSettings writerSettings = null,
            IBsonSerializer<TNominalType> serializer = null,
            Action<BsonSerializationContext.Builder> configurator = null,
            BsonSerializationArgs args = default(BsonSerializationArgs))
        {
            return ToMyJson(obj, typeof(TNominalType), writerSettings, serializer, configurator, args);
        }

        /// <summary>
        /// Serializes an object to a JSON string.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="nominalType">The nominal type of the objectt.</param>
        /// <param name="writerSettings">The JsonWriter settings.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configurator">The serialization context configurator.</param>
        /// <param name="args">The serialization args.</param>
        /// <returns>
        /// A JSON string.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">nominalType</exception>
        /// <exception cref="System.ArgumentException">serializer</exception>
        public static string ToMyJson(
            this object obj,
            Type nominalType,
            JsonWriterSettings writerSettings = null,
            IBsonSerializer serializer = null,
            Action<BsonSerializationContext.Builder> configurator = null,
            BsonSerializationArgs args = default(BsonSerializationArgs))
        {
            if (nominalType == null)
            {
                throw new ArgumentNullException("nominalType");
            }

            if (serializer == null)
            {
                serializer = BsonSerializer.LookupSerializer(nominalType);
            }
            if (serializer.ValueType != nominalType)
            {
                var message = string.Format("Serializer type {0} value type does not match document types {1}.", serializer.GetType().FullName, nominalType.FullName);
                throw new ArgumentException(message, "serializer");
            }

            using (var stringWriter = new StringWriter())
            {
                using (var bsonWriter = new CustomJsonWriter(stringWriter, writerSettings ?? JsonWriterSettings.Defaults))
                {
                    var context = BsonSerializationContext.CreateRoot(bsonWriter, configurator);
                    args.NominalType = nominalType;
                    serializer.Serialize(context, args, obj);
                }
                return stringWriter.ToString();
            }
        }
    }
}
