#if UNITY_EDITOR

using Assets.Scripts.Common;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof(Screenshot))]
    public class ScreenshotEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (Screenshot) target;

            if (GUILayout.Button("Take"))
            {
                script.Take();
            }
        }
    }
}

#endif