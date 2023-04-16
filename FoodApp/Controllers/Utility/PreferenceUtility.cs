﻿using FoodApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;

namespace FoodApp.Controllers.Utility
{
    public partial class MainUtility
    {
        private void GetUserMenuChoice()
        {
            throw new NotImplementedException();
        }

        public static bool SetUserPreferences(FormSelection model)
        {
            try
            {
                string emailAddress = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("preferred_username").Value;
                var db = new DbContext();
                string readStatement = $"SELECT * FROM EVENTS WHERE active = 1";
                Debug.WriteLine(readStatement);
                var result = db.ExecuteQuery(readStatement, null).Rows;
                var userIDResult = db.ExecuteQuery($"SELECT user_id FROM USERS WHERE user_email = '{emailAddress}'", null).Rows;
                int userID = (int)userIDResult[0][0];
                if (model.Days == 4)
                {
                    return true;
                }
                foreach (DataRow row in result)
                {
                    if ((int)row[2] == 1 && (model.Days == 1 || model.Days == 3))
                    {
                        db.ExecuteNonQuery($"INSERT INTO BOOKINGS (user_id,event_id,cuisine_options_id) VALUES ({userID}, {(int)row[0]}, {model.wedFood})", null);
                    }
                    else if ((int)row[2] == 2 && (model.Days == 2 || model.Days == 3))
                    {
                        db.ExecuteNonQuery($"INSERT INTO BOOKINGS (user_id,event_id,cuisine_options_id) VALUES ({userID}, {(int)row[0]}, {model.thursFood})", null);
                    }
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void GetUserPreferences()
        {
            throw new NotImplementedException();
        }

        private void GetWeeklyMenu()
        {
            throw new NotImplementedException();
        }
    }
}