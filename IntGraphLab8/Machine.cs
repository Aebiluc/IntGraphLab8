using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace APIMAchine
{
    public enum ColorTank {NONE, A,B,C,D};

    public class Machine
    {
        private string _ip;
        private int _port;

        private UdpClient _udpClient;
        private IPEndPoint _sender;
        private string _answer;
       

        private bool _BucketsLoading;
        private ColorTank CurrentDeliveryColor;

        public Machine(string ip, int port)
        {
            _ip = ip;
            _port = port;
            _udpClient = new UdpClient(_ip, _port);
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            _sender = new IPEndPoint(IPAddress.Any, 0);
            _BucketsLoading = true;
        }

        private void Send(string command , out string answer)
        {
            byte[] commandBytes, answerBytes;
            // clean all pending messages from receive buffer before sending command
            while (_udpClient.Available > 0)
                _udpClient.Receive(ref _sender);
            // send command as a packet of bytes
            commandBytes = Encoding.ASCII.GetBytes(command);
            _udpClient.Send(commandBytes, commandBytes.Length);
            try {
                // wait and get answer as a packet of bytes, convert to string
                answerBytes = _udpClient.Receive(ref _sender);
                answer = Encoding.ASCII.GetString(answerBytes);
                //return 1;
            }
            catch (SocketException) {
                answer = null;
                throw new SocketException();
            }
        }

        public string IpAdress
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public bool Connected { get; set; }

        public bool ConveyorOn
        {
            get
            {
                Send("ConveyorMoving", out _answer);
                if (_answer == "True")
                    return true;
                else
                    return false;
            }
        }

        public void StartConveyor()
        {
            Send("ConveyorStart", out _answer);
        }

        public void StopConveyor()
        {
            Send("ConveyorStop", out _answer);
        }

        public bool BucketLoadingEnabled
        {
            set
            {
                if (value)
                {
                    Send("EnableBucketsLoading", out _answer);
                    if (_answer == "Ok")
                        _BucketsLoading = true;
                }
                else
                {
                    Send("DisableBucketsLoading", out _answer);
                    if (_answer == "Ok")
                        _BucketsLoading = false;
                }
            }
            get { return _BucketsLoading; }
        }

        public bool BucketLocked
        {
            get
            {
                Send("BucketLocked", out _answer);
                if (_answer == "True")
                    return true;
                else if (_answer == "False")
                    return false;
                return false;
            }
        }

        public ColorTank ColorTank
        {
            set
            {
                switch (value)
                {
                    case ColorTank.A:
                        Send("PaintA", out _answer);
                        if (_answer == "Ok")
                        {
                            CurrentDeliveryColor = ColorTank.A;
                            return;
                        } 
                        break;
                    case ColorTank.B:
                        Send("PaintB", out _answer);
                        if (_answer == "Ok")
                        {
                            CurrentDeliveryColor = ColorTank.B;
                            return;
                        }
                        break;
                    case ColorTank.C:
                        Send("PaintC", out _answer);
                        if (_answer == "Ok")
                        {
                            CurrentDeliveryColor = ColorTank.C;
                            return;
                        }
                        break;
                    case ColorTank.D:
                        Send("PaintD", out _answer);
                        if (_answer == "Ok")
                        {
                            CurrentDeliveryColor = ColorTank.D;
                            return;
                        }
                        break;
                    case ColorTank.NONE:
                        Send("PaintNone", out _answer);
                        if (_answer == "Ok")
                        {
                            CurrentDeliveryColor = ColorTank.NONE;
                            return;
                        }
                        break;
                }
            }

            get
            {

                return CurrentDeliveryColor;
            }              
        }
    }
}
