using UnityEngine;

namespace TextMesh_Pro.Scripts
{
    
    public class TMPro_InstructionOverlay : MonoBehaviour
    {

        public enum FpsCounterAnchorPositions { TopLeft, BottomLeft, TopRight, BottomRight };

        public FpsCounterAnchorPositions AnchorPosition = FpsCounterAnchorPositions.BottomLeft;

        private const string instructions = "Camera Control - <#ffff00>Shift + RMB\n</color>Zoom - <#ffff00>Mouse wheel.";

        private TMPro.TextMeshPro m_TextMeshPro;
        private TMPro.TextContainer m_textContainer;
        private Transform m_frameCounter_transform;
        private Camera m_camera;

        //private FpsCounterAnchorPositions last_AnchorPosition;

        void Awake()
        {
            if (!this.enabled) {
                return;
            }

            this.m_camera = Camera.main;

            GameObject frameCounter = new GameObject("Frame Counter");
            this.m_frameCounter_transform = frameCounter.transform;
            this.m_frameCounter_transform.parent = this.m_camera.transform;
            this.m_frameCounter_transform.localRotation = Quaternion.identity;


            this.m_TextMeshPro = frameCounter.AddComponent<TMPro.TextMeshPro>();
            this.m_TextMeshPro.font = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            this.m_TextMeshPro.fontSharedMaterial = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Overlay");

            this.m_TextMeshPro.fontSize = 30;

            this.m_TextMeshPro.isOverlay = true;
            this.m_textContainer = frameCounter.GetComponent<TMPro.TextContainer>();

            this.Set_FrameCounter_Position(this.AnchorPosition);
            //last_AnchorPosition = AnchorPosition;

            this.m_TextMeshPro.text = instructions;

        }




        void Set_FrameCounter_Position(FpsCounterAnchorPositions anchor_position)
        {

            switch (anchor_position)
            {
                case FpsCounterAnchorPositions.TopLeft:
                    //m_TextMeshPro.anchor = AnchorPositions.TopLeft;
                    this.m_textContainer.anchorPosition = TMPro.TextContainerAnchors.TopLeft;
                    this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0, 1, 100.0f));
                    break;
                case FpsCounterAnchorPositions.BottomLeft:
                    //m_TextMeshPro.anchor = AnchorPositions.BottomLeft;
                    this.m_textContainer.anchorPosition = TMPro.TextContainerAnchors.BottomLeft;
                    this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(0, 0, 100.0f));
                    break;
                case FpsCounterAnchorPositions.TopRight:
                    //m_TextMeshPro.anchor = AnchorPositions.TopRight;
                    this.m_textContainer.anchorPosition = TMPro.TextContainerAnchors.TopRight;
                    this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(1, 1, 100.0f));
                    break;
                case FpsCounterAnchorPositions.BottomRight:
                    //m_TextMeshPro.anchor = AnchorPositions.BottomRight;
                    this.m_textContainer.anchorPosition = TMPro.TextContainerAnchors.BottomRight;
                    this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new Vector3(1, 0, 100.0f));
                    break;
            }
        }
    }
}
