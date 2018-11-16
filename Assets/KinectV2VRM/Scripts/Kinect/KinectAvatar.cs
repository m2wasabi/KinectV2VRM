using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;
using System.Linq;

namespace KinectV2VRM.Kinect
{
    public class KinectAvatar : MonoBehaviour {

        [SerializeField]
        bool IsMirror = true;

        BodySourceManager _bodyManager;
        private Animator _animator;

        // Kinectのローカル回転を正規化されたVRMの回転に変換する設定
        // Ref: https://social.msdn.microsoft.com/Forums/en-US/f2e6a544-705c-43ed-a0e1-731ad907b776/meaning-of-rotation-data-of-k4w-v2?forum=kinectv2sdk
        private static class _K4W_to_VRM
        {
            public static Quaternion center =   Quaternion.AngleAxis( 180, new Vector3( 0, 1, 0 ) );
            public static Quaternion toLeft =   Quaternion.AngleAxis( 180, new Vector3( 0, 1, 0 ) ) *
                                                Quaternion.AngleAxis( -90, new Vector3( 0, 0, 1 ) );
            public static Quaternion toRight =  Quaternion.AngleAxis( 180, new Vector3( 0, 1, 0 ) ) *
                                                Quaternion.AngleAxis( 90, new Vector3( 0, 0, 1 ) );
            public static Quaternion armLeft =  Quaternion.AngleAxis( -90, new Vector3( 0, 1, 0 ) ) *
                                                Quaternion.AngleAxis( -90, new Vector3( 0, 0, 1 ) );
            public static Quaternion armRight = Quaternion.AngleAxis( -90, new Vector3( 0, 1, 0 ) ) *
                                                Quaternion.AngleAxis( 90, new Vector3( 0, 0, 1 ) );
            public static Quaternion legLeft =  Quaternion.AngleAxis( 90, new Vector3( 0, 1, 0 ) ) *
                                                Quaternion.AngleAxis( 180, new Vector3( 0, 0, 1 ) );
            public static Quaternion legRight = Quaternion.AngleAxis( -90, new Vector3( 0, 1, 0 ) ) *
                                                Quaternion.AngleAxis( 180, new Vector3( 0, 0, 1 ) );
        }

        // Use this for initialization
        void Start ()
        {
            _bodyManager = transform.GetComponent<BodySourceManager>();

        }

