using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Experimental.Recorder
{
    [ExecuteInEditMode]
    [Serializable]
    public class AnimationRecorderSettings : Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings
    {
        public string outputPath = "AnimRecorder"+takeToken+"/"+goToken+"_"+inputToken;
        public int take = 1;
        public static string goToken = "<goName>";
        public static string takeToken = "<take>";
        public static string inputToken = "<input>";
        public override List<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting>()
            {
                NewInputSettingsObj<AnimationInputSettings>("Animation") 
            };
        }
        
        public override bool isPlatformSupported
        {
            get
            {
                return Application.platform == RuntimePlatform.LinuxEditor ||
                       Application.platform == RuntimePlatform.OSXEditor ||
                       Application.platform == RuntimePlatform.WindowsEditor;
            }
        }

        public override List<Unity_Technologies.Recorder.Framework.Core.Engine.InputGroupFilter> GetInputGroups()
        {
            return new List<Unity_Technologies.Recorder.Framework.Core.Engine.InputGroupFilter>()
            {
                new Unity_Technologies.Recorder.Framework.Core.Engine.InputGroupFilter()
                {
                    title = "Animation",
                    typesFilter = new List<Unity_Technologies.Recorder.Framework.Core.Engine.InputFilter>()
                    {
                        new Unity_Technologies.Recorder.Framework.Core.Engine.TInputFilter<AnimationClipSettings>("GameObject Recorder"),
                    }
                }
            };
        }

        public override bool ValidityCheck( List<string> errors )
        {
            var ok = base.ValidityCheck(errors);

            if (inputsSettings == null)
            {
                ok = false;
                errors.Add("Invalid state!");
            }
            if (!inputsSettings.Cast<AnimationInputSettings>().Any(x => x != null && x.enabled && x.gameObject != null ))
            {
                ok = false;
                errors.Add("No input object set/enabled.");
            }

            if ( string.IsNullOrEmpty(outputPath))
            {
                ok = false;
                errors.Add("Invalid output path.");
            }

            return ok; 
        }
    }
}