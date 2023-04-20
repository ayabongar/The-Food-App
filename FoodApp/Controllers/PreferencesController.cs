﻿using FoodApp.Controllers.Utility;
using FoodApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FoodApp.Controllers
{
    public class PreferencesController : Controller
    {
        
        public ActionResult Preferences()
        {
            
            //query to retrieve user id and the user's settings row 
            string emailAddress = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("preferred_username").Value;
            var db = new DbContext();
            string userQuery = $"SELECT user_id FROM Users WHERE user_email = '{emailAddress}'";
            var userResult = db.ExecuteQuery(userQuery, null);
            DataRow res1 = userResult.Rows[0];
            int userId = res1.Field<int>("user_id");

            string prefQuery = $"SELECT user_id, day_id, preference_id from Settings WHERE user_id = {userId}";
            var prefResult = db.ExecuteQuery(prefQuery, null);

            // Create the view model and set any existing preferences
            var viewModel = new FormSelection();
            if (prefResult.Rows.Count > 0)
            {
                
                DataRow prefRow = prefResult.Rows[0];
                ViewData["DietryRequirements"] = prefRow.Field<int>("preference_id").ToString();
                ViewData["Days"]= viewModel.Days = prefRow.Field<int>("day_id");
                //viewModel.DietaryRequirements = prefRow.Field<int>("preference_id").ToString();
                //viewModel.Days = prefRow.Field<int>("day_id");
                
            }

            var menu = MainUtility.GetWeeklyMenu();
            ViewData["wednesdayOptions"] = menu[0];
            ViewData["thursdayOptions"] = menu[1];
            return View(viewModel);
        }
        

     

        [Authorize]
        public ActionResult SavePreferences(FormSelection model)
        {

            string userfullname = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("name").Value;
            string emailAddress = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("preferred_username").Value;
            Debug.WriteLine(userfullname);
            Debug.WriteLine(emailAddress);

            var db = new DbContext();
            string userQuery = $"SELECT user_id FROM Users WHERE user_email = '{emailAddress}'";
            var userResult = db.ExecuteQuery(userQuery, null);
            DataRow res1 = userResult.Rows[0];
            int userId = res1.Field<int>("user_id");
     
            string prefQuery = $"SELECT preference_id FROM Preferences WHERE preference_type = '{model.DietaryRequirements.ToString()}'";
            var prefResult = db.ExecuteQuery(prefQuery, null);
            DataRow res2 = prefResult.Rows[0];
            int preferenceId = res2.Field<int>("preference_id");
            int dayId = model.Days;

            // check if we should update or insert
            string checkQuery = $"SELECT COUNT(*) FROM Settings WHERE user_id = {userId}";
            var checkResult = db.ExecuteQuery(checkQuery, null);
            if (checkResult.Rows[0][0] != DBNull.Value && Convert.ToInt32(checkResult.Rows[0][0]) > 0)
            {
                string updateQuery = $"UPDATE Settings SET preference_id = {preferenceId}, day_id = {dayId} WHERE user_id = {userId}";
                var updateResult = db.ExecuteQuery(updateQuery, null);
            }
            else
            {
                string insertQuery = $"INSERT INTO Settings VALUES ({userId}, {dayId}, {preferenceId})";
                var insertResult = db.ExecuteQuery(insertQuery, null);
            }     
            
            //Redirect to menu
            MainUtility.SetUserPreferences(model);
            return RedirectToAction("/SubmitPreferences");
        }

        public ActionResult SubmitPreferences()
        {
            return View();
        }
    }
}
