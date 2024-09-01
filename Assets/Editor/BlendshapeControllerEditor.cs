﻿using System.Collections.Generic;
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
		SerializedProperty SerializedTargetAnimator;

		private List<string> ExceedLimitBlendshape = new List<string>();

		void OnEnable() {
			SerializedTargetSkinnedMeshRenderer = serializedObject.FindProperty("TargetSkinnedMeshRenderer");
			SerializedTargetAnimator = serializedObject.FindProperty("TargetAnimator");
		}

        public override void OnInspectorGUI() {
			serializedObject.Update();
			BlendshapeController Instance = (BlendshapeController)target;
			EditorGUILayout.PropertyField(SerializedTargetSkinnedMeshRenderer, new GUIContent("스킨드 메쉬 렌더러"));
			EditorGUILayout.PropertyField(SerializedTargetAnimator, new GUIContent("애니메이터"));
			if (Instance.BlendShapeList.Count > 0) {
				for (int Index = 0; Index < Instance.BlendShapeList.Count; Index++) {
					string BlendShapeName = Instance.BlendShapeList.Keys.ElementAt(Index);
					float CurrentValue = Instance.TargetSkinnedMeshRenderer.GetBlendShapeWeight(Instance.BlendShapeList.Values.ElementAt(Index));
					if (CurrentValue < 0.0f || CurrentValue > 100.0f) {
						if (!ExceedLimitBlendshape.Exists(Item => Item == BlendShapeName)) {
							ExceedLimitBlendshape.Add(BlendShapeName);
						}
					}
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
			if (ExceedLimitBlendshape.Count > 0) {
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				EditorGUILayout.LabelField("수치를 초과한 Blendshape");
				EditorGUI.indentLevel++;
				foreach (string ExceedBlendshape in ExceedLimitBlendshape) {
					EditorGUILayout.LabelField("▶ " + ExceedBlendshape);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			serializedObject.ApplyModifiedProperties();
			if (GUILayout.Button("리스트 업데이트")) {
				(target as BlendshapeController).UpdateBlendshapeList();
				ExceedLimitBlendshape = new List<string>();
			}
			return;
		}
    }
}

