using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Experimental.Recorder
{
    [Unity_Technologies.Recorder.Framework.Core.Engine.Recorder(typeof(AnimationRecorderSettings), "Animation Clips", "Unity/Animation Recording")]
    public class AnimationRecorder : Unity_Technologies.Recorder.Framework.Core.Engine.GenericRecorder<AnimationRecorderSettings>
    {
        public override void RecordFrame(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession session)
        {
        }


        public override void EndRecording(Unity_Technologies.Recorder.Framework.Core.Engine.RecordingSession ctx)
        {
            var ars = ctx.settings as AnimationRecorderSettings;

            for (int i = 0; i < this.m_Inputs.Count; ++i)
            {
                var set = (this.settings.inputsSettings[i] as AnimationInputSettings);
                if (set.enabled)
                {                  
                    var dir = "Assets/" + ars.outputPath;
                    var idx = dir.LastIndexOf('/');
                    if (idx > -1)
                    {
                        dir = dir.Substring(0,idx);
                    }
                    dir = this.ReplaceTokens(dir, ars, set);
                    Directory.CreateDirectory(dir);
                    
                    var aInput = this.m_Inputs[i] as AnimationInput;
                    AnimationClip clip = new AnimationClip();
                    var clipName = this.ReplaceTokens(("Assets/" + ars.outputPath),ars, set)+".anim";
                    clipName = AssetDatabase.GenerateUniqueAssetPath(clipName);
                    AssetDatabase.CreateAsset(clip, clipName);
                    aInput.m_gameObjectRecorder.SaveToClip(clip);
                    aInput.m_gameObjectRecorder.ResetRecording();
                }
            }

            ars.take++;
            base.EndRecording(ctx);
        }

        private string ReplaceTokens(string input, AnimationRecorderSettings ars, AnimationInputSettings  ais)
        {
            var idx = this.m_Inputs.Select(x => x.settings).ToList().IndexOf(ais);
            return input.Replace(AnimationRecorderSettings.goToken, ais.gameObject.name)
                       .Replace(AnimationRecorderSettings.inputToken,(idx+1).ToString("00"))
                       .Replace(AnimationRecorderSettings.takeToken, ars.take.ToString("000"));
        }
    }
}