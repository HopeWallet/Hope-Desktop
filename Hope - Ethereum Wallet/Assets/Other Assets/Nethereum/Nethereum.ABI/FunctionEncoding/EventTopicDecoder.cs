using System;
using System.Linq;
using System.Reflection;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Nethereum.ABI.FunctionEncoding
{
    public class EventTopicDecoder : ParameterDecoder
    {
        public T DecodeTopics<T>(object[] topics, string data) where T : new()
        {
            var result = new T();
            return DecodeTopics<T>(result, topics, data);
        }

        public T DecodeTopics<T>(T eventDTO, object[] topics, string data)
        {
            var type = typeof(T);
           
            var properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);

            var indexedProperties = properties.Where(x => x.GetCustomAttribute<ParameterAttribute>(true).Parameter.Indexed == true).OrderBy(x => x.GetCustomAttribute<ParameterAttribute>(true).Order).ToArray();
            var dataProperties = properties.Where(x => x.GetCustomAttribute<ParameterAttribute>(true).Parameter.Indexed == false).OrderBy(x => x.GetCustomAttribute<ParameterAttribute>(true).Order).ToArray();

            var topicNumber = 0;
            foreach (var topic in topics)
            {
                //skip the first one as it is the signature
                if (topicNumber > 0)
                {
                    var property = indexedProperties[topicNumber - 1];
                        
                    var attribute = property.GetCustomAttribute<ParameterAttribute>(true);
                    //skip dynamic types as the topic value is the sha3 keccak
                    if (!attribute.Parameter.ABIType.IsDynamic())
                    {
                        eventDTO = DecodeAttributes(topic.ToString(), eventDTO, property);
                    }
                    else
                    {
                        if (property.PropertyType != typeof(string))
                            throw new Exception(
                                "Indexed Dynamic Types (string, arrays) value is the Keccak SHA3 of the value, the property type of " +
                                property.Name + "should be a string");
#if DOTNET35
                        property.SetValue(eventDTO, topic.ToString(), null);
#else
                        property.SetValue(eventDTO, topic.ToString());
#endif
                    }
                }
                topicNumber = topicNumber + 1;
            }
            eventDTO = DecodeAttributes(data, eventDTO, dataProperties.ToArray());
            return eventDTO;
        }
    }
}