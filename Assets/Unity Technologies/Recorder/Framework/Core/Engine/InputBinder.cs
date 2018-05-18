using System;
using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{
    /// <summary>
    /// What is it: Place holder for an input setting, in a recorder settings asset, for an input setting that is stored in the scene.
    /// Motivation: Input settings can be flagged to persist in the scene and not in the asset. This is to facilitate settings that target specific scene objects.
    ///             When settings are saved in the scene, need a place holder in the recorder asset that will indicate that the real settings should be read from the scene.
    /// </summary>
    public class InputBinder : RecorderInputSetting
    {
        [UnityEngine.SerializeField]
        string m_TypeName;

        public string typeName
        {
            get
            {
                return this.m_TypeName;
            }
            set
            {
                if( string.IsNullOrEmpty(value) )
                    throw new ArgumentException("Invalid type name value being set!");
                this.m_TypeName = value;
            }
        }

        public override Type inputType
        {
            get
            {
                if( string.IsNullOrEmpty(this.typeName) )
                    throw new Exception("Invalid/uninitialized type!");

                return Type.GetType(this.typeName);
            }
        }

        public override bool ValidityCheck( List<string> errors )
        {
            return false;
        }
    }
}
