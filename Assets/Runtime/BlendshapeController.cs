#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Animations;

/*
 * VRSuya AnimatorView
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.animatorview {

	[ExecuteInEditMode]
	[AddComponentMenu("VRSuya/VRSuya Blendshape Viewer")]
	public class BlendshapeController : MonoBehaviour {

		public SkinnedMeshRenderer TargetSkinnedMeshRenderer = null;
		public Animator TargetAnimator = null;
		public GameObject SliderPrefab;

		private string[] TargetBlendShapeNames = new string[0];
		private Dictionary<string, Slider> BlendShapeSliders = new Dictionary<string, Slider>();
		private readonly string[] dictHeadNames = new string[] { "Body", "Head", "Face" };

		void Start() {
			if (!TargetSkinnedMeshRenderer) TargetSkinnedMeshRenderer = this.gameObject.GetComponent<SkinnedMeshRenderer>();
			if (!TargetAnimator) TargetAnimator = this.transform.parent.GetComponent<Animator>();
			CreateSlidersForBlendshapes();
			return;
		}

		private void CreateSlidersForBlendshapes() {
			Mesh TargetMesh = TargetSkinnedMeshRenderer.sharedMesh;
			int BlendShapeCount = TargetMesh.blendShapeCount;
			if (TargetAnimator) {
				AnimationClip[] AllAnimationClips = GetAnimationClips((AnimatorController)TargetAnimator.runtimeAnimatorController);
				foreach (AnimationClip TargetAnimationClip in AllAnimationClips) {
					foreach (var Binding in AnimationUtility.GetCurveBindings(TargetAnimationClip)) {
						if (Array.Exists(dictHeadNames, Name => Binding.path == Name)) {
							if (Binding.type == typeof(SkinnedMeshRenderer)) {
								string BlendshapeName = Binding.propertyName.Remove(0, 11);
								if (!Array.Exists(TargetBlendShapeNames, Item => BlendshapeName == Item)) {
									TargetBlendShapeNames = TargetBlendShapeNames.Concat(new string[] { BlendshapeName }).ToArray();
								}
							}
						}
					}
				}
			}
			for (int Index = 0; Index < BlendShapeCount; Index++) {
				if (Array.Exists(TargetBlendShapeNames, Item => TargetMesh.GetBlendShapeName(Index) == Item)) {
					string BlendShapeName = TargetMesh.GetBlendShapeName(Index);
					float CurrentValue = TargetSkinnedMeshRenderer.GetBlendShapeWeight(Index);
					GameObject TargetSliderObj = Instantiate(SliderPrefab, transform);
					Slider TargetSlider = TargetSliderObj.GetComponent<Slider>();
					TargetSlider.minValue = 0;
					TargetSlider.maxValue = 100;
					TargetSlider.value = CurrentValue;
					TargetSlider.name = BlendShapeName;
					int BlendShapeIndex = Index;
					TargetSlider.onValueChanged.AddListener(BlendShapeValue => {
						TargetSkinnedMeshRenderer.SetBlendShapeWeight(BlendShapeIndex, BlendShapeValue);
					});
					BlendShapeSliders.Add(BlendShapeName, TargetSlider);
				}
			}
			return;
		}

		public void UpdateBlendshape(string BlendShapeName, float BlendShapeValue) {
			if (BlendShapeSliders.ContainsKey(BlendShapeName)) {
				BlendShapeSliders[BlendShapeName].value = BlendShapeValue;
				int index = TargetSkinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(BlendShapeName);
				TargetSkinnedMeshRenderer.SetBlendShapeWeight(index, BlendShapeValue);
			}
			return;
		}

		private AnimationClip[] GetAnimationClips(AnimatorController TargetAnimatorController) {
			List<AnimatorStateMachine> RootStateMachines = TargetAnimatorController.layers.Select(AnimationLayer => AnimationLayer.stateMachine).ToList();
			List<AnimatorStateMachine> AllStateMachines = new List<AnimatorStateMachine>();
			List<AnimatorState> AllAnimatorState = new List<AnimatorState>();
			List<AnimationClip> AllAnimationClips = new List<AnimationClip>();
			foreach (AnimatorStateMachine SubStateMachine in RootStateMachines) {
				AllStateMachines.AddRange(GetAllStateMachines(SubStateMachine));
			}
			foreach (AnimatorStateMachine SubStateMachine in AllStateMachines) {
				AllAnimatorState.AddRange(GetAllStates(SubStateMachine));
			}
			if (AllAnimatorState.Count > 0) {
				List<Motion> AllMotion = AllAnimatorState.Select(State => State.motion).ToList();
				foreach (Motion SubMotion in AllMotion) {
					AllAnimationClips.AddRange(GetAnimationClips(SubMotion));
				}
			}
			AllAnimationClips = AllAnimationClips.Distinct().ToList();
			AllAnimationClips.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
			return AllAnimationClips.ToArray();
		}

		private AnimatorState[] GetAllStates(AnimatorStateMachine TargetStateMachine) {
			AnimatorState[] States = TargetStateMachine.states.Select(ExistChildState => ExistChildState.state).ToArray();
			if (TargetStateMachine.stateMachines.Length > 0) {
				foreach (var TargetChildStatetMachine in TargetStateMachine.stateMachines) {
					States = States.Concat(GetAllStates(TargetChildStatetMachine.stateMachine)).ToArray();
				}
			}
			return States;
		}

		private AnimatorStateMachine[] GetAllStateMachines(AnimatorStateMachine TargetStateMachine) {
			AnimatorStateMachine[] StateMachines = new AnimatorStateMachine[] { TargetStateMachine };
			if (TargetStateMachine.stateMachines.Length > 0) {
				foreach (var TargetChildStateMachine in TargetStateMachine.stateMachines) {
					StateMachines = StateMachines.Concat(GetAllStateMachines(TargetChildStateMachine.stateMachine)).ToArray();
				}
			}
			return StateMachines;
		}

		private AnimationClip[] GetAnimationClips(Motion TargetMotion) {
			AnimationClip[] MotionAnimationClips = new AnimationClip[0];
			if (TargetMotion is AnimationClip) {
				MotionAnimationClips = MotionAnimationClips.Concat(new AnimationClip[] { (AnimationClip)TargetMotion }).ToArray();
			} else if (TargetMotion is BlendTree ChildBlendTree) {
				foreach (ChildMotion ChildMotion in ChildBlendTree.children) {
					MotionAnimationClips = MotionAnimationClips.Concat(GetAnimationClips(ChildMotion.motion)).ToArray();
				}
			}
			return MotionAnimationClips;
		}
	}
}
#endif