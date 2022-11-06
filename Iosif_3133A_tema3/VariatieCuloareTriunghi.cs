using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iosif_3133A_tema3
{
    class VariatieCuloareTriunghi : GameWindow
    {
        private float[,] coordonate;

        private float[] RGBA, AntRGBA;

        private const float MIN_VALUE = 0f;
        private const float MAX_VALUE = 1f;

        private float targetX = 0;
        private float targetY = 10;
        private float targetZ = 5;

        private float cameraX = 0;
        private float cameraY = 30;
        private float cameraZ = 60;

        private double angleXZ;
        private float UnghiRotatie = MathHelper.PiOver6 / 5;

        private float[,] culoareVarf;
        private float[] culoareTriunghi;
        private float[,] culoareVarfLaRandare;
        private int varfCurent = 0;
        private bool prelucrareVarfIndividual = false;

        private MouseState AntMouseState;
        private KeyboardState AntKeyboardState;
        Matrix4 camera;

        public VariatieCuloareTriunghi() : base(800, 600)
        {
            VSync = VSyncMode.On;

            string numeFisier = ConfigurationManager.AppSettings["numeFisier"];
            string locatieFisier = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            coordonate = new float[3, 3];
            using (StreamReader streamReader = new StreamReader(locatieFisier + "//" + numeFisier))
            {
                for (int i = 0; i < 3; i++)
                {
                    string linie = streamReader.ReadLine().Trim();

                    int j = 0;
                    foreach (string coordonata in linie.Split(' '))
                    {
                        coordonate[i, j++] = float.Parse(coordonata);
                    }
                }
            }

            getCameraXZAngle();

            RGBA = new float[4];
            AntRGBA = new float[4];
            culoareVarf = new float[3, 4];
            culoareTriunghi = new float[4];
            culoareVarfLaRandare = new float[3, 4];
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(Color.White);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Fastest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            double aspect_ratio = Width / (double)Height;

            Matrix4 perspectiva = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 250);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspectiva);

            camera = Matrix4.LookAt(cameraX, cameraY, cameraZ, targetX, targetY, targetZ, 0, 1, 0 /*daca ultimele 3 campuri sunt 0 nu se randeaza nimic*/);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref camera);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState CurrentKeyboardState = Keyboard.GetState();
            MouseState CurrentMouseState = Mouse.GetCursorState();


            if (AntKeyboardState[Key.I] && !CurrentKeyboardState[Key.I])
            {
                prelucrareVarfIndividual = !prelucrareVarfIndividual;
            }

            int i, j;

            if (prelucrareVarfIndividual)
            {
                if (CurrentKeyboardState[Key.Right] && !AntKeyboardState[Key.Right])
                {
                    varfCurent = (varfCurent + 1) % 3;
                }

                if (CurrentKeyboardState[Key.Left] && !AntKeyboardState[Key.Left])
                {
                    if (varfCurent == 0)
                    {
                        varfCurent = 3;
                    }

                    varfCurent--;
                }

                for (i = 0; i < 4; i++)
                {
                    RGBA[i] = culoareVarf[varfCurent, i];
                }
            }
            else
            {
                for (i = 0; i < 4; i++)
                {
                    RGBA[i] = culoareTriunghi[i];
                }
            }

            if (CurrentKeyboardState[Key.R])
            {
                if (CurrentKeyboardState[Key.Plus] && RGBA[0] < MAX_VALUE)
                {
                    RGBA[0] += 0.01f;
                }
                else if (CurrentKeyboardState[Key.Minus] && RGBA[0] > MIN_VALUE)
                {
                    RGBA[0] -= 0.01f;
                }
            }

            if (CurrentKeyboardState[Key.G])
            {
                if (CurrentKeyboardState[Key.Plus] && RGBA[1] < MAX_VALUE)
                {
                    RGBA[1] += 0.01f;
                }
                else if (CurrentKeyboardState[Key.Minus] && RGBA[1] > MIN_VALUE)
                {
                    RGBA[1] -= 0.01f;
                }
            }

            if (CurrentKeyboardState[Key.B])
            {
                if (CurrentKeyboardState[Key.Plus] && RGBA[2] < MAX_VALUE)
                {
                    RGBA[2] += 0.01f;
                }
                else if (CurrentKeyboardState[Key.Minus] && RGBA[2] > MIN_VALUE)
                {
                    RGBA[2] -= 0.01f;
                }
            }

            if (CurrentKeyboardState[Key.T])
            {
                if (CurrentKeyboardState[Key.Plus] && RGBA[3] < MAX_VALUE)
                {
                    RGBA[3] += 0.01f;
                }
                else if (CurrentKeyboardState[Key.Minus] && RGBA[3] > MIN_VALUE)
                {
                    RGBA[3] -= 0.01f;
                }
            }

            if (prelucrareVarfIndividual)
            {
                for (i = 0; i < 4; i++)
                {
                    culoareVarf[varfCurent, i] = RGBA[i];
                }

                for (i = 0; i < 3; i++)
                {
                    for (j = 0; j < 4; j++)
                    {
                        culoareVarfLaRandare[i, j] = culoareVarf[i, j];
                    }
                }
            }
            else
            {
                for (i = 0; i < 4; i++)
                {
                    culoareVarfLaRandare[0, i] = culoareVarfLaRandare[1, i] = culoareVarfLaRandare[2, i] = culoareTriunghi[i] = RGBA[i];
                }
            }


            if (CurrentMouseState[MouseButton.Left])
            {
                if (!(CurrentMouseState.X == AntMouseState.X))
                {
                    if (AntMouseState.X - CurrentMouseState.X > 0)
                    {
                        CameraRotation(-1);
                    }
                    else
                    {
                        CameraRotation(1);
                    }
                }
            }

            if (S_aModificatCuloareaUnuiVarf())
            {
                printRGBAValues();
            }

            AntMouseState = CurrentMouseState;
            AntKeyboardState = CurrentKeyboardState;
            RGBA.CopyTo(AntRGBA, 0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawTriangle();
            DrawAxes();
            DrawSquare();

            SwapBuffers();
        }

        /// <summary>
        /// Desenarea triunghi-ului folosind coordonatele citite din fisier
        /// </summary>
        private void DrawTriangle()
        {
            GL.Begin(PrimitiveType.TriangleStrip);

            for (int i = 0; i < 3; i++)
            {
                GL.Color4(culoareVarfLaRandare[i, 0], culoareVarfLaRandare[i, 1], culoareVarfLaRandare[i, 2], culoareVarfLaRandare[i, 3]);
                GL.Vertex3(coordonate[i, 0], coordonate[i, 1], coordonate[i, 2]);
            }

            GL.End();
        }

        /// <summary>
        /// Fie D(x,y,z) vectorul ce indica directia de privire a camerei. Aceasta functie obtine unghi-ul facut de axa Ox cu vectorul (x,z)-proiectia in planul XZ a vectorului directie.
        /// Incrementarea acestui unghi conduce la rotirea spre dreapta a camerei, in timp ce decrementarea acestui unghi are ca efect rotirea spre stanga a camerei
        /// </summary>
        private void getCameraXZAngle()
        {
            float x = targetX - cameraX;
            float z = targetZ - cameraZ;
            double LungIpotenuza = Math.Sqrt(x * x + z * z);

            double sin = z / LungIpotenuza;
            double cos = x / LungIpotenuza;

            double asin = Math.Asin(sin);
            double acos = Math.Acos(cos);

            if (asin >= 0)
            {
                angleXZ = acos;
            }
            else
            {
                angleXZ = -(acos);
            }
        }


        /// <summary>
        /// Roteste camera stanga sau dreapta imprejur.
        /// </summary>
        private void CameraRotation(int sens)
        {
            angleXZ = angleXZ + sens * UnghiRotatie;
            float x = targetX - cameraX;
            float z = targetZ - cameraZ;
            double LungIpotenuza = Math.Sqrt(x * x + z * z);

            x = (float)(LungIpotenuza * Math.Cos(angleXZ));
            z = (float)(LungIpotenuza * Math.Sin(angleXZ));

            targetX = x + cameraX;
            targetZ = z + cameraZ;

            camera = Matrix4.LookAt(cameraX, cameraY, cameraZ, targetX, targetY, targetZ, 0, 1, 0 /*daca ultimele 3 campuri sunt 0 nu se randeaza nimic*/);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref camera);
        }


        /// <summary>
        /// Desenarea axelor de coordonate pentru a putea verifica mai usor ca camera chiar se roteste imprejur.
        /// </summary>
        private void DrawAxes()
        {
            GL.LineWidth(5f);
            GL.Begin(PrimitiveType.LineStrip);
            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(20, 0, 0);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 20, 0);
            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 20);
            GL.End();
        }

        private void DrawSquare()
        {
            GL.Begin(PrimitiveType.Polygon);
            GL.Color3(Color.Blue);

            GL.Vertex3(3, 0, -10);
            GL.Vertex3(3, 10, -10);
            GL.Vertex3(13, 10, -10);
            GL.Vertex3(13, 0, -10);

            GL.End();
        }

        /// <summary>
        /// Afiseaza valorile RGBA pentru fiecare varf al triunghi-ului
        /// </summary>
        private void printRGBAValues()
        {

            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(String.Format("VERTEX[{0:D}]: R ={1,3:D}%, G ={2,3:D}%, B ={3,3:D}%, A ={4,3:D}%", i, (int)(culoareVarfLaRandare[i, 0] * 100), (int)(culoareVarfLaRandare[i, 1] * 100), (int)(culoareVarfLaRandare[i, 2] * 100), (int)(culoareVarfLaRandare[i, 3] * 100)));
            }

            Console.WriteLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true daca s-a modificat culoarea unui varf, altfel false</returns>
        private bool S_aModificatCuloareaUnuiVarf()
        {
            for (int i = 0; i < 4; i++)
            {
                if (RGBA[i] != AntRGBA[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
