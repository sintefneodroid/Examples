using System;
using System.IO;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Misc
{
    [Serializable]
    public class DataPath
    {
        public enum Root
        {
            Absolute,
            Current,
            PersistentData,
            StreamingAssets,
            TemporaryCache,
            DataPath,
        }

        [SerializeField] Root m_root = Root.Current;
        [SerializeField] string m_leaf = string.Empty;
#if UNITY_EDITOR
        [SerializeField] bool m_readOnly = false; // just for inspector
#endif

        public Root root
        {
            get { return this.m_root; }
            set { this.m_root = value; }
        }
        public string leaf
        {
            get { return this.m_leaf; }
            set { this.m_leaf = value; }
        }
        public bool readOnly
        {
#if UNITY_EDITOR
            get { return this.m_readOnly; }
            set { this.m_readOnly = value; }
#else
            get { return false; }
            set { }
#endif
        }

        public DataPath() { }
        public DataPath(Root root, string leaf)
        {
            this.m_root = root;
            this.m_leaf = leaf;
        }

        public DataPath(string path)
        {
            if (path.Contains(Application.streamingAssetsPath))
            {
                this.m_root = Root.StreamingAssets;
                this.m_leaf = path.Replace(Application.streamingAssetsPath, "").TrimStart('/');
            }
            else if (path.Contains(Application.dataPath))
            {
                this.m_root = Root.DataPath;
                this.m_leaf = path.Replace(Application.dataPath, "").TrimStart('/');
            }
            else if (path.Contains(Application.persistentDataPath))
            {
                this.m_root = Root.PersistentData;
                this.m_leaf = path.Replace(Application.persistentDataPath, "").TrimStart('/');
            }
            else if (path.Contains(Application.temporaryCachePath))
            {
                this.m_root = Root.TemporaryCache;
                this.m_leaf = path.Replace(Application.temporaryCachePath, "").TrimStart('/');
            }
            else
            {
                var cur = Directory.GetCurrentDirectory().Replace("\\", "/");
                if (path.Contains(cur))
                {
                    this.m_root = Root.Current;
                    this.m_leaf = path.Replace(cur, "").TrimStart('/');
                }
                else
                {
                    this.m_root = Root.Absolute;
                    this.m_leaf = path;
                }
            }
        }

        public string GetFullPath()
        {
            if (this.m_root == Root.Absolute)
            {
                return this.m_leaf;
            }
            if (this.m_root == Root.Current)
            {
                return this.m_leaf.Length == 0 ? "." : "./" + this.m_leaf;
            }

            string ret = "";
            switch (this.m_root)
            {
                case Root.PersistentData:
                    ret = Application.persistentDataPath;
                    break;
                case Root.StreamingAssets:
                    ret = Application.streamingAssetsPath;
                    break;
                case Root.TemporaryCache:
                    ret = Application.temporaryCachePath;
                    break;
                case Root.DataPath:
                    ret = Application.dataPath;
                    break;
            }

            if (!this.m_leaf.StartsWith("/"))
            {
                ret += "/";
            }
            ret += this.m_leaf;
            return ret;
        }

        public void CreateDirectory()
        {
            try
            {
                var path = this.GetFullPath();
                if(path.Length > 0)
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch(Exception)
            {
            }
        }
    }
}