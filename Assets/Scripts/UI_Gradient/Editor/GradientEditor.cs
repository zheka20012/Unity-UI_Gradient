using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
    /// Editor class used to edit UI Images.
    /// </summary>
    [CustomEditor(typeof(UI_Gradient), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom editor for Gradient.
    /// </summary>
    public class GradientEditor : GraphicEditor
    {
        SerializedProperty m_Gradient;
        SerializedProperty m_Invert;

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }


        [MenuItem("GameObject/UI/Gradient", false, 2003)]
        public static void AddGradient(MenuCommand menuCommand)
        {
            GameObject child = new GameObject("Gradient");
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100f, 100f);
            child.AddComponent<UI_Gradient>();

            GameObject parent = menuCommand.context as GameObject;

            if (parent == null)
            {
                Debug.Log("Can't find Canvas object");
                return;
            }

            // Setting the element to be a child of an element already in the scene should
            // be sufficient to also move the element to that scene.
            // However, it seems the element needs to be already in its destination scene when the
            // RegisterCreatedObjectUndo is performed; otherwise the scene it was created in is dirtied.
            SceneManager.MoveGameObjectToScene(child, parent.scene);

            Undo.RegisterCreatedObjectUndo(child, "Create " + child.name);

            if (child.transform.parent == null)
            {
                Undo.SetTransformParent(child.transform, parent.transform, "Parent " + child.name);
            }

            GameObjectUtility.EnsureUniqueNameForSibling(child);

            // We have to fix up the undo name since the name of the object was only known after reparenting it.
            Undo.SetCurrentGroupName("Create " + child.name);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Gradient = serializedObject.FindProperty("m_Gradient");
            m_Invert = serializedObject.FindProperty("m_Invert");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Gradient);
            EditorGUILayout.PropertyField(m_Invert);
            EditorGUILayout.PropertyField(m_Material);
            RaycastControlsGUI();
            NativeSizeButtonGUI();


            serializedObject.ApplyModifiedProperties();

        }

        /// <summary>
        /// Allow the texture to be previewed.
        /// </summary>
        public override bool HasPreviewGUI()
        {
            return true;
        }

        /// <summary>
        /// Draw the Gradient preview.
        /// </summary>
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            UI_Gradient gradient = target as UI_Gradient;
            if (gradient == null) return;
            Texture gradTexture= gradient.mainTexture;
            if (gradTexture == null) return;

            // Create the texture rectangle that is centered inside rect.
            Rect outerRect = rect;
            outerRect.width = Mathf.Abs(rect.width);
            outerRect.height = Mathf.Abs(rect.height);

            if (outerRect.width > 0f)
            {
                float f = rect.width / outerRect.width;
                outerRect.width *= f;
                outerRect.height *= f;
            }

            if (rect.height > outerRect.height)
            {
                outerRect.y += (rect.height - outerRect.height) * 0.5f;
            }
            else if (outerRect.height > rect.height)
            {
                float f = rect.height / outerRect.height;
                outerRect.width *= f;
                outerRect.height *= f;
            }

            if (rect.width > outerRect.width)
                outerRect.x += (rect.width - outerRect.width) * 0.5f;


            EditorGUI.DrawPreviewTexture(outerRect, gradTexture, gradient.material, ScaleMode.ScaleToFit, 1f);
        }

        /// <summary>
        /// Info String drawn at the bottom of the Preview
        /// </summary>
        public override string GetInfoString()
        {
            UI_Gradient gradient = target as UI_Gradient;

            if (gradient == null) return "Ooops, looks like something went wrong!";

            // Image size Text
            return string.Format("Gradient Size: {0}x{1}", Mathf.RoundToInt(Mathf.Abs(gradient.rectTransform.rect.width)), Mathf.RoundToInt(Mathf.Abs(gradient.rectTransform.rect.height)));
        }
}