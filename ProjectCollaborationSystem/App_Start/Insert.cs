using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCollaborationSystem.App_Start;
using ProjectCollaborationSystem.Models;
using System.Net.Mail;

namespace ProjectCollaborationSystem.App_Start
{
    public class Insert
    {
        public string first { get; set; }
        public string last { get; set; }
        public string user { get; set; }
        public string iPath { get; set; }
        public string taskstatus { get; set; }
        public string subtaskstatus { get; set; }
        public int projectid { get; set; }
        public int infoID { get; set; }

        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public void signup(string fname, string lname, string contact, string email, string username, string pass, string specialty, string imagePath)
        {



            string iQuery = "INSERT INTO Information (FirstName,LastName,ContactNumber,EmailAddress,Username,Pword,Specialty,ImagePath) VALUES (@fname,@lname,@contact,@email,@username,@pass,@specialty,@imagePath)";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("fname", fname);
                    comm.Parameters.AddWithValue("lname", lname);
                    comm.Parameters.AddWithValue("contact", contact);
                    comm.Parameters.AddWithValue("email", email);
                    comm.Parameters.AddWithValue("username", username);
                    comm.Parameters.AddWithValue("pass", pass);
                    comm.Parameters.AddWithValue("specialty", specialty);
                    comm.Parameters.AddWithValue("imagePath", imagePath);


                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                        first = fname;
                        user = username;

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


            string checkInvitequery = "SELECT * FROM Invite WHERE EmailAdd = @email ";

            using (SqlConnection projConn = new SqlConnection(connString))
            {
                using (SqlCommand projComm = new SqlCommand(checkInvitequery, projConn))
                {
                    projComm.Parameters.AddWithValue("email", email);

                    try
                    {
                        projConn.Open();



                        using (SqlDataReader projrdr = projComm.ExecuteReader())
                        {

                            while (projrdr.Read())
                            {

                                projectid = Convert.ToInt32(projrdr["ProjectID"]);

                                deletefrominvte(email);
                                Information infoid = new Information();
                                string iQueryy = "SELECT TOP 1 * FROM Information ORDER BY InformationID DESC ";

                                using (SqlConnection connn = new SqlConnection(connString))
                                {
                                    using (SqlCommand commm = new SqlCommand(iQueryy, connn))
                                    {
                                        try
                                        {
                                            connn.Open();



                                            using (SqlDataReader reder = commm.ExecuteReader())
                                            {

                                                while (reder.Read())
                                                {
                                                    infoid.InformationID = Convert.ToInt32(reder["InformationID"]);
                                                    infoid.FirstName = reder["FirstName"].ToString();
                                                    infoid.LastName = reder["LastName"].ToString();
                                                    infoid.EmailAddress = reder["EmailAddress"].ToString();



                                                    string action = infoid.FirstName + " " + infoid.LastName + " (" + infoid.EmailAddress + ") joined the project.";

                                                    ProjectHistoryJoin(projectid, action);
                                                }


                                            }

                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }
                                        finally
                                        {
                                            connn.Close();
                                        }
                                    }
                                }


                                insertTeam(infoid.InformationID, projectid);

                            }

                        }





                    }



                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        projConn.Close();
                    }
                }
            }
        }

        //invite

        public void Invite(string projectassoc, int projectid)
        {
            string iQueryyy = "INSERT INTO Invite (EmailAdd, ProjectID) VALUES (@projassoc, @projctID)";

            using (SqlConnection connnn = new SqlConnection(connString))
            {
                using (SqlCommand commmm = new SqlCommand(iQueryyy, connnn))
                {
                    commmm.Parameters.AddWithValue("projassoc", projectassoc);
                    commmm.Parameters.AddWithValue("projctID", projectid);



                    try
                    {
                        connnn.Open();
                        commmm.ExecuteNonQuery();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connnn.Close();
                    }
                }
            }
        }

