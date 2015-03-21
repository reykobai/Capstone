using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ProjectCollaborationSystem.Models;

namespace ProjectCollaborationSystem.App_Code
{
    [HubName("chatHub")]
    public class MyHub : Hub
    {
        public void SendToGroup(string groupname,string username,string message)
        {
            string servertime = DateTime.Now.ToShortTimeString();
            // Send the message to the clients that belong to the same group
            //username = DateTime.Now.ToShortTimeString() + " " + username;
            //message = string.Format("Group name: {0}, message: {1}", Context.ConnectionId, message);
            Clients.Group(groupname).broadcastGroupMessage(username,message,servertime);

        }


        public void SetGroupChat(string groupname)
        {
            Groups.Add(Context.ConnectionId, groupname);
        }
    }
}