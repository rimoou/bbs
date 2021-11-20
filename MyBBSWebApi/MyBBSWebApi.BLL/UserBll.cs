﻿using MyBBSWebApi.Common;
using MyBBSWebApi.DAL;
using MyBBSWebApi.Models;
using System;
using System.Collections.Generic;

namespace MyBBSWebApi.BLL
{
    public class UserBll : IUserBll
    {
        UserDal userDal = new UserDal();

        public List<Users> GetAll()
        {
            return userDal.GetAll().FindAll(m => !m.IsDelete);
        }

        public Users CheckLogin(string userNo, string password)
        {
            List<Users> userlist = userDal.GetUserByUserNoAndPassword(userNo, password.ToMd5());
            if (userlist.Count <= 0)
            {
                 userlist = userDal.GetUserByUserNoAndAutoLoginTag(userNo,password);
                if(userlist.Count<=0)
                {
                    return default;
                }
                else
                {
                    return GetLoginResult(userlist,false);
                }

            }
            else
            {
                return GetLoginResult(userlist,true);
            }

        }


        private Users GetLoginResult(List<Users> userlist,bool isPasswordLogin)
        {
            Users user = userlist.Find(m => !m.IsDelete);
                user.Token = Guid.NewGuid();

                //用户使用密码登录则创建新的atuologintag
                if(isPasswordLogin)
                {
                    user.AutoLoginTag = Guid.NewGuid();
                }
                UpdateUser(user.Id, user.UserNo, user.UserName, user.Password, user.UserLevel, user.Token,user.AutoLoginTag);
                return user;
        }


        public string AddUser(string UserNo, string UserName, int Userlevel, string Password)
        {
            
            int effectedRows = userDal.AddUser(UserNo, UserName, Userlevel, Password);

            if (effectedRows > 0)
            {
                return "success!";
            }
            else
            {

                return "false!";
            }
        }


        public string UpdateUser(int id, string userNo, string userName, string password, int? userLevel, Guid? token,Guid? autoLoginTag)
        {

            
            int effectedRows = userDal.UpdateUser(id, userNo, userName, password, userLevel, token,autoLoginTag);
            if (effectedRows > 0)
            {
                return "success!";
            }
            else
            {
                return "false!";
            }
        }


        public string RemoveUser(int id)
        {
            
            int effectedRows = userDal.RemoveUser(id);
            if (effectedRows > 0)
            {
                return "success!";
            }
            else
            {
                return "false!";
            }
        }

        public Users GetUsersByToken(string token)
        {
            Users user = userDal.GetUserByToken(token);
            if (user == null)
            {
                throw new Exception("token错误");
            }
            
            return user;
        }
    }
}
