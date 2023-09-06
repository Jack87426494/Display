using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SocketGameProtocol;

namespace Server.Data
{
    internal class UserData
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public bool Register(MainPack pack,MySqlConnection sqlConnection)
        {
            string userName = pack.LoginPack.UserName;
            string password = pack.LoginPack.Password;
            try
            {
                //string sql = $"SELECT * FROM sys.userdata where userName={userName}";
                //MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlConnection);
                //MySqlDataReader read = mySqlCommand.ExecuteReader();
                //if(read.Read())
                //{
                //    Console.WriteLine("用户已被注册");
                //    return false;
                //}

                string sql = $"INSERT INTO sys.userdata (userName, password) VALUES ({userName}, {password})";

                MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlConnection);

                mySqlCommand.ExecuteNonQuery();

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("注册出错" + ex.Message);
                return false;
            }
           
        }

        public bool LogIn(MainPack pack,MySqlConnection sqlConnection)
        {
            string userName = pack.LoginPack.UserName;
            string password = pack.LoginPack.Password;
            try
            {
                string sql = $"SELECT * FROM sys.userdata WHERE userName={userName} AND password={password}";

                MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlConnection);

                using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("注册出错" + ex.Message);
                return false;
            }
        }
    }
}
