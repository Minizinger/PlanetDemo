using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PlanetDemo
{
    public class Scene
    {
        ContentManager cnt;

        Camera3D camera;

        Planet planet;

        SpriteFont font;

        bool wireframe = false;

        public Scene(Camera3D cam)
        {
            camera = cam;
            camera.Position = new Vector3(10, -15, 10);
        }
        public void LoadContent(ContentManager Content)
        {
            cnt = Content;
            //sphere = new IcoSphere();
            //sphere.LoadContent(Content);
            planet = new Planet(Vector3.Zero, Quaternion.Identity, new Vector3(10));
            planet.LoadContent(Content);
            //camera.CameraMovementSpeed *= 10;

            font = Content.Load<SpriteFont>("test");
        }
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime)
        {
            camera.Update(gameTime);
            if (InputManager.Instance.KeyPressed(Keys.PageUp))
                planet.LevelOfSubdivision++;
            if (InputManager.Instance.KeyPressed(Keys.PageDown))
                planet.LevelOfSubdivision--;
            if (InputManager.Instance.KeyPressed(Keys.Z))
                wireframe = !wireframe;
            if (InputManager.Instance.KeyPressed(Keys.N))
                LoadContent(cnt);
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, GameTime gameTime)
        {
            //sphere.DrawSphere(camera, graphics);
            planet.Draw(camera, graphics, gameTime, wireframe);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "PageUP to increase sphere divisions \n PageDown to decrease it \n Z to turn wireframe on \n N to generate new planet \n Esc to exit application \n Currently drawing " + planet.VerticiesToDraw.Length / 3 + " polygons", new Vector2(0, 0), Color.White);
            spriteBatch.End();
        }
    }
}
