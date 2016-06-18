using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PlanetDemo
{
    public class Camera3D
    {
        public float CameraMovementSpeed, RotationRate;
        float yaw, pitch;

        GraphicsDeviceManager graphics;

        Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                if (value != position)
                {
                    position = value;
                    UpdateVP();
                }
            }
        }
        Matrix rotation;
        public Matrix Rotation
        {
            get { return rotation; }
            set
            {
                if(value != rotation)
                {
                    rotation = value;
                    UpdateVP();
                }
            }
        }

        Matrix view, projection;
        public Matrix View { get { return view; } }
        public Matrix Projection { get { return projection; } }
        
        public Camera3D(GraphicsDeviceManager gr)
        {
            position = Vector3.Zero;
            rotation = Matrix.Identity;

            graphics = gr;

            CameraMovementSpeed = 5;
            RotationRate = .5f;

            UpdateVP();
        }
        public Camera3D(GraphicsDeviceManager gr, Vector3 pos)
        {
            position = pos;
            rotation = Matrix.Identity;

            graphics = gr;

            UpdateVP();
        }
        public Camera3D(GraphicsDeviceManager gr, Vector3 pos, Matrix rot)
        {
            position = pos;
            rotation = rot;

            graphics = gr;

            UpdateVP();
        }

        void UpdateVP()
        {
            view = Matrix.CreateLookAt(Position, Position + Rotation.Forward, Rotation.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), graphics.GraphicsDevice.Viewport.AspectRatio, 0.1f, 500f);
        }

        public void Update(GameTime gameTime)
        {
            yaw -= InputManager.Instance.normalizedCursorDelta.X;
            if (yaw > MathHelper.Pi * 2)
                yaw = 0;
            pitch -= InputManager.Instance.normalizedCursorDelta.Y;
            if (pitch > MathHelper.Pi * 2)
                pitch = 0;


            Rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);

            if (InputManager.Instance.KeyDown(Keys.W))
                Position += Rotation.Forward * (float)gameTime.ElapsedGameTime.TotalSeconds * CameraMovementSpeed;
            if (InputManager.Instance.KeyDown(Keys.S))
                Position += Rotation.Backward * (float)gameTime.ElapsedGameTime.TotalSeconds * CameraMovementSpeed;
            if (InputManager.Instance.KeyDown(Keys.D))
                Position += Rotation.Right * (float)gameTime.ElapsedGameTime.TotalSeconds * CameraMovementSpeed;
            if (InputManager.Instance.KeyDown(Keys.A))
                Position += Rotation.Left * (float)gameTime.ElapsedGameTime.TotalSeconds * CameraMovementSpeed;
            if (InputManager.Instance.KeyDown(Keys.Space))
                Position += Rotation.Up * (float)gameTime.ElapsedGameTime.TotalSeconds * CameraMovementSpeed;
            if (InputManager.Instance.KeyDown(Keys.C))
                Position += Rotation.Down * (float)gameTime.ElapsedGameTime.TotalSeconds * CameraMovementSpeed;
        }
    }
}
