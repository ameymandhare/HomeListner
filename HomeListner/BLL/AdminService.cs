using HomeListner.DAL;
using HomeListner.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeListner.BLL
{
    public sealed class AdminService
    {
        private AdminDataAcess adminDataAcess;

        public AdminService()
        {
            adminDataAcess = new AdminDataAcess();
        }

        public IList<Log> GetLogs()
        {
            return adminDataAcess.GetLogs();
        }
    }
}
