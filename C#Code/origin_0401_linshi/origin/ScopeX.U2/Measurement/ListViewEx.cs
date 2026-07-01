using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    internal class ListViewEx : Control
    {
        public enum ColorOrientation
        {
            Horizontal,
            Vertical,
        }
        public sealed class ColumnInfo
        {
            public ColumnInfo()
            {

            }
            public ColumnInfo(String text, Int32 width)
            {
                Text = text;
                Width = width;
            }

            public String Text { get; set; }
            public Int32 Width { get; set; }
            public Object Tag { get; set; }
            internal Int32 FixedWidth { get; set; }
        }
        private Color[] _BackColor = new Color[] { DefaultBackColor };
        private ListControl _ListControl;
        private Font _Font = DefaultFont;
        private Color[] _ForeColor = new Color[] { DefaultForeColor };
        private System.Drawing.Color _GridColor = Color.Gray;
        private Boolean _GridLine = true;
        private Int32 _GridWidth = 1;
        private Color _HeaderBackColor = DefaultBackColor;
        private Font _HeaderFont = DefaultFont;
        private Color _HeaderForeColor = Color.White;
        private Int32 _HeaderHeight = 30;
        private Int32 _ItemHeight = 30;
        private System.Drawing.Color _SelectedBackColor = Color.Blue;
        private System.Drawing.Color _SelectedForeColor = Color.White;
        private System.Windows.Forms.DataGridViewContentAlignment _TextAlignment = DataGridViewContentAlignment.MiddleLeft;
        private DataGridViewContentAlignment _HeaderTextAlignment = DataGridViewContentAlignment.MiddleLeft;
        private Header _Header;
        private Boolean _HeaderVisility = true;
        private ColorOrientation _Orientation;
        private Boolean _EnbleSelect = true;
        public ListViewEx()
        {
            _ListControl = new ListControl(this);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserMouse, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.StandardDoubleClick, false);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            _Scroll = new ScrollBar(this);
            _Header = new Header(this);
            Font = new Font("MiSans", Font.Size);
            HeaderFont = new Font("MiSans", HeaderFont.Size);
        }
        public void SetScrollPosition(Point point) => _ListControl.SetScrollPosition(point);
        public DataGridViewContentAlignment HeaderTextAlignment
        {
            get => _HeaderTextAlignment;
            set => PropertyChanged(ref _HeaderTextAlignment, value);
        }
        public Boolean EnbleSelect { get => _EnbleSelect; set => PropertyChanged(ref _EnbleSelect, value); }
        public ColorOrientation Orientation { get => _Orientation; set => PropertyChanged(ref _Orientation, value); }
        public List<ColumnInfo> Columns => _ListControl.Columns;
        public List<String[]> Items => _ListControl.Items;
        public Boolean HeaderVisility { get => _HeaderVisility; set => PropertyChanged(ref _HeaderVisility, value); }
        public new Color[] BackColor { get => _BackColor; set => PropertyChanged(ref _BackColor, value); }
        public override Rectangle DisplayRectangle => _ListControl.DisplayRectangle;
        public new Font Font { get => _Font; set => PropertyChanged(ref _Font, value); }
        public Font HeaderFont { get => _HeaderFont; set => PropertyChanged(ref _HeaderFont, value); }
        public new Color[] ForeColor
        {
            get => _ForeColor;
            set
            {
                PropertyChanged(ref _ForeColor, value);
            }
        }
        public Color GridColor
        {
            get => _GridColor;
            set => PropertyChanged(ref _GridColor, value);
        }

        public Boolean GridLine
        {
            get => _GridLine;
            set => PropertyChanged(ref _GridLine, value);
        }

        public Int32 GridWidth
        {
            get => _GridWidth;
            set => PropertyChanged(ref _GridWidth, value);
        }

        public System.Drawing.Color HeaderBackColor
        {
            get => _HeaderBackColor;
            set => PropertyChanged(ref _HeaderBackColor, value);
        }


        public System.Drawing.Color HeaderForeColor
        {
            get => _HeaderForeColor;
            set => PropertyChanged(ref _HeaderForeColor, value);
        }

        public Int32 HeaderHeight
        {
            get => HeaderVisility ? _HeaderHeight : 0;
            set => PropertyChanged(ref _HeaderHeight, value);
        }
        public Int32 ItemHeight
        {
            get => _ItemHeight;
            set => PropertyChanged(ref _ItemHeight, value);
        }

        public System.Drawing.Color SelectedBackColor
        {
            get => _SelectedBackColor;
            set => PropertyChanged(ref _SelectedBackColor, value);
        }

        public System.Drawing.Color SelectedForeColor
        {
            get => _SelectedForeColor;
            set => PropertyChanged(ref _SelectedForeColor, value);
        }

        private Int32 _SelectedIndex;

        public Int32 SelectedIndex
        {
            get => EnbleSelect ? _SelectedIndex : -1;
            set => PropertyChanged(ref _SelectedIndex, value);
        }

        public System.Windows.Forms.DataGridViewContentAlignment TextAlignment
        {
            get => _TextAlignment;
            set => PropertyChanged(ref _TextAlignment, value);
        }

        internal Point AutoScrollPosition => _ListControl.AutoScrollPosition;
        internal Rectangle DisplayTableRectangle => _ListControl.Bounds;
        internal Int32 ListHeight
        {
            get
            {
                Int32 tempheight = Height - HeaderHeight;
                if ((_Scroll.Position & ScrollBar.ScrollBarPosition.Top) == ScrollBar.ScrollBarPosition.Top)
                {
                    tempheight -= _Scroll.Height;
                }
                else
                {

                }

                if ((_Scroll.Position & ScrollBar.ScrollBarPosition.Bottom) == ScrollBar.ScrollBarPosition.Bottom)
                {
                    tempheight -= _Scroll.Height;
                }
                else
                {

                }

                if (tempheight <= 0)
                {
                    tempheight = 40;
                }
                else
                {

                }
                return tempheight;
            }
        }

        internal Int32 ListWidth
        {
            get
            {
                Int32 tempwidth = Width;
                if ((_Scroll.Position & ScrollBar.ScrollBarPosition.Left) == ScrollBar.ScrollBarPosition.Left)
                {
                    tempwidth -= _Scroll.Height;
                }
                else
                {

                }

                if ((_Scroll.Position & ScrollBar.ScrollBarPosition.Right) == ScrollBar.ScrollBarPosition.Right)
                {
                    tempwidth -= _Scroll.Height;
                }
                else
                {

                }
                if (tempwidth <= 0)
                {
                    tempwidth = 20;
                }
                else
                {

                }
                return tempwidth;
            }
        }

        private ScrollBar _Scroll;

        public override void Refresh()
        {
            if (this.Width == 0 || this.Height == 0)
            {
                return;
            }
            Size maxsize = _ListControl.GetMaxSize();
            ScrollBar.ScrollBarPosition position = ScrollBar.ScrollBarPosition.None;

            if (maxsize.Height > Height - HeaderHeight)
            {
                position |= ScrollBar.ScrollBarPosition.Right;
                _ListControl.Top = 0;
            }
            else
            {
                _ListControl.Top = 0;
            }


            _ListControl.CalcColnumWidth();
            maxsize = _ListControl.GetMaxSize();

            if (maxsize.Width > Width)
            {
                position |= ScrollBar.ScrollBarPosition.Bottom;
                _ListControl.Left = 0;
            }
            else
            {
                _ListControl.Left = 0;
            }


            _Scroll.Position = position;
            _Scroll.Refresh();
            if (HeaderVisility) _Header.Refresh();
            _ListControl.Refresh();
            this.Invalidate(false);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_ListControl.Bounds.Contains(e.Location))
            {
                _ListControl.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X - _ListControl.Left, e.Y - _ListControl.Top, e.Delta));
            }
            else
            {
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_ListControl.Bounds.Contains(e.Location))
            {
                _ListControl.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X - _ListControl.Left, e.Y - _ListControl.Top, e.Delta));
            }
            else
            {
            }
        }
        protected override void Dispose(Boolean disposing)
        {
            _Header?.Dispose();
            _ListControl?.Dispose();
            _Scroll?.Dispose();
            base.Dispose(disposing);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_ListControl.Bounds.Contains(e.Location))
            {
                _ListControl.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X - _ListControl.Left, e.Y - _ListControl.Top, e.Delta));
            }
            else
            {
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (_ListControl.Bounds.Contains(e.Location))
            {
                _ListControl.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, e.X - _ListControl.Left, e.Y - _ListControl.Top, e.Delta));
            }
            else
            {
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (HeaderVisility)
            {
                lock(_Header.BitmapLock)
                {
                    var header_image = _Header.GetBitmap();
                    e.Graphics.DrawImageUnscaled(header_image, AutoScrollPosition.X, 0);
                }
            }
            e.Graphics.SetClip(new Rectangle(new Point(_ListControl.Bounds.X, _ListControl.Bounds.Y + HeaderHeight), _ListControl.Bounds.Size));
            lock (_ListControl.BitmapLock)
            {
                var list_image = _ListControl.GetBitmap();
                e.Graphics.DrawImageUnscaled(list_image, AutoScrollPosition.X, AutoScrollPosition.Y + HeaderHeight, _ListControl.Width, _ListControl.Height);

            }
            e.Graphics.ResetClip();
            lock(_Scroll.BitmapLock)
            {
                var scroll_image = _Scroll.GetBitmap();
                e.Graphics.DrawImageUnscaled(scroll_image, new Point(0, HeaderHeight));
            }
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            _ListControl.OnPreviewKeyDown(e);
            base.OnPreviewKeyDown(e);
        }
        private void PropertyChanged<T>(ref T field, T value)
        {
            if (Object.Equals(field, value))
            {
            }
            else
            {
                field = value;
                _ListControl.Refresh();
                this.Invalidate(false);
            }
        }

        #region ListControl

        internal class ListControl : IDisposable
        {
            public Object BitmapLock = new Object();
            private Boolean _ActiveChangeWidth = false;
            private Bitmap _Bitmap;
            private Boolean _FirstInit = true;
            private Point _LastMouseDownPoint = new Point();
            private Int32 _Left;
            private Object _Locker = new Object();
            private ListViewEx _Owner;
            private Int32 _SelectedColnumindex = -1;
            private Int32 _Top;
            private Boolean disposedValue;

            public ListControl(ListViewEx owner)
            {
                _Owner = owner;
            }
            public event EventHandler<EventArgs> SelectedIndexChnaged;

            public Color[] BackColor => _Owner.BackColor;
            public ColorOrientation ColorOrientation => _Owner.Orientation;

            public Rectangle Bounds => new Rectangle(Left, Top, Width, Height);


            public void SetScrollPosition(Point point)
            {
                AutoScrollPosition = point;
            }

            public Cursor Cursor
            {
                set => _Owner.Cursor = value;
            }

            public Rectangle DisplayRectangle
            {
                get
                {
                    lock (_Locker)
                    {
                        Rectangle rectangle = new Rectangle();
                        rectangle.Width = Columns.Sum(x => x.Width + x.FixedWidth);



                        if (rectangle.Width == 0)
                        {
                            if (Width == 0)
                            {
                                rectangle.Width = 50;
                            }
                            else
                            {
                                rectangle.Width = Width;
                            }
                        }
                        else
                        {
                        }

                        rectangle.Height = Items.Count * ItemHeight;


                        if (rectangle.Height == 0)
                        {
                            if (Height == 0)
                            {
                                rectangle.Height = 50;
                            }
                            else
                            {
                                rectangle.Height = Height;
                            }
                        }
                        else
                        {
                        }
                        rectangle.Height = Math.Max(rectangle.Height, _Owner.ListHeight);

                        return rectangle;
                    }
                }
            }

            public Font Font => _Owner.Font;

            public Color[] ForeColor => _Owner.ForeColor;

            public System.Drawing.Color GridColor => _Owner.GridColor;

            public Boolean HeaderVisility => _Owner.HeaderVisility;
            public Boolean GridLine => _Owner.GridLine;

            public Int32 GridWidth => _Owner.GridWidth;

            public Int32 Height => _Owner.ListHeight;


            public Int32 ItemHeight => _Owner.ItemHeight;
            public Int32 Left
            {
                get { return _Left; }
                set { _Left = value; }
            }

            public Int32 PageItemCount => Height / ItemHeight;
            public Control Parent => _Owner;
            public System.Drawing.Color SelectedBackColor => _Owner.SelectedBackColor;
            public System.Drawing.Color SelectedForeColor => _Owner.SelectedForeColor;
            public Boolean EnbleSelect => _Owner.EnbleSelect;
            public Int32 SelectedIndex
            {
                get => _Owner.SelectedIndex;
                set => _Owner.SelectedIndex = value;
            }
            public DataGridViewContentAlignment TextAlignment => _Owner.TextAlignment;


            public Int32 Top
            {
                get { return _Top; }
                set { _Top = value; }
            }

            public Int32 WheelDelta => 10;

            public Int32 Width => _Owner.ListWidth;

            internal Point AutoScrollPosition { get; set; }

            public List<ColumnInfo> Columns { get; } = new List<ColumnInfo>();


            private Boolean IsDrawGrid => GridLine && GridWidth > 0 && GridColor != Color.Transparent;

            public List<String[]> Items { get; } = new List<String[]>();

            public Size GetMaxSize()
            {
                Size size = new Size();
                if (Columns == null || Columns.Count == 0)
                {
                }
                else
                {
                    size.Width = Columns.Sum(x => x.Width + x.FixedWidth);
                    size.Height = Items.Count * ItemHeight;
                }
                return size;
            }

            public void Init()
            {
                if (!_FirstInit)
                {
                    return;
                }
                else
                {
                    _FirstInit = false;
                }
            }

            public void Refresh()
            {
                DrawBitMap();
            }


            private void ScrollToItemIndex(Int32 itemindex)
            {
                if (itemindex < 0)
                {
                    AutoScrollPosition = new Point(AutoScrollPosition.X, 0);
                }
                else if (itemindex >= Items.Count)
                {
                    AutoScrollPosition = new Point(AutoScrollPosition.X, Height - DisplayRectangle.Height);
                }
                else
                {
                    Int32 yposition = (itemindex + 1) * ItemHeight + AutoScrollPosition.Y;
                    if (yposition >= Height)
                    {
                        AutoScrollPosition = new Point(AutoScrollPosition.X, Math.Clamp(Height - (itemindex + 1) * ItemHeight, Height - DisplayRectangle.Height, 0));
                    }
                    else if (yposition < 0)
                    {
                        AutoScrollPosition = new Point(AutoScrollPosition.X, Math.Clamp(Height - (itemindex + 1 + PageItemCount) * ItemHeight, Height - DisplayRectangle.Height, 0));
                    }
                    else
                    {

                    }
                }
            }
            internal void CalcColnumWidth()
            {
                if (Columns.Count == 0) return;
                if (Columns.Sum(x => x.Width) < _Owner.DisplayTableRectangle.Width)
                {
                    Columns[^1].FixedWidth = _Owner.DisplayTableRectangle.Width - (Columns.Sum(x => x.Width) - Columns[^1].Width) - Columns[^1].Width;
                }
                else
                {
                    Columns[^1].FixedWidth = 0;
                }

            }


            internal Bitmap GetBitmap()
            {
                lock (BitmapLock)
                {
                    if (_Bitmap == null || _Bitmap.Width != DisplayRectangle.Width || _Bitmap.Height != DisplayRectangle.Height)
                    {
                        _Bitmap?.Dispose();
                        _Bitmap = new Bitmap(DisplayRectangle.Width, DisplayRectangle.Height);
                    }
                    else
                    {
                    }
                    return _Bitmap;
                }
            }
            internal void OnMouseDown(MouseEventArgs e)
            {
                _LastMouseDownPoint = e.Location;
                lock (_Locker)
                {
                    List<Int32> xposition = new List<Int32>();
                    if (Columns.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        Columns.ForEach(x =>
                        {
                            if (xposition.Count == 0)
                            {
                                xposition.Add(x.Width + x.FixedWidth);
                            }
                            else
                            {
                                xposition.Add(xposition[^1] + x.Width + x.FixedWidth);
                            }
                        });
                    }
                    if (e.Button == MouseButtons.Left)
                    {
                        if (HeaderVisility && _Owner.HeaderHeight > 0 && e.Location.Y < _Owner.HeaderHeight)
                        {
                            var tempx = e.Location.X - AutoScrollPosition.X;
                            var colindex = xposition.FindIndex(x => tempx >= x - 10 && tempx <= x + 10);
                            if (xposition.Any(x => tempx >= x - 10 && tempx <= x + 10) && HeaderVisility)
                            {
                                _ActiveChangeWidth = HeaderVisility && colindex >= 0;
                                _SelectedColnumindex = colindex;
                            }
                            else Cursor = Cursors.Default;
                        }
                        else if (PositionToRowColnum(new Point(e.Location.X, e.Location.Y - _Owner.HeaderHeight), out Int32 rowindex, out Int32 colindex, out Boolean isheader) && !isheader)
                        {
                            SelectedIndex = rowindex;
                        }
                        else
                        {


                        }
                    }
                    else
                    {
                    }
                }
            }

            internal void OnMouseMove(MouseEventArgs e)
            {
                lock (_Locker)
                {
                    List<Int32> xposition = new List<Int32>();

                    if (Columns.Count == 0)
                    {
                        _LastMouseDownPoint = e.Location;
                        return;
                    }
                    else
                    {
                        Columns.ForEach(x =>
                        {
                            if (xposition.Count == 0)
                            {
                                xposition.Add(x.Width + x.FixedWidth);
                            }
                            else
                            {
                                xposition.Add(xposition[^1] + x.Width + x.FixedWidth);
                            }
                        });
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        if (_SelectedColnumindex >= 0 && HeaderVisility && _ActiveChangeWidth)
                        {
                            if (_SelectedColnumindex == 0)
                            {
                                Columns[_SelectedColnumindex].Width = e.Location.X - AutoScrollPosition.X;
                            }
                            else
                            {
                                Columns[_SelectedColnumindex].Width = e.Location.X - xposition[_SelectedColnumindex - 1] - AutoScrollPosition.X;
                            }
                            _Owner.Refresh();
                        }
                        else
                        {
                            Int32 posx = AutoScrollPosition.X + e.Location.X - _LastMouseDownPoint.X;
                            Int32 posy = AutoScrollPosition.Y + e.Location.Y - _LastMouseDownPoint.Y;
                            Int32 minwidth = Width - DisplayRectangle.Width;

                            if (minwidth > 0)
                            {
                                minwidth = 0;
                            }
                            else
                            {
                            }

                            Int32 minheight = Height - DisplayRectangle.Height;

                            if (minheight > 0)
                            {
                                minheight = 0;
                            }
                            else
                            {
                            }

                            AutoScrollPosition = new Point(Math.Clamp(posx, minwidth, 0), Math.Clamp(posy, minheight, 0));
                            _Owner.Invalidate(false);
                        }

                        _LastMouseDownPoint = e.Location;
                    }
                    else if (e.Button == MouseButtons.None)
                    {
                        if (HeaderVisility && _Owner.HeaderHeight > 0 && e.Location.Y < _Owner.HeaderHeight)
                        {
                            var tempx = e.Location.X - AutoScrollPosition.X;
                            if (xposition.Any(x => tempx >= x - 10 && tempx <= x + 10) && HeaderVisility)
                            {
                                Cursor = Cursors.VSplit;
                            }
                            else Cursor = Cursors.Default;
                        }
                        else if (PositionToRowColnum(new Point(e.Location.X, e.Location.Y - _Owner.HeaderHeight), out Int32 rowindex, out Int32 colindex, out Boolean isheader) && !isheader)
                        {
                            Cursor = Cursors.Hand;
                        }
                        else
                        {

                        }
                    }
                }
            }

            internal void OnMouseUp(MouseEventArgs e)
            {
                _ActiveChangeWidth = false;
                _SelectedColnumindex = -1;
                Cursor = Cursors.Default;
            }

            internal void OnMouseWheel(MouseEventArgs e)
            {

                Int32 minheight = Height - DisplayRectangle.Height;

                if (minheight > 0)
                {
                    minheight = 0;
                }
                else
                {
                }

                AutoScrollPosition = new Point(AutoScrollPosition.X, Math.Clamp(AutoScrollPosition.Y + (Int32)Math.CopySign(WheelDelta, e.Delta), minheight, 0));
                _Owner.Invalidate(false);
            }

            internal void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
            {


                switch (e.KeyCode)
                {
                    case Keys.Up:
                        if (SelectedIndex > 0)
                        {
                            SelectedIndex--;
                            ScrollToItemIndex(SelectedIndex);
                        }
                        else
                        {

                        }
                        break;
                    case Keys.Down:
                        SelectedIndex++;
                        ScrollToItemIndex(SelectedIndex);
                        break;
                    case Keys.PageDown:
                        SelectedIndex += PageItemCount;
                        ScrollToItemIndex(SelectedIndex);
                        break;
                    case Keys.PageUp:
                        {
                            Int32 tempindex = SelectedIndex - PageItemCount;

                            if (tempindex < 0)
                            {
                                tempindex = 0;
                            }
                            else
                            {

                            }
                            SelectedIndex = tempindex;
                            ScrollToItemIndex(SelectedIndex);
                        }
                        break;
                }
            }

            private void DrawBitMap()
            {
                Int32 offetx = 0;
                Int32 offety = 0;
                Int32 tempheight = DisplayRectangle.Height;
                lock (BitmapLock)
                {
                    var image = GetBitmap();
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        graphics.Clear(Color.Transparent);
                        if (Columns.Count == 0)
                        {
                            graphics.Dispose();
                            return;
                        }
                        else
                        {
                        }
                        if (BackColor.Length == 1)
                        {
                            graphics.FillRectangle(new SolidBrush(BackColor[0]), new Rectangle(offetx, offety, DisplayRectangle.Width, tempheight));
                        }
                        else if (BackColor.Length > 1)
                        {
                            if (ColorOrientation == ColorOrientation.Horizontal)
                            {
                                Int32 tempwidth = offetx;
                                for (Int32 index = 0; index < Columns.Count; index++)
                                {
                                    graphics.FillRectangle(new SolidBrush(BackColor[index % BackColor.Length]), new Rectangle(tempwidth, offety, Columns[index].Width + Columns[index].FixedWidth, tempheight));
                                    tempwidth += Columns[index].FixedWidth + Columns[index].Width;
                                }
                            }
                            else
                            {
                                for (Int32 index = 0; index < Items.Count; index++)
                                {
                                    graphics.FillRectangle(new SolidBrush(BackColor[index % BackColor.Length]), new Rectangle(offetx, offety + index * ItemHeight, DisplayRectangle.Width, ItemHeight));
                                }
                            }
                        }
                        if (SelectedIndex >= 0 && Items.Count > 0 && SelectedIndex < Items.Count && EnbleSelect)
                        {
                            graphics.FillRectangle(new SolidBrush(SelectedBackColor), new Rectangle(offetx, SelectedIndex * ItemHeight + offety, DisplayRectangle.Width, ItemHeight));
                        }
                        else
                        {
                        }

                        if (IsDrawGrid)
                        {
                            System.Drawing.Drawing2D.GraphicsPath gridpath = new System.Drawing.Drawing2D.GraphicsPath();
                            gridpath.StartFigure();
                            gridpath.AddRectangle(new Rectangle(DisplayRectangle.X, DisplayRectangle.Y, DisplayRectangle.Width - GridWidth, tempheight - GridWidth));
                            gridpath.CloseFigure();
                            gridpath.StartFigure();
                            gridpath.AddLine(new Point(offetx, offety), new Point(offetx + DisplayRectangle.Width, offety));
                            gridpath.CloseFigure();
                            Int32 temp = 0;
                            Int32 count = (DisplayRectangle.Height) / ItemHeight;
                            for (Int32 index = 0; index < count - 1; index++)
                            {
                                gridpath.StartFigure();
                                gridpath.AddLine(new Point(offetx, offety + (index + 1) * ItemHeight), new Point(offetx + DisplayRectangle.Width, offety + (index + 1) * ItemHeight));
                                gridpath.CloseFigure();
                            }

                            for (Int32 index = 0; index < Columns.Count - 1; index++)
                            {
                                gridpath.StartFigure();
                                gridpath.AddLine(new Point(offetx + Columns[index].Width + temp + Columns[index].FixedWidth, offety), new Point(offetx + Columns[index].Width + temp + Columns[index].FixedWidth, offety + tempheight));
                                gridpath.CloseFigure();
                                temp += Columns[index].Width + Columns[index].FixedWidth;
                            }

                            Pen gridpen = new Pen(GridColor, GridWidth);
                            graphics.DrawPath(gridpen, gridpath);
                        }
                        else
                        {
                        }

                        Int32 tempx = 0;
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


                        for (Int32 index = 0; index < Items.Count; index++)
                        {
                            tempx = 0;

                            String[] tempstr = Items[index];

                            if (tempstr == null)
                            {
                                tempstr = new String[Columns.Count];
                            }
                            else
                            {
                            }
                            tempstr = tempstr.Take(Columns.Count).ToArray();

                            for (Int32 valueindex = 0; valueindex < Columns.Count; valueindex++)
                            {
                                Color tempcolor = Color.Empty;
                                if (ColorOrientation == ColorOrientation.Horizontal)
                                {
                                    tempcolor = ForeColor[valueindex % ForeColor.Length];
                                }
                                else
                                {
                                    tempcolor = ForeColor[index % ForeColor.Length];
                                }

                                graphics.DrawString(tempstr[valueindex], Font, new SolidBrush(tempcolor), new Rectangle()
                                {
                                    X = tempx + offetx,
                                    Y = index * ItemHeight + offety,
                                    Width = Columns[valueindex].Width + Columns[valueindex].FixedWidth,
                                    Height = ItemHeight,
                                }, GetTextFormat());

                                tempx += Columns[valueindex].Width + Columns[valueindex].FixedWidth;
                            }
                        }
                    }
                }
            }

            private StringFormat GetTextFormat()
            {
                StringFormat format = StringFormat.GenericDefault;
                format.FormatFlags = StringFormatFlags.NoWrap;
                switch (TextAlignment)
                {
                    case DataGridViewContentAlignment.NotSet:
                        break;

                    case DataGridViewContentAlignment.TopLeft:
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Near;
                        break;

                    case DataGridViewContentAlignment.TopCenter:
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Near;
                        break;

                    case DataGridViewContentAlignment.TopRight:
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Near;
                        break;

                    case DataGridViewContentAlignment.MiddleLeft:
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Center;
                        break;

                    case DataGridViewContentAlignment.MiddleCenter:
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        break;

                    case DataGridViewContentAlignment.MiddleRight:
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Center;
                        break;

                    case DataGridViewContentAlignment.BottomLeft:
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Far;
                        break;

                    case DataGridViewContentAlignment.BottomCenter:
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Far;
                        break;

                    case DataGridViewContentAlignment.BottomRight:
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Far;
                        break;
                }
                return format;
            }

            private Point PointToClient(Point point)
            {
                Point temppoint = _Owner.PointToClient(point);
                temppoint.X -= Left;
                temppoint.Y -= Top;
                return temppoint;
            }

            private Boolean PositionToRowColnum(Point point, out Int32 rowindex, out Int32 colnumindex, out Boolean isheader)
            {
                Int32 yposition = point.Y - AutoScrollPosition.Y;
                Int32 xposition = point.X - AutoScrollPosition.X;
                rowindex = -1;
                colnumindex = -1;
                isheader = false;

                if (yposition < 0 || yposition > DisplayRectangle.Height || xposition < 0 || xposition > DisplayRectangle.Width)
                {
                    return false;
                }
                else
                {
                }

                if (yposition < _Owner.HeaderHeight)
                {
                    isheader = true;
                }
                else
                {
                }

                rowindex = yposition / ItemHeight;
                Int32 temp = 0;

                for (Int32 index = 0; index < Columns.Count; index++)
                {
                    if (xposition > temp && xposition < temp + Columns[index].Width)
                    {
                        colnumindex = index;
                        return true;
                    }
                    else
                    {
                    }

                    temp += Columns[index].Width;
                }
                return false;
            }


            protected virtual void Dispose(Boolean disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        lock (BitmapLock)
                        {
                            _Bitmap?.Dispose();
                            _Bitmap = null; 
                        }
                        // TODO: 释放托管状态(托管对象)
                    }

                    // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                    // TODO: 将大型字段设置为 null
                    disposedValue = true;
                }
            }

            // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
            // ~ListControl()
            // {
            //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        #endregion EventListControl


        private sealed class Header : IDisposable
        {
            public Object BitmapLock = new Object();
            private Bitmap _Bitmap;
            private ListViewEx _Owner;
            private Boolean disposedValue;

            public Header(ListViewEx owner)
            {
                _Owner = owner;
            }
            public Bitmap GetBitmap()
            {
                lock (BitmapLock)
                {
                    Int32 width = _Owner.Columns.Sum(x => x.Width + x.FixedWidth);
                    if (width == 0)
                    {
                        width = 1;
                    }
                    if (_Bitmap == null || _Bitmap.Width != width || _Bitmap.Height != _Owner.HeaderHeight)
                    {
                        _Bitmap?.Dispose();
                        _Bitmap = new Bitmap(width, _Owner.HeaderHeight);
                    }
                    return _Bitmap; 
                }
            }

            private StringFormat GetTextFormat()
            {
                StringFormat format = StringFormat.GenericDefault;
                format.FormatFlags = StringFormatFlags.NoWrap;
                switch (_Owner.HeaderTextAlignment)
                {
                    case DataGridViewContentAlignment.NotSet:
                        break;

                    case DataGridViewContentAlignment.TopLeft:
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Near;
                        break;

                    case DataGridViewContentAlignment.TopCenter:
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Near;
                        break;

                    case DataGridViewContentAlignment.TopRight:
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Near;
                        break;

                    case DataGridViewContentAlignment.MiddleLeft:
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Center;
                        break;

                    case DataGridViewContentAlignment.MiddleCenter:
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        break;

                    case DataGridViewContentAlignment.MiddleRight:
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Center;
                        break;

                    case DataGridViewContentAlignment.BottomLeft:
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Far;
                        break;

                    case DataGridViewContentAlignment.BottomCenter:
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Far;
                        break;

                    case DataGridViewContentAlignment.BottomRight:
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Far;
                        break;
                }
                return format;
            }

            public void Refresh()
            {
                lock (BitmapLock)
                {
                    Bitmap bitmap = GetBitmap();
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CompositingQuality = CompositingQuality.HighSpeed;
                        g.SmoothingMode = SmoothingMode.HighSpeed;
                        g.Clear(_Owner.HeaderBackColor);

                        if (_Owner.GridLine && _Owner.GridColor.A > 0)
                        {
                            g.DrawRectangle(new Pen(_Owner.GridColor, 1), new Rectangle(new Point(), bitmap.Size));
                        }
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        Int32 x = 0;
                        Int32 tempwidth = 0;
                        var format = GetTextFormat();
                        for (Int32 index = 0; index < _Owner.Columns.Count; index++)
                        {
                            tempwidth = _Owner.Columns[index].Width + _Owner.Columns[index].FixedWidth;
                            if (_Owner.GridLine && _Owner.GridColor.A > 0 && index < _Owner.Columns.Count - 1)
                            {
                                g.DrawLine(new Pen(_Owner.GridColor, 1), new Point(x + tempwidth, 0), new Point(x + tempwidth, _Owner.HeaderHeight));
                            }
                            if (!String.IsNullOrEmpty(_Owner.Columns[index].Text))
                            {
                                g.DrawString(_Owner.Columns[index].Text, _Owner.HeaderFont, new SolidBrush(_Owner.HeaderForeColor), new Rectangle(new Point(x, 0), new Size(tempwidth, _Owner.HeaderHeight)), format);
                            }
                            x += tempwidth;
                        }
                        g.Save();
                    } 
                }
            }

            private void Dispose(Boolean disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        lock (BitmapLock)
                        {
                            _Bitmap?.Dispose();
                            _Bitmap = null; 
                        }
                        // TODO: 释放托管状态(托管对象)
                    }

                    // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                    // TODO: 将大型字段设置为 null
                    disposedValue = true;
                }
            }

            // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
            // ~Header()
            // {
            //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        #region ScrollBar

        private class ScrollBar : IDisposable
        {
            public Object BitmapLock = new Object();
            private Bitmap _Bitmap;
            private ListViewEx _Owner;
            private Boolean disposedValue;

            public ScrollBar(ListViewEx owner)
            {
                if (owner == null)
                {
                    throw new ArgumentNullException(nameof(owner));
                }
                else
                {
                    _Owner = owner;
                }
            }

            public enum ScrollBarPosition
            {
                None,
                Left = 1,
                Top = 2,
                Right = 4,
                Bottom = 8,
                All = Left | Top | Right | Bottom,
            }

            public Color BackColor { get; set; } = Color.Transparent;
            public Int32 CornerRadius { get; set; } = 4;
            public Color ForeColor { get; set; } = Color.FromArgb(120, Color.Gray);
            public Int32 Height { get; set; } = 14;
            public ScrollBarPosition Position { get; set; } = ScrollBarPosition.All;
            public Int32 ScroolBarHeight { get; set; } = 10;

            public Bitmap GetBitmap()
            {
                lock (BitmapLock)
                {
                    Int32 width = _Owner.Width;
                    if (width == 0)
                    {
                        width = 1;
                    }
                    if (_Bitmap == null || _Bitmap.Width != _Owner.Width || _Bitmap.Height != _Owner.ListHeight)
                    {
                        _Bitmap?.Dispose();
                        _Bitmap = new Bitmap(width, _Owner.ListHeight);
                    }
                    return _Bitmap;
                }
            }

            public void Refresh()
            {
                DrawScrollBar();
            }

            private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, Int32 cornerRadius)
            {
                GraphicsPath roundedRect = new GraphicsPath();
                roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
                roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
                roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
                roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
                roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
                roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
                roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
                roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
                roundedRect.CloseFigure();
                return roundedRect;
            }

            private void DrawScrollBar()
            {
                lock (BitmapLock)
                {
                    var image = GetBitmap();
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.Clear(Color.Transparent);
                        GraphicsPath backpath = new GraphicsPath();
                        GraphicsPath scrollpath = new GraphicsPath();

                        Int32 tempposition = 1;
                        for (Int32 index = 0; index < 4; index++)
                        {
                            ScrollBarPosition scrollposition = (ScrollBarPosition)tempposition;
                            if (((scrollposition & Position) == scrollposition) && GetScrollBarPoints(scrollposition, out Rectangle rec, out GraphicsPath path))
                            {
                                backpath.StartFigure();
                                backpath.AddRectangle(rec);
                                backpath.CloseFigure();
                                scrollpath.StartFigure();
                                scrollpath.AddPath(path, true);
                                scrollpath.CloseFigure();
                            }
                            tempposition <<= 1;
                        }
                        graphics.FillPath(new SolidBrush(BackColor), backpath);
                        graphics.FillPath(new SolidBrush(ForeColor), scrollpath);
                    } 
                }
                //graphics.Dispose();
            }

            private Boolean GetScrollBarPoints(ScrollBarPosition position, out Rectangle backrec, out GraphicsPath scroolBarPath)
            {
                Boolean result = false;
                backrec = default;
                scroolBarPath = new GraphicsPath();

                Rectangle scrollrec = new Rectangle();
                switch (position)
                {
                    case ScrollBarPosition.Left:
                        backrec = new Rectangle(0, 0, Height, _Owner.ListHeight);
                        scrollrec.Height = _Owner.DisplayTableRectangle.Height * _Owner.DisplayTableRectangle.Height / _Owner.DisplayRectangle.Height;
                        scrollrec.Width = ScroolBarHeight;
                        scrollrec.X = backrec.X + (Height - ScroolBarHeight) / 2;
                        scrollrec.Y = (Int32)(Math.Abs(_Owner.AutoScrollPosition.Y) / (Double)_Owner.DisplayRectangle.Height * _Owner.DisplayTableRectangle.Height) + _Owner.DisplayTableRectangle.Y;
                        break;

                    case ScrollBarPosition.Right:
                        backrec = new Rectangle(_Owner.Width - Height, 0, Height, _Owner.ListHeight);
                        scrollrec.Height = _Owner.DisplayTableRectangle.Height * _Owner.DisplayTableRectangle.Height / _Owner.DisplayRectangle.Height;
                        scrollrec.Width = ScroolBarHeight;
                        scrollrec.X = backrec.X + (Height - ScroolBarHeight) / 2;
                        scrollrec.Y = (Int32)(Math.Abs(_Owner.AutoScrollPosition.Y) / (Double)_Owner.DisplayRectangle.Height * _Owner.DisplayTableRectangle.Height) + _Owner.DisplayTableRectangle.Y;
                        break;

                    case ScrollBarPosition.Top:
                        backrec = new Rectangle(0, 0, _Owner.Width, Height);
                        scrollrec.Width = _Owner.DisplayTableRectangle.Width * _Owner.DisplayTableRectangle.Width / _Owner.DisplayRectangle.Width;
                        scrollrec.Height = ScroolBarHeight;
                        scrollrec.Y = backrec.Y + (Height - ScroolBarHeight) / 2;
                        scrollrec.X = (Int32)(Math.Abs(_Owner.AutoScrollPosition.X) / (Double)_Owner.DisplayRectangle.Width * _Owner.DisplayTableRectangle.Width) + _Owner.DisplayTableRectangle.X;
                        break;

                    case ScrollBarPosition.Bottom:
                        backrec = new Rectangle(0, _Owner.ListHeight - Height, _Owner.Width, Height);
                        scrollrec.Width = _Owner.DisplayTableRectangle.Width * _Owner.DisplayTableRectangle.Width / _Owner.DisplayRectangle.Width;
                        scrollrec.Height = ScroolBarHeight;
                        scrollrec.Y = backrec.Y + (Height - ScroolBarHeight) / 2;
                        scrollrec.X = (Int32)(Math.Abs(_Owner.AutoScrollPosition.X) / (Double)_Owner.DisplayRectangle.Width * _Owner.DisplayTableRectangle.Width) + _Owner.DisplayTableRectangle.X;
                        break;
                }

                if (CornerRadius <= 0 || ScroolBarHeight <= 2)
                {
                    scroolBarPath.StartFigure();
                    scroolBarPath.AddPolygon(new Point[4]
                    {
                        new Point(scrollrec.X,scrollrec.Y),
                        new Point(scrollrec.X+scrollrec.Width,scrollrec.Y),
                        new Point(scrollrec.X+scrollrec.Width,scrollrec.Y+scrollrec.Height),
                        new Point(scrollrec.X,scrollrec.Y+scrollrec.Height),
                    });
                    scroolBarPath.CloseFigure();
                    result = true;
                }
                else
                {
                    if (CornerRadius <= ScroolBarHeight / 2)
                    {
                        scroolBarPath.StartFigure();
                        scroolBarPath.AddPath(CreateRoundedRectanglePath(scrollrec, CornerRadius), false);
                        scroolBarPath.CloseFigure();
                        result = true;
                    }
                    else
                    {
                    }
                }

                return result;
            }

            protected virtual void Dispose(Boolean disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        lock(BitmapLock)
                        {
                            _Bitmap?.Dispose();
                            _Bitmap = null;
                        }
                        // TODO: 释放托管状态(托管对象)
                    }

                    // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                    // TODO: 将大型字段设置为 null
                    disposedValue = true;
                }
            }

            // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
            // ~ScrollBar()
            // {
            //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        #endregion ScrollBar
    }


}
