using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntGraphLab8
{
    public enum UserType { None, Operator, Manager, Admin }

    public class User
    {
        public UserType UserStatus { get; set; }
        
        public User(UserType type)
        {
            UserStatus = type;
        }
    }
}
