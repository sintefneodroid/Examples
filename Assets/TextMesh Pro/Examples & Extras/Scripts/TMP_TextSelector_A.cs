using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TextMesh_Pro.Scripts
{

    public class TMP_TextSelector_A : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private TextMeshPro m_TextMeshPro;

        private Camera m_Camera;

        private bool m_isHoveringObject;
        private int m_selectedLink = -1;
        private int m_lastCharIndex = -1;
        private int m_lastWordIndex = -1;

        void Awake()
        {
            this.m_TextMeshPro = this.gameObject.GetComponent<TextMeshPro>();
            this.m_Camera = Camera.main;

            // Force generation of the text object so we have valid data to work with. This is needed since LateUpdate() will be called before the text object has a chance to generated when entering play mode.
            this.m_TextMeshPro.ForceMeshUpdate();
        }


        void LateUpdate()
        {
            this.m_isHoveringObject = false;

            if (TMP_TextUtilities.IsIntersectingRectTransform(this.m_TextMeshPro.rectTransform, Input.mousePosition, Camera.main))
            {
                this.m_isHoveringObject = true;
            }

            if (this.m_isHoveringObject)
            {
                #region Example of Character Selection
                int charIndex = TMP_TextUtilities.FindIntersectingCharacter(this.m_TextMeshPro, Input.mousePosition, Camera.main, true);
                if (charIndex != -1 && charIndex != this.m_lastCharIndex && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                {
                    //Debug.Log("[" + m_TextMeshPro.textInfo.characterInfo[charIndex].character + "] has been selected.");

                    this.m_lastCharIndex = charIndex;

                    int meshIndex = this.m_TextMeshPro.textInfo.characterInfo[charIndex].materialReferenceIndex;

                    int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[charIndex].vertexIndex;

                    Color32 c = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);

                    Color32[] vertexColors = this.m_TextMeshPro.textInfo.meshInfo[meshIndex].colors32;

                    vertexColors[vertexIndex + 0] = c;
                    vertexColors[vertexIndex + 1] = c;
                    vertexColors[vertexIndex + 2] = c;
                    vertexColors[vertexIndex + 3] = c;

                    //m_TextMeshPro.mesh.colors32 = vertexColors;
                    this.m_TextMeshPro.textInfo.meshInfo[meshIndex].mesh.colors32 = vertexColors;
                }
                #endregion

                #region Example of Link Handling
                // Check if mouse intersects with any links.
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(this.m_TextMeshPro, Input.mousePosition, this.m_Camera);

                // Clear previous link selection if one existed.
                if ((linkIndex == -1 && this.m_selectedLink != -1) || linkIndex != this.m_selectedLink)
                {
                    //m_TextPopup_RectTransform.gameObject.SetActive(false);
                    this.m_selectedLink = -1;
                }

                // Handle new Link selection.
                if (linkIndex != -1 && linkIndex != this.m_selectedLink)
                {
                    this.m_selectedLink = linkIndex;

                    TMP_LinkInfo linkInfo = this.m_TextMeshPro.textInfo.linkInfo[linkIndex];

                    // The following provides an example of how to access the link properties.
                    Debug.Log("Link ID: \"" + linkInfo.GetLinkID() + "\"   Link Text: \"" + linkInfo.GetLinkText() + "\""); // Example of how to retrieve the Link ID and Link Text.

                    Vector3 worldPointInRectangle = Vector3.zero;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_TextMeshPro.rectTransform, Input.mousePosition, this.m_Camera, out worldPointInRectangle);

                    switch (linkInfo.GetLinkID())
                    {
                        case "id_01": // 100041637: // id_01
                                      //m_TextPopup_RectTransform.position = worldPointInRectangle;
                                      //m_TextPopup_RectTransform.gameObject.SetActive(true);
                                      //m_TextPopup_TMPComponent.text = k_LinkText + " ID 01";
                            break;
                        case "id_02": // 100041638: // id_02
                                      //m_TextPopup_RectTransform.position = worldPointInRectangle;
                                      //m_TextPopup_RectTransform.gameObject.SetActive(true);
                                      //m_TextPopup_TMPComponent.text = k_LinkText + " ID 02";
                            break;
                    }
                }
                #endregion


                #region Example of Word Selection
                // Check if Mouse intersects any words and if so assign a random color to that word.
                int wordIndex = TMP_TextUtilities.FindIntersectingWord(this.m_TextMeshPro, Input.mousePosition, Camera.main);
                if (wordIndex != -1 && wordIndex != this.m_lastWordIndex)
                {
                    this.m_lastWordIndex = wordIndex;

                    TMP_WordInfo wInfo = this.m_TextMeshPro.textInfo.wordInfo[wordIndex];

                    Vector3 wordPOS = this.m_TextMeshPro.transform.TransformPoint(this.m_TextMeshPro.textInfo.characterInfo[wInfo.firstCharacterIndex].bottomLeft);
                    wordPOS = Camera.main.WorldToScreenPoint(wordPOS);

                    //Debug.Log("Mouse Position: " + Input.mousePosition.ToString("f3") + "  Word Position: " + wordPOS.ToString("f3"));

                    Color32[] vertexColors = this.m_TextMeshPro.textInfo.meshInfo[0].colors32;

                    Color32 c = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
                    for (int i = 0; i < wInfo.characterCount; i++)
                    {
                        int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[wInfo.firstCharacterIndex + i].vertexIndex;

                        vertexColors[vertexIndex + 0] = c;
                        vertexColors[vertexIndex + 1] = c;
                        vertexColors[vertexIndex + 2] = c;
                        vertexColors[vertexIndex + 3] = c;
                    }

                    this.m_TextMeshPro.mesh.colors32 = vertexColors;
                }
                #endregion
            }
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("OnPointerEnter()");
            this.m_isHoveringObject = true;
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("OnPointerExit()");
            this.m_isHoveringObject = false;
        }

    }
}
