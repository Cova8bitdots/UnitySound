using UnityEngine;
using UnityEditor;

using CovaTech.Lib;
using CovaTech.EditorTool;
using CovaTech.UnitySound;

namespace CovaTech.UnitySound.EditorTool
{

    [CustomEditor(typeof(CovaTech.UnitySound.SoundItem))]
    public class SoundItemEditor : EditorBase
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.LabelField("-------------DEBUG-------------");

            var item = target as SoundItem;

            EditorGUILayout.IntField("ID", item.GetHandler());
            EditorGUILayout.LabelField("[State]");
            EditorGUI.indentLevel++;
            {
                EditorGUILayout.EnumPopup("Current", item.CurrentState);
                EditorGUILayout.EnumPopup("Prev", item.CurrentState);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EnumPopup("AUDIO_TYPE", item.AudioType);
            EditorGUILayout.Toggle("IsFading", item.IsFading);
            EditorGUILayout.Toggle("IsPlaying", item.IsPlaying);
            EditorGUILayout.Toggle("IsUsing", item.IsUsed());

        }
    }
}
