using System;
using System.Collections.Generic;
using UnityEngine;
namespace EventBusTest
{
    public interface IOnEvent<T>
    {
        void OnEvent(T arg);
    }
    interface IRegistrations
    {

    }

    class Registrations<T> : IRegistrations
    {
        public Action<T> OnEvent = obj => { };
    }
    public class EventBus
    {
        private static Dictionary<Type, IRegistrations> eventDictionary = new Dictionary<Type, IRegistrations>();
        public static void TriggerEvent<T>(T message)
        {
            var type = typeof(T);
            (eventDictionary[type] as Registrations<T>).OnEvent?.Invoke(message);
        }
        public static IUnRegister RegisterEvent<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (!eventDictionary.ContainsKey(type))
            {
                eventDictionary.Add(typeof(T), new Registrations<T>());
            }
           (eventDictionary[type] as Registrations<T>).OnEvent += callback;

            return new UnRegister<T>()
            {
                OnEvent = callback,
            };
        }
        public static void UnRegisterEvent<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (eventDictionary.ContainsKey(type))
            {
                (eventDictionary[type] as Registrations<T>).OnEvent -= callback;
            }
        }
        public static void UnRegisterAllEvent<T>()
        {
            var type = typeof(T);
            if (eventDictionary.ContainsKey(type))
            {
                (eventDictionary[type] as Registrations<T>).OnEvent = null;
            }
        }
    }
    public static class EventBusExtentions
    {
        public static IUnRegister RegisterEvent<T>(this IOnEvent<T> iOnEvent, Action<T> action)
        {
            EventBus.RegisterEvent<T>(action);
            return new UnRegister<T>()
            {
                OnEvent = action,
            };
        }
        public static void UnRegisterEvent<T>(this IOnEvent<T> iOnEvent, Action<T> action)
        {
            EventBus.UnRegisterEvent<T>(action);
        }
        public static void UnRegisterAllEvent<T>(this IOnEvent<T> iOnEvent)
        {
            EventBus.UnRegisterAllEvent<T>();
        }
    }



    public interface IUnRegister
    {
        void UnRegisterEvent();
    }
    public class UnRegister<T> : IUnRegister
    {
        public Action<T> OnEvent { get; set; }

        public void UnRegisterEvent()
        {
            EventBus.UnRegisterEvent(OnEvent);
            OnEvent = null;
        }
    }
    public class UnRegisterOnDestroyTrigger : MonoBehaviour
    {
        private HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister)
        {
            mUnRegisters.Add(unRegister);
        }

        private void OnDestroy()
        {
            foreach (var unRegister in mUnRegisters)
            {
                unRegister.UnRegisterEvent();
            }

            mUnRegisters.Clear();
        }
    }
    public static class UnRegisterExtension
    {
        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
            }

            trigger.AddUnRegister(unRegister);
        }
    }
}