        // Update is called once per frame
        void Update () {
            if ( _bodyManager == null )
            {
                _bodyManager = gameObject.AddComponent<BodySourceManager>();
                return;
            }

            if ( _animator == null ) {
                return;
            }

            // Bodyデータを取得する
            var data = _bodyManager.GetData();
            if ( data == null ) {
                return;
            }

            // 最初に追跡している人を取得する
            var body = data.FirstOrDefault( b => b.IsTracked );
            if ( body == null ) {
                return;
            }

            // 床の傾きを取得する
            var floorPlane = _bodyManager.FloorClipPlane;
            var comp = Quaternion.FromToRotation(
                new Vector3( floorPlane.X, floorPlane.Y, floorPlane.Z ), Vector3.up );

            // 関節の回転を取得する
            var joints = body.JointOrientations;

            Quaternion SpineBase;
            Quaternion SpineMid;
            Quaternion SpineShoulder;
            Quaternion ShoulderLeft;
            Quaternion ShoulderRight;
            Quaternion ElbowLeft;
            Quaternion WristLeft;
            Quaternion HandLeft;
            Quaternion ElbowRight;
            Quaternion WristRight;
            Quaternion HandRight;
            Quaternion HipLeft;
            Quaternion KneeLeft;
            Quaternion AnkleLeft;
            Quaternion HipRight;
            Quaternion KneeRight;
            Quaternion AnkleRight;
            Quaternion Neck;
            Quaternion Head;

            // 鏡
            if ( IsMirror ) {
                SpineBase = joints[JointType.SpineBase].Orientation.ToMirror().ToQuaternion( comp );
                SpineMid = joints[JointType.SpineMid].Orientation.ToMirror().ToQuaternion( comp );
                SpineShoulder = joints[JointType.SpineShoulder].Orientation.ToMirror().ToQuaternion( comp );
                Neck = joints[JointType.Neck].Orientation.ToMirror().ToQuaternion( comp );
                Head = joints[JointType.Head].Orientation.ToMirror().ToQuaternion( comp );
                ShoulderLeft = joints[JointType.ShoulderRight].Orientation.ToMirror().ToQuaternion( comp );
                ShoulderRight = joints[JointType.ShoulderLeft].Orientation.ToMirror().ToQuaternion( comp );
                ElbowLeft = joints[JointType.ElbowRight].Orientation.ToMirror().ToQuaternion( comp );
                WristLeft = joints[JointType.WristRight].Orientation.ToMirror().ToQuaternion( comp );
                HandLeft = joints[JointType.HandRight].Orientation.ToMirror().ToQuaternion( comp );
                ElbowRight = joints[JointType.ElbowLeft].Orientation.ToMirror().ToQuaternion( comp );
                WristRight = joints[JointType.WristLeft].Orientation.ToMirror().ToQuaternion( comp );
                HandRight = joints[JointType.HandLeft].Orientation.ToMirror().ToQuaternion( comp );
                HipLeft = joints[JointType.HipRight].Orientation.ToMirror().ToQuaternion( comp );
                KneeLeft = joints[JointType.KneeRight].Orientation.ToMirror().ToQuaternion( comp );
                AnkleLeft = joints[JointType.AnkleRight].Orientation.ToMirror().ToQuaternion( comp );
                HipRight = joints[JointType.HipLeft].Orientation.ToMirror().ToQuaternion( comp );
                KneeRight = joints[JointType.KneeLeft].Orientation.ToMirror().ToQuaternion( comp );
                AnkleRight = joints[JointType.AnkleLeft].Orientation.ToMirror().ToQuaternion( comp );
            }
            // そのまま
            else {
                SpineBase = joints[JointType.SpineBase].Orientation.ToQuaternion( comp );
                SpineMid = joints[JointType.SpineMid].Orientation.ToQuaternion( comp );
                SpineShoulder = joints[JointType.SpineShoulder].Orientation.ToQuaternion( comp );
                Neck = joints[JointType.Neck].Orientation.ToQuaternion( comp );
                Head = joints[JointType.Head].Orientation.ToQuaternion( comp );
                ShoulderLeft = joints[JointType.ShoulderLeft].Orientation.ToQuaternion( comp );
                ShoulderRight = joints[JointType.ShoulderRight].Orientation.ToQuaternion( comp );
                ElbowLeft = joints[JointType.ElbowLeft].Orientation.ToQuaternion( comp );
                WristLeft = joints[JointType.WristLeft].Orientation.ToQuaternion( comp );
                HandLeft = joints[JointType.HandLeft].Orientation.ToQuaternion( comp );
                ElbowRight = joints[JointType.ElbowRight].Orientation.ToQuaternion( comp );
                WristRight = joints[JointType.WristRight].Orientation.ToQuaternion( comp );
                HandRight = joints[JointType.HandRight].Orientation.ToQuaternion( comp );
                HipLeft = joints[JointType.HipLeft].Orientation.ToQuaternion( comp );
                KneeLeft = joints[JointType.KneeLeft].Orientation.ToQuaternion( comp );
                AnkleLeft = joints[JointType.AnkleLeft].Orientation.ToQuaternion( comp );
                HipRight = joints[JointType.HipRight].Orientation.ToQuaternion( comp );
                KneeRight = joints[JointType.KneeRight].Orientation.ToQuaternion( comp );
                AnkleRight = joints[JointType.AnkleRight].Orientation.ToQuaternion( comp );
            }

            // 関節の回転を計算する
            var q = transform.rotation;
            transform.rotation = Quaternion.identity;

            _animator.GetBoneTransform(HumanBodyBones.Spine).rotation = SpineMid * _K4W_to_VRM.center;
            if(_animator.GetBoneTransform(HumanBodyBones.UpperChest) != null)
                _animator.GetBoneTransform(HumanBodyBones.UpperChest).rotation = SpineShoulder * _K4W_to_VRM.center;
            _animator.GetBoneTransform(HumanBodyBones.Head).rotation = Neck * _K4W_to_VRM.center;

            if(_animator.GetBoneTransform(HumanBodyBones.RightShoulder) != null)
                _animator.GetBoneTransform(HumanBodyBones.RightShoulder).rotation = ShoulderRight * _K4W_to_VRM.toRight;
            _animator.GetBoneTransform(HumanBodyBones.RightUpperArm).rotation = ElbowRight * _K4W_to_VRM.armRight;
            _animator.GetBoneTransform(HumanBodyBones.RightLowerArm).rotation = WristRight * _K4W_to_VRM.armRight;
            _animator.GetBoneTransform(HumanBodyBones.RightHand).rotation = HandRight * _K4W_to_VRM.armRight;

            if(_animator.GetBoneTransform(HumanBodyBones.LeftShoulder) != null)
                _animator.GetBoneTransform(HumanBodyBones.LeftShoulder).rotation = ShoulderLeft * _K4W_to_VRM.toLeft;
            _animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).rotation = ElbowLeft * _K4W_to_VRM.armLeft;
            _animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).rotation = WristLeft * _K4W_to_VRM.armLeft;
            _animator.GetBoneTransform(HumanBodyBones.LeftHand).rotation = HandLeft * _K4W_to_VRM.armLeft;

            _animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).rotation = KneeRight * _K4W_to_VRM.legRight;
            _animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).rotation = AnkleRight * _K4W_to_VRM.legRight;
    
            _animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).rotation = KneeLeft * _K4W_to_VRM.legLeft;
            _animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).rotation = AnkleLeft * _K4W_to_VRM.legLeft;

            // モデルの回転を設定する
            _animator.gameObject.transform.rotation = q;

            // モデルの位置を移動する
            var pos = body.Joints[JointType.SpineBase].Position;
            _animator.gameObject.transform.position = new Vector3( -pos.X, pos.Y, -pos.Z );
        }

        public void SetAnimator(Animator animator)
        {
            _animator = animator;
        }
    }
}
