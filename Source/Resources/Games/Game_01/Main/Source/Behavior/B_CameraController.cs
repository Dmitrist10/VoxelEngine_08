// using System.Numerics;
// using VoxelEngine.Core;
// using VoxelEngine.Diagnostics;
// using VoxelEngine.Inputs;

// namespace CustomGame;

// /// <summary>
// /// Unity-style editor camera controller.
// /// WASD + QE movement, mouse look (right-click), sprint, scroll speed, projection toggle, zoom.
// /// </summary>
// internal class B_CameraController : Behavior, IUpdatable
// {
//     // ── Movement ────────────────────────────────────────────────────
//     private float baseSpeed = 5f;
//     private float currentSpeed = 5f;
//     private float minSpeed = 0.5f;
//     private float maxSpeed = 100f;
//     private float sprintMultiplier = 3f;
//     private float scrollSpeedStep = 0.5f;
//     private float smoothTime = 0.1f;

//     private Vector3 _smoothVelocity;

//     // ── Mouse Look ─────────────────────────────────────────────────
//     private float sensitivity = 0.003f;
//     private float _yaw;
//     private float _pitch;

//     // ── Zoom / FOV ─────────────────────────────────────────────────
//     private float fovScrollStep = 0.02f;
//     private float orthoScrollStep = 0.5f;
//     private float minFov = 0.15f; // ~8.5 degrees
//     private float maxFov = 2.0f; // ~115 degrees
//     private float minOrthoSize = 1f;
//     private float maxOrthoSize = 100f;

//     private const MouseButton movementBtn = MouseButton.Right;

//     private CameraProjectionType projecting = CameraProjectionType.Perspective;

//     private bool isFPS = false;

//     public B_CameraController()
//     {
//     }

//     public override void OnAwake()
//     {
//         base.OnAwake();
//         currentSpeed = baseSpeed;
//     }

//     public void OnUpdate()
//     {
//         HandleMovement();
//         HandleMouseLook();
//         HandleSpeedScroll();
//         HandleZoom();
//         HandleProjectionSwitch();
//         HandleClipTroughSwitch();
//         HandleFPSControllerSwitch();
//     }

//     private void HandleFPSControllerSwitch()
//     {
//         if (Input.GetKeyDown(Key.F))
//         {
//             isFPS = !isFPS;

//             ref C_BoxCollider boxCollider = ref Self.GetComponent<C_BoxCollider>();
//             Vector3 size = boxCollider.Size;

//             if (isFPS)
//             {
//                 Self.GetComponent<C_RigidBody>().IsKinematic = false;
//                 Self.GetComponent<C_RigidBody>().UseGravity = true;
//                 size.Y = 2f;
//             }
//             else
//             {
//                 Self.GetComponent<C_RigidBody>().IsKinematic = true;
//                 size.Y = 0.5f;
//             }

//             boxCollider.Size = size;
//             boxCollider.IsDirty = true;
//         }
//     }

//     private void HandleClipTroughSwitch()
//     {
//         if (Input.GetKeyDown(Key.C))
//         {
//             Self.GetComponent<C_RigidBody>().IsKinematic = !Self.GetComponent<C_RigidBody>().IsKinematic;
//         }
//     }

//     // ── WASD + QE Movement with Shift Sprint ───────────────────────
//     // Movement is driven through the rigid body's velocity so the physics
//     // solver handles collision resolution (prevents clipping through walls).
//     private void HandleMovement()
//     {
//         float h = Input.GetAxis("Horizontal");
//         float v = Input.GetAxis("Vertical");
//         float ud = Input.GetAxis("UP_DOWN");

//         // if (projecting == CameraProjectionType.Orthographic)
//         // {
//         //     float temp = ud;
//         //     ud = v;
//         //     v = temp;
//         // }

//         // Sprint
//         float speed = currentSpeed;
//         if (Input.GetKey(Key.RightShift) || Input.GetKey(Key.LeftShift))
//             speed *= sprintMultiplier;

//         // Build direction relative to camera orientation
//         Vector3 forward = Self.Forward;
//         Vector3 right = Self.Right;
//         Vector3 up = Vector3.UnitY;

//         Vector3 targetVelocity = (right * h + up * ud + forward * v) * speed;

