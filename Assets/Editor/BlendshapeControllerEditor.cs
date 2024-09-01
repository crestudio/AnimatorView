using System.Linq;

using UnityEngine;
using UnityEditor;

/*
 * VRSuya AnimatorView Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.animatorview {

    [CustomEditor(typeof(BlendshapeController))]
    public class BlendshapeControllerEditor : Editor {

		SerializedProperty SerializedTargetSkinnedMeshRenderer;
		SerializedProperty SerializedTargetAnimatorController;
		SerializedProperty SerializedBlendShapeSliders;

		void OnEnable() {
			SerializedTargetSkinnedMeshRenderer = serializedObject.FindProperty("TargetSkinnedMeshRenderer");
			SerializedTargetAnimatorController = serializedObject.FindProperty("TargetAnimatorController");
			SerializedBlendShapeSliders = serializedObject.FindProperty("BlendShapeSliders");
		}

        public override void OnInspectorGUI() {
			serializedObject.Update();
			BlendshapeController Instance = (BlendshapeController)target;
			EditorGUILayout.PropertyField(SerializedTargetSkinnedMeshRenderer, new GUIContent("스킨드 메쉬 렌더러"));
			EditorGUILayout.PropertyField(SerializedTargetAnimatorController, new GUIContent("애니메이터"));
			if (Instance.BlendShapeList.Count > 0) {
				for (int Index = 0; Index < Instance.BlendShapeList.Count; Index++) {
					string BlendShapeName = Instance.BlendShapeList.Keys.ElementAt(Index);
					float CurrentValue = Instance.TargetSkinnedMeshRenderer.GetBlendShapeWeight(Instance.BlendShapeList.Values.ElementAt(Index));
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(BlendShapeName);
					EditorGUI.BeginChangeCheck();
					float NewValue = EditorGUILayout.Slider(CurrentValue, 0, 100);
					EditorGUILayout.EndHorizontal();
					if (EditorGUI.EndChangeCheck()) {
						Undo.RecordObject(Instance.TargetSkinnedMeshRenderer, "Changed Blendshape");
						Instance.TargetSkinnedMeshRenderer.SetBlendShapeWeight(Instance.BlendShapeList.Values.ElementAt(Index), NewValue);
						EditorUtility.SetDirty(Instance.TargetSkinnedMeshRenderer);
					}
				}
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			serializedObject.ApplyModifiedProperties();
			if (GUILayout.Button("리스트 업데이트")) {
				(target as BlendshapeController).UpdateBlendshapeList();
			}
		}
    }
}