        public void InsertProject(string projtitle, string projdesc, string projprio, DateTime projdue, string projstatus, int informID, string UploadFile)
        {



            string iQuery = "INSERT INTO Project (ProjectTitle,DueDate,ProjectDescription,ProjectStatus,ProjectPriority,InformationID, FileUploaded) VALUES (@projtitle,@projdue,@projdesc,@projstatus,@projprio,@informID,@FileUploaded)";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("projtitle", projtitle);
                    comm.Parameters.AddWithValue("projdue", projdue);
                    comm.Parameters.AddWithValue("projdesc", projdesc);
                    comm.Parameters.AddWithValue("projstatus", projstatus);
                    comm.Parameters.AddWithValue("projprio", projprio);
                    comm.Parameters.AddWithValue("informID", informID);
                    comm.Parameters.AddWithValue("FileUploaded", UploadFile);


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


        //addtask

        public void InsertTask(string tasktitle, string taskdesc, string taskprio, DateTime taskdue, string informID, int projectID, string taskUploadFile)
        {
            taskstatus = "Open";

            string iQuery = "INSERT INTO Task (TaskTitle,DueDate,TaskDescription,TaskStatus,TaskPriority,InformationID,ProjectID, FileUploaded) VALUES (@tasktitle,@taskdue,@taskdesc,@taskstatus,@taskprio,@informID,@projectID,@taskUploadFile)";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("tasktitle", tasktitle);
                    comm.Parameters.AddWithValue("taskdue", taskdue);
                    comm.Parameters.AddWithValue("taskdesc", taskdesc);
                    comm.Parameters.AddWithValue("taskstatus", taskstatus);
                    comm.Parameters.AddWithValue("taskprio", taskprio);
                    comm.Parameters.AddWithValue("informID", informID);
                    comm.Parameters.AddWithValue("projectID", projectID);
                    comm.Parameters.AddWithValue("taskUploadFile", taskUploadFile);


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

        public void UploadFile(string title, HttpPostedFileBase taskUploadFile)
        {

            //upload file


            if (taskUploadFile != null && taskUploadFile.ContentLength > 0)
                try
                {
                    string filename = Path.GetFileName(taskUploadFile.FileName);
                    iPath = "Files/" + title + "/" + filename;
                    // string path = Path.Combine(Server.MapPath("~/Files"), Path.GetFileName(taskUploadFile.FileName));
                    taskUploadFile.SaveAs(iPath);



                }
                catch (Exception)
                {

                }
            else
            {

            }

        }

        //subtask



        internal void InsertSubTask(string subtasktitle, string subtaskdesc, string subtaskprio, DateTime subtaskdue, string subtaskassign, int id, int ProjectID, int TaskID, string iPath)
        {
            subtaskstatus = "Open";

            string iQuery = "INSERT INTO Subtask (SubtaskTitle,DueDate,SubtaskDescription,SubtaskStatus,SubtaskPriority,InformationID,ProjectID, TaskID, FileUploaded) VALUES (@SubtaskTitle,@Subtaskdue,@Subtaskdesc,@Subtaskstatus,@Subtaskprio,@subtaskassign,@projectID,@TaskID, @SubtaskUploadFile)";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("SubtaskTitle", subtasktitle);
                    comm.Parameters.AddWithValue("Subtaskdue", subtaskdue);
                    comm.Parameters.AddWithValue("Subtaskdesc", subtaskdesc);
                    comm.Parameters.AddWithValue("Subtaskstatus", subtaskstatus);
                    comm.Parameters.AddWithValue("Subtaskprio", subtaskprio);
                    comm.Parameters.AddWithValue("subtaskassign", subtaskassign);
                    comm.Parameters.AddWithValue("informID", id);
                    comm.Parameters.AddWithValue("projectID", ProjectID);
                    comm.Parameters.AddWithValue("SubtaskUploadFile", iPath);
                    comm.Parameters.AddWithValue("TaskID", TaskID);

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


        //insert assoc


        public void insertAssoc(string projassoc1, int projectID, string title, string fname, string lname, int informID)
        {
            string loginQueryy = "SELECT * FROM Information where EmailAddress=@projassoc1";

            Information info = new Information();
            using (SqlConnection connn = new SqlConnection(connString))
            {
                using (SqlCommand commm = new SqlCommand(loginQueryy, connn))
                {
                    commm.Parameters.AddWithValue("projassoc1", projassoc1);


                    try
                    {
                        connn.Open();

                        using (SqlDataReader rdr = commm.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                info.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                info.FirstName = rdr["FirstName"].ToString();
                                info.LastName = rdr["LastName"].ToString();

                                string action = info.FirstName + " " + info.LastName + " was added";
                                ProjectHistory(projectID, informID, action);


                                insertTeam(info.InformationID, projectID);

                                MailMessage mailMessage = new MailMessage("koalawares@gmail.com", projassoc1);
                                mailMessage.Subject = "Koalaboration Invite";
                                mailMessage.Body = "You are being invited by " + fname + " " + lname + " to a project entitled " + title + " " + "Click this link to log in. http://localhost:2365/Login/Login. ";

                                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                                smtpClient.Credentials = new System.Net.NetworkCredential()
                                {
                                    UserName = "koalawares@gmail.com",
                                    Password = "Pasword21"
                                };

                                smtpClient.EnableSsl = true;
                                smtpClient.Send(mailMessage);

                            }

                            else
                            {

                                string action = projassoc1 + " was added";
                                ProjectHistory(projectID, informID, action);

                                Invite(projassoc1, projectID);
                                MailMessage mailMessage = new MailMessage("koalawares@gmail.com", projassoc1);
                                mailMessage.Subject = "Koalaboration Invite";
                                mailMessage.Body = "You are being invited by " + fname + " " + lname + " to a project entitled " + title + " " + "Click this link to register. http://localhost:2365/Login/SignUp. ";

                                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                                smtpClient.Credentials = new System.Net.NetworkCredential()
                                {
                                    UserName = "koalawares@gmail.com",
                                    Password = "Pasword21"
                                };

                                smtpClient.EnableSsl = true;
                                smtpClient.Send(mailMessage);
                            }



                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        connn.Close();
                    }
                }


            }



        }

        //insert team
        public void insertTeam(int infoID, int projectid)
        {
            string iQueryyy = "INSERT INTO Team (ProjectID, InformationID) VALUES ( @projectID, @infoID)";

            using (SqlConnection connnn = new SqlConnection(connString))
            {
                using (SqlCommand commmm = new SqlCommand(iQueryyy, connnn))
                {
                    commmm.Parameters.AddWithValue("infoID", infoID);
                    commmm.Parameters.AddWithValue("projectID", projectid);



                    try
                    {
                        connnn.Open();
                        commmm.ExecuteNonQuery();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connnn.Close();
                    }
                }
            }
        }

        public void AddCommentProj(string commentMessage, int projectID, int informationID)
        {
            string stat = "Active";
            string commentQuery = "INSERT INTO Comment(CommentMessage,ProjectID,InformationID,CommentStatus) VALUES (@CommentMessage,@ProjectID,@InformationID,@stat)";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(commentQuery, conn))
                {
                    comm.Parameters.AddWithValue("CommentMessage", commentMessage);
                    comm.Parameters.AddWithValue("ProjectID", projectID);
                    comm.Parameters.AddWithValue("InformationID", informationID);
                    comm.Parameters.AddWithValue("stat", stat);

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

        public void AddCommentTask(string commentMessage, int TaskID, int informationID)
        {
            string stat = "Active";
            string commentQuery = "INSERT INTO Comment(CommentMessage,InformationID,TaskID,CommentStatus) VALUES (@CommentMessage,@InformationID,@TaskID,@stat)";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(commentQuery, conn))
                {
                    comm.Parameters.AddWithValue("CommentMessage", commentMessage);
                    comm.Parameters.AddWithValue("TaskID", TaskID);
                    comm.Parameters.AddWithValue("InformationID", informationID);
                    comm.Parameters.AddWithValue("stat", stat);

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


        public void AddCommentSubtask(string commentMessage, int SubtaskID, int informationID)
        {
            string stat = "Active";
            string commentQuery = "INSERT INTO Comment(CommentMessage,InformationID,SubtaskID,CommentStatus) VALUES (@CommentMessage,@InformationID,@SubtaskID,@stat)";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(commentQuery, conn))
                {
                    comm.Parameters.AddWithValue("CommentMessage", commentMessage);
                    comm.Parameters.AddWithValue("SubtaskID", SubtaskID);
                    comm.Parameters.AddWithValue("InformationID", informationID);
                    comm.Parameters.AddWithValue("stat", stat);

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

        public void DeleteComment(int id)
        {
            string stat = "Deleted";
            string query = "UPDATE Comment SET CommentStatus=@stat WHERE CommentID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);
                    comm.Parameters.AddWithValue("stat", stat);

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

        public void deletefrominvte(string email)
        {

            string query = "DELETE FROM Invite WHERE EmailAdd=@email";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("email", email);


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


        public void DeleteAssoc(int id)
        {
            string delQuery = "DELETE FROM Team WHERE InformationID = @id";
            
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(delQuery,conn))
                {
                    comm.Parameters.AddWithValue("id", id);

                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch(Exception)
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


        public void UpdateProject(string projtitle, string projdesc, string projprio, DateTime projdue, int id)
        {
            string query = "UPDATE Project SET ProjectTitle=@ProjectTitle, ProjectDescription=@ProjectDescription, ProjectPriority=@ProjectPriority, DueDate=@DueDate WHERE ProjectID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("ProjectTitle", projtitle);
                    comm.Parameters.AddWithValue("ProjectDescription", projdesc);
                    comm.Parameters.AddWithValue("ProjectPriority", projprio);
                    comm.Parameters.AddWithValue("DueDate", projdue);
                    comm.Parameters.AddWithValue("id", id);


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

        public void ResolveProject(int id)
        {
            string query = "UPDATE Project SET ProjectStatus='Resolved' WHERE ProjectID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


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

        public void ReOpenProject(int id)
        {
            string query = "UPDATE Project SET ProjectStatus='Re-open' WHERE ProjectID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


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

        public void ProgressProject(int id)
        {
            string query = "UPDATE Project SET ProjectStatus='In Progress' WHERE ProjectID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


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
        //task
        public void ResolveTask(int id)
        {
            string query = "UPDATE Task SET TaskStatus='Resolved' WHERE TaskID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


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

        public void ReOpenTask(int id)
        {
          
            string query = "UPDATE Task SET TaskStatus='Re-open' WHERE TaskID=@id";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


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

        public void ProgressTask(int id)
        {
           
            string query = "UPDATE Task SET TaskStatus='In Progress' WHERE TaskID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


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

        //subtask
        public void ResolveSubtask(int id)
        {
            string query = "UPDATE Subtask SET SubtaskStatus='Resolved' WHERE SubtaskID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


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

        public void ReOpenSubtask(int id)
        {

            string query = "UPDATE Subtask SET SubtaskStatus='Re-open' WHERE SubtaskID=@id";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


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

        public void ProgressSubtask(int id)
        {

            string query = "UPDATE Subtask SET SubtaskStatus='In Progress' WHERE SubtaskID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


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


        public void UpdateSubtask(string subtasktitle, string subtaskdesc, string subtaskprio, DateTime subtaskdue, int id, int subtaskassign)
        {
            string query = "UPDATE Subtask SET SubtaskTitle=@SubtaskTitle, SubtaskDescription=@SubtaskDescription, SubtaskPriority=@SubtaskPriority, DueDate=@DueDate, InformationID=@InformationID WHERE SubtaskID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("SubtaskTitle", subtasktitle);
                    comm.Parameters.AddWithValue("SubtaskDescription", subtaskdesc);
                    comm.Parameters.AddWithValue("SubtaskPriority", subtaskprio);
                    comm.Parameters.AddWithValue("DueDate", subtaskdue);
                    comm.Parameters.AddWithValue("id", id);
                    comm.Parameters.AddWithValue("InformationID", subtaskassign);



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


        public void UpdateTask(string tasktitle, string taskdesc, string taskprio, DateTime taskdue, int id, int taskassign)
        {
            string query = "UPDATE Task SET TaskTitle=@TaskTitle, TaskDescription=@TaskDescription, TaskPriority=@TaskPriority, DueDate=@DueDate, InformationID=@InformationID WHERE TaskID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.AddWithValue("TaskTitle", tasktitle);
                    comm.Parameters.AddWithValue("TaskDescription", taskdesc);
                    comm.Parameters.AddWithValue("TaskPriority", taskprio);
                    comm.Parameters.AddWithValue("DueDate", taskdue);
                    comm.Parameters.AddWithValue("id", id);
                    comm.Parameters.AddWithValue("InformationID", taskassign);



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

        public void UpdateTaskAssignee(int id, int taskassign)
        {
            string query = "UPDATE Task SET  InformationID=@InformationID WHERE TaskID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    
                    comm.Parameters.AddWithValue("id", id);
                    comm.Parameters.AddWithValue("InformationID", taskassign);



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

        public void editSubtaskAssignee(int id, int taskassign)
        {
            string query = "UPDATE Subtask SET  InformationID=@InformationID WHERE SubtaskID=@id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(query, conn))
                {

                    comm.Parameters.AddWithValue("id", id);
                    comm.Parameters.AddWithValue("InformationID", taskassign);



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

        public void ProjectHistory(int projectID, int informationID, string action)
        {
            string histStat = "History";
            string iQueryyy = "INSERT INTO History (ProjectID, InformationID, Action, HistoryStat) VALUES ( @projectID, @informationID,@action, @historyStat)";

            using (SqlConnection connnn = new SqlConnection(connString))
            {
                using (SqlCommand commmm = new SqlCommand(iQueryyy, connnn))
                {
                    commmm.Parameters.AddWithValue("informationID", informationID);
                    commmm.Parameters.AddWithValue("projectID", projectID);
                    commmm.Parameters.AddWithValue("action", action);
                    commmm.Parameters.AddWithValue("historyStat", histStat);



                    try
                    {
                        connnn.Open();
                        commmm.ExecuteNonQuery();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connnn.Close();
                    }
                }
            }
           
        }

        public void ProjectHistoryJoin(int projectID, string action)
        {
            string histStat = "join";
            string iQueryyy = "INSERT INTO History (ProjectID, Action, HistoryStat) VALUES ( @projectID, @action,@historyStat )";

            using (SqlConnection connnn = new SqlConnection(connString))
            {
                using (SqlCommand commmm = new SqlCommand(iQueryyy, connnn))
                {
             
                    commmm.Parameters.AddWithValue("projectID", projectID);
                    commmm.Parameters.AddWithValue("action", action);
                    commmm.Parameters.AddWithValue("historyStat", histStat);




                    try
                    {
                        connnn.Open();
                        commmm.ExecuteNonQuery();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connnn.Close();
                    }
                }
            }

        }

        public void TaskHistory(int taskID, int informationID, string action)
        {
            string histStat = "History";
            string iQueryyy = "INSERT INTO History (TaskID, InformationID, Action, HistoryStat) VALUES ( @taskID, @informationID,@action,@historyStat )";

            using (SqlConnection connnn = new SqlConnection(connString))
            {
                using (SqlCommand commmm = new SqlCommand(iQueryyy, connnn))
                {
                    commmm.Parameters.AddWithValue("informationID", informationID);
                    commmm.Parameters.AddWithValue("taskID", taskID);
                    commmm.Parameters.AddWithValue("action", action);
                    commmm.Parameters.AddWithValue("historyStat", histStat);


                    try
                    {
                        connnn.Open();
                        commmm.ExecuteNonQuery();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connnn.Close();
                    }
                }
            }
        
        }

        public void SubtaskHistory(int subtaskID, int informationID, string action)
        {
            string histStat = "History";
            string iQueryyy = "INSERT INTO History (SubtaskID, InformationID, Action, HistoryStat) VALUES ( @subtaskID, @informationID,@action,@histStat)";

            using (SqlConnection connnn = new SqlConnection(connString))
            {
                using (SqlCommand commmm = new SqlCommand(iQueryyy, connnn))
                {
                    commmm.Parameters.AddWithValue("informationID", informationID);
                    commmm.Parameters.AddWithValue("subtaskID", subtaskID);
                    commmm.Parameters.AddWithValue("action", action);
                    commmm.Parameters.AddWithValue("histStat", histStat);



                    try
                    {
                        connnn.Open();
                        commmm.ExecuteNonQuery();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connnn.Close();
                    }
                }
            }

        }

        //endparse

        
    }



}