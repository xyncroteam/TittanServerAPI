﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using wscore.Entities;

namespace newapi.Helpers
{
    public static class Utils
    {
        public static void Map(User userModel ,  RegisterUserRequest user, string type)
        {
            if(type == "Create")
            {
                userModel.FirstName = user.FirstName;
                userModel.LastName = user.LastName;
                userModel.Username = user.Username;
                userModel.Email = user.Email;
                userModel.Password = user.Password;
                userModel.Group = user.Group;
            }
            else
            {
                Console.WriteLine("Im here");
            }
        }
    }
}
