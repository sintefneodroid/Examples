using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Recorder
{
    class RecorderSelector
    {
        string m_Category;
        string[] m_RecorderNames;
        string[] m_Categories;
        List<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInfo> m_Recorders;
        bool m_SettingsAreAssets;
        bool m_CategoryIsReadonly = false;

        public Type selectedRecorder { get; private set; }

        Action m_SetRecorderCallback;

        public string category {
            get { return this.m_Category;}
        }

        public RecorderSelector(Action setRecorderCallback, bool categoryIsReadonly)
        {
            this.m_CategoryIsReadonly = categoryIsReadonly;
            this.m_Categories = Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.availableCategories;
            this.m_SetRecorderCallback = setRecorderCallback;
        }

        public void Init( Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings settings, string startingCategory = "" )
        {
            // Pre existing settings obj?
            if( settings != null )
            {
                var recInfo = Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.GetRecorderInfo(settings.recorderType);

                // category value overrides existing settings.
                if (!string.IsNullOrEmpty(startingCategory))
                {
                    if (string.Compare(recInfo.category, startingCategory, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        // forced another category, flush existing settings obj.
                        this.SetCategory(startingCategory);
                        this.SelectRecorder(this.GetRecorderFromIndex(0) );
                    }
                }

                // Not invalidated by category, so set and we are done
                if( settings != null )
                {
                    this.SetCategory(recInfo.category);
                    this.SelectRecorder(settings.recorderType);
                    return;
                }
            }
            else
                this.SetCategory(startingCategory);
        }

        int GetCategoryIndex()
        {
            for (int i = 0; i < this.m_Categories.Length; i++)
                if (this.m_Categories[i] == this.m_Category)
                    return i;

            if (this.m_Categories.Length > 0)
                return 0;
            else
                return -1;
        }

        void SetCategory(string category)
        {
            this.m_Category = category;
            if (string.IsNullOrEmpty(this.m_Category) && this.m_Categories.Length > 0) this.m_Category = "Video"; // default

            if (string.IsNullOrEmpty(this.m_Category))
            {
                this.m_Category = string.Empty;
                this.m_RecorderNames = new string[0];                
            }
            else
            {
                this.m_Recorders = Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.recordersByCategory[this.m_Category];
                this.m_RecorderNames = Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.recordersByCategory[this.m_Category]
                    .Select(x => x.displayName)
                    .ToArray();
            }
        }

        void SetCategoryFromIndex(int index)
        {
            if (index >= 0)
            {
                var newCategory = Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.availableCategories[index];
                if (string.Compare(this.m_Category, newCategory, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return;
                this.SetCategory(newCategory);
            }
            else
            {
                this.SetCategory(string.Empty);
            }
        }

        int GetRecorderIndex()
        {
            if (this.m_Recorders.Count == 0)
                return -1;
            
            for (int i = 0; i < this.m_Recorders.Count; i++)
                if (this.m_Recorders[i].recorderType == this.selectedRecorder)
                    return i;

            if (this.m_Recorders.Count > 0)
                return 0;
            else
                return -1;
        }

        Type GetRecorderFromIndex(int index)
        {
            if (index >= 0)
                return Unity_Technologies.Recorder.Framework.Core.Engine.RecordersInventory.recordersByCategory[this.m_Category][index].recorderType;

            return null;
        }

        public void OnGui()
        {
            // Group selection
            if (!this.m_CategoryIsReadonly)
            {
                EditorGUILayout.BeginHorizontal();
                this.SetCategoryFromIndex(EditorGUILayout.Popup("Recorder category:", this.GetCategoryIndex(), this.m_Categories));
                EditorGUILayout.EndHorizontal();
            }

            // Recorder in group selection
            EditorGUILayout.BeginHorizontal();
            var oldIndex = this.GetRecorderIndex();
            var newIndex = EditorGUILayout.Popup("Selected recorder:", oldIndex, this.m_RecorderNames);
            this.SelectRecorder(this.GetRecorderFromIndex(newIndex));

            EditorGUILayout.EndHorizontal();
        }

        void SelectRecorder( Type newSelection )
        {
            if (this.selectedRecorder == newSelection)
                return;

            var recorderAttribs = newSelection.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            if (recorderAttribs.Length > 0 )
                Debug.LogWarning( "Recorder " + ((ObsoleteAttribute)recorderAttribs[0]).Message);

            this.selectedRecorder = newSelection;
            this.m_SetRecorderCallback();
        }
    }
}