//         // Smooth damp for fluid movement
//         _smoothVelocity = EMathe.LerpExp(_smoothVelocity, targetVelocity, smoothTime, Time.DeltaTime);

//         if (Self.HasComponent<C_RigidBody>())
//         {
//             ref C_RigidBody rb = ref Self.GetComponent<C_RigidBody>();
//             if (rb.IsKinematic)
//             {
//                 // Noclip mode: kinematic bodies are synced by position in EP_Physics.
//                 // Writing velocity has no effect here, so we move the transform directly.
//                 Self.Position += _smoothVelocity * Time.DeltaTime;
//             }
//             else
//             {
//                 // Collision mode: velocity-driven. Bepu resolves collisions and writes position back.
//                 rb.Velocity = _smoothVelocity;
//             }
//         }
//         else
//         {
//             Self.Position += _smoothVelocity * Time.DeltaTime;
//         }
//     }

//     // ── Right-Click Mouse Look ─────────────────────────────────────
//     private void HandleMouseLook()
//     {
//         if (!Input.GetMouseButton(movementBtn))
//             return;

//         _yaw -= Input.MouseDelta.X * sensitivity;
//         _pitch -= -Input.MouseDelta.Y * sensitivity;

//         // Clamp pitch to ~89 degrees to prevent gimbal flip
//         _pitch = System.Math.Clamp(_pitch, -1.55f, 1.55f);

//         Self.Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
//     }

//     // ── Scroll Wheel Speed Adjustment ──────────────────────────────
//     private void HandleSpeedScroll()
//     {
//         // Only adjust speed when right-click is NOT held (zoom uses scroll when looking)
//         // Adjust speed with Ctrl + Scroll
//         if (!Input.GetMouseButton(movementBtn))
//             return;

//         float scrollY = Input.MouseScrollDelta.Y;
//         if (System.MathF.Abs(scrollY) > 0.01f)
//         {
//             currentSpeed += scrollY * scrollSpeedStep;
//             currentSpeed = System.Math.Clamp(currentSpeed, minSpeed, maxSpeed);
//         }
//     }

//     // ── Zoom: FOV (Perspective) or OrthoSize (Orthographic) ────────
//     private void HandleZoom()
//     {
//         if (Input.GetMouseButton(movementBtn))
//             return;

//         float scrollY = Input.MouseScrollDelta.Y;
//         if (projecting == CameraProjectionType.Orthographic)
//         {
//             scrollY += Input.GetAxis("Vertical");
//         }
//         if (System.MathF.Abs(scrollY) < 0.01f)
//             return;

//         if (!Self.HasComponent<C_Camera>())
//             return;

//         ref var cam = ref Self.GetComponent<C_Camera>();

//         if (cam.CameraType == CameraProjectionType.Perspective)
//         {
//             cam.FieldOfView -= scrollY * fovScrollStep;
//             cam.FieldOfView = System.Math.Clamp(cam.FieldOfView, minFov, maxFov);
//         }
//         else
//         {
//             cam.OrthographicSize -= scrollY * orthoScrollStep;
//             cam.OrthographicSize = System.Math.Clamp(cam.OrthographicSize, minOrthoSize, maxOrthoSize);
//         }

//         cam.UpdateProjection();
//     }

//     // ── F1 = Perspective, F2 = Orthographic ────────────────────────
//     private void HandleProjectionSwitch()
//     {
//         if (!Self.HasComponent<C_Camera>())
//             return;

//         bool switchedToPerspective = Input.GetKeyDown(Key.F1);
//         bool switchedToOrtho = Input.GetKeyDown(Key.F2);

//         if (!switchedToPerspective && !switchedToOrtho)
//             return;

//         ref var cam = ref Self.GetComponent<C_Camera>();

//         if (switchedToPerspective && cam.CameraType != CameraProjectionType.Perspective)
//         {
//             cam.CameraType = CameraProjectionType.Perspective;
//             projecting = CameraProjectionType.Perspective;
//             cam.UpdateProjection();
//             Logger.Info("Camera → Perspective");
//         }
//         else if (switchedToOrtho && cam.CameraType != CameraProjectionType.Orthographic)
//         {
//             cam.CameraType = CameraProjectionType.Orthographic;
//             projecting = CameraProjectionType.Orthographic;
//             cam.UpdateProjection();
//             Logger.Info("Camera → Orthographic");
//         }
//     }

// }