using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity_Technologies.Recorder.Framework.Core.Editor;

namespace Unity_Technologies.Recorder.Framework.Inputs.Animation.Editor
{
    [CustomEditor(typeof(AnimationInputSettings))]
    public class AnimationInputSettingsEditor : InputEditor
    {
        public override void OnInspectorGUI()
        {
            var animImputSetting = this.target as AnimationInputSettings;;
                   
            EditorGUI.BeginChangeCheck();
            animImputSetting.gameObject = EditorGUILayout.ObjectField("Game Object",animImputSetting.gameObject, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                animImputSetting.enabled = animImputSetting.gameObject != null;

                if (animImputSetting.gameObject != null)
                {
                    animImputSetting.bindingTypeName.Add(animImputSetting.gameObject.GetComponent<Component>().GetType().AssemblyQualifiedName);
                }
            }

            if (animImputSetting.gameObject != null)
            {
                var compos = animImputSetting.gameObject.GetComponents<Component>()
                    .Where(x => x != null)
                    .Select(x => x.GetType());
                if (animImputSetting.recursive)
                {
                    compos = compos.Union(animImputSetting.gameObject.GetComponentsInChildren<Component>()
                        .Where(x => x != null)
                        .Select(x => x.GetType()));
                }
                
#if UNITY_2018_2_OR_NEWER
                compos = compos.Distinct()
                    .Where( x => x != typeof(Animator)) // black list
                    .ToList();
#else
                compos = compos.Distinct()
                    .Where(x => !typeof(MonoBehaviour).IsAssignableFrom(x) && x != typeof(Animator)) // black list
                    .ToList();
#endif
                var compoNames = compos.Select(x => x.AssemblyQualifiedName).ToList();

                int flags = 0;
                foreach (var t in animImputSetting.bindingTypeName)
                {
                    var found = compoNames.IndexOf(t);
                    if (found != -1)
                        flags |= 1 << found;
                }
                EditorGUI.BeginChangeCheck();
                flags = EditorGUILayout.MaskField("Recorded Target(s)", flags, compos.Select(x => x.Name).ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    animImputSetting.bindingTypeName = new List<string>();
                    for (int i=0;i<compoNames.Count;++i)                               
                    {
                        if ((flags & (1 << i )) == 1 << i )
                        {
                            animImputSetting.bindingTypeName.Add(compoNames[i]);
                        }
                    }
                }
            }

            animImputSetting.recursive = EditorGUILayout.Toggle("Recursive",animImputSetting.recursive);   
        }
    }
    

    
}