using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EventBusTest
{
    public class EventMsg
    {
        public int id;
        public string message;
        public EventMsg(int id, string message)
        {
            this.id = id;
            this.message = message;
        }
    }
    public class EventBusExample : MonoBehaviour, IOnEvent<EventMsg>
    {
        // Start is called before the first frame update
        void Start()
        {
            EventBus.RegisterEvent<EventMsg>(OnEvent).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent(OnEvent).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public void OnEvent(EventMsg eventMsg)
        {
            Debug.Log("Event Triggered! id:" + eventMsg.id + " msg:" + eventMsg.message);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EventBus.TriggerEvent(new EventMsg(1, "Space key pressed"));
            }
        }

    }

}