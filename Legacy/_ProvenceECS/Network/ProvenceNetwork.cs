using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lidgren.Network;
using ProvenceECS;
using UnityEngine;

namespace ProvenceECS.Network{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ProvencePacket: Attribute {
        protected ushort id;

        public ProvencePacket(ushort id){
            this.id = id;
        }

        public ushort ID{
            get { return id; }
        }
    }

    public partial class ProvenceNetwork{

        public static bool isHost = false;

        protected static Dictionary<ushort,Type> packetDict;
        public static Dictionary<ushort,Type> PacketDict{
            get{
                if(packetDict == null) GeneratePacketDictionary();
                return packetDict;
            }
        }

        public static void GeneratePacketDictionary(){
            if(packetDict == null) packetDict = new Dictionary<ushort, Type>();
            List<Type> typeList = Helpers.GetAllTypesFromBaseType(typeof(ProvenceEventArgs), packetDict.Values.ToArray());
                foreach(Type type in typeList){
                    ushort packetID = GetPacketID(type);
                    if(packetID != 0){
                        if(packetDict.ContainsKey(packetID)) Debug.Log("duplicate packet ids: " + packetID);
                        packetDict[packetID] = type;
                    }
                }
        }

        public static ushort GetPacketID(Type type){
            ProvencePacket[] packetData = (ProvencePacket[])type.GetCustomAttributes(typeof(ProvencePacket),false);
            if(packetData.Length > 0) return packetData[0].ID;
            Debug.LogWarning("Missing Packet Type ID for: " + type.ToString());
            return 0;
        }

        public static Type GetPacketTypeFromID(ushort id){
            if(id == 0){
                Debug.Log("Missing Packet; Type ID:" + id);
                return null;
            }
            if(packetDict == null) packetDict = new Dictionary<ushort, Type>();
            if(packetDict.ContainsKey(id)) return packetDict[id];
            else{
                List<Type> typeList = Helpers.GetAllTypesFromBaseType(typeof(ProvenceEventArgs), packetDict.Values.ToArray());
                foreach(Type type in typeList){
                    ushort packetID = GetPacketID(type);
                    if(packetID != 0 && packetID == id){
                        Type setType = type;
                        if(type.IsGenericType) setType = type.GetGenericTypeDefinition();
                        packetDict[id] = setType;
                        return setType;
                    }
                }
            }
            return null;
        }

        public static void WriteEventToMessage<T>(T pEvent, NetOutgoingMessage message) where T : ProvenceEventArgs{
            try{
                message.Write(GetPacketID(typeof(T)));
                if(typeof(T).IsGenericType){
                    message.Write(typeof(T).GenericTypeArguments[0].LowCostSerializeObject());
                }
                FieldInfo[] fields = pEvent.GetType().GetFields();
                for(int i = 0; i < fields.Length; i++){
                    Type fieldType = fields[i].FieldType;
                    try{
                        Helpers.InvokeStaticGenericMethod<ProvenceNetwork>("WriteFieldToMessage", fieldType, (dynamic) fields[i].GetValue(pEvent), message);
                    }catch{
                        Helpers.InvokeStaticGenericMethod<ProvenceNetwork>("WriteFieldToMessageFallback", fieldType, (dynamic) fields[i].GetValue(pEvent), message);
                    }
                }
            }catch(Exception e){
                Debug.LogWarning(e);
            }
        }

        public static void WriteFieldToMessageFallback<T>(T field, NetOutgoingMessage message){
            message.Write(field.NetSerializeObject());
        }

        public static ProvenceEventArgs ReadEventFromMessage(NetIncomingMessage message){
            ProvenceEventArgs pEvent;
            Type packetType = GetPacketTypeFromID(message.ReadUInt16());
            if(packetType.ContainsGenericParameters){
                pEvent = (ProvenceEventArgs)Helpers.CreateGenericObject(packetType,message.ReadString().LowCostDeserializeObject<Type>());
            }else pEvent = (ProvenceEventArgs)Activator.CreateInstance(packetType);            
            //go through field info get values
            FieldInfo[] fields = pEvent.GetType().GetFields();
            for(int i = 0; i < fields.Length; i++){
                Type fieldType = fields[i].FieldType;
                try{
                    fields[i].SetValue(pEvent, Helpers.InvokeStaticGenericMethod<ProvenceNetwork>("ReadFieldToMessage", fieldType, message));
                }catch{
                    fields[i].SetValue(pEvent, Helpers.InvokeStaticGenericMethod<ProvenceNetwork>("ReadFieldToMessageFallback", fieldType, message));
                }
            }
            return pEvent;
        }

        public static T ReadFieldToMessageFallback<T>(NetIncomingMessage message){
            string msg = message.ReadString();
            return msg.NetDeserializeObject<T>();
        }

       /*  public static void WriteFieldToMessage<Byte>(byte field, NetOutgoingMessage message){
            //Debug.Log("writing byte");
            message.Write(field);
        }

        public static byte ReadFieldToMessage<Byte>(NetIncomingMessage message){
            //Debug.Log("Reading Byte");
            return message.ReadByte();
        } */

    }

}