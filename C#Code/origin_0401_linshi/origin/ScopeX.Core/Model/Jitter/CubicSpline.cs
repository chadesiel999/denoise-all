using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.U2
{
    public class CubicSplineCodeGeex
    {
        public static void Main(String[] args)
        {
            List<Double> xValues = new List<Double> { 0, 1, 2, 3, 4 };
            List<Double> yValues = new List<Double> { 0, 1, 4, 9, 16 };

            Double xNew = 2.5;

            Double interpolatedY = CubicSplineInterpolation(xValues, yValues, xNew);

            Console.WriteLine($"The interpolated value at x = {xNew} is: {interpolatedY}");
        }

        public static Double CubicSplineInterpolation(List<Double> xValues, List<Double> yValues, Double xNew)
        {
            if (xValues.Count != yValues.Count)
            {
                throw new ArgumentException("The number of xValues and yValues must be the same.");
            }

            Int32 n = xValues.Count;
            Double[] a = new Double[n - 1];
            Double[] b = new Double[n];
            Double[] c = new Double[n];
            Double[] d = new Double[n];

            for (Int32 i = 0; i < n - 1; i++)
            {
                a[i] = (yValues[i + 1] - yValues[i]) / (xValues[i + 1] - xValues[i]);
                b[i] = 2 * (xValues[i + 1] - xValues[i]) / (xValues[i + 1] - xValues[i - 1]) - (yValues[i + 1] - yValues[i - 1]) / (xValues[i + 1] - xValues[i]);
                c[i] = (yValues[i] - yValues[i - 1]) / (xValues[i] - xValues[i - 1]) - 2 * (xValues[i] - xValues[i - 1]) * (yValues[i + 1] - yValues[i]) / (xValues[i + 1] - xValues[i - 1]);
                d[i] = (xValues[i + 1] - xValues[i - 1]) * (yValues[i + 1] - yValues[i]) / (xValues[i + 1] - xValues[i - 1]) - (yValues[i + 1] - yValues[i]) * (xValues[i + 1] - xValues[i]) / (xValues[i + 2] - xValues[i]);
            }

            Double[] w = new Double[n - 1];
            Double[] z = new Double[n - 1];

            for (Int32 i = 0; i < n - 2; i++)
            {
                w[i] = a[i] / b[i];
                z[i] = c[i] / b[i];
            }

            Double t0 = 0;
            Double t1 = (xNew - xValues[0]) / (xValues[1] - xValues[0]);
            Double t2 = (xNew - xValues[1]) / (xValues[2] - xValues[1]);
            Double t3 = (xNew - xValues[2]) / (xValues[3] - xValues[2]);

            Double u0 = 0;
            Double u1 = 3 * (yValues[1] - yValues[0]) / (xValues[1] - xValues[0]) - 3 * (yValues[2] - yValues[1]) / (xValues[2] - xValues[1]) + 2 * (yValues[3] - yValues[2]) / (xValues[3] - xValues[2]);
            Double u2 = -3 * (yValues[1] - yValues[0]) / (xValues[1] - xValues[0]) + 3 * (yValues[2] - yValues[1]) / (xValues[2] - xValues[1]) - (yValues[3] - yValues[2]) / (xValues[3] - xValues[2]);
            Double u3 = (yValues[1] - yValues[0]) / (xValues[1] - xValues[0]) - (yValues[2] - yValues[1]) / (xValues[2] - xValues[1]);

            Double result = t0 * u0 + t1 * u1 + t2 * u2 + t3 * u3;

            return result;
        }
    }

    /// <summary>
    /// Cubic spline interpolation.
    /// Call Fit (or use the corrector constructor) to compute spline coefficients, then Eval to evaluate the spline at other X coordinates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is implemented based on the wikipedia article:
    /// http://en.wikipedia.org/wiki/Spline_interpolation
    /// I'm not sure I have the right to include a copy of the article so the equation numbers referenced in 
    /// comments will end up being wrong at some point.
    /// </para>
    /// <para>
    /// This is not optimized, and is not MT safe.
    /// This can extrapolate off the ends of the splines.
    /// You must provide points in X sort order.
    /// </para>
    /// </remarks>
    public class CubicSpline
    {
        #region Fields

        // N-1 spline coefficients for N points
        private Single[] _A;
        private Single[] _B;

        // Save the original x and y for Eval
        private Single[] _XOrig;
        private Single[] _YOrig;

        #endregion

        #region Ctor

        /// <summary>
        /// Default ctor.
        /// </summary>
        public CubicSpline()
        {
        }

        /// <summary>
        /// Construct and call Fit.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        public CubicSpline(Single[] x, Single[] y, Single startSlope = Single.NaN, Single endSlope = Single.NaN, Boolean debug = false)
        {
            Fit(x, y, startSlope, endSlope, debug);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Throws if Fit has not been called.
        /// </summary>
        private void CheckAlreadyFitted()
        {
            if (_A == null) throw new Exception("Fit must be called before you can evaluate.");
        }

        private Int32 _LastIndex = 0;

        /// <summary>
        /// Find where in xOrig the specified x falls, by simultaneous traverse.
        /// This allows xs to be less than x[0] and/or greater than x[n-1]. So allows extrapolation.
        /// This keeps state, so requires that x be sorted and xs called in ascending order, and is not multi-thread safe.
        /// </summary>
        private Int32 GetNextXIndex(Single x)
        {
            if (x < _XOrig[_LastIndex])
            {
                throw new ArgumentException("The X values to evaluate must be sorted.");
            }

            while ((_LastIndex < _XOrig.Length - 2) && (x > _XOrig[_LastIndex + 1]))
            {
                _LastIndex++;
            }

            return _LastIndex;
        }

        /// <summary>
        /// Evaluate the specified x value using the specified spline.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="j">Which spline to use.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The y value.</returns>
        private Single EvalSpline(Single x, Int32 j, Boolean debug = false)
        {
            Single dx = _XOrig[j + 1] - _XOrig[j];
            Single t = (x - _XOrig[j]) / dx;
            Single y = (1 - t) * _YOrig[j] + t * _YOrig[j + 1] + t * (1 - t) * (_A[j] * (1 - t) + _B[j] * t); // equation 9
            if (debug) Console.WriteLine("xs = {0}, j = {1}, t = {2}", x, j, t);
            return y;
        }

        #endregion

        #region Fit*

        /// <summary>
        /// Fit x,y and then eval at points xs and return the corresponding y's.
        /// This does the "natural spline" style for ends.
        /// This can extrapolate off the ends of the splines.
        /// You must provide points in X sort order.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each xs.</returns>
        public Single[] FitAndEval(Single[] x, Single[] y, Single[] xs, Single startSlope = Single.NaN, Single endSlope = Single.NaN, Boolean debug = false)
        {
            Fit(x, y, startSlope, endSlope, debug);
            return Eval(xs, debug);
        }

        /// <summary>
        /// Compute spline coefficients for the specified x,y points.
        /// This does the "natural spline" style for ends.
        /// This can extrapolate off the ends of the splines.
        /// You must provide points in X sort order.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        public void Fit(Single[] x, Single[] y, Single startSlope = Single.NaN, Single endSlope = Single.NaN, Boolean debug = false)
        {
            if (Single.IsInfinity(startSlope) || Single.IsInfinity(endSlope))
            {
                throw new Exception("startSlope and endSlope cannot be infinity.");
            }

            // Save x and y for eval
            this._XOrig = x;
            this._YOrig = y;

            Int32 n = x.Length;
            Single[] r = new Single[n]; // the right hand side numbers: wikipedia page overloads b

            TriDiagonalMatrixF m = new TriDiagonalMatrixF(n);
            Single dx1, dx2, dy1, dy2;

            // First row is different (equation 16 from the article)
            if (Single.IsNaN(startSlope))
            {
                dx1 = x[1] - x[0];
                m.C[0] = 1.0f / dx1;
                m.B[0] = 2.0f * m.C[0];
                r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);
            }
            else
            {
                m.B[0] = 1;
                r[0] = startSlope;
            }

            // Body rows (equation 15 from the article)
            for (Int32 i = 1; i < n - 1; i++)
            {
                dx1 = x[i] - x[i - 1];
                dx2 = x[i + 1] - x[i];

                m.A[i] = 1.0f / dx1;
                m.C[i] = 1.0f / dx2;
                m.B[i] = 2.0f * (m.A[i] + m.C[i]);

                dy1 = y[i] - y[i - 1];
                dy2 = y[i + 1] - y[i];
                r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
            }

            // Last row also different (equation 17 from the article)
            if (Single.IsNaN(endSlope))
            {
                dx1 = x[n - 1] - x[n - 2];
                dy1 = y[n - 1] - y[n - 2];
                m.A[n - 1] = 1.0f / dx1;
                m.B[n - 1] = 2.0f * m.A[n - 1];
                r[n - 1] = 3 * (dy1 / (dx1 * dx1));
            }
            else
            {
                m.B[n - 1] = 1;
                r[n - 1] = endSlope;
            }

            if (debug) Console.WriteLine("Tri-diagonal matrix:\n{0}", m.ToDisplayString(":0.0000", "  "));
            //if (debug) Console.WriteLine("r: {0}", ArrayUtil.ToString<Single>(r));

            // k is the solution to the matrix
            Single[] k = m.Solve(r);
            //if (debug) Console.WriteLine("k = {0}", ArrayUtil.ToString<Single>(k));

            // a and b are each spline's coefficients
            this._A = new Single[n - 1];
            this._B = new Single[n - 1];

            for (Int32 i = 1; i < n; i++)
            {
                dx1 = x[i] - x[i - 1];
                dy1 = y[i] - y[i - 1];
                _A[i - 1] = k[i - 1] * dx1 - dy1; // equation 10 from the article
                _B[i - 1] = -k[i] * dx1 + dy1; // equation 11 from the article
            }

            //if (debug) Console.WriteLine("a: {0}", ArrayUtil.ToString<Single>(a));
            //if (debug) Console.WriteLine("b: {0}", ArrayUtil.ToString<Single>(b));
        }

        #endregion

        #region Eval*

        /// <summary>
        /// Evaluate the spline at the specified x coordinates.
        /// This can extrapolate off the ends of the splines.
        /// You must provide X's in ascending order.
        /// The spline must already be computed before calling this, meaning you must have already called Fit() or FitAndEval().
        /// </summary>
        /// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each x.</returns>
        public Single[] Eval(Single[] x, Boolean debug = false)
        {
            CheckAlreadyFitted();

            Int32 n = x.Length;
            Single[] y = new Single[n];
            _LastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

            for (Int32 i = 0; i < n; i++)
            {
                // Find which spline can be used to compute this x (by simultaneous traverse)
                Int32 j = GetNextXIndex(x[i]);

                // Evaluate using j'th spline
                y[i] = EvalSpline(x[i], j, debug);
            }

            return y;
        }

        /// <summary>
        /// Evaluate (compute) the slope of the spline at the specified x coordinates.
        /// This can extrapolate off the ends of the splines.
        /// You must provide X's in ascending order.
        /// The spline must already be computed before calling this, meaning you must have already called Fit() or FitAndEval().
        /// </summary>
        /// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each x.</returns>
        public Single[] EvalSlope(Single[] x, Boolean debug = false)
        {
            CheckAlreadyFitted();

            Int32 n = x.Length;
            Single[] qPrime = new Single[n];
            _LastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

            for (Int32 i = 0; i < n; i++)
            {
                // Find which spline can be used to compute this x (by simultaneous traverse)
                Int32 j = GetNextXIndex(x[i]);

                // Evaluate using j'th spline
                Single dx = _XOrig[j + 1] - _XOrig[j];
                Single dy = _YOrig[j + 1] - _YOrig[j];
                Single t = (x[i] - _XOrig[j]) / dx;

                // From equation 5 we could also compute q' (qp) which is the slope at this x
                qPrime[i] = dy / dx
                    + (1 - 2 * t) * (_A[j] * (1 - t) + _B[j] * t) / dx
                    + t * (1 - t) * (_B[j] - _A[j]) / dx;

                if (debug) Console.WriteLine("[{0}]: xs = {1}, j = {2}, t = {3}", i, x[i], j, t);
            }

            return qPrime;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Static all-in-one method to fit the splines and evaluate at X coordinates.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each xs.</returns>
        public static Single[] Compute(Single[] x, Single[] y, Single[] xs, Single startSlope = Single.NaN, Single endSlope = Single.NaN, Boolean debug = false)
        {
            CubicSpline spline = new CubicSpline();
            return spline.FitAndEval(x, y, xs, startSlope, endSlope, debug);
        }

        /// <summary>
        /// Fit the input x,y points using the parametric approach, so that y does not have to be an explicit
        /// function of x, meaning there does not need to be a single value of y for each x.
        /// </summary>
        /// <param name="x">Input x coordinates.</param>
        /// <param name="y">Input y coordinates.</param>
        /// <param name="nOutputPoints">How many output points to create.</param>
        /// <param name="xs">Output (interpolated) x values.</param>
        /// <param name="ys">Output (interpolated) y values.</param>
        /// <param name="firstDx">Optionally specifies the first point's slope in combination with firstDy. Together they
        /// are a vector describing the direction of the parametric spline of the starting point. The vector does
        /// not need to be normalized. If either is NaN then neither is used.</param>
        /// <param name="firstDy">See description of dx0.</param>
        /// <param name="lastDx">Optionally specifies the last point's slope in combination with lastDy. Together they
        /// are a vector describing the direction of the parametric spline of the last point. The vector does
        /// not need to be normalized. If either is NaN then neither is used.</param>
        /// <param name="lastDy">See description of dxN.</param>
        public static void FitParametric(Single[] x, Single[] y, Int32 nOutputPoints, out Single[] xs, out Single[] ys,
            Single firstDx = Single.NaN, Single firstDy = Single.NaN, Single lastDx = Single.NaN, Single lastDy = Single.NaN)
        {
            // Compute distances
            Int32 n = x.Length;
            Single[] dists = new Single[n]; // cumulative distance
            dists[0] = 0;
            Single totalDist = 0;

            for (Int32 i = 1; i < n; i++)
            {
                Single dx = x[i] - x[i - 1];
                Single dy = y[i] - y[i - 1];
                Single dist = (Single)Math.Sqrt(dx * dx + dy * dy);
                totalDist += dist;
                dists[i] = totalDist;
            }

            // Create 'times' to interpolate to
            Single dt = totalDist / (nOutputPoints - 1);
            Single[] times = new Single[nOutputPoints];
            times[0] = 0;

            for (Int32 i = 1; i < nOutputPoints; i++)
            {
                times[i] = times[i - 1] + dt;
            }

            // Normalize the slopes, if specified
            NormalizeVector(ref firstDx, ref firstDy);
            NormalizeVector(ref lastDx, ref lastDy);

            // Spline fit both x and y to times
            CubicSpline xSpline = new CubicSpline();
            xs = xSpline.FitAndEval(dists, x, times, firstDx / dt, lastDx / dt);

            CubicSpline ySpline = new CubicSpline();
            ys = ySpline.FitAndEval(dists, y, times, firstDy / dt, lastDy / dt);
        }

        private static void NormalizeVector(ref Single dx, ref Single dy)
        {
            if (!Single.IsNaN(dx) && !Single.IsNaN(dy))
            {
                Single d = (Single)Math.Sqrt(dx * dx + dy * dy);

                if (d > Single.Epsilon) // probably not conservative enough, but catches the (0,0) case at least
                {
                    dx = dx / d;
                    dy = dy / d;
                }
                else
                {
                    throw new ArgumentException("The input vector is too small to be normalized.");
                }
            }
            else
            {
                // In case one is NaN and not the other
                dx = dy = Single.NaN;
            }
        }

        #endregion
    }

    public static class InterpExtend
    {
        public static void CubicSplineInterpEdgesByRaise(this CubicSpline spline, Double[] samples, Double mid, Int32 xresult0, Int32 xresult1, ref Double result, ref Int32 resultcnt)
        {
            for (Int64 i = xresult0 + 1; i <= xresult1; ++i)
            {
                if (i - 2 > 0 && i + 1 < samples.Length - 1)
                {
                    if (samples[i - 1] <= mid && samples[i] > mid)
                    {
                        Int32 ratio = 10;
                        Int32 fitLength = 4;
                        Int32 totallength = fitLength * ratio;
                        Double midvaluepos = 0;
                        var x = new Single[4] { i - 2, i - 1, i, i + 1 };
                        var y = new Single[4] { (Single)samples[i - 2], (Single)samples[i - 1], (Single)samples[i], (Single)samples[i + 1] };
                        var xs = new Single[totallength];
                        Single stepsize = (x[x.Length - 1] - x[0]) / (totallength - 1);//10倍 插值
                        for (Int32 k = 0; k < totallength; k++)
                        {
                            xs[k] = x[0] + k * stepsize;
                        }
                        Single[] ys = spline.FitAndEval(x, y, xs);
                        for (Int32 l = 0; l < totallength - 1; l++)
                        {
                            if (ys[l] <= mid && ys[l + 1] > mid)
                            {
                                midvaluepos = stepsize * (1 + (mid - ys[l]) / (ys[l + 1] - ys[l]));  //目前暂时采用线性插值，后面考虑三次样条插值

                            }
                        }

                        result = result + i - 1 + midvaluepos;  //目前暂时采用线性插值，后面考虑三次样条插值
                        resultcnt++;
                    }
                }
            }
        }

        public static void CubicSplineInterpEdgesByFall(this CubicSpline spline, Double[] samples, Double mid, Int32 xresult0, Int32 xresult1, ref Double result, ref Int32 resultcnt)
        {
            for (Int64 i = xresult1 + 1; i <= xresult0; ++i)
            {
                if (i - 2 > 0 && i + 1 < samples.Length - 1)
                {
                    if (samples[i - 1] >= mid && samples[i] < mid)
                    {
                        Int32 ratio = 10;
                        Int32 fitlength = 4;
                        Int32 totallength = fitlength * ratio;
                        Double midValuePos = 0;
                        var x = new Single[4] { i - 2, i - 1, i, i + 1 };
                        var y = new Single[4] { (Single)samples[i - 2], (Single)samples[i - 1], (Single)samples[i], (Single)samples[i + 1] };
                        var xs = new Single[totallength];
                        Single stepsize = (x[x.Length - 1] - x[0]) / (totallength - 1);//10倍 插值
                        for (Int32 k = 0; k < totallength; k++)
                        {
                            xs[k] = x[0] + k * stepsize;
                        }
                        Single[] ys = spline.FitAndEval(x, y, xs);
                        for (Int32 l = 0; l < totallength - 1; l++)
                        {
                            if (ys[l] >= mid && ys[l + 1] < mid)
                            {
                                midValuePos = stepsize * (1 + (mid - ys[l]) / (ys[l + 1] - ys[l]));  //目前暂时采用线性插值，后面考虑三次样条插值

                            }
                        }

                        result = result + i - 1 + midValuePos;  //目前暂时采用线性插值，后面考虑三次样条插值
                        resultcnt++;
                    }
                }
            }
        }

        public static void LinearInterpEdgesByRaise(Double[] samples, Double mid, Int32 xresult0, Int32 xresult1, ref Double result, ref Int32 resultcnt)
        {
            for (Int64 i = xresult0 + 1; i <= xresult1; ++i)
            {
                if (samples[i - 1] <= mid && samples[i] > mid)
                {
                    result = result + i - 1 + (mid - samples[i - 1]) / (samples[i] - samples[i - 1]);  //目前暂时采用线性插值，后面考虑三次样条插值
                    resultcnt++;
                }
            }
        }

        public static void LinearInterpEdgesByFall(Double[] samples, Double mid, Int32 xresult0, Int32 xresult1, ref Double result, ref Int32 resultcnt)
        {
            for (Int64 i = xresult1 + 1; i <= xresult0; ++i)
            {
                if (samples[i - 1] >= mid && samples[i] < mid)
                {
                    result = result + i - 1 + (samples[i - 1] - mid) / (samples[i - 1] - samples[i]);
                    resultcnt++;
                }

            }
        }
    }
    /// <summary>
	/// A tri-diagonal matrix has non-zero entries only on the main diagonal, the diagonal above the main (super), and the
	/// diagonal below the main (sub).
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is based on the wikipedia article: http://en.wikipedia.org/wiki/Tridiagonal_matrix_algorithm
	/// </para>
	/// <para>
	/// The entries in the matrix on a particular row are A[i], B[i], and C[i] where i is the row index.
	/// B is the main diagonal, and so for an NxN matrix B is length N and all elements are used.
	/// So for row 0, the first two values are B[0] and C[0].
	/// And for row N-1, the last two values are A[N-1] and B[N-1].
	/// That means that A[0] is not actually on the matrix and is therefore never used, and same with C[N-1].
	/// </para>
	/// </remarks>
	public class TriDiagonalMatrixF
    {
        /// <summary>
        /// The values for the sub-diagonal. A[0] is never used.
        /// </summary>
        public Single[] A;

        /// <summary>
        /// The values for the main diagonal.
        /// </summary>
        public Single[] B;

        /// <summary>
        /// The values for the super-diagonal. C[C.Length-1] is never used.
        /// </summary>
        public Single[] C;

        /// <summary>
        /// The width and height of this matrix.
        /// </summary>
        public Int32 N
        {
            get { return (A != null ? A.Length : 0); }
        }

        /// <summary>
        /// Indexer. Setter throws an exception if you try to set any not on the super, main, or sub diagonals.
        /// </summary>
        public Single this[Int32 row, Int32 col]
        {
            get
            {
                Int32 di = row - col;

                if (di == 0)
                {
                    return B[row];
                }
                else if (di == -1)
                {
                    Debug.Assert(row < N - 1);
                    return C[row];
                }
                else if (di == 1)
                {
                    Debug.Assert(row > 0);
                    return A[row];
                }
                else return 0;
            }
            set
            {
                Int32 di = row - col;

                if (di == 0)
                {
                    B[row] = value;
                }
                else if (di == -1)
                {
                    Debug.Assert(row < N - 1);
                    C[row] = value;
                }
                else if (di == 1)
                {
                    Debug.Assert(row > 0);
                    A[row] = value;
                }
                else
                {
                    throw new ArgumentException("Only the main, super, and sub diagonals can be set.");
                }
            }
        }

        /// <summary>
        /// Construct an NxN matrix.
        /// </summary>
        public TriDiagonalMatrixF(Int32 n)
        {
            this.A = new Single[n];
            this.B = new Single[n];
            this.C = new Single[n];
        }

        /// <summary>
        /// Produce a String representation of the contents of this matrix.
        /// </summary>
        /// <param name="fmt">Optional. For String.Format. Must include the colon. Examples are ':0.000' and ',5:0.00' </param>
        /// <param name="prefix">Optional. Per-line indentation prefix.</param>
        public String ToDisplayString(String fmt = "", String prefix = "")
        {
            if (this.N > 0)
            {
                var s = new StringBuilder();
                String formatString = "{0" + fmt + "}";

                for (Int32 r = 0; r < N; r++)
                {
                    s.Append(prefix);

                    for (Int32 c = 0; c < N; c++)
                    {
                        s.AppendFormat(formatString, this[r, c]);
                        if (c < N - 1) s.Append(", ");
                    }

                    s.AppendLine();
                }

                return s.ToString();
            }
            else
            {
                return prefix + "0x0 Matrix";
            }
        }

        /// <summary>
        /// Solve the system of equations this*x=d given the specified d.
        /// </summary>
        /// <remarks>
        /// Uses the Thomas algorithm described in the wikipedia article: http://en.wikipedia.org/wiki/Tridiagonal_matrix_algorithm
        /// Not optimized. Not destructive.
        /// </remarks>
        /// <param name="d">Right side of the equation.</param>
        public Single[] Solve(Single[] d)
        {
            Int32 n = this.N;

            if (d.Length != n)
            {
                throw new ArgumentException("The input d is not the same size as this matrix.");
            }

            // cPrime
            Single[] cPrime = new Single[n];
            cPrime[0] = C[0] / B[0];

            for (Int32 i = 1; i < n; i++)
            {
                cPrime[i] = C[i] / (B[i] - cPrime[i - 1] * A[i]);
            }

            // dPrime
            Single[] dPrime = new Single[n];
            dPrime[0] = d[0] / B[0];

            for (Int32 i = 1; i < n; i++)
            {
                dPrime[i] = (d[i] - dPrime[i - 1] * A[i]) / (B[i] - cPrime[i - 1] * A[i]);
            }

            // Back substitution
            Single[] x = new Single[n];
            x[n - 1] = dPrime[n - 1];

            for (Int32 i = n - 2; i >= 0; i--)
            {
                x[i] = dPrime[i] - cPrime[i] * x[i + 1];
            }

            return x;
        }
    }
}
