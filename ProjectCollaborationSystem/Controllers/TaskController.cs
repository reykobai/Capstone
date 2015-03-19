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

namespace ProjectCollaborationSystem.Controllers
{

    public class TaskController : Controller
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        public int iID { get; set; }
        public string iPath { get; set; }
        public int taskid { get; set; }
        //
        // GET: /Task/




        public ActionResult Index(string id)
        {

            List<Information> inform = new List<Information>();

            string iQueryy = "SELECT * FROM Team WHERE ProjectID=@id";

            using (SqlConnection connn = new SqlConnection(connString))
            {
                using (SqlCommand commm = new SqlCommand(iQueryy, connn))
                {
                    commm.Parameters.AddWithValue("id", id);
                    try
                    {
                        connn.Open();



                        using (SqlDataReader reder = commm.ExecuteReader())
                        {

                            while (reder.Read())
                            {
                                Team teams = new Team();


                                teams.ProjectID = Convert.ToInt32(reder["ProjectID"]);
                                teams.InformationID = Convert.ToInt32(reder["InformationID"]);


                                //information---------------------------

                                string InfoQuery = "SELECT * FROM Information WHERE InformationID=@id";

                                using (SqlConnection InfoConn = new SqlConnection(connString))
                                {
                                    using (SqlCommand InfoComm = new SqlCommand(InfoQuery, InfoConn))
                                    {
                                        InfoComm.Parameters.AddWithValue("id", teams.InformationID);
                                        try
                                        {
                                            InfoConn.Open();



                                            using (SqlDataReader Infordr = InfoComm.ExecuteReader())
                                            {

                                                while (Infordr.Read())
                                                {
                                                    Information information = new Information();


                                                    information.FirstName = Infordr["FirstName"].ToString();
                                                    information.LastName = Infordr["LastName"].ToString();
                                                    information.EmailAddress = Infordr["EmailAddress"].ToString();
                                                    information.InformationID = Convert.ToInt32(Infordr["InformationID"]);


                                                    inform.Add(information);


                                                }


                                            }


                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }
                                        finally
                                        {
                                            InfoConn.Close();
                                        }
                                    }
                                }


                                //end info

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




            return View(inform);


        }





        [HttpPost]
        public ActionResult Index( int id, string tasktitle, string taskdesc, string taskprio, DateTime taskdue, string taskassign, HttpPostedFileBase taskUploadFile)
        {
            Project project = new Project();
            //project
            string iQueryProject = "SELECT * FROM Project WHERE ProjectID = @ProjectID";

            using (SqlConnection connProj = new SqlConnection(connString))
            {
                using (SqlCommand commProj = new SqlCommand(iQueryProject, connProj))
                {
                    commProj.Parameters.AddWithValue("ProjectID", id);
                    try
                    {
                        connProj.Open();

                        using (SqlDataReader rdrProj = commProj.ExecuteReader())
                        {
                            while (rdrProj.Read())
                            {
                                

                                project.ProjectID = Convert.ToInt32(rdrProj["ProjectID"]);
                                project.ProjectTitle = rdrProj["ProjectTitle"].ToString();
                                project.DueDate = Convert.ToDateTime(rdrProj["DueDate"]);
                                project.ProjectStatus = rdrProj["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdrProj["DateAdded"]);
                                project.InformationID = Convert.ToInt32(rdrProj["InformationID"]);
                                project.FileUploaded = rdrProj["FileUploaded"].ToString();
                                project.ProjectPriority = rdrProj["ProjectPriority"].ToString();
                                project.ProjectDescription = rdrProj["ProjectDescription"].ToString();


                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connProj.Close();
                    }
                }
            }

            int informID = Convert.ToInt32(Session["id"]);

            Insert insertTask = new Insert();
            try
            {
                insertTask.UploadFile(tasktitle, taskUploadFile);
                iPath = insertTask.iPath;
            }
            catch (Exception)
            {
                throw;
            }

            if(project.DueDate < taskdue)
            {
                TempData["error"] = "The Date you entered is beyond the Project's Due Date.";
            }
            else
            {
                insertTask.InsertTask(tasktitle, taskdesc, taskprio, taskdue, taskassign, id, iPath);

                string iQueryyTask = "SELECT TOP 1 * FROM Task ORDER BY TaskID DESC ";

                using (SqlConnection connnTask = new SqlConnection(connString))
                {
                    using (SqlCommand commmTask = new SqlCommand(iQueryyTask, connnTask))
                    {
                        try
                        {
                            connnTask.Open();



                            using (SqlDataReader rederTask = commmTask.ExecuteReader())
                            {

                                while (rederTask.Read())
                                {
                                    taskid = Convert.ToInt32(rederTask["TaskID"]);

                                }


                            }


                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            connnTask.Close();
                        }
                    }
                }

                string action = "Task is created";
                int informedID = Convert.ToInt32(Session["id"]);
                insertTask.TaskHistory(taskid, informedID, action);
         


                return RedirectToAction("DisplayProjet", "Project", new { id = id });
            }

            //if error 
            List<Information> inform = new List<Information>();

            string iQueryy = "SELECT * FROM Team WHERE ProjectID=@id";

            using (SqlConnection connn = new SqlConnection(connString))
            {
                using (SqlCommand commm = new SqlCommand(iQueryy, connn))
                {
                    commm.Parameters.AddWithValue("id", id);
                    try
                    {
                        connn.Open();



                        using (SqlDataReader reder = commm.ExecuteReader())
                        {

                            while (reder.Read())
                            {
                                Team teams = new Team();


                                teams.ProjectID = Convert.ToInt32(reder["ProjectID"]);
                                teams.InformationID = Convert.ToInt32(reder["InformationID"]);


                                //information---------------------------

                                string InfoQuery = "SELECT * FROM Information WHERE InformationID=@id";

                                using (SqlConnection InfoConn = new SqlConnection(connString))
                                {
                                    using (SqlCommand InfoComm = new SqlCommand(InfoQuery, InfoConn))
                                    {
                                        InfoComm.Parameters.AddWithValue("id", teams.InformationID);
                                        try
                                        {
                                            InfoConn.Open();



                                            using (SqlDataReader Infordr = InfoComm.ExecuteReader())
                                            {

                                                while (Infordr.Read())
                                                {
                                                    Information information = new Information();


                                                    information.FirstName = Infordr["FirstName"].ToString();
                                                    information.LastName = Infordr["LastName"].ToString();
                                                    information.EmailAddress = Infordr["EmailAddress"].ToString();
                                                    information.InformationID = Convert.ToInt32(Infordr["InformationID"]);


                                                    inform.Add(information);


                                                }


                                            }


                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }
                                        finally
                                        {
                                            InfoConn.Close();
                                        }
                                    }
                                }


                                //end info

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




            return View(inform);
            
            //

         
           
            //insertProject.InsertProject(projtitle, projdesc, projprio, projdue, projstatus, informID, iPath);
            
        }



        public ActionResult DisplayTask(string id)
        {

            Task task = new Task();
            Project project = new Project();

            Information info = new Information();
            List<Information> infoing = new List<Information>();
            List<Subtask> subtasking = new List<Subtask>();

            string iQuery = "SELECT * FROM Task  WHERE TaskID = @TaskID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("TaskID", id);
                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                task.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                task.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                task.TaskID = Convert.ToInt32(rdr["TaskID"]);
                                task.TaskTitle = rdr["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                task.TaskDescription = rdr["TaskDescription"].ToString();
                                task.TaskStatus = rdr["TaskStatus"].ToString();
                                task.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                task.TaskPriority = rdr["TaskPriority"].ToString();
                                task.FileUploaded = rdr["FileUploaded"].ToString();

                            }
                        }
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

            //teaminfo


            List<Team_Information> teaminfos = new List<Team_Information>();
            string iQueryTeam = "SELECT * FROM Team team INNER JOIN Information info ON team.InformationID = info.InformationID WHERE team.ProjectID = @ProjectID";

            using (SqlConnection connTeam = new SqlConnection(connString))
            {
                using (SqlCommand commTeam = new SqlCommand(iQueryTeam, connTeam))
                {
                    commTeam.Parameters.AddWithValue("ProjectID", task.ProjectID);
                    try
                    {
                        connTeam.Open();

                        using (SqlDataReader rdrTeam = commTeam.ExecuteReader())
                        {
                            while (rdrTeam.Read())
                            {
                                Team_Information infoteam = new Team_Information();

                                Team teams = new Team();
                                teams.ProjectID = Convert.ToInt32(rdrTeam["ProjectID"]);
                                teams.InformationID = Convert.ToInt32(rdrTeam["InformationID"]);



                                Information infos = new Information();
                                infos.InformationID = Convert.ToInt32(rdrTeam["InformationID"]);

                                infoteam.inform = infos;
                                infoteam.teams = teams;

                                teaminfos.Add(infoteam);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connTeam.Close();
                    }
                }
            }

            //project
            string iQueryProject = "SELECT * FROM Project WHERE ProjectID = @ProjectID";

            using (SqlConnection connProj = new SqlConnection(connString))
            {
                using (SqlCommand commProj = new SqlCommand(iQueryProject, connProj))
                {
                    commProj.Parameters.AddWithValue("ProjectID", task.ProjectID);
                    try
                    {
                        connProj.Open();

                        using (SqlDataReader rdrProj = commProj.ExecuteReader())
                        {
                            while (rdrProj.Read())
                            {

                                project.ProjectID = Convert.ToInt32(rdrProj["ProjectID"]);
                                project.ProjectTitle = rdrProj["ProjectTitle"].ToString();
                                project.DueDate = Convert.ToDateTime(rdrProj["DueDate"]);
                                project.ProjectStatus = rdrProj["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdrProj["DateAdded"]);
                                project.InformationID = Convert.ToInt32(rdrProj["InformationID"]);
                                project.FileUploaded = rdrProj["FileUploaded"].ToString();
                                project.ProjectPriority = rdrProj["ProjectPriority"].ToString();
                                project.ProjectDescription = rdrProj["ProjectDescription"].ToString();
                              

                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connProj.Close();
                    }
                }
            }

            //info

            string iQueryInfo = "SELECT * FROM Information WHERE InformationID = @InformationID";

            using (SqlConnection conns = new SqlConnection(connString))
            {
                using (SqlCommand comms = new SqlCommand(iQueryInfo, conns))
                {
                    comms.Parameters.AddWithValue("InformationID", task.InformationID);
                    try
                    {
                        conns.Open();

                        using (SqlDataReader rdrs = comms.ExecuteReader())
                        {
                            while (rdrs.Read())
                            {

                                info.FirstName = rdrs["FirstName"].ToString();
                                info.LastName = rdrs["LastName"].ToString();
                                info.InformationID = Convert.ToInt32(rdrs["InformationID"]);

                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        conns.Close();
                    }
                }
            }

            //task


            List<Subtask_Information> taskinfo = new List<Subtask_Information>();

            string taskQuery = "SELECT * FROM Subtask t JOIN Information i on t.InformationID = i.InformationID WHERE t.TaskID = @proj";

            using (SqlConnection taskconn = new SqlConnection(connString))
            {
                using (SqlCommand taskcomm = new SqlCommand(taskQuery, taskconn))
                {
                    taskcomm.Parameters.AddWithValue("proj", task.TaskID);
                    try
                    {
                        taskconn.Open();
                        using (SqlDataReader taskrdr = taskcomm.ExecuteReader())
                        {
                            while (taskrdr.Read())
                            {
                                Subtask_Information taskinfos = new Subtask_Information();

                                Subtask tasked = new Subtask();

                                tasked.SubtaskID = Convert.ToInt32(taskrdr["SubtaskID"]);
                                tasked.SubtaskTitle = taskrdr["SubtaskTitle"].ToString();
                                tasked.DueDate = Convert.ToDateTime(taskrdr["DueDate"]);
                                tasked.SubtaskStatus = taskrdr["SubtaskStatus"].ToString();
                                tasked.DateAdded = Convert.ToDateTime(taskrdr["DateAdded"]);
                                tasked.InformationID = Convert.ToInt32(taskrdr["InformationID"]);
                                tasked.SubtaskPriority = taskrdr["SubtaskPriority"].ToString();

                                Information information = new Information();


                                information.FirstName = taskrdr["FirstName"].ToString();
                                information.LastName = taskrdr["LastName"].ToString();
                                information.EmailAddress = taskrdr["EmailAddress"].ToString();
                                information.InformationID = Convert.ToInt32(taskrdr["InformationID"]);

                                taskinfos.info = information;
                          
                                taskinfos.subtask = tasked;

                                //end info

                                taskinfo.Add(taskinfos);

                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        taskconn.Close();
                    }
                }
            }


            //comment

            List<Comment_Information> cis = new List<Comment_Information>();
            string commentQuery = "SELECT * FROM Comment c JOIN Information i ON c.InformationID = i.InformationID AND c.CommentStatus = 'Active' WHERE c.TaskID = @id ORDER BY c.DateAdded DESC";

            using (SqlConnection commentConn = new SqlConnection(connString))
            {
                using (SqlCommand commentComm = new SqlCommand(commentQuery, commentConn))
                {
                    commentComm.Parameters.AddWithValue("id", id);
                    try
                    {
                        commentConn.Open();



                        using (SqlDataReader commentrdr = commentComm.ExecuteReader())
                        {

                            while (commentrdr.Read())
                            {
                                Comment_Information ci = new Comment_Information();

                                Comment comments = new Comment();

                                comments.CommentID = Convert.ToInt32(commentrdr["CommentID"]);
                                comments.CommentMessage = commentrdr["CommentMessage"].ToString();
                                comments.CommentStatus = commentrdr["CommentStatus"].ToString();
                                comments.DateAdded = Convert.ToDateTime(commentrdr["DateAdded"]);


                                Information commentInfo = new Information();
                                commentInfo.InformationID = Convert.ToInt32(commentrdr["InformationID"]);
                                commentInfo.FirstName = commentrdr["FirstName"].ToString();
                                commentInfo.LastName = commentrdr["LastName"].ToString();
                                commentInfo.Username = commentrdr["Username"].ToString();
                                commentInfo.DateAdded = Convert.ToDateTime(commentrdr["DateAdded"]);
                                commentInfo.ImagePath = commentrdr["ImagePath"].ToString();
                      

                                ci.info = commentInfo;
                                ci.comments = comments;

                                cis.Add(ci);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        commentConn.Close();
                    }
                }
            }


            //History


            List<History_Information> his = new List<History_Information>();
            string historyQuery = "SELECT * FROM History c JOIN Information i ON c.InformationID = i.InformationID WHERE c.TaskID = @id ORDER BY c.DateAdded DESC";

            using (SqlConnection historyConn = new SqlConnection(connString))
            {
                using (SqlCommand historyComm = new SqlCommand(historyQuery, historyConn))
                {
                    historyComm.Parameters.AddWithValue("id", id);
                    try
                    {
                        historyConn.Open();



                        using (SqlDataReader historyrdr = historyComm.ExecuteReader())
                        {

                            while (historyrdr.Read())
                            {
                                History_Information hi = new History_Information();

                                History histowry = new History();


                                histowry.TaskID = Convert.ToInt32(historyrdr["TaskID"]);
                                histowry.InformationID = Convert.ToInt32(historyrdr["InformationID"]);
                                histowry.Action = historyrdr["Action"].ToString();
                                histowry.DateAdded = Convert.ToDateTime(historyrdr["DateAdded"]);

                                Information commentInfo = new Information();
                                commentInfo.InformationID = Convert.ToInt32(historyrdr["InformationID"]);
                                commentInfo.FirstName = historyrdr["FirstName"].ToString();
                                commentInfo.LastName = historyrdr["LastName"].ToString();
                                commentInfo.Username = historyrdr["Username"].ToString();
                                commentInfo.DateAdded = Convert.ToDateTime(historyrdr["DateAdded"]);
                                commentInfo.ImagePath = historyrdr["ImagePath"].ToString();


                                hi.history = histowry;
                                hi.information = commentInfo;

                                his.Add(hi);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        historyConn.Close();
                    }
                }
            }



            Information_Project ip = new Information_Project();
            ip.info = info;
            ip.tasking = task;
            ip.subtasking = subtasking;
            ip.information = infoing;
            ip.cinfo = cis;
            ip.subinfo = taskinfo;
            ip.project = project;
            ip.teaminfo = teaminfos;
            ip.Hist_inf = his;
            return View(ip);
        }

        [HttpPost]
        public ActionResult DisplayTask(int id, string CommentMessage)
        {
            int commentor = Convert.ToInt32(Session["id"]);
            Insert Addcomment = new Insert();
            Addcomment.AddCommentTask(CommentMessage, id, commentor);

            return RedirectToAction("DisplayTask", "Task", new { id = id });

        }

        public ActionResult Tasks()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Task> tasks = new List<Task>();
            string tasksQuery = "SELECT TOP 5 * FROM Task WHERE InformationID = @id AND TaskStatus = 'Open'  AND DueDate >= @time ORDER BY DateAdded DESC";

            using (SqlConnection tasksConn = new SqlConnection(connString))
            {
                using (SqlCommand tasksComm = new SqlCommand(tasksQuery, tasksConn))
                {
                    tasksComm.Parameters.AddWithValue("id", infoID);
                    tasksComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        tasksConn.Open();



                        using (SqlDataReader tasksrdr = tasksComm.ExecuteReader())
                        {

                            while (tasksrdr.Read())
                            {
                                Task task = new Task();

                                task.InformationID = Convert.ToInt32(tasksrdr["InformationID"]);
                                task.ProjectID = Convert.ToInt32(tasksrdr["ProjectID"]);
                                task.TaskID = Convert.ToInt32(tasksrdr["TaskID"]);
                                task.TaskTitle = tasksrdr["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(tasksrdr["DueDate"]);
                                task.TaskDescription = tasksrdr["TaskDescription"].ToString();
                                task.TaskStatus = tasksrdr["TaskStatus"].ToString();
                                task.DateAdded = Convert.ToDateTime(tasksrdr["DateAdded"]);
                                task.TaskPriority = tasksrdr["TaskPriority"].ToString();
                                task.FileUploaded = tasksrdr["FileUploaded"].ToString();

                                tasks.Add(task);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        tasksConn.Close();
                    }
                }
            }


            //current




            List<Task> taskCurrent = new List<Task>();
            string taskCurrentsQuery = "SELECT TOP 5 * FROM Task WHERE InformationID = @id AND TaskStatus = 'In Progress' ORDER BY DateAdded DESC";

            using (SqlConnection taskCurrentsConn = new SqlConnection(connString))
            {
                using (SqlCommand taskCurrentsComm = new SqlCommand(taskCurrentsQuery, taskCurrentsConn))
                {
                    taskCurrentsComm.Parameters.AddWithValue("id", infoID);
                    try
                    {
                        taskCurrentsConn.Open();



                        using (SqlDataReader taskrdr = taskCurrentsComm.ExecuteReader())
                        {

                            while (taskrdr.Read())
                            {
                                Task taskCurrents = new Task();



                                taskCurrents.InformationID = Convert.ToInt32(taskrdr["InformationID"]);
                                taskCurrents.ProjectID = Convert.ToInt32(taskrdr["ProjectID"]);
                                taskCurrents.TaskID = Convert.ToInt32(taskrdr["TaskID"]);
                                taskCurrents.TaskTitle = taskrdr["TaskTitle"].ToString();
                                taskCurrents.DueDate = Convert.ToDateTime(taskrdr["DueDate"]);
                                taskCurrents.TaskDescription = taskrdr["TaskDescription"].ToString();
                                taskCurrents.TaskStatus = taskrdr["TaskStatus"].ToString();
                                taskCurrents.DateAdded = Convert.ToDateTime(taskrdr["DateAdded"]);
                                taskCurrents.TaskPriority = taskrdr["TaskPriority"].ToString();
                                taskCurrents.FileUploaded = taskrdr["FileUploaded"].ToString();

                                taskCurrent.Add(taskCurrents);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        taskCurrentsConn.Close();
                    }
                }
            }

            //delay

            DateTime delayedTime = System.DateTime.Now;
            List<Task> delayedtask = new List<Task>();
            string delayedtaskQuery = "SELECT TOP 5 * FROM Task WHERE InformationID = @id AND DueDate < @delayedTime ORDER BY DateAdded DESC";

            using (SqlConnection delayedtaskConn = new SqlConnection(connString))
            {
                using (SqlCommand delayedtaskComm = new SqlCommand(delayedtaskQuery, delayedtaskConn))
                {
                    delayedtaskComm.Parameters.AddWithValue("id", infoID);
                    delayedtaskComm.Parameters.AddWithValue("delayedTime", delayedTime);

                    try
                    {
                        delayedtaskConn.Open();



                        using (SqlDataReader delayedtaskrdr = delayedtaskComm.ExecuteReader())
                        {

                            while (delayedtaskrdr.Read())
                            {
                                Task delayedtasks = new Task();



                                delayedtasks.InformationID = Convert.ToInt32(delayedtaskrdr["InformationID"]);
                                delayedtasks.ProjectID = Convert.ToInt32(delayedtaskrdr["ProjectID"]);
                                delayedtasks.TaskID = Convert.ToInt32(delayedtaskrdr["TaskID"]);
                                delayedtasks.TaskTitle = delayedtaskrdr["TaskTitle"].ToString();
                                delayedtasks.DueDate = Convert.ToDateTime(delayedtaskrdr["DueDate"]);
                                delayedtasks.TaskDescription = delayedtaskrdr["TaskDescription"].ToString();
                                delayedtasks.TaskStatus = delayedtaskrdr["TaskStatus"].ToString();
                                delayedtasks.DateAdded = Convert.ToDateTime(delayedtaskrdr["DateAdded"]);
                                delayedtasks.TaskPriority = delayedtaskrdr["TaskPriority"].ToString();
                                delayedtasks.FileUploaded = delayedtaskrdr["FileUploaded"].ToString();

                                delayedtask.Add(delayedtasks);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        delayedtaskConn.Close();
                    }
                }
            }

            Task_information taskStat = new Task_information();
            taskStat.tas = tasks;
            taskStat.ta = taskCurrent;
            taskStat.t = delayedtask;

            return View(taskStat);

        }

        public ActionResult DeleteComment(int id, int TaskID)
        {
            Insert delComment = new Insert();
            delComment.DeleteComment(id);

            return RedirectToAction("DisplayTask", "Task", new { id = TaskID });
        }

        public ActionResult Resolve(int id)
        {

            string taskQuery = "SELECT * FROM  Subtask WHERE TaskID = @id";


            using (SqlConnection taskconn = new SqlConnection(connString))
            {
                using (SqlCommand taskcomm = new SqlCommand(taskQuery, taskconn))
                {
                    taskcomm.Parameters.AddWithValue("id", id);
                    try
                    {
                        taskconn.Open();
                        using (SqlDataReader taskrdr = taskcomm.ExecuteReader())
                        {
                            if (taskrdr.Read())
                            {
                                TempData["error"] = "You can't resolve this task. It may have any unresolved subtask/s.";

                                return RedirectToAction("DisplayTask", "Task", new { id = id });

                            }
                            else
                            {
                                Insert resolve = new Insert();
                                int infoID = Convert.ToInt32(Session["id"]);
                                string action = "Status changed to Resolved.";

                                resolve.TaskHistory(id, infoID, action);
                                resolve.ResolveTask(id);
                                return RedirectToAction("DisplayTask", "Task", new { id = id });
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        taskconn.Close();
                    }
                }
            }

          
        }

        public ActionResult Reopen(int id)
        {
            Insert resolve = new Insert();

            int infoID = Convert.ToInt32(Session["id"]);
            string action = "Status changed to Re-open.";

            resolve.TaskHistory(id, infoID, action);
            resolve.ReOpenTask(id);
            return RedirectToAction("DisplayTask", "Task", new { id = id });
        }

        public ActionResult InProgress(int id)
        {
            Insert resolve = new Insert();
            int infoID = Convert.ToInt32(Session["id"]);
            string action = "Status changed to In-Progress.";

            resolve.TaskHistory(id, infoID, action);
            resolve.ProgressTask(id);
            return RedirectToAction("DisplayTask", "Task", new { id = id });
        }


        public ActionResult editTask(int id)
        {
            Project project = new Project();
            Task task = new Task();

            //subtask
            string iQuery = "SELECT * FROM Task WHERE TaskID = @TaskID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("TaskID", id);
                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                task.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                task.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                task.TaskID = Convert.ToInt32(rdr["TaskID"]);
                                task.TaskTitle = rdr["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                task.TaskDescription = rdr["TaskDescription"].ToString();
                                task.TaskStatus = rdr["TaskStatus"].ToString();
                                task.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                task.TaskPriority = rdr["TaskPriority"].ToString();
                                task.FileUploaded = rdr["FileUploaded"].ToString();

                            }
                        }
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

            //project
            string projQuery = "SELECT * FROM Project WHERE ProjectID = @ProjectID";

            using (SqlConnection connproj = new SqlConnection(connString))
            {
                using (SqlCommand commproj = new SqlCommand(projQuery, connproj))
                {
                    commproj.Parameters.AddWithValue("ProjectID", task.ProjectID);
                    try
                    {
                        connproj.Open();

                        using (SqlDataReader rdrproj = commproj.ExecuteReader())
                        {
                            while (rdrproj.Read())
                            {
                                project.ProjectID = Convert.ToInt32(rdrproj["ProjectID"]);
                                project.ProjectTitle = rdrproj["ProjectTitle"].ToString();
                                project.DueDate = Convert.ToDateTime(rdrproj["DueDate"]);
                                project.ProjectStatus = rdrproj["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdrproj["DateAdded"]);
                                project.InformationID = Convert.ToInt32(rdrproj["InformationID"]);
                                project.FileUploaded = rdrproj["FileUploaded"].ToString();
                                project.ProjectPriority = rdrproj["ProjectPriority"].ToString();
                                project.ProjectDescription = rdrproj["ProjectDescription"].ToString();

                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connproj.Close();
                    }
                }
            }


            //team
            List<Team_Information> teaminfoList = new List<Team_Information>();

            string iQueryInfo = "SELECT * FROM Team T INNER JOIN Information I on T.InformationID = I.InformationID WHERE T.ProjectID = @ProjectID";

            using (SqlConnection conns = new SqlConnection(connString))
            {
                using (SqlCommand comms = new SqlCommand(iQueryInfo, conns))
                {
                    comms.Parameters.AddWithValue("ProjectID", task.ProjectID);
                    try
                    {
                        conns.Open();

                        using (SqlDataReader rdrs = comms.ExecuteReader())
                        {
                            while (rdrs.Read())
                            {


                                Team_Information bulok = new Team_Information();
                                Information info = new Information();


                                info.InformationID = Convert.ToInt32(rdrs["InformationID"]); ;
                                info.FirstName = rdrs["FirstName"].ToString();
                                info.LastName = rdrs["LastName"].ToString();
                                info.EmailAddress = rdrs["EmailAddress"].ToString();

                                bulok.inform = info;



                                teaminfoList.Add(bulok);




                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        conns.Close();
                    }
                }
            }



            Information_Project ip = new Information_Project();
            ip.project = project;
            ip.teaminfo = teaminfoList;
            ip.tasking = task;



            return View(ip);
        }

        [HttpPost]

        public ActionResult editTask(string tasktitle, string taskdesc, string taskprio, DateTime taskdue, int id, int taskassign)
        {
            DateTime due = Convert.ToDateTime(taskdue);
            Insert insert = new Insert();

            // if error
            Project project = new Project();
            Task task = new Task();

            //subtask
            string iQuery = "SELECT * FROM Task WHERE TaskID = @TaskID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("TaskID", id);
                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                task.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                task.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                task.TaskID = Convert.ToInt32(rdr["TaskID"]);
                                task.TaskTitle = rdr["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                task.TaskDescription = rdr["TaskDescription"].ToString();
                                task.TaskStatus = rdr["TaskStatus"].ToString();
                                task.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                task.TaskPriority = rdr["TaskPriority"].ToString();
                                task.FileUploaded = rdr["FileUploaded"].ToString();

                            }
                        }
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

            //project
            string projQuery = "SELECT * FROM Project WHERE ProjectID = @ProjectID";

            using (SqlConnection connproj = new SqlConnection(connString))
            {
                using (SqlCommand commproj = new SqlCommand(projQuery, connproj))
                {
                    commproj.Parameters.AddWithValue("ProjectID", task.ProjectID);
                    try
                    {
                        connproj.Open();

                        using (SqlDataReader rdrproj = commproj.ExecuteReader())
                        {
                            while (rdrproj.Read())
                            {
                                project.ProjectID = Convert.ToInt32(rdrproj["ProjectID"]);
                                project.ProjectTitle = rdrproj["ProjectTitle"].ToString();
                                project.DueDate = Convert.ToDateTime(rdrproj["DueDate"]);
                                project.ProjectStatus = rdrproj["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdrproj["DateAdded"]);
                                project.InformationID = Convert.ToInt32(rdrproj["InformationID"]);
                                project.FileUploaded = rdrproj["FileUploaded"].ToString();
                                project.ProjectPriority = rdrproj["ProjectPriority"].ToString();
                                project.ProjectDescription = rdrproj["ProjectDescription"].ToString();

                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connproj.Close();
                    }
                }
            }


            //team
            List<Team_Information> teaminfoList = new List<Team_Information>();

            string iQueryInfo = "SELECT * FROM Team T INNER JOIN Information I on T.InformationID = I.InformationID WHERE T.ProjectID = @ProjectID";

            using (SqlConnection conns = new SqlConnection(connString))
            {
                using (SqlCommand comms = new SqlCommand(iQueryInfo, conns))
                {
                    comms.Parameters.AddWithValue("ProjectID", task.ProjectID);
                    try
                    {
                        conns.Open();

                        using (SqlDataReader rdrs = comms.ExecuteReader())
                        {
                            while (rdrs.Read())
                            {


                                Team_Information bulok = new Team_Information();
                                Information info = new Information();


                                info.InformationID = Convert.ToInt32(rdrs["InformationID"]); ;
                                info.FirstName = rdrs["FirstName"].ToString();
                                info.LastName = rdrs["LastName"].ToString();
                                info.EmailAddress = rdrs["EmailAddress"].ToString();

                                bulok.inform = info;



                                teaminfoList.Add(bulok);




                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        conns.Close();
                    }
                }
            }



            Information_Project ip = new Information_Project();
            ip.project = project;
            ip.teaminfo = teaminfoList;
            ip.tasking = task;



         
           
            //end if error

            if (project.DueDate < taskdue)
            {
                TempData["error"] = "The Date you entered is beyond the Project's Due Date.";
                return View(ip);
            }
            else
            {

                int informedID = Convert.ToInt32(Session["id"]);
                //check assignee
                Information info = new Information();
                Information infox = new Information();
                string loginQuery = "SELECT * FROM Information info JOIN Task task ON info.InformationID = task.InformationID WHERE TaskID = @id";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand comm = new SqlCommand(loginQuery, conn))
                    {
                        comm.Parameters.AddWithValue("id", id);


                        try
                        {
                            conn.Open();

                            using (SqlDataReader rdr = comm.ExecuteReader())
                            {
                                while (rdr.Read())
                                {

                                    info.InformationID = Convert.ToInt32(rdr["InformationID"]);

                                    if (info.InformationID == taskassign)
                                    {
                                        string action = "The task was updated";
                                        insert.TaskHistory(id, informedID, action);

                                    }
                                    else
                                    {
                                        string loginQueryx = "SELECT * FROM Information WHERE InformationID = @infoid";
                                        using (SqlConnection connx = new SqlConnection(connString))
                                        {
                                            using (SqlCommand commx = new SqlCommand(loginQueryx, connx))
                                            {

                                                commx.Parameters.AddWithValue("infoid", taskassign);

                                                try
                                                {
                                                    connx.Open();

                                                    using (SqlDataReader rdrx = commx.ExecuteReader())
                                                    {
                                                        while (rdrx.Read())
                                                        {


                                                            infox.Username = rdrx["Username"].ToString();
                                                            infox.FirstName = rdrx["FirstName"].ToString();
                                                            infox.LastName = rdrx["LastName"].ToString();
                                                            infox.EmailAddress = rdrx["EmailAddress"].ToString();
                                                            infox.ImagePath = rdrx["ImagePath"].ToString();
                                                            infox.Specialty = rdrx["Specialty"].ToString();
                                                            infox.ContactNumber = rdrx["ContactNumber"].ToString();
                                                            infox.InformationID = Convert.ToInt32(rdrx["InformationID"]);
                                                            infox.DateAdded = Convert.ToDateTime(rdrx["DateAdded"]);



                                                        }




                                                    }
                                                }
                                                catch
                                                {
                                                    throw;
                                                }
                                                finally
                                                {
                                                    connx.Close();
                                                }
                                            }


                                        }

                                        string action = "The task was updated. Re-assign the subtask to " + infox.FirstName + " " + infox.LastName;
                                        insert.TaskHistory(id, informedID, action);
                                    }
                                    //return RedirectToAction("Index", "Dashboard");
                                }





                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }


                }

                //
                insert.UpdateTask(tasktitle, taskdesc, taskprio, due, id, taskassign);



                return RedirectToAction("DisplayTask", "Task", new { id = id });
                
            }
          
        }
        [HttpPost]
        public ActionResult editTaskAssignee( int id, int taskassign)
        {
            Information info = new Information();
            string loginQuery = "SELECT * FROM Information WHERE InformationID=@taskassign";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(loginQuery, conn))
                {
                    comm.Parameters.AddWithValue("taskassign", taskassign);


                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {

                            
                                info.Username = rdr["Username"].ToString();
                                info.FirstName = rdr["FirstName"].ToString();
                                info.LastName = rdr["LastName"].ToString();
                                info.EmailAddress = rdr["EmailAddress"].ToString();
                                info.ImagePath = rdr["ImagePath"].ToString();
                                info.Specialty = rdr["Specialty"].ToString();
                                info.ContactNumber = rdr["ContactNumber"].ToString();
                                info.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                info.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);

                        

                                //return RedirectToAction("Index", "Dashboard");
                            }




                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }


            }
            //updateAssignee

            Insert insert = new Insert();
            int informedID = Convert.ToInt32(Session["id"]);
            string action = "Re-assign task to" + " " + @info.FirstName + " " + @info.LastName;
            insert.TaskHistory(id, informedID, action);

            insert.UpdateTaskAssignee( id, taskassign);
            return RedirectToAction("DisplayTask", "Task", new { id = id });
        }

        public ActionResult PendingTask()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Task> tasks = new List<Task>();
            string tasksQuery = "SELECT  * FROM Task WHERE InformationID = @id AND TaskStatus = 'Open'  AND DueDate >= @time ORDER BY DueDate DESC";

            using (SqlConnection tasksConn = new SqlConnection(connString))
            {
                using (SqlCommand tasksComm = new SqlCommand(tasksQuery, tasksConn))
                {
                    tasksComm.Parameters.AddWithValue("id", infoID);
                    tasksComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        tasksConn.Open();



                        using (SqlDataReader tasksrdr = tasksComm.ExecuteReader())
                        {

                            while (tasksrdr.Read())
                            {
                                Task task = new Task();

                                task.InformationID = Convert.ToInt32(tasksrdr["InformationID"]);
                                task.ProjectID = Convert.ToInt32(tasksrdr["ProjectID"]);
                                task.TaskID = Convert.ToInt32(tasksrdr["TaskID"]);
                                task.TaskTitle = tasksrdr["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(tasksrdr["DueDate"]);
                                task.TaskDescription = tasksrdr["TaskDescription"].ToString();
                                task.TaskStatus = tasksrdr["TaskStatus"].ToString();
                                task.DateAdded = Convert.ToDateTime(tasksrdr["DateAdded"]);
                                task.TaskPriority = tasksrdr["TaskPriority"].ToString();
                                task.FileUploaded = tasksrdr["FileUploaded"].ToString();

                                tasks.Add(task);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        tasksConn.Close();
                    }
                }
            }

            return View(tasks);
        }

        public ActionResult CurrentTask()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Task> tasks = new List<Task>();
            string tasksQuery = "SELECT  * FROM Task WHERE InformationID = @id AND TaskStatus = 'In Progress'  AND DueDate >= @time ORDER BY DueDate DESC";

            using (SqlConnection tasksConn = new SqlConnection(connString))
            {
                using (SqlCommand tasksComm = new SqlCommand(tasksQuery, tasksConn))
                {
                    tasksComm.Parameters.AddWithValue("id", infoID);
                    tasksComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        tasksConn.Open();



                        using (SqlDataReader tasksrdr = tasksComm.ExecuteReader())
                        {

                            while (tasksrdr.Read())
                            {
                                Task task = new Task();

                                task.InformationID = Convert.ToInt32(tasksrdr["InformationID"]);
                                task.ProjectID = Convert.ToInt32(tasksrdr["ProjectID"]);
                                task.TaskID = Convert.ToInt32(tasksrdr["TaskID"]);
                                task.TaskTitle = tasksrdr["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(tasksrdr["DueDate"]);
                                task.TaskDescription = tasksrdr["TaskDescription"].ToString();
                                task.TaskStatus = tasksrdr["TaskStatus"].ToString();
                                task.DateAdded = Convert.ToDateTime(tasksrdr["DateAdded"]);
                                task.TaskPriority = tasksrdr["TaskPriority"].ToString();
                                task.FileUploaded = tasksrdr["FileUploaded"].ToString();

                                tasks.Add(task);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        tasksConn.Close();
                    }
                }
            }

            return View(tasks);

        }

        public ActionResult DelayedTask()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Task> tasks = new List<Task>();
            string tasksQuery = "SELECT  * FROM Task WHERE InformationID = @id AND TaskStatus != 'Resolved'  AND DueDate < @time ORDER BY DueDate DESC";

            using (SqlConnection tasksConn = new SqlConnection(connString))
            {
                using (SqlCommand tasksComm = new SqlCommand(tasksQuery, tasksConn))
                {
                    tasksComm.Parameters.AddWithValue("id", infoID);
                    tasksComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        tasksConn.Open();



                        using (SqlDataReader tasksrdr = tasksComm.ExecuteReader())
                        {

                            while (tasksrdr.Read())
                            {
                                Task task = new Task();

                                task.InformationID = Convert.ToInt32(tasksrdr["InformationID"]);
                                task.ProjectID = Convert.ToInt32(tasksrdr["ProjectID"]);
                                task.TaskID = Convert.ToInt32(tasksrdr["TaskID"]);
                                task.TaskTitle = tasksrdr["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(tasksrdr["DueDate"]);
                                task.TaskDescription = tasksrdr["TaskDescription"].ToString();
                                task.TaskStatus = tasksrdr["TaskStatus"].ToString();
                                task.DateAdded = Convert.ToDateTime(tasksrdr["DateAdded"]);
                                task.TaskPriority = tasksrdr["TaskPriority"].ToString();
                                task.FileUploaded = tasksrdr["FileUploaded"].ToString();

                                tasks.Add(task);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        tasksConn.Close();
                    }
                }
            }

            return View(tasks);
        
        }

        public ActionResult ViewAllTasks()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Task> proj = new List<Task>();
            string projCurrentQuery = "SELECT  * FROM Task WHERE InformationID = @id ORDER BY DueDate DESC";

            using (SqlConnection projConn = new SqlConnection(connString))
            {
                using (SqlCommand projComm = new SqlCommand(projCurrentQuery, projConn))
                {
                    projComm.Parameters.AddWithValue("id", infoID);

                    try
                    {
                        projConn.Open();



                        using (SqlDataReader projrdr = projComm.ExecuteReader())
                        {

                            while (projrdr.Read())
                            {
                                Task projs = new Task();


                                projs.TaskID = Convert.ToInt32(projrdr["TaskID"]);
                                projs.TaskTitle = projrdr["TaskTitle"].ToString();
                                projs.DueDate = Convert.ToDateTime(projrdr["DueDate"]);
                                projs.TaskStatus = projrdr["TaskStatus"].ToString();
                                projs.TaskDescription = projrdr["TaskDescription"].ToString();
                                projs.TaskPriority = projrdr["TaskPriority"].ToString();
                                projs.ProjectID = Convert.ToInt32(projrdr["ProjectID"]);
                                projs.DateAdded = Convert.ToDateTime(projrdr["DateAdded"]);
                                projs.InformationID = Convert.ToInt32(projrdr["InformationID"]);
                                projs.FileUploaded = projrdr["FileUploaded"].ToString();
                             

                                proj.Add(projs);



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







            return View(proj);

        }

      //endParse
    }


}
