using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid.Common;
using Veldrid.Common.Tools;
using Veldrid.Sdl2;

namespace Veldrid.Common
{
    public class Camera
    {
        private float _fov = 1f;
        private float _near = 0.1f;
        private float _far = 100f;
        private float _xangle = 0;
        private float _yangle = 0;
        private float _zangle = 0;

        private Matrix4x4 _viewMatrix;
        private Matrix4x4 _projectionMatrix;
        private Matrix4x4 _orthographicMatrix;
        private Matrix4x4 _ymirror = new Matrix4x4(1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        private Vector3 _position = new Vector3(0, 0, 5);
        private Vector3 _lookDirection = new Vector3(0, 0, 0);
        private float _moveSpeed = 1.0f;

        private float _yaw;
        private float _pitch;
        private float _roll;

        private Vector2 _mousePressedPos;
        private bool _mousePressed = false;
        private GraphicsDevice _gd;
        private bool _useReverseDepth;
        private float _windowWidth;
        private float _windowHeight;
        private Sdl2Window _window;
        private InputTracker inputTracker;
        private Matrix4x4 _modelMarix;
        public Matrix4x4 OrthographicMatrix => _orthographicMatrix;
        public event Action<Matrix4x4> ProjectionChanged;
        public event Action<Matrix4x4> ViewChanged;
        public event Action<Matrix4x4> ModelChanged;
        public float XAngle { get => _xangle;set { _xangle = value;UpdateModel(); } }

        public float YAngle { get => _yangle; set { _yangle = value; UpdateModel(); } }

        public float ZAngle { get => _zangle; set { _zangle = value; UpdateModel(); } }
        private void UpdateModel()
        {
            _modelMarix = Matrix4x4.CreateRotationZ(_zangle) * Matrix4x4.CreateRotationY(_yangle) * Matrix4x4.CreateRotationX(_xangle);
            ModelChanged?.Invoke(_modelMarix);
        }

        public Camera(GraphicsDevice gd, Sdl2Window window)
        {

            _modelMarix = Matrix4x4.Identity;
            _gd = gd;
            _useReverseDepth = gd.IsDepthRangeZeroToOne;
            _window = window;
            _windowWidth = window.Width;
            _windowHeight = window.Height;
            UpdatePerspectiveMatrix();
            UpdateOrthographicMatrix();
            UpdateViewMatrix();
            inputTracker= new InputTracker();
        }

        public Matrix4x4 ModelMatrix => _modelMarix;
        public Matrix4x4 ViewMatrix => _viewMatrix;
        public Matrix4x4 ProjectionMatrix => _projectionMatrix;

        public Vector3 Position { get => _position; set { _position = value; UpdateViewMatrix(); } }
        public Vector3 LookDirection => _lookDirection;

        public float FarDistance => _far;

        public float FieldOfView => _fov;
        public float NearDistance => _near;

        public float AspectRatio => _windowWidth / _windowHeight;

        public float Yaw { get => _yaw; set { _yaw = value; UpdateViewMatrix(); } }
        public float Pitch { get => _pitch; set { _pitch = value; UpdateViewMatrix(); } }
        public float Roll { get => _roll;set{ _roll = value;UpdateViewMatrix(); } }


        public void Update(float deltaSeconds,InputSnapshot inputSnapshot)
        {
            inputTracker.UpdateFrameInput(inputSnapshot);
            //float sprintFactor = inputTracker.GetKey(Key.ControlLeft)
            //    ? 0.1f
            //    : inputTracker.GetKey(Key.ShiftLeft)
            //        ? 2.5f
            //        : 1f;
            //Vector3 motionDir = Vector3.Zero;
            //if (inputTracker.GetKey(Key.A))
            //{
            //    motionDir += -Vector3.UnitX;
            //}
            //if (inputTracker.GetKey(Key.D))
            //{
            //    motionDir += Vector3.UnitX;
            //}
            //if (inputTracker.GetKey(Key.W))
            //{
            //    motionDir += -Vector3.UnitZ;
            //}
            //if (inputTracker.GetKey(Key.S))
            //{
            //    motionDir += Vector3.UnitZ;
            //}
            //if (inputTracker.GetKey(Key.Q))
            //{
            //    motionDir += -Vector3.UnitY;
            //}
            //if (inputTracker.GetKey(Key.E))
            //{
            //    motionDir += Vector3.UnitY;
            //}

            //if (motionDir != Vector3.Zero|| true)
            //{
            //    Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, Roll);
            //    motionDir = Vector3.Transform(motionDir, lookRotation);
            //    _position += motionDir * _moveSpeed * sprintFactor * deltaSeconds;
            //    UpdateViewMatrix();
            //}

            if ((inputTracker.GetMouseButton(MouseButton.Left)))
            {
                if (!_mousePressed)
                {
                    _mousePressed = true;
                    _mousePressedPos = inputTracker.MousePosition;
                    //Sdl2Native.SDL_ShowCursor(0);
                    //Sdl2Native.SDL_SetWindowGrab(_window.SdlWindowHandle, true); 
                }
                Vector2 mouseDelta = _mousePressedPos - inputTracker.MousePosition;
                //Sdl2Native.SDL_WarpMouseInWindow(_window.SdlWindowHandle, (int)_mousePressedPos.X, (int)_mousePressedPos.Y);
                _yangle -= mouseDelta.X * 0.01f;
                _xangle -= mouseDelta.Y * 0.01f;
                _mousePressedPos= inputTracker.MousePosition;
                UpdateModel();
            }
            else if(_mousePressed)
            {
               // Sdl2Native.SDL_WarpMouseInWindow(_window.SdlWindowHandle, (int)_mousePressedPos.X, (int)_mousePressedPos.Y);
                //Sdl2Native.SDL_SetWindowGrab(_window.SdlWindowHandle, false);
                //Sdl2Native.SDL_ShowCursor(1);
                _mousePressed = false;
            }
            
            if(inputSnapshot.WheelDelta!=0)
            {
                _fov -= inputSnapshot.WheelDelta * 0.05f;
                if (_fov <= 0) _fov = 0.05f;
                if (_fov >= MathF.PI) _fov = 3.14f;
                UpdatePerspectiveMatrix();
            }
            Pitch = Math.Clamp(Pitch, -1.55f, 1.55f);
            UpdateViewMatrix();
        }

        public void WindowResized(float width, float height)
        {
            _windowWidth = width;
            _windowHeight = height;
            UpdatePerspectiveMatrix();
            UpdateOrthographicMatrix();
        }

        private void UpdatePerspectiveMatrix()
        {
            //_projectionMatrix = Util.CreatePerspective(
            //    _gd,
            //    _useReverseDepth,
            //    _fov,
            //    _windowWidth / _windowHeight,
            //    _near,
            //    _far);
            _projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(_fov,_windowWidth/_windowHeight,_near,_far);
            ProjectionChanged?.Invoke(_projectionMatrix);
        }
        internal Matrix4x4 GetOrthographicMatrix(UInt32 width, UInt32 height) => Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, 0, -1);
        private void UpdateOrthographicMatrix()
        {
            _orthographicMatrix = Matrix4x4.CreateOrthographicOffCenter(0, _windowWidth, _windowHeight, 0, 0, -1);
        }

        internal Matrix4x4 GetLineMatrix(UInt32 windowwidth,UInt32 windowheight, Veldrid.Common.Padding margin, LineRange lineRange,Boolean yinverted = false)
        {
            float width = windowwidth - (margin.Left + margin.Right);
            float height = windowheight - (margin.Top + margin.Bottom);
            float hscale = width / lineRange.XLenght;
            float vscale = height / lineRange.YLenght;
            var tran = Matrix4x4.CreateTranslation(0, -1 + (height / windowheight - 1), 0);
            return Matrix4x4.CreateScale(hscale, vscale, 1, new Vector3(-1, 1, 1)) * tran * Matrix4x4.CreateTranslation(margin.Left * 2 / windowwidth, margin.Top * 2 / windowheight, 0) * (!yinverted ? _ymirror : Matrix4x4.Identity);


        }

        /// <summary>
        /// 根据<paramref name="margin"/>参数和<paramref name="lineRange"/>参数获取转换矩阵
        /// </summary>
        /// <param name="margin"></param>
        /// <param name="lineRange"></param>
        /// <returns></returns>
        internal Matrix4x4 GetLineMatrix(Veldrid.Common.Padding margin, LineRange lineRange)
        {
            float width = _windowWidth - (margin.Left + margin.Right);
            float height = _windowHeight - (margin.Top + margin.Bottom);
            float hscale = width / lineRange.XLenght;
            float vscale = height / lineRange.YLenght;
            var tran = Matrix4x4.CreateTranslation(0,-1+(height/_windowHeight-1), 0);
            return Matrix4x4.CreateScale(hscale, vscale, 1, new Vector3(-1, 1, 1)) * tran * Matrix4x4.CreateTranslation(margin.Left*2/_windowWidth,margin.Top*2/_windowHeight, 0) * (!_gd.IsClipSpaceYInverted? _ymirror:Matrix4x4.Identity);


        }

        private void UpdateViewMatrix()
        {

            Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, Roll);
            Vector3 lookDir = Vector3.Transform(-Vector3.UnitZ, lookRotation);
            _lookDirection = lookDir;
            _viewMatrix = Matrix4x4.CreateLookAt(_position, _position + _lookDirection, Vector3.UnitY);
            ViewChanged?.Invoke(_viewMatrix);
        }

        public CameraInfo GetCameraInfo() => new CameraInfo
        {
            CameraPosition_WorldSpace = _position,
            CameraLookDirection = _lookDirection
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraInfo
    {
        public Vector3 CameraPosition_WorldSpace;
        private float _padding1;
        public Vector3 CameraLookDirection;
        private float _padding2;
    }
}
