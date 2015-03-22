using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ProjectCollaborationSystem.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace ProjectCollaborationSystem.App_Code
{
    [HubName("chatHub")]
    public class MyHub : Hub
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public void SendToGroup(string groupname, string username, string message, string ProjectID, string InformationID)
        {
            string servertime = DateTime.Now.ToShortTimeString();
            // Send the message to the clients that belong to the same group
            //username = DateTime.Now.ToShortTimeString() + " " + username;
            //message = string.Format("Group name: {0}, message: {1}", Context.ConnectionId, message);
            Clients.Group(groupname).broadcastGroupMessage(username, message, servertime);

            string iQuery = "INSERT INTO Chat (ProjectID,InformationID, Message) VALUES (@projectid, @informationid, @message) ";
            using (SqlConnection conn = new SqlConnection(connString))
          {
                    using (SqlCommand comm = new SqlCommand(iQuery, conn))
                    {
                        comm.Parameters.AddWithValue("projectid", ProjectID);
                        comm.Parameters.AddWithValue("informationid", InformationID);
                        comm.Parameters.AddWithValue("message", message);

                        try
                        {

                            conn.Open();
                            comm.ExecuteNonQuery();

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
           

        }
        public void SetGroupChat(string groupname)
        {
            Groups.Add(Context.ConnectionId, groupname);
        }
    }
}