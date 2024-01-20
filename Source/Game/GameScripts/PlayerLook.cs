using FlaxEngine;

namespace Game
{
    /// <summary>
    /// PlayerLook Script.
    /// </summary>
    public class PlayerLook : Script
    {
        private Camera camera;
        private Actor playerActor;

        private float sensitivity = 10;
        private float rotationMultiplier = 10;

        private float _yaw;
        private float _pitch;

        private static Quaternion playerRotation;

        public override void OnStart()
        {
            Screen.CursorLock = CursorLockMode.Locked;
            Screen.CursorVisible = false;

            camera = Actor.GetChild<Camera>();
            playerActor = Actor;
            
            GameSession.Instance.localPlayer.Rotation = Actor.Orientation;
        }

        public override void OnUpdate()
        {
            Look();
        }

        private void Look()
        {
            var mouseX = PlayerInput.Instance.MouseX;
            var mouseY = PlayerInput.Instance.MouseY;

            _pitch = Mathf.Clamp(_pitch + mouseY, -88, 88);
            _yaw += mouseX;

            var camFactor = Mathf.Saturate(20f * Time.DeltaTime);
            camera.LocalOrientation = Quaternion.Lerp(camera.LocalOrientation, Quaternion.Euler(_pitch, 0, 0), camFactor);
            playerActor.Orientation = Quaternion.Lerp(playerActor.LocalOrientation, Quaternion.Euler(0, _yaw, 0), camFactor);
            playerRotation = playerActor.Orientation;
        }

        public static void SendRotationPacket(ref PlayerTransformPacket transformPacket)
        {
            transformPacket.rotation = playerRotation;
        }
    }
}