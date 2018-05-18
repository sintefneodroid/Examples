using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{
    [Serializable]
    public class InputSettingsList : IEnumerable<RecorderInputSetting>
    {
        [SerializeField]
        List<RecorderInputSetting> m_InputsSettingsAssets;
        List<RecorderInputSetting> m_InputsSettings;

        public string ownerRecorderSettingsAssetId;

        public void OnEnable( string ownerSettingsAssetId )
        {
            this.ownerRecorderSettingsAssetId = ownerSettingsAssetId;
            this.Reset();
        }

        public void Reset()
        {
            if(this.m_InputsSettingsAssets == null)
                this.m_InputsSettingsAssets = new List<RecorderInputSetting>();

            this.Rebuild();
        }

        public void Rebuild()
        {
            this.m_InputsSettings = new List<RecorderInputSetting>();

            foreach (var inputAsset in this.m_InputsSettingsAssets)
            {
                if (inputAsset is InputBinder)
                {
                    var sceneInputs = SceneHook.GetInputsComponent(this.ownerRecorderSettingsAssetId);
                    bool found = false;
                    foreach (var input in sceneInputs.m_Settings)
                    {
                        if (input.m_Id == inputAsset.m_Id)
                        {
                            this.m_InputsSettings.Add(input);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        var binder = inputAsset as InputBinder;
                        if( string.IsNullOrEmpty( binder.typeName) )
                            Debug.LogError("Recorder Input asset in invalid!");
                        else
                        {
                            if( Application.isPlaying )
                                Debug.LogError("Recorder input setting missing from scene, adding with default state.");
                            else if( Verbose.enabled )
                                Debug.Log("Recorder input setting missing from scene, adding with default state.");
                            var replacementInput = ScriptableObject.CreateInstance(binder.inputType) as RecorderInputSetting;
                            replacementInput.m_Id = inputAsset.m_Id;
                            this.m_InputsSettings.Add(replacementInput);
                        }
                    }
                }
                else
                    this.m_InputsSettings.Add(inputAsset);
            }            
        }

        public bool ValidityCheck( List<string> errors  )
        {
            foreach( var x in this.m_InputsSettings )
                if (!x.ValidityCheck(errors))
                    return false;
            return true;
        }

        public bool hasBrokenBindings
        {
            get
            {
                foreach( var x in this.m_InputsSettings.ToList() )
                    if (x == null || x is InputBinder)
                        return true;
                return false;
            }
        }

        public RecorderInputSetting this [int index]
        {
            get
            {
                return this.m_InputsSettings[index]; 
            }

            set
            {
                this.ReplaceAt(index, value);
            }
        }

        public IEnumerator<RecorderInputSetting> GetEnumerator()
        {
            return ((IEnumerable<RecorderInputSetting>)this.m_InputsSettings).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void AddRange(List<RecorderInputSetting> list)
        {
            foreach (var value in list)
                this.Add(value);
        }

        public void Add(RecorderInputSetting input)
        {
            this.m_InputsSettings.Add(null);
            this.m_InputsSettingsAssets.Add(null);
            this.ReplaceAt(this.m_InputsSettings.Count - 1, input);
        }

        public int Count
        {
            get
            {
                return this.m_InputsSettings.Count; 
            }
        }

        public void Rebind(RecorderInputSetting input)
        {
            if (input is InputBinder)
            {
                Debug.LogError("Cannot bind a InputBinder object!");
                return;
            }

            for (int i = 0; i < this.m_InputsSettings.Count; i++)
            {
                var x = this.m_InputsSettings[i];
                var ib = x as InputBinder;
                if ( ib != null && ib.m_Id == input.m_Id) 
                {
                    this.m_InputsSettings[i] = input;
                    return;
                }
            }
        }

        public void Remove(RecorderInputSetting input)
        {
            for (int i = 0; i < this.m_InputsSettings.Count; i++)
            {
                if (this.m_InputsSettings[i] == input)
                {
                    this.ReleaseAt(i);
                    this.m_InputsSettings.RemoveAt(i);
                    this.m_InputsSettingsAssets.RemoveAt(i);
                }
            }
        }

        public void ReplaceAt(int index, RecorderInputSetting input)
        {
            if (this.m_InputsSettingsAssets == null || this.m_InputsSettings.Count <= index)
                throw new ArgumentException("Index out of range");

            // Release input
            this.ReleaseAt(index);

            this.m_InputsSettings[index] = input;
            if (input.storeInScene)
            {
                var binder = ScriptableObject.CreateInstance<InputBinder>();
                binder.name = "Scene-Stored";
                binder.m_DisplayName = input.m_DisplayName;
                binder.typeName = input.GetType().AssemblyQualifiedName;
                binder.m_Id = input.m_Id;
                this.m_InputsSettingsAssets[index] = binder;
                SceneHook.RegisterInputSettingObj(this.ownerRecorderSettingsAssetId, input);

#if UNITY_EDITOR
                var assetPath = AssetDatabase.GUIDToAssetPath(this.ownerRecorderSettingsAssetId);
                AssetDatabase.AddObjectToAsset(binder, assetPath);
                AssetDatabase.SaveAssets();
#endif

            }
            else
            {
                this.m_InputsSettingsAssets[index] = input;
#if UNITY_EDITOR
                AssetDatabase.AddObjectToAsset(input, AssetDatabase.GUIDToAssetPath(this.ownerRecorderSettingsAssetId));
                AssetDatabase.SaveAssets();
#endif
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

        }

        void ReleaseAt(int index)
        {
#if UNITY_EDITOR
            bool isBinder = this.m_InputsSettingsAssets[index] is InputBinder;
            if ( isBinder ) 
                SceneHook.UnregisterInputSettingObj(this.ownerRecorderSettingsAssetId, this.m_InputsSettings[index]);
#endif
            UnityHelpers.Destroy(this.m_InputsSettingsAssets[index],true);
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif

            this.m_InputsSettings[index] = null;
            this.m_InputsSettingsAssets[index] = null;
        }

#if UNITY_EDITOR
        public void RepareMissingBindings()
        {
            for (int i = 0; i < this.m_InputsSettingsAssets.Count; i++)
            {
                var ib = this.m_InputsSettingsAssets[i] as InputBinder;
                if (ib != null && this.m_InputsSettings[i] == null)
                {
                    var newInput = ScriptableObject.CreateInstance(ib.inputType) as RecorderInputSetting;
                    newInput.m_DisplayName = ib.m_DisplayName;
                    newInput.m_Id = ib.m_Id;
                    this.m_InputsSettings[i] = newInput;
                    SceneHook.RegisterInputSettingObj(this.ownerRecorderSettingsAssetId, newInput);
                }
            }            
        }
#endif

        public void OnDestroy()
        {
            for (int i = 0; i < this.m_InputsSettingsAssets.Count; i++)
            {
                if (this.m_InputsSettingsAssets[i] is InputBinder)
                    SceneHook.UnregisterInputSettingObj(this.ownerRecorderSettingsAssetId, this.m_InputsSettings[i]);

                UnityHelpers.Destroy(this.m_InputsSettingsAssets[i], true);
            }
        }

    }
}