using System;
using System.Collections.Generic;
using Core;

namespace Core
{
    public class Component
    {
        private GameObject gameObject;
        public bool active;

        public Component(GameObject parent)
        {
            this.gameObject = parent;
            active = true;
        }

        public virtual void Update()
        {
            if (!active)
                return;
        }

        public virtual void GOReferenceFunction(GameObject g)
        {
        }

        public GameObject GameObject()
        {
            return gameObject;
        }
    }
}