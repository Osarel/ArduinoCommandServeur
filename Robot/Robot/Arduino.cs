using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Robot
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Arduino : UpdatableElement
    {
        string[] STOPCHAR = { "\r\n", "\n", "\r" };
        private string _data = "";

        [JsonProperty]
        public string PortName { get; set; }
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public string ID;
        [JsonProperty]
        public int BaudRate { get; set; }
        private SerialPort port;

        public class ThreadActionPin
        {
            Action<Dictionary<string, object>, object> action;
            object obj;

            public ThreadActionPin(Action<Dictionary<string, object>, object> action, object obj)
            {
                this.action = action;
                this.obj = obj;
            }

            public Action<Dictionary<string, object>, object> GetAction()
            {
                return action;
            }
            public object GetObject()
            {
                return obj;
            }
        }

        public IDictionary<int, List<ThreadActionPin>> PinAction { get; private set; }

        public void AddAction(int pin, ThreadActionPin actionpin)
        {
            if (!PinAction.ContainsKey(pin))
            {
                PinAction.Add(pin, new List<ThreadActionPin>());
            } 
                PinAction[pin].Add(actionpin);
            Console.WriteLine("a " + PinAction[pin].Count);
        }
        public Arduino(string Name, string ID, string PortName, int BaudRate)
        {
            PinAction = new Dictionary<int, List<ThreadActionPin>>();
            this.PortName = PortName;
            this.Name = Name;
            this.ID = ID;
            this.BaudRate = BaudRate;
        }
        public static string[] SerialPorts()
        {
            return SerialPort.GetPortNames();
        }



        public bool StartCommunication()
        {
            //Début de la communication avec le com port

            if ((port == null || !port.IsOpen) && PortName.Length > 0 && !PortName.Equals("No port"))
            {
                port = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
                port.Handshake = Handshake.XOnXOff;
                port.ReadTimeout = 999999;
                port.WriteTimeout = 999999;
                port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);
                try
                {
                    port.Open();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        private void MethodToCall(string valeur) {
            Console.WriteLine(valeur);
            try
            {
                Dictionary<string, object> request = JsonConvert.DeserializeObject<Dictionary<string, object>>(valeur);
                List<ThreadActionPin> ap = PinAction[(int)(long)request["pin"]];
                
                foreach (ThreadActionPin i in ap)
                {
                    if (i != null)
                    {
                        i.GetAction()(request, i.GetObject());
                    }
                    //déclanchement du callback
                    
                }
            } catch
            {
                Console.WriteLine("Erreur reception arduino");
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (port.IsOpen && port != null)
            {
                string incomingData = port.ReadExisting();
                bool endsWithStop = EndsWithStop(incomingData);
                string[] dataArray = incomingData.Split(STOPCHAR, StringSplitOptions.None);
                for (int i = 0; i < dataArray.Length; i++)
                {
                    string newData = dataArray[i];
                    if (!endsWithStop && i == dataArray.Length - 1)
                    {
                        _data += newData;
                    }

                    else
                    {
                        string dataToSend = _data + newData;
                        if (dataToSend != "")
                        {
                            MethodToCall(dataToSend);
                            _data = "";
                        }
                    }
                }

                if (_data.Length > 1000)
                {
                    _data = "";
                }

            }
        }



        private bool EndsWithStop(string incomingData)
        {
            for (int i = 0; i < STOPCHAR.Length; i++)
            {
                if (incomingData.EndsWith(STOPCHAR[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public bool Started()
        {
            return port != null && PortName.Length > 0 && port.IsOpen;
        }

        public bool Write(string data)
        {
            //Si arduino démarrer
            if (Started())
            {
                //ecriture des données dans l'arduino
                Console.WriteLine(data);
                port.WriteLine(data + "\n");
                return true; 
            }
            return false;
        }

        public bool Stop()
        {

            //Déconnexion de l'arduino
            if (port != null && port.IsOpen)
            {
                port.Close();
                return true;
            }
            return false;
        }
        public ArduinoStatus GetArduinoStatus()
        {
            int pos = Array.IndexOf(SerialPorts(), PortName);
            return new ArduinoStatus(Name, port != null && port.IsOpen, pos > -1);
        }

        public bool Save()
        {
            ArduinoCommand.robot.SaveArduino();
            return true;
        }

        public bool AddToList()
        {
            ArduinoCommand.robot.Arduinos[ID] = this;
            return true;
        }

        public UpdatableElement GetLastInstance()
        {
            return ArduinoCommand.robot.GetArduinoByUUID(ID);
        }
    }


    public class ArduinoStatus{

        public string Name { get; private set; }
        public bool Connected { get; private set; }
        public bool Ready { get; private set; }

        public ArduinoStatus(string Name, bool connected, bool ready)
        {
            this.Name = Name;
            Connected = connected;
            Ready = ready;
        }
    }
}
