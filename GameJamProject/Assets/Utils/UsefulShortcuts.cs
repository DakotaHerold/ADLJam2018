using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
static class UsefulShortcuts {
    [MenuItem("Tools/Clear Console %#c")] // CMD + SHIFT + C
    public static void ClearConsole() {
        Debug.ClearDeveloperConsole();
    }

    //[MenuItem("Tools/DAQ Device Status? %#d")] // CMD + SHIFT + C
    //public static void DAQDeviceStatus() {
    //    Debug.Log("Emotiv EPOC+ status: " + DAQDeviceConnectionManager.EmotivConnection);
    //    Debug.Log("PPG status: " + DAQDeviceConnectionManager.PPGConnection);
    //}
}
#endif
static class MatrixMath {
    public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return q;
    }
}