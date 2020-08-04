using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class ClientMaster
    {
        private UdpClient _udpClient;
        private IPEndPoint _remoteEndPoint;

        private string _core;
        private string _database;
        private string _multicastIPAddress;
        private int _multicastPort;

        SqlMaster sqlMaster;

        public ClientMaster()
        {
            InitializeSettings();
        }
        private void InitializeSettings()
        {
            Settings settings = CommonSerializer.Deserialize<Settings>("settings.xml");
            if (settings != null)
            {
                _multicastPort = settings.MulticastPort;
                _multicastIPAddress = settings.MulticastIPAddress;
                _database = settings.DataBase;
                _core = settings.ServerCore;
            }

            string connectionString = $@"Data Source=.\{_core};Initial Catalog={_database};Integrated Security=True";
            sqlMaster = new SqlMaster(connectionString);
        }

        public async Task Start()
        {
            Console.WriteLine("Starting client...");

            using (_udpClient = new UdpClient())
            {
                IPEndPoint localEP = new IPEndPoint(IPAddress.Any, _multicastPort);
                _remoteEndPoint = new IPEndPoint(IPAddress.Parse(_multicastIPAddress), _multicastPort);

                _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _udpClient.ExclusiveAddressUse = false;

                _udpClient.Client.Bind(localEP);

                _udpClient.JoinMulticastGroup(IPAddress.Parse(_multicastIPAddress));
                _udpClient.MulticastLoopback = true;

                _udpClient.BeginReceive(UDPReceiveCallback, null);

                await Task.Factory.StartNew(() => ProcessUserRequest());
            }
        }
        
        #region user request
        private void ProcessUserRequest()
        {
            try
            {
                string cmd;
                while ((cmd = Console.ReadLine()) != "exit")
                    switch (cmd)
                    {
                        case "--info":
                            DataReport.PrintInfo(GetAverage(), GetStandardDeviation(), GetMode(), GetMedian(), GetLostPackages(), GetCount());
                            break;

                        case "--stdev":
                            DataReport.PrintSingleValue("Стандартное отклонение", GetStandardDeviation());
                            break;

                        case "--avg":
                            DataReport.PrintSingleValue("Среднее арифметическое", GetAverage());
                            break;

                        case "--count":
                            DataReport.PrintSingleValue("Количество записей", GetCount());
                            break;

                        case "--mode":
                            DataReport.PrintSingleValue("Мода", GetMode());
                            break;

                        case "--median":
                            DataReport.PrintSingleValue("Медиана", GetMedian());
                            break;

                        case "--lp":
                            DataReport.PrintSingleValue("Количество потерянных пакетов", GetLostPackages());
                            break;

                        case "--help":
                            DataReport.PrintHelp();
                            break;

                        default:
                            DataReport.PrintSingleValue("Unknown command", cmd);
                            break;
                    }
            }
            catch (Exception e)
            {
                //nLog.Error(e);
                Console.WriteLine(e);
            }
        }

        private string GetLostPackages()
        {
            string cmdText = $"select ((max(id_package) - min(id_package) + 1) - count(id)) from quotes";
            return sqlMaster.ExecuteScalarCommand(cmdText).ToString();
        }
        private string GetMedian()
        {
            string cmdText = $"select" +
                                    "(" +
                                    "(select max(package) from " +
                                    "(select top 50 percent package from Quotes order by package) as bottomHalf)" +
                                    "+" +
                                    "(select min(package) from " +
                                    "(select top 50 percent package from quotes order by package desc) as topHalf)" +
                                    ")/2 as median";

            return sqlMaster.ExecuteScalarCommand(cmdText).ToString();
        }
        private string GetMode()
        {
            string cmdText = $"select top 1 with ties package " +
                                    "from quotes " +
                                    "where package is not null " +
                                    "group by package " +
                                    "order by count(*) desc";
            return sqlMaster.ExecuteScalarCommand(cmdText).ToString();
        }
        private string GetCount()
        {
            string cmdText = $"select count(*) from quotes";
            return sqlMaster.ExecuteScalarCommand(cmdText).ToString();
        }
        private string GetAverage()
        {
            string cmdText = $"select avg(cast(package as bigint)) from quotes";
            return sqlMaster.ExecuteScalarCommand(cmdText).ToString();
        }
        private string GetStandardDeviation()
        {
            string cmdText = $"select stdev(package) from quotes";
            return sqlMaster.ExecuteScalarCommand(cmdText).ToString();
        }
        #endregion

        private void UDPReceiveCallback(IAsyncResult ar)
        {
            byte[] receiveBytes = _udpClient.EndReceive(ar, ref _remoteEndPoint);

            string returnData = Encoding.UTF8.GetString(receiveBytes);

            Task.Factory.StartNew(() => InsertRecord(returnData));

            _udpClient.BeginReceive(UDPReceiveCallback, null);
        }
        private void InsertRecord(string returnData)
        {
            //returnData json
            try
            {
                string[] temp = returnData.Split(':');
                string sqlFormattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string cmdText = $"insert into quotes (id_package, package, send_date) values ({temp[0]}, {temp[1]}, '{sqlFormattedDate}')";

                sqlMaster.ExecuteNonQueryCommand(cmdText);
            }
            catch (Exception e)
            {
                //nLog.Error(e);
                Console.WriteLine(e);
            }
        }

        #region test
        //private void TestLostPackages()
        //{
        //    using SqlConnection connection = new SqlConnection(connectionString);
        //    using (SqlCommand command = new SqlCommand("LostPackages", connection))
        //    {
        //        command.CommandType = CommandType.StoredProcedure;

        //        var returnParameter = command.Parameters.Add("@retValue", SqlDbType.Int);
        //        returnParameter.Direction = ParameterDirection.ReturnValue;

        //        connection.Open();
        //        command.ExecuteNonQuery();

        //        int retval = (int)command.Parameters["@retValue"].Value;
        //        ConsoleOutput.Print("Количество потерянных пакетов", retval);
        //    }
        //}
        #endregion
    }
}
