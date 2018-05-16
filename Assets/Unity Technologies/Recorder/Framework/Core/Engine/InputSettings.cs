using System;
using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{
    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    public abstract class  RecorderInputSetting : UnityEngine.ScriptableObject
    {
        public abstract Type inputType { get; }
        public abstract bool ValidityCheck(List<string> errors);
        public string m_DisplayName;
        public string m_Id;

        protected virtual void OnEnable()
        {
            if (string.IsNullOrEmpty(this.m_Id))
                this.m_Id = Guid.NewGuid().ToString();
        }


        public bool storeInScene
        {
            get { return Attribute.GetCustomAttribute(this.GetType(), typeof(StoreInSceneAttribute)) != null; }
        }
    }

}
