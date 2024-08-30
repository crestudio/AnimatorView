#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

/*
 * VRSuya AnimatorView
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.animatorview {

	[InitializeOnLoad]
	public class AnimatorView : MonoBehaviour {

		static AnimatorView() {
			EditorApplication.update += OnEditorUpdate;
			return;
		}

		/// <summary>해당 오브젝트를 기준으로 Scene 뷰를 정렬합니다.</summary>
		private static void OnEditorUpdate() {
			if (AnimatorViewEditor.IsSceneViewLocked) {
				if ((GameObject)AnimatorViewEditor.TargetGameObject) {
					foreach (SceneView TargetSceneView in SceneView.sceneViews) {
						TargetSceneView.pivot = ((GameObject)AnimatorViewEditor.TargetGameObject).transform.position + AnimatorViewEditor.TargetOffset;
						TargetSceneView.size = AnimatorViewEditor.TargetSceneZoom;
						TargetSceneView.Repaint();
					}
				}
			}
			return;
		}

		[ExecuteInEditMode]
		public class AnimatorViewEditor : EditorWindow {

			public static bool IsSceneViewLocked = false;
			public static Object TargetGameObject = null;
			public static Vector3 TargetOffset = Vector3.zero;
			public static float TargetSceneZoom = 0.1f;

			[MenuItem("Tools/VRSuya/AnimatorView", priority = 1000)]
			static void CreateWindow() {
				AnimatorViewEditor AppWindow = (AnimatorViewEditor)GetWindowWithRect(typeof(AnimatorViewEditor), new Rect(0, 0, 230, 160));
				AppWindow.titleContent = new GUIContent("AnimatorView");
			}

			void OnGUI() {
				EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label("Track the GameObject", EditorStyles.boldLabel);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				IsSceneViewLocked = EditorGUILayout.Toggle("Active", IsSceneViewLocked, GUILayout.Width(200));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				TargetGameObject = EditorGUILayout.ObjectField(GUIContent.none, TargetGameObject, typeof(GameObject), true, GUILayout.Width(200));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				TargetOffset = EditorGUILayout.Vector3Field(GUIContent.none, TargetOffset, GUILayout.Width(200));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				TargetSceneZoom = EditorGUILayout.Slider(GUIContent.none, TargetSceneZoom, 0.0f, 5.0f, GUILayout.Width(200));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
				if (GUILayout.Button("Close", GUILayout.Width(100))) {
					Close();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			}
		}
	}
}
#endif