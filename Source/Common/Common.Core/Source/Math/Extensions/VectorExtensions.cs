// using System.Numerics;

// namespace VoxelEngine.Core;

// /// <summary>
// /// Unity-like extension methods for System.Numerics.Vector3
// /// </summary>
// public static class Vector3Extensions
// {
//     // Distance
//     extension(Vector3 from)
//     {
//         public float Distance(Vector3 to)
//             => Vector3.Distance(from, to);

//         public float DistanceSquared(Vector3 to)
//             => Vector3.DistanceSquared(from, to);

//         public Vector3 MoveTowards(Vector3 target, float maxDistanceDelta)
//         {
//             Vector3 direction = target - from;
//             float distance = direction.Length();

//             if (distance <= maxDistanceDelta || distance == 0f)
//                 return target;

//             return from + direction / distance * maxDistanceDelta;
//         }

//         public Vector3 Normalized()
//         {
//             float length = from.Length();
//             return length > float.Epsilon ? from / length : Vector3.Zero;
//         }

//         public Vector3 Lerp(Vector3 b, float t)
//             => Vector3.Lerp(from, b, t);

//         public Vector3 LerpUnclamped(Vector3 b, float t)
//             => from + (b - from) * t;

//         public Vector3 ClampMagnitude(float maxLength)
//         {
//             float sqrMagnitude = from.LengthSquared();
//             if (sqrMagnitude > maxLength * maxLength)
//                 return from / MathF.Sqrt(sqrMagnitude) * maxLength;
//             return from;
//         }

//         public Vector3 Project(Vector3 onNormal)
//         {
//             float sqrMag = onNormal.LengthSquared();
//             if (sqrMag < float.Epsilon)
//                 return Vector3.Zero;
//             return onNormal * Vector3.Dot(from, onNormal) / sqrMag;
//         }

//         public Vector3 ProjectOnPlane(Vector3 planeNormal)
//         {
//             return from - from.Project(planeNormal);
//         }

//         public Vector3 Reflect(Vector3 normal)
//             => Vector3.Reflect(from, normal);

//         public float Angle(Vector3 to)
//         {
//             float denominator = MathF.Sqrt(from.LengthSquared() * to.LengthSquared());
//             if (denominator < 1e-15f)
//                 return 0f;

//             float dot = System.Math.Clamp(Vector3.Dot(from, to) / denominator, -1f, 1f);
//             return MathF.Acos(dot) * (180f / MathF.PI);
//         }

//         public float SignedAngle(Vector3 to, Vector3 axis)
//         {
//             float unsignedAngle = from.Angle(to);
//             float sign = MathF.Sign(Vector3.Dot(axis, Vector3.Cross(from, to)));
//             return unsignedAngle * sign;
//         }

//         public Vector3 WithX(float x)
//             => new Vector3(x, from.Y, from.Z);

//         public Vector3 WithY(float y)
//             => new Vector3(from.X, y, from.Z);

//         public Vector3 WithZ(float z)
//             => new Vector3(from.X, from.Y, z);

//         public Vector2 XY()
//             => new Vector2(from.X, from.Y);

//         public Vector2 XZ()
//             => new Vector2(from.X, from.Z);
//     }
// }

// /// <summary>
// /// Unity-like extension methods for System.Numerics.Vector2
// /// </summary>
// public static class Vector2Extensions
// {
//     extension(Vector2 from)
//     {
//         public float Distance(Vector2 to)
//             => Vector2.Distance(from, to);

//         public Vector2 MoveTowards(Vector2 target, float maxDistanceDelta)
//         {
//             Vector2 direction = target - from;
//             float distance = direction.Length();

//             if (distance <= maxDistanceDelta || distance == 0f)
//                 return target;

//             return from + direction / distance * maxDistanceDelta;
//         }

//         public Vector2 Normalized()
//         {
//             float length = from.Length();
//             return length > float.Epsilon ? from / length : Vector2.Zero;
//         }

//         public Vector2 Perpendicular()
//             => new Vector2(-from.Y, from.X);

//         public float Angle(Vector2 to)
//         {
//             float denominator = MathF.Sqrt(from.LengthSquared() * to.LengthSquared());
//             if (denominator < 1e-15f)
//                 return 0f;

//             float dot = System.Math.Clamp(Vector2.Dot(from, to) / denominator, -1f, 1f);
//             return MathF.Acos(dot) * (180f / MathF.PI);
//         }
//     }
// }

// /// <summary>
// /// Static helper methods for creating quaternions
// /// </summary>
// public static class QuaternionHelpers
// {
//     /// <summary>
//     /// Creates a rotation which rotates from fromDirection to toDirection
//     /// </summary>
//     public static Quaternion LookRotation(Vector3 forward, Vector3 up)
//     {
//         forward = Vector3.Normalize(forward);
//         Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
//         up = Vector3.Cross(forward, right);

//         float m00 = right.X, m01 = right.Y, m02 = right.Z;
//         float m10 = up.X, m11 = up.Y, m12 = up.Z;
//         float m20 = forward.X, m21 = forward.Y, m22 = forward.Z;

//         float trace = m00 + m11 + m22;
//         Quaternion qua = Quaternion.Identity;

//         if (trace > 0f)
//         {
//             float s = MathF.Sqrt(trace + 1f) * 2f;
//             qua = new Quaternion(
//                 (m21 - m12) / s,
//                 (m02 - m20) / s,
//                 (m10 - m01) / s,
//                 0.25f * s
//             );
//         }
//         else if (m00 > m11 && m00 > m22)
//         {
//             float s = MathF.Sqrt(1f + m00 - m11 - m22) * 2f;
//             qua = new Quaternion(
//                 0.25f * s,
//                 (m01 + m10) / s,
//                 (m02 + m20) / s,
//                 (m21 - m12) / s
//             );
//         }
//         else if (m11 > m22)
//         {
//             float s = MathF.Sqrt(1f + m11 - m00 - m22) * 2f;
//             qua = new Quaternion(
//                 (m01 + m10) / s,
//                 0.25f * s,
//                 (m12 + m21) / s,
//                 (m02 - m20) / s
//             );
//         }
//         else
//         {
//             float s = MathF.Sqrt(1f + m22 - m00 - m11) * 2f;
//             qua = new Quaternion(
//                 (m02 + m20) / s,
//                 (m12 + m21) / s,
//                 0.25f * s,
//                 (m10 - m01) / s
//             );
//         }

//         return qua;
//     }
// }

// /// <summary>
// /// Unity-like extension methods for System.Numerics.Quaternion
// /// </summary>
// public static class QuaternionExtensions
// {
//     extension(Quaternion q)
//     {
//         /// <summary>
//         /// Convert quaternion to Euler angles (in degrees)
//         /// </summary>
//         public Vector3 ToEulerAngles()
//         {
//             Vector3 angles = Vector3.Zero;

//             // Roll (x-axis rotation)
//             float sinr_cosp = 2f * (q.W * q.X + q.Y * q.Z);
//             float cosr_cosp = 1f - 2f * (q.X * q.X + q.Y * q.Y);
//             angles.X = MathF.Atan2(sinr_cosp, cosr_cosp);

//             // Pitch (y-axis rotation)
//             float sinp = 2f * (q.W * q.Y - q.Z * q.X);
//             if (MathF.Abs(sinp) >= 1f)
//                 angles.Y = MathF.CopySign(MathF.PI / 2f, sinp);
//             else
//                 angles.Y = MathF.Asin(sinp);

//             // Yaw (z-axis rotation)
//             float siny_cosp = 2f * (q.W * q.Z + q.X * q.Y);
//             float cosy_cosp = 1f - 2f * (q.Y * q.Y + q.Z * q.Z);
//             angles.Z = MathF.Atan2(siny_cosp, cosy_cosp);

//             return angles * (180f / MathF.PI);
//         }

//         /// <summary>
//         /// Returns the angle in degrees between two rotations
//         /// </summary>
//         public float Angle(Quaternion b)
//         {
//             float dot = System.Math.Clamp(Quaternion.Dot(q, b), -1f, 1f);
//             return MathF.Acos(dot) * 2f * (180f / MathF.PI);
//         }
//     }
// }

