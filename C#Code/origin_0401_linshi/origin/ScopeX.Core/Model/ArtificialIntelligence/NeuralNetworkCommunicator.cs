using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    /// <summary>
    /// 神经网络通信器 - 负责与服务器的TCP通信，包括发送训练数据和接收ONNX模型
    /// </summary>
    internal class NeuralNetworkCommunicator : IDisposable
    {
        // ================= TCP 通信部分 =================
        private TcpClient _TcpClient = new TcpClient();
        private NetworkStream? _NetworkStream = null;
        private volatile bool _IsWorking = false;
        private Thread? _ReceiveThread = null;
        private volatile bool _initTcpClientFlag = false;   
        private Dictionary<int, List<byte[]>> _allFrames = new Dictionary<int, List<byte[]>>();
        private static readonly byte[] HeaderMagic = Encoding.UTF8.GetBytes("DATA");
        private volatile bool _isWaitingForServer = false;

        /// <summary>
        /// 模型接收完成事件
        /// </summary>
        public event Action<string>? OnModelReceived;

        /// <summary>
        /// 是否正在等待服务器响应
        /// </summary>
        public bool IsWaitingForServer => _isWaitingForServer;

        /// <summary>
        /// TCP客户端是否已初始化
        /// </summary>
        public bool IsConnected => _initTcpClientFlag;

        /// <summary>
        /// 初始化TCP客户端连接
        /// </summary>
        public void InitializeConnection()
        {
            if (!_initTcpClientFlag)
            {
                try
                {
                    String serverIp = Environment.GetEnvironmentVariable("SERVER_IP") ?? "192.168.1.3";
                    String port = Environment.GetEnvironmentVariable("SERVER_PORT") ?? "8888";

                    _TcpClient.Connect(serverIp, int.Parse(port));
                    _NetworkStream = _TcpClient.GetStream();

                    _ReceiveThread = new Thread(Receive) { IsBackground = false };
                    _ReceiveThread.Start();
                    _initTcpClientFlag = true;

                    Console.WriteLine($"已连接到服务器: {serverIp}:{port}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TCP连接失败: {ex.Message}");
                    _initTcpClientFlag = false;
                }
            }
        }


        public String? NewModelPath { get; set; }

        /// <summary>
        /// 发送训练数据到服务器
        /// </summary>
        /// <param name="data">UInt16类型的原始数据</param>
        public void SendTrainingData(UInt16[] data)
        {
            if (data == null || data.Length == 0)
            {
                Console.WriteLine("发送数据为空，取消发送");
                return;
            }

            if (!_initTcpClientFlag || _NetworkStream == null)
            {
                Console.WriteLine("TCP未连接，无法发送数据");
                return;
            }

            _isWaitingForServer = true;
            Task.Run(() => SendData(data));
        }

        /// <summary>
        /// 内部发送数据方法
        /// </summary>
        private void SendData(UInt16[] data)
        {
            try
            {
                // 读取单帧最大数据量（以 UInt16 个数为单位），默认 1024 个
                string singleFrameCountStr = Environment.GetEnvironmentVariable("SINGLE_FRAME_LENGTH");
                if (!int.TryParse(singleFrameCountStr, out int maxItemsPerFrame) || maxItemsPerFrame <= 0)
                    maxItemsPerFrame = 1024;

                // 计算帧数（向上取整）
                int framecnt = (data.Length + maxItemsPerFrame - 1) / maxItemsPerFrame;

                // 预分配缓冲区：每帧最多 maxItemsPerFrame 个 UInt16 → 每个 2 字节
                int maxPayloadSize = maxItemsPerFrame * sizeof(UInt16);
                byte[] buffer = new byte[HeaderMagic.Length + 4 + 4 + 4 + maxPayloadSize]; // 头部 + 数据

                // 复用：拷贝魔数到缓冲区
                Array.Copy(HeaderMagic, 0, buffer, 0, HeaderMagic.Length);

                Console.WriteLine($"开始发送数据，总共 {framecnt} 帧，数据量: {data.Length}");

                for (int frameid = 0; frameid < framecnt; frameid++)
                {
                    int offset = HeaderMagic.Length; // 重置偏移

                    int startIdx = frameid * maxItemsPerFrame;
                    int count = Math.Min(maxItemsPerFrame, data.Length - startIdx);
                    int currentPayloadSize = count * sizeof(UInt16);

                    // 写入当前帧数据长度（字节数）
                    Array.Copy(BitConverter.GetBytes(currentPayloadSize), 0, buffer, offset, 4);
                    offset += 4;

                    // 写入总帧数
                    Array.Copy(BitConverter.GetBytes(framecnt), 0, buffer, offset, 4);
                    offset += 4;

                    // 写入帧 ID
                    Array.Copy(BitConverter.GetBytes(frameid), 0, buffer, offset, 4);
                    offset += 4;

                    // 将当前帧的 UInt16 数据写入缓冲区
                    for (int i = 0; i < count; i++)
                    {
                        byte[] bytes = BitConverter.GetBytes(data[startIdx + i]);
                        Array.Copy(bytes, 0, buffer, offset + i * sizeof(UInt16), sizeof(UInt16));
                    }
                    offset += currentPayloadSize;

                    // 发送整帧
                    _NetworkStream?.Write(buffer, 0, offset);

                    if (frameid % 100 == 0 || frameid == framecnt - 1)
                    {
                        Console.WriteLine($"已发送帧 {frameid + 1}/{framecnt}");
                    }
                }

                _NetworkStream?.Flush();
                Console.WriteLine("数据发送完成，等待服务器返回模型");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送数据异常: {ex.Message}");
                _isWaitingForServer = false;
            }
        }

        /// <summary>
        /// 接收数据线程
        /// </summary>
        private void Receive()
        {
            byte[] receiveBuffer = new byte[65536]; // 接收缓冲区
            List<byte> dataBuffer = new List<byte>(); // 用于累积数据

            Console.WriteLine("接收线程已启动");

            while (true)
            {
                _IsWorking = true;

                try
                {
                    if (_NetworkStream == null)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    int bytesRead = _NetworkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                    if (bytesRead <= 0)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    // 将读取的数据添加到缓冲区
                    dataBuffer.AddRange(receiveBuffer.Take(bytesRead));

                    while (dataBuffer.Count >= 16) // 确保至少有头部
                    {
                        // 解析头部
                        string frameTitle = Encoding.ASCII.GetString(dataBuffer.Take(4).ToArray());
                        if (frameTitle != "ONNX")
                        {
                            // 清除无效数据并继续
                            dataBuffer.RemoveAt(0);
                            continue;
                        }

                        // 解析头部字段
                        int expectedFrameLength = BitConverter.ToInt32(dataBuffer.Skip(4).Take(4).ToArray(), 0);
                        int frameCount = BitConverter.ToInt32(dataBuffer.Skip(8).Take(4).ToArray(), 0);
                        int frameId = BitConverter.ToInt32(dataBuffer.Skip(12).Take(4).ToArray(), 0);

                        // 确保有足够的数据读取整个包
                        if (dataBuffer.Count >= 16 + expectedFrameLength)
                        {
                            Console.WriteLine($"收到帧: {frameTitle} 长度:{expectedFrameLength} 帧ID:{frameId}/{frameCount}");

                            // 提取数据体
                            byte[] frameData = dataBuffer.Skip(16).Take(expectedFrameLength).ToArray();

                            // 处理数据帧
                            ProcessDataFrame(frameData, frameId, frameCount);

                            // 移除已处理的数据
                            dataBuffer.RemoveRange(0, 16 + expectedFrameLength);
                        }
                        else
                        {
                            // 数据不足，等待下一次读取
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"接收错误: {ex.Message}");
                    Thread.Sleep(100);
                }

                _IsWorking = false;
            }
        }

        /// <summary>
        /// 处理接收到的数据帧
        /// </summary>
        private void ProcessDataFrame(byte[] frameData, int frameId, int frameCount)
        {
            try
            {
                // 如果是第一帧，清空之前的数据
                if (frameId == 0)
                {
                    _allFrames.Clear();
                    Console.WriteLine("开始接收新数据包序列");
                }

                // 存储帧数据
                if (!_allFrames.ContainsKey(frameId))
                {
                    _allFrames[frameId] = new List<byte[]>();
                }
                _allFrames[frameId].Add(frameData);

                Console.WriteLine($"已接收帧 {frameId + 1}/{frameCount}，数据长度: {frameData.Length} 字节");

                // 如果是最后一帧，重组所有数据
                if (frameId == frameCount - 1)
                {
                    ReassembleAndSaveModel(frameCount);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理数据帧错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 重组并保存接收到的模型文件
        /// </summary>
        private void ReassembleAndSaveModel(int totalFrames)
        {
            try
            {
                List<byte> combinedData = new List<byte>();
                for (int i = 0; i < totalFrames; i++)
                {
                    if (_allFrames.ContainsKey(i))
                    {
                        foreach (byte[] frame in _allFrames[i]) 
                            combinedData.AddRange(frame);
                    }
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                NewModelPath = $"StartNet_Model_{timestamp}.onnx";
                // 保存到当前目录或指定的模型目录
                string fileName = Path.Combine(Directory.GetCurrentDirectory(), NewModelPath);

                File.WriteAllBytes(fileName, combinedData.ToArray());
                Console.WriteLine($"收到新模型，保存至: {fileName}");

                // 清空帧缓存
                _allFrames.Clear();
                _isWaitingForServer = false;

                // 触发模型接收完成事件
                OnModelReceived?.Invoke(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"模型重组失败: {ex.Message}");
                _isWaitingForServer = false;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                _NetworkStream?.Close();
                _TcpClient?.Close();
                _initTcpClientFlag = false;
                Console.WriteLine("TCP连接已关闭");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"关闭连接异常: {ex.Message}");
            }
        }
    }
}
