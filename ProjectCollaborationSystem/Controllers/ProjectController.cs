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
    public class ProjectController : Controller
    {

       
        public int projctID { get; set; }
        public string iPath { get; set; }
        public int wapakz { get; set; }

        public int taskid { get; set; }
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //
        // GET: /Project/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Index(string projtitle, string projdesc, string projprio, DateTime projdue, string projassoc1, string projassoc2, string projassoc3, string projassoc4, string projassoc5, HttpPostedFileBase UploadFile)
        {
            //upload file
            if (UploadFile != null && UploadFile.ContentLength > 0)
                try
                {
                    string filename = Path.GetFileName(UploadFile.FileName);
                    iPath = "Files/" + filename;
                    string path = Path.Combine(Server.MapPath("~/Files"),
                                               Path.GetFileName(UploadFile.FileName));
                    UploadFile.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                    Response.Write("File uploaded successfully");

                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    Response.Write("File uploaded unsuccessfully");
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
                Response.Write("You have not specified a file.");
            }



            //



            int informID = Convert.ToInt32(Session["id"]);
            string projstatus = "Open";



            Insert insertProject = new Insert();
            insertProject.InsertProject(projtitle, projdesc, projprio, projdue, projstatus, informID, iPath);


            string iQueryy = "SELECT TOP 1 * FROM Project ORDER BY ProjectID DESC ";

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
                                projctID = Convert.ToInt32(reder["ProjectID"]);

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

            


            Insert insertAssoc = new Insert();
            string action = "Project is created";
            int informedID = Convert.ToInt32(Session["id"]);
       
            insertAssoc.ProjectHistory(projctID,informedID, action );

            string fname = Session["first"].ToString();
            string lname = Session["last"].ToString();


            if (projassoc1 != "")
            {

                insertAssoc.insertAssoc(projassoc1, projctID, projtitle, fname, lname, informedID);


            }

            if (projassoc2 != "")
            {
                insertAssoc.insertAssoc(projassoc2, projctID, projtitle, fname, lname, informedID);
            }

            if (projassoc3 != "")
            {
                insertAssoc.insertAssoc(projassoc3, projctID, projtitle, fname, lname, informedID);
            }

            if (projassoc4 != "")
            {
                insertAssoc.insertAssoc(projassoc4, projctID, projtitle, fname, lname, informedID);
            }

            if (projassoc5 != "")
            {
                insertAssoc.insertAssoc(projassoc5, projctID, projtitle, fname, lname, informedID);
            }
            try
            {

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception)
            {
                return View();
            }


        }



        public ActionResult DisplayProjet(string id)
        {
            Project project = new Project();

            Information info = new Information();
            List<Information> infoing = new List<Information>();
            List<Task> tasking = new List<Task>();

            string iQuery = "SELECT * FROM Project WHERE ProjectID = @ProjectID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("ProjectID", id);
                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                project.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                project.ProjectTitle = rdr["ProjectTitle"].ToString();
                                project.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                project.ProjectStatus = rdr["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                project.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                project.FileUploaded = rdr["FileUploaded"].ToString();
                                project.ProjectPriority = rdr["ProjectPriority"].ToString();
                                project.ProjectDescription = rdr["ProjectDescription"].ToString();

                              
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

            //info

            string iQueryInfo = "SELECT * FROM Information WHERE InformationID = @InformationID";

            using (SqlConnection conns = new SqlConnection(connString))
            {
                using (SqlCommand comms = new SqlCommand(iQueryInfo, conns))
                {
                    comms.Parameters.AddWithValue("InformationID", project.InformationID);
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
            //teaminfo


            List<Team_Information> teaminfos = new List<Team_Information>();
            string iQueryTeam = "SELECT * FROM Team team INNER JOIN Information info ON team.InformationID = info.InformationID WHERE team.ProjectID = @ProjectID";

            using (SqlConnection connTeam = new SqlConnection(connString))
            {
                using (SqlCommand commTeam = new SqlCommand(iQueryTeam, connTeam))
                {
                    commTeam.Parameters.AddWithValue("ProjectID", id);
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

            //task


            List<Task_information> taskinfo = new List<Task_information>();

            string taskQuery = "SELECT * FROM Task t JOIN Information i on t.InformationID = i.InformationID WHERE t.ProjectID = @proj";

            using (SqlConnection taskconn = new SqlConnection(connString))
            {
                using (SqlCommand taskcomm = new SqlCommand(taskQuery, taskconn))
                {
                    taskcomm.Parameters.AddWithValue("proj", project.ProjectID);
                    try
                    {
                        taskconn.Open();
                        using (SqlDataReader taskrdr = taskcomm.ExecuteReader())
                        {
                            while (taskrdr.Read())
                            {
                                Task_information taskinfos = new Task_information();

                                Task tasked = new Task();

                                tasked.TaskID = Convert.ToInt32(taskrdr["TaskID"]);
                                tasked.TaskTitle = taskrdr["TaskTitle"].ToString();
                                tasked.DueDate = Convert.ToDateTime(taskrdr["DueDate"]);
                                tasked.TaskStatus = taskrdr["TaskStatus"].ToString();
                                tasked.DateAdded = Convert.ToDateTime(taskrdr["DateAdded"]);
                                tasked.InformationID = Convert.ToInt32(taskrdr["InformationID"]);
                                tasked.TaskPriority = taskrdr["TaskPriority"].ToString();

                                Information information = new Information();


                                information.FirstName = taskrdr["FirstName"].ToString();
                                information.LastName = taskrdr["LastName"].ToString();
                                information.EmailAddress = taskrdr["EmailAddress"].ToString();
                                information.InformationID = Convert.ToInt32(taskrdr["InformationID"]);

                                taskinfos.info = information;
                                taskinfos.tasked = tasked;
                                
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



            //comments


            List<Comment_Information> cis = new List<Comment_Information>();
            string commentQuery = "SELECT * FROM Comment c JOIN Information i ON c.InformationID = i.InformationID AND c.CommentStatus = 'Active' WHERE c.ProjectID = @id ORDER BY c.DateAdded DESC";

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

                                //ci.comments.CommentMessage = commentrdr["CommentMessage"].ToString();
                                //ci.comments.CommentID = Convert.ToInt32(commentrdr["CommentID"]);
                                //string commentMessage = commentrdr["CommentMessage"].ToString();

                                //ci.info.InformationID = Convert.ToInt32(commentrdr["InformationID"]);
                                //ci.info.FirstName = commentrdr["FirstName"].ToString();
                                //ci.info.LastName = commentrdr["LastName"].ToString();
                                //ci.info.Username = commentrdr["Username"].ToString();
                                //ci.comments.DateAdded = Convert.ToDateTime(commentrdr["DateAdded"]);

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



            //history



            List<History> his = new List<History>();
            List<Information> hiss = new List<Information>();
            string historyQuery = "SELECT * FROM History  WHERE ProjectID = @id ORDER BY DateAdded DESC";

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
                              
                                History histowry = new History();

                                histowry.ProjectID = Convert.ToInt32(historyrdr["ProjectID"]);
                                string s = historyrdr["InformationID"].ToString();
                                if(s== "")
                                {
                        
                                }
                                else
                                {
                                    histowry.InformationID = Convert.ToInt32(historyrdr["InformationID"]);
                                }
                               
                                histowry.Action = historyrdr["Action"].ToString();
                                histowry.DateAdded = Convert.ToDateTime(historyrdr["DateAdded"]);
                                histowry.HistoryStat = historyrdr["HistoryStat"].ToString();

                                //information
                                string histinfoQuery = "SELECT * FROM Information  WHERE InformationID = @id ORDER BY DateAdded DESC";

                                using (SqlConnection histinfoConn = new SqlConnection(connString))
                                {
                                    using (SqlCommand histinfoComm = new SqlCommand(histinfoQuery, histinfoConn))
                                    {
                                        histinfoComm.Parameters.AddWithValue("id", histowry.InformationID);
                                        try
                                        {
                                            histinfoConn.Open();



                                            using (SqlDataReader histinfordr = histinfoComm.ExecuteReader())
                                            {

                                                while (histinfordr.Read())
                                                {
                                                    


                                                    Information commentInfo = new Information();
                                                    commentInfo.InformationID = Convert.ToInt32(histinfordr["InformationID"]);
                                                    commentInfo.FirstName = histinfordr["FirstName"].ToString();
                                                    commentInfo.LastName = histinfordr["LastName"].ToString();
                                                    commentInfo.Username = histinfordr["Username"].ToString();
                                                    commentInfo.DateAdded = Convert.ToDateTime(histinfordr["DateAdded"]);
                                                    commentInfo.ImagePath = histinfordr["ImagePath"].ToString();

                                             
                                                

                                                    hiss.Add(commentInfo);



                                                }


                                            }


                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }
                                        finally
                                        {
                                            histinfoConn.Close();
                                        }
                                    }
                                }
                                //endinfo

                           
                                

                                his.Add(histowry);



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
            ip.project = project;
            ip.task = tasking;
            ip.information = infoing;
            ip.cinfo = cis;
            ip.taskinfo = taskinfo;
            ip.teaminfo = teaminfos;
            ip.historys = his;
            ip.information = hiss;

            return View(ip);
        }


      
        [HttpPost]

        public ActionResult DisplayProjet(int id, string CommentMessage)
        {
            int commentor = Convert.ToInt32(Session["id"]);
            Insert Addcomment = new Insert();
            Addcomment.AddCommentProj(CommentMessage, id, commentor);

            return RedirectToAction("DisplayProjet", "Project", new { id = id });

        }

        public ActionResult Projects()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Project> proj = new List<Project>();
            string projCurrentQuery = "SELECT TOP 5 * FROM Project WHERE InformationID = @id AND ProjectStatus = 'Open'  AND DueDate >= @time ORDER BY DueDate DESC";

            using (SqlConnection projConn = new SqlConnection(connString))
            {
                using (SqlCommand projComm = new SqlCommand(projCurrentQuery, projConn))
                {
                    projComm.Parameters.AddWithValue("id", infoID);
                    projComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        projConn.Open();



                        using (SqlDataReader projrdr = projComm.ExecuteReader())
                        {

                            while (projrdr.Read())
                            {
                                Project projs = new Project();



                                projs.ProjectID = Convert.ToInt32(projrdr["ProjectID"]);
                                projs.ProjectTitle = projrdr["ProjectTitle"].ToString();
                                projs.DueDate = Convert.ToDateTime(projrdr["DueDate"]);
                                projs.ProjectStatus = projrdr["ProjectStatus"].ToString();
                                projs.DateAdded = Convert.ToDateTime(projrdr["DateAdded"]);
                                projs.InformationID = Convert.ToInt32(projrdr["InformationID"]);
                                projs.FileUploaded = projrdr["FileUploaded"].ToString();
                                projs.ProjectDescription = projrdr["ProjectDescription"].ToString();
                                projs.ProjectPriority = projrdr["ProjectPriority"].ToString();

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


            //current




            List<Project> projCurrent = new List<Project>();
            string projCurrentsQuery = "SELECT TOP 5 * FROM Project WHERE InformationID = @id AND ProjectStatus = 'In Progress' ORDER BY DateAdded DESC";

            using (SqlConnection projCurrentsConn = new SqlConnection(connString))
            {
                using (SqlCommand projCurrentsComm = new SqlCommand(projCurrentsQuery, projCurrentsConn))
                {
                    projCurrentsComm.Parameters.AddWithValue("id", infoID);
                    try
                    {
                        projCurrentsConn.Open();



                        using (SqlDataReader projectrdr = projCurrentsComm.ExecuteReader())
                        {

                            while (projectrdr.Read())
                            {
                                Project projCurrents = new Project();



                                projCurrents.ProjectID = Convert.ToInt32(projectrdr["ProjectID"]);
                                projCurrents.ProjectTitle = projectrdr["ProjectTitle"].ToString();
                                projCurrents.DueDate = Convert.ToDateTime(projectrdr["DueDate"]);
                                projCurrents.ProjectStatus = projectrdr["ProjectStatus"].ToString();
                                projCurrents.DateAdded = Convert.ToDateTime(projectrdr["DateAdded"]);
                                projCurrents.InformationID = Convert.ToInt32(projectrdr["InformationID"]);
                                projCurrents.FileUploaded = projectrdr["FileUploaded"].ToString();
                                projCurrents.ProjectDescription = projectrdr["ProjectDescription"].ToString();
                                projCurrents.ProjectPriority = projectrdr["ProjectPriority"].ToString();

                                projCurrent.Add(projCurrents);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        projCurrentsConn.Close();
                    }
                }
            }

            //delay

            DateTime delayedTime = System.DateTime.Now;
            List<Project> delayedProj = new List<Project>();
            string delayedProjQuery = "SELECT TOP 5 * FROM Project WHERE InformationID = @id AND ProjectStatus = 'In Progress'  AND DueDate < @delayedTime ORDER BY DateAdded DESC";

            using (SqlConnection delayedProjConn = new SqlConnection(connString))
            {
                using (SqlCommand delayedProjComm = new SqlCommand(delayedProjQuery, delayedProjConn))
                {
                    delayedProjComm.Parameters.AddWithValue("id", infoID);
                    delayedProjComm.Parameters.AddWithValue("delayedTime", delayedTime);

                    try
                    {
                        delayedProjConn.Open();



                        using (SqlDataReader delayedProjrdr = delayedProjComm.ExecuteReader())
                        {

                            while (delayedProjrdr.Read())
                            {
                                Project delayedProjs = new Project();



                                delayedProjs.ProjectID = Convert.ToInt32(delayedProjrdr["ProjectID"]);
                                delayedProjs.ProjectTitle = delayedProjrdr["ProjectTitle"].ToString();
                                delayedProjs.DueDate = Convert.ToDateTime(delayedProjrdr["DueDate"]);
                                delayedProjs.ProjectStatus = delayedProjrdr["ProjectStatus"].ToString();
                                delayedProjs.ProjectPriority = delayedProjrdr["ProjectPriority"].ToString();
                                delayedProjs.DateAdded = Convert.ToDateTime(delayedProjrdr["DateAdded"]);
                                delayedProjs.InformationID = Convert.ToInt32(delayedProjrdr["InformationID"]);
                                delayedProjs.FileUploaded = delayedProjrdr["FileUploaded"].ToString();
                                delayedProjs.ProjectDescription = delayedProjrdr["ProjectDescription"].ToString();

                                delayedProj.Add(delayedProjs);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        delayedProjConn.Close();
                    }
                }
            }

            Task_information projectStat = new Task_information();
            projectStat.proj = proj;
            projectStat.proje = projCurrent;
            projectStat.projec = delayedProj;

            return View(projectStat);

        }

        public ActionResult DeleteComment(int id, int ProjectID)
        {
            Insert delComment = new Insert();
            delComment.DeleteComment(id);

            return RedirectToAction("DisplayProjet", "Project", new { id = ProjectID });
        }

        public ActionResult EditProject(int id)
        {
            Project project = new Project();


            string iQuery = "SELECT * FROM Project WHERE ProjectID = @ProjectID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("ProjectID", id);
                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                project.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                project.ProjectTitle = rdr["ProjectTitle"].ToString();
                                project.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                project.ProjectStatus = rdr["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                project.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                project.FileUploaded = rdr["FileUploaded"].ToString();
                                project.ProjectDescription = rdr["ProjectDescription"].ToString();
                                project.ProjectPriority = rdr["ProjectPriority"].ToString();
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

            //info


            List<Team_Information> teaminfoList = new List<Team_Information>();

            string iQueryInfo = "SELECT * FROM Team T INNER JOIN Information I on T.InformationID = I.InformationID WHERE T.ProjectID = @ProjectID";

            using (SqlConnection conns = new SqlConnection(connString))
            {
                using (SqlCommand comms = new SqlCommand(iQueryInfo, conns))
                {
                    comms.Parameters.AddWithValue("ProjectID", project.ProjectID);
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
           
       
            
            return View(ip);
        }

        [HttpPost]
        public ActionResult editProject(string projtitle, string projdesc, string projprio, string projdue, int id, string projassoc1, string projassoc2, string projassoc3, string projassoc4, string projassoc5)
        {
           

            DateTime due = Convert.ToDateTime(projdue);
            Insert insert = new Insert();
            //
            int informedID = Convert.ToInt32(Session["id"]);
            string action = "Project was updated";
            insert.ProjectHistory(id, informedID, action);
            
            //

            insert.UpdateProject(projtitle, projdesc, projprio, due, id);
          



            Insert insertAssoc = new Insert();

            string fname = Session["first"].ToString();
            string lname = Session["last"].ToString();

            //


            //

            if (projassoc1 != "")
            {


                insertAssoc.insertAssoc(projassoc1, id, projtitle, fname, lname, informedID);


            }

            if (projassoc2 != "")
            {
                insertAssoc.insertAssoc(projassoc2, id, projtitle, fname, lname, informedID);
            }

            if (projassoc3 != "")
            {
                insertAssoc.insertAssoc(projassoc3, id, projtitle, fname, lname, informedID);
            }

            if (projassoc4 != "")
            {
                insertAssoc.insertAssoc(projassoc4, id, projtitle, fname, lname , informedID);
            }

            if (projassoc5 != "")
            {
                insertAssoc.insertAssoc(projassoc5, id, projtitle, fname, lname, informedID);
            }

            return RedirectToAction("DisplayProjet", "Project", new { id = id });
            
        }

        public ActionResult DeleteAssoc(int id, int ProjectID)
        {
            int proj;
          
            List<Task> tasks = new List<Task>();
            string assignQuery = "SELECT * FROM Task WHERE ProjectID = @ProjectID AND InformationID = @informID ";

            using(SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(assignQuery,conn))
                {
                    comm.Parameters.AddWithValue("informID", id);
                    comm.Parameters.AddWithValue("ProjectID", ProjectID);


                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                Task tasking = new Task();



                                tasking.TaskID = Convert.ToInt32(rdr["TaskID"]);
                                tasking.TaskTitle = rdr["TaskTitle"].ToString();
                                tasking.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                tasking.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                tasking.TaskStatus = rdr["TaskStatus"].ToString();
                                tasking.TaskPriority = rdr["TaskPriority"].ToString();
                                tasking.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                tasking.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                tasking.FileUploaded = rdr["FileUploaded"].ToString();
                                tasking.TaskDescription = rdr["TaskDescription"].ToString();


                                tasks.Add(tasking);

                                wapakz = 1;
                            }
                            if (wapakz == 1)
                            {
                             
                                proj = 1;
                            }
                            else 
                            
                            {
                                proj = 0;
                                
                            }


                        }
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


            int info;

            List<Subtask> subtasks = new List<Subtask>();

            string assignQueryy = "SELECT * FROM Subtask WHERE ProjectID = @ProjectID AND InformationID = @informID ";

            using (SqlConnection connn = new SqlConnection(connString))
            {
                using (SqlCommand commm = new SqlCommand(assignQueryy, connn))
                {

                    commm.Parameters.AddWithValue("ProjectID",ProjectID );
                    commm.Parameters.AddWithValue("informID", id);
                    try
                    {
                        connn.Open();

                        using (SqlDataReader rdrr = commm.ExecuteReader())
                        {
                            while (rdrr.Read())
                            {
                                Subtask subtasking = new Subtask();




                                subtasking.SubtaskID = Convert.ToInt32(rdrr["SubtaskID"]);
                                subtasking.SubtaskTitle = rdrr["SubtaskTitle"].ToString();
                                subtasking.DueDate = Convert.ToDateTime(rdrr["DueDate"]);
                                subtasking.DateAdded = Convert.ToDateTime(rdrr["DateAdded"]);
                                subtasking.SubtaskPriority = rdrr["SubtaskPriority"].ToString();
                                subtasking.SubtaskStatus = rdrr["SubtaskStatus"].ToString();
                                subtasking.InformationID = Convert.ToInt32(rdrr["InformationID"]);
                                subtasking.ProjectID = Convert.ToInt32(rdrr["ProjectID"]);
                                subtasking.FileUploaded = rdrr["FileUploaded"].ToString();
                                subtasking.SubtaskDescription = rdrr["SubtaskDescription"].ToString();



                                subtasks.Add(subtasking);
                                wapakz = 1;
                            }
                            if (wapakz == 1)
                            {

                                info = 1;
                            }
                            else
                            {
                                info = 0;

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

            //information


            Information infos = new Information();

            string infoQuery = "SELECT * FROM Information WHERE InformationID = @informID ";

            using (SqlConnection connns = new SqlConnection(connString))
            {
                using (SqlCommand commms = new SqlCommand(infoQuery, connns))
                {

 
                    commms.Parameters.AddWithValue("informID", id);
                    try
                    {
                        connns.Open();

                        using (SqlDataReader rdrs = commms.ExecuteReader())
                        {
                            if (rdrs.Read())
                            {

                                infos.InformationID = Convert.ToInt32(rdrs["InformationID"]);
                                infos.FirstName = rdrs["FirstName"].ToString();
                                infos.LastName = rdrs["LastName"].ToString();


                       

                               
                            }
                         


                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connns.Close();
                    }
                }
            }

            //

            if ((proj == 0) && (info == 0))
            {
                Insert del = new Insert();

                //check assoc
                Information infox = new Information();
                string loginQueryx = "SELECT * FROM Information WHERE InformationID = @infoid";
                using (SqlConnection connx = new SqlConnection(connString))
                {
                    using (SqlCommand commx = new SqlCommand(loginQueryx, connx))
                    {

                        commx.Parameters.AddWithValue("infoid", id);

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

                                    int informedID = Convert.ToInt32(Session["id"]);
                                    string action = infox.FirstName + " " + infox.LastName +" was being removed";
                                    del.ProjectHistory(ProjectID, informedID, action);
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

                

                //

                del.DeleteAssoc(id);
                return RedirectToAction("EditProject", "Project", new { id = ProjectID});
            } 
            else if ((proj == 1) || (info == 1))
            {
                Task_information taskSubtasks = new Task_information();
                taskSubtasks.subtask = subtasks;
                taskSubtasks.task = tasks;
                taskSubtasks.info = infos;
                return View(taskSubtasks);
            }

            Task_information taskSubtask = new Task_information();
            taskSubtask.subtask = subtasks;
            taskSubtask.task = tasks;
            taskSubtask.info = infos;

            return View(taskSubtask);

        }

       public ActionResult Resolve(int id)
        {

            string taskQuery = "SELECT * FROM Task task JOIN Subtask subtask ON task.ProjectID = subtask.ProjectID WHERE task.ProjectID = @id";
          

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
                                TempData["error"] = "You can't resolve this project. It may have any unresolved tasks.";
                           
                                return RedirectToAction("DisplayProjet", "Project", new { id = id });

                            }
                            else
                            {
                                Insert resolve = new Insert();
                                string action = "Status changed to Resolved.";
                                int infoID = Convert.ToInt32(Session["id"]);



                                resolve.ProjectHistory(id, infoID, action);
                                resolve.ResolveProject(id);
                                return RedirectToAction("DisplayProjet", "Project", new { id = id });
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
           string action = "Status changed to Re-open.";
           int infoID = Convert.ToInt32(Session["id"]);
           
           

           resolve.ProjectHistory(id, infoID, action);
           resolve.ReOpenProject(id);
           return RedirectToAction("DisplayProjet", "Project", new { id = id });
       }

       public ActionResult InProgress(int id)
       {
           Insert resolve = new Insert();

           string action = "Status changed to In-Progress.";
           int infoID = Convert.ToInt32(Session["id"]);



           resolve.ProjectHistory(id, infoID, action);

           resolve.ProgressProject(id);
           return RedirectToAction("DisplayProjet", "Project", new { id = id });
       }

        public ActionResult DelayedProject()
       {
           int infoID = Convert.ToInt32(Session["id"]);
           DateTime time = System.DateTime.Now;
           List<Project> proj = new List<Project>();
           string projCurrentQuery = " SELECT  * FROM Project WHERE InformationID=@id AND ProjectStatus != 'Resolved'  AND DueDate < @time ORDER BY DueDate DESC";
           

           using (SqlConnection projConn = new SqlConnection(connString))
           {
               using (SqlCommand projComm = new SqlCommand(projCurrentQuery, projConn))
               {
                   projComm.Parameters.AddWithValue("id", infoID);
                   projComm.Parameters.AddWithValue("time", time);
                   try
                   {
                       projConn.Open();



                       using (SqlDataReader projrdr = projComm.ExecuteReader())
                       {

                           while (projrdr.Read())
                           {
                               Project projs = new Project();



                               projs.ProjectID = Convert.ToInt32(projrdr["ProjectID"]);
                               projs.ProjectTitle = projrdr["ProjectTitle"].ToString();
                               projs.DueDate = Convert.ToDateTime(projrdr["DueDate"]);
                               projs.ProjectStatus = projrdr["ProjectStatus"].ToString();
                               projs.DateAdded = Convert.ToDateTime(projrdr["DateAdded"]);
                               projs.InformationID = Convert.ToInt32(projrdr["InformationID"]);
                               projs.FileUploaded = projrdr["FileUploaded"].ToString();
                               projs.ProjectDescription = projrdr["ProjectDescription"].ToString();
                               projs.ProjectPriority = projrdr["ProjectPriority"].ToString();

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

        public ActionResult PendingProject()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Project> proj = new List<Project>();
            string projCurrentQuery = " SELECT  * FROM Project WHERE InformationID = @id AND ProjectStatus = 'Open'  AND DueDate >= @time ORDER BY DueDate DESC";


            using (SqlConnection projConn = new SqlConnection(connString))
            {
                using (SqlCommand projComm = new SqlCommand(projCurrentQuery, projConn))
                {
                    projComm.Parameters.AddWithValue("id", infoID);
                    projComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        projConn.Open();



                        using (SqlDataReader projrdr = projComm.ExecuteReader())
                        {

                            while (projrdr.Read())
                            {
                                Project projs = new Project();



                                projs.ProjectID = Convert.ToInt32(projrdr["ProjectID"]);
                                projs.ProjectTitle = projrdr["ProjectTitle"].ToString();
                                projs.DueDate = Convert.ToDateTime(projrdr["DueDate"]);
                                projs.ProjectStatus = projrdr["ProjectStatus"].ToString();
                                projs.DateAdded = Convert.ToDateTime(projrdr["DateAdded"]);
                                projs.InformationID = Convert.ToInt32(projrdr["InformationID"]);
                                projs.FileUploaded = projrdr["FileUploaded"].ToString();
                                projs.ProjectDescription = projrdr["ProjectDescription"].ToString();
                                projs.ProjectPriority = projrdr["ProjectPriority"].ToString();

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

        public ActionResult CurrentProject()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Project> proj = new List<Project>();
            string projCurrentQuery = "SELECT * FROM Project WHERE InformationID = @id AND ProjectStatus = 'In Progress' ORDER BY DueDate DESC";


            using (SqlConnection projConn = new SqlConnection(connString))
            {
                using (SqlCommand projComm = new SqlCommand(projCurrentQuery, projConn))
                {
                    projComm.Parameters.AddWithValue("id", infoID);
                    projComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        projConn.Open();



                        using (SqlDataReader projrdr = projComm.ExecuteReader())
                        {

                            while (projrdr.Read())
                            {
                                Project projs = new Project();



                                projs.ProjectID = Convert.ToInt32(projrdr["ProjectID"]);
                                projs.ProjectTitle = projrdr["ProjectTitle"].ToString();
                                projs.DueDate = Convert.ToDateTime(projrdr["DueDate"]);
                                projs.ProjectStatus = projrdr["ProjectStatus"].ToString();
                                projs.DateAdded = Convert.ToDateTime(projrdr["DateAdded"]);
                                projs.InformationID = Convert.ToInt32(projrdr["InformationID"]);
                                projs.FileUploaded = projrdr["FileUploaded"].ToString();
                                projs.ProjectDescription = projrdr["ProjectDescription"].ToString();
                                projs.ProjectPriority = projrdr["ProjectPriority"].ToString();

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


        public ActionResult ViewAllProjects()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Project> proj = new List<Project>();
            string projCurrentQuery = "SELECT  * FROM Project WHERE InformationID = @id ORDER BY DueDate DESC";

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
                                Project projs = new Project();



                                projs.ProjectID = Convert.ToInt32(projrdr["ProjectID"]);
                                projs.ProjectTitle = projrdr["ProjectTitle"].ToString();
                                projs.DueDate = Convert.ToDateTime(projrdr["DueDate"]);
                                projs.ProjectStatus = projrdr["ProjectStatus"].ToString();
                                projs.DateAdded = Convert.ToDateTime(projrdr["DateAdded"]);
                                projs.InformationID = Convert.ToInt32(projrdr["InformationID"]);
                                projs.FileUploaded = projrdr["FileUploaded"].ToString();
                                projs.ProjectDescription = projrdr["ProjectDescription"].ToString();
                                projs.ProjectPriority = projrdr["ProjectPriority"].ToString();

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

    }
}
