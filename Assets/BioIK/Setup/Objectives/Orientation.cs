using UnityEngine;

namespace BioIK {

	//This objective aims to minimise the rotational distance between the transform and the target.
	[AddComponentMenu("")]
	public class Orientation : BioObjective {

		[SerializeField] private Transform Target;
		[SerializeField] private double TRX, TRY, TRZ, TRW;
		[SerializeField] private double MaximumError = 0.1;

		public override ObjectiveType GetObjectiveType() {
			return ObjectiveType.Orientation;
		}

		public override void UpdateData() {
			if(Segment.Character.Evolution == null) {
				return;
			}
			if(Target != null) {
				Quaternion rotation = Target.rotation;
				TRX = rotation.x;
				TRY = rotation.y;
				TRZ = rotation.z;
				TRW = rotation.w;
			}
		}

		public override double ComputeLoss(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double d = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(d < 0.0) {
				d = -d;
				if(d > 1.0) {
					d = 1.0;
				}
			} else if(d > 1.0) {
				d = 1.0;
			}
			double loss = 2.0 * System.Math.Acos(d);
			return Weight * loss * loss;
		}

		public override bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double d = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(d < 0.0) {
				d = -d;
				if(d > 1.0) {
					d = 1.0;
				}
			} else if(d > 1.0) {
				d = 1.0;
			}
			return 2.0 * System.Math.Acos(d) <= Utility.Deg2Rad * MaximumError;
		}

		public override double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double d = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(d < 0.0) {
				d = -d;
				if(d > 1.0) {
					d = 1.0;
				}
			} else if(d > 1.0) {
				d = 1.0;
			}
			return Utility.Rad2Deg * 2.0 * System.Math.Acos(d);
		}

		public void SetTargetTransform(Transform target) {
			Target = target;
			if(Target != null) {
				SetTargetRotation(Target.rotation);
			}
		}

		public Transform GetTargetTransform() {
			return Target;
		}

		public void SetTargetRotation(Quaternion rotation) {
			TRX = rotation.x;
			TRY = rotation.y;
			TRZ = rotation.z;
			TRW = rotation.w;
		}

		public void SetTargetRotation(Vector3 angles) {
			SetTargetRotation(Quaternion.Euler(angles));
		}

		public Vector3 GetTargetRotattion() {
			return new Quaternion((float)TRX, (float)TRY, (float)TRZ, (float)TRW).eulerAngles;
		}

		public void SetMaximumError(double degrees) {
			MaximumError = degrees;
		}

		public double GetMaximumError() {
			return MaximumError;
		}
		
	}

}