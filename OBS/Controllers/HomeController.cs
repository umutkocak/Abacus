using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OBS.Models;
using OBS.ViewModels;


namespace OBS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        OBSEntities _db = new OBSEntities();
        public ActionResult Index()
        {
            ViewBag.UserId = Session["userId"];
            ViewBag.UserName = Session["userName"];
            return View();
        }

        #region ActionResult

        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login(string returnUrl)
        {

            if (Request.IsAuthenticated)
            {
                ViewBag.ReturnUrl = returnUrl;
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Route("users")]
        public ActionResult Register()
        {
            var id = Convert.ToInt32(Session["userId"]);
            var user = _db.Users.FirstOrDefault(x => x.ID == id && x.StudentId == null && x.TeacherId == null);
            if (user != null)
            {
                return View();
            }
            return RedirectToAction("Page403", "Error");
        }

        [Route("students")]
        public ActionResult Students()
        {
            return View();
        }

        [Route("teachers")]
        public ActionResult Teachers()
        {
            return View();
        }

        [Route("teachers/profession")]
        public ActionResult Profession()
        {
            return View();
        }

        [Route("lessons")]
        public ActionResult Lessons()
        {
            return View();
        }

        [Route("class")]
        public ActionResult Class()
        {
            return View();
        }

        [Route("class/students")]
        public ActionResult ClassStudents()
        {
            return View();
        }

        [Route("class/schedule")]
        public ActionResult ClassSchedule()
        {
            return View();
        }

        [Route("announcement")]
        public ActionResult Announcement()
        {
            return View();
        }

        [Route("user/changepassword")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Route("user/profile")]
        public ActionResult Profile()
        {
            return View();
        }

        [Route("homework")]
        public ActionResult HomeWork()
        {
            return View();
        }

        #endregion

        #region JsonResults

        #region Image Upload

        [Route("upload/image")]
        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Uploads"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        #endregion

        #region Authentication

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public JsonResult Login(Users us, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                var isValidate = _db.Users.FirstOrDefault(x => x.Username == us.Username);
                if (isValidate == null)
                {
                    var studentCheck = _db.Students.FirstOrDefault(x => x.TCNumber == us.Username || x.StudentNumber == us.Username);
                    if (studentCheck != null)
                    {
                        var std = _db.Users.FirstOrDefault(x =>
                            x.Password == us.Password && x.StudentId == studentCheck.ID);
                        if (std != null)
                        {
                            FormsAuthentication.SetAuthCookie(us.Username, true);
                            Session["userId"] = std.ID;
                            Session["roleName"] = "Student";
                            Session["className"] = _db.ClassStudents.FirstOrDefault(x => x.StudentID == std.StudentId)
                                ?.Classes.ClassName;
                            Session["userName"] = _db.Teachers.FirstOrDefault(x => x.ID == std.TeacherId)?.FullName ??
                                                  _db.Students.FirstOrDefault(x => x.ID == std.StudentId)?.FullName ??
                                                  std.Username;
                            Session["picture"] = _db.Teachers.FirstOrDefault(x => x.ID == std.TeacherId)?.Picture ??
                                                  _db.Students.FirstOrDefault(x => x.ID == std.StudentId)?.Picture ?? _db.Users.FirstOrDefault(x => x.Username == us.Username)?.Picture;
                            return Json("yes");
                        }
                        else
                        {
                            return Json("Şifreniz yanlıştır.");
                        }
                    }
                    else
                    {
                        var teacherCheck = _db.Teachers.FirstOrDefault(x => x.TCNumber == us.Username);
                        if (teacherCheck != null)
                        {
                            var tch = _db.Users.FirstOrDefault(x =>
                                x.Password == us.Password && x.StudentId == studentCheck.ID);
                            if (tch != null)
                            {
                                FormsAuthentication.SetAuthCookie(us.Username, true);
                                Session["userId"] = tch.ID;
                                Session["roleName"] = "Teacher";
                                Session["userName"] = _db.Teachers.FirstOrDefault(x => x.ID == tch.TeacherId)?.FullName ??
                                                      _db.Students.FirstOrDefault(x => x.ID == tch.StudentId)?.FullName ??
                                                      tch.Username;
                                Session["picture"] = _db.Teachers.FirstOrDefault(x => x.ID == tch.TeacherId)?.Picture ??
                                                     _db.Students.FirstOrDefault(x => x.ID == tch.StudentId)?.Picture ?? _db.Users.FirstOrDefault(x => x.Username == us.Username)?.Picture;
                                return Json("yes");
                            }
                            else
                            {
                                return Json("Şifreniz yanlıştır.");
                            }
                        }
                    }
                }
                else
                {
                    if (isValidate.Password != us.Password) return Json("Şifreniz yanlıştır.");
                    FormsAuthentication.SetAuthCookie(us.Username, true);
                    Session["userId"] = isValidate.ID;
                    if (isValidate.TeacherId == null && isValidate.StudentId == null)
                    {
                        Session["roleName"] = "Admin";
                    }
                    else if (isValidate.TeacherId != null)
                    {
                        Session["roleName"] = "Teacher";
                    }
                    else if (isValidate.StudentId != null)
                    {
                        Session["roleName"] = "Student";
                    }

                    Session["userName"] = _db.Teachers.FirstOrDefault(x => x.ID == isValidate.TeacherId)?.FullName ??
                                          _db.Students.FirstOrDefault(x => x.ID == isValidate.StudentId)?.FullName ??
                                          isValidate.Username;
                    Session["picture"] = _db.Teachers.FirstOrDefault(x => x.ID == isValidate.TeacherId)?.Picture ??
                                         _db.Students.FirstOrDefault(x => x.ID == isValidate.StudentId)?.Picture ?? _db.Users.FirstOrDefault(x => x.Username == us.Username)?.Picture;
                    Session["className"] = _db.ClassStudents.FirstOrDefault(x => x.StudentID == isValidate.StudentId)?.Classes.ClassName;
                    return Json("yes");
                }
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }

        [Route("register")]
        [HttpPost]
        public JsonResult Register(Users us)
        {
            if (ModelState.IsValid)
            {
                if (us.ID > 0)
                {
                    _db.Entry(us).State = EntityState.Modified;
                    return Json(_db.SaveChanges() > 0 ? "Update" : "Error");
                }
                else
                {
                    var userControl = _db.Users.FirstOrDefault(x => x.Email == us.Email);
                    if (userControl != null)
                    {
                        return Json("Mail Kayıtlı");
                    }
                    else
                    {
                        _db.Users.Add(us);
                    }
                }
                return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
            }

            return Json(us, JsonRequestBehavior.AllowGet);
        }

        [Route("user/changepassword")]
        [HttpPost]
        public JsonResult ChangePassword(Users us, string oldPassword)
        {
            if (us.ID > 0)
            {
                var user = _db.Users.FirstOrDefault(x => x.ID == us.ID);
                if (user != null)
                {
                    if (user.Password == oldPassword)
                    {
                        var data = new Users
                        {
                            ID = us.ID,
                            CreatedDate = user.CreatedDate,
                            Password = us.Password,
                            Email = user.Email,
                            StudentId = user.StudentId,
                            TeacherId = user.TeacherId,
                            Username = user.Username
                        };
                        //_db.Entry(data).State = EntityState.Modified;
                        _db.Set<Users>().AddOrUpdate(data);

                        return Json(_db.SaveChanges() > 0 ? "Update" : "NoChanges");
                    }
                    else
                    {
                        return Json("Incorrect");
                    }
                }
            }
            else if (us.Username != null)
            {
                var user = _db.Users.FirstOrDefault(x => x.Username == us.Username);
                if (user != null)
                {
                    if (user.Username == us.Username)
                    {
                        var data = new Users
                        {
                            ID = us.ID,
                            CreatedDate = user.CreatedDate,
                            Password = us.Password,
                            Email = user.Email,
                            StudentId = user.StudentId,
                            TeacherId = user.TeacherId,
                            Username = user.Username
                        };
                        //_db.Entry(data).State = EntityState.Modified;
                        _db.Set<Users>().AddOrUpdate(data);

                        return Json(_db.SaveChanges() > 0 ? "Update" : "NoChanges");
                    }
                    else
                    {
                        return Json("Incorrect");
                    }
                }
            }
            else
            {
                return Json("Error");
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region User Profile

        [Route("user/profile/change"), HttpPost]
        public JsonResult ProfileEdit(Users us, string password)
        {
            if (us.ID > 0)
            {
                var user = _db.Users.FirstOrDefault(x => x.ID == us.ID);
                if (user != null && user.Password == password)
                {
                    var data = new Users
                    {
                        ID = us.ID,
                        Email = us.Email,
                        Username = us.Username,
                        CreatedDate = user.CreatedDate,
                        Password = user.Password,
                        StudentId = user.StudentId,
                        TeacherId = user.TeacherId,
                        Picture = us.Picture ?? user.Picture
                    };

                    _db.Set<Users>().AddOrUpdate(data);
                    return Json(_db.SaveChanges() > 0 ? "Update" : "NoChanges");
                }
                else
                {
                    return Json("Incorrect");
                }
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Users
        [HttpGet]
        [Route("json/users")]
        public JsonResult GetUsers()
        {
            var dataList = new List<UserViewModel>();
            var data = _db.Users.OrderBy(a => a.ID).ToList();
            foreach (var item in data)
            {
                var userList = new UserViewModel
                {
                    Email = item.Email,
                    ID = item.ID,
                    CreatedDate = item.CreatedDate,
                    Username = item.Username,
                    Student = _db.Students.FirstOrDefault(x => x.ID == item.StudentId)?.FullName,
                    StudentImage = _db.Students.FirstOrDefault(x => x.ID == item.StudentId)?.Picture,
                    Teacher = _db.Teachers.FirstOrDefault(x => x.ID == item.TeacherId)?.FullName,
                    TeacherImage = _db.Teachers.FirstOrDefault(x => x.ID == item.TeacherId)?.Picture
                };
                dataList.Add(userList);
            }
            return Json(new { data = dataList }, JsonRequestBehavior.AllowGet);
        }

        [Route("users/details/{id?}"), HttpGet]
        public JsonResult UsersDetails(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var dataList = new List<UserViewModel>();
            var data = _db.Users.OrderBy(a => a.ID).FirstOrDefault(x => x.ID == id);
            if (data != null)
            {
                var userList = new UserViewModel
                {
                    Email = data.Email,
                    ID = data.ID,
                    CreatedDate = data.CreatedDate,
                    Username = data.Username,
                    Password = data.Password,
                    StudentId = _db.Students.FirstOrDefault(x => x.ID == data.StudentId)?.ID,
                    Student = _db.Students.FirstOrDefault(x => x.ID == data.StudentId)?.FullName,
                    TeacherId = _db.Teachers.FirstOrDefault(x => x.ID == data.TeacherId)?.ID,
                    Teacher = _db.Teachers.FirstOrDefault(x => x.ID == data.TeacherId)?.FullName,

                };
                dataList.Add(userList);
            }

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        [Route("users/delete/{id?}")]
        [HttpPost]
        public JsonResult UsersDelete(int id)
        {
            var user = _db.Users.ToList().Find(x => x.ID == id);
            if (user != null)
            {
                _db.Users.Remove(user);
                return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
            }

            return Json(user, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Students

        [Route("json/students"), HttpGet]
        public JsonResult StudentList()
        {
            var dataList = _db.Students.ToList().Select(p => new StudentViewModel
            {
                Student = new StudentsViewModel
                {
                    ID = p.ID,
                    TCNumber = p.TCNumber,
                    FullName = p.FullName,
                    StudentNumber = p.StudentNumber,
                    Birthday = p.Birthday,
                    Image = p.Picture
                },
                ClassId = _db.ClassStudents.FirstOrDefault(s => s.StudentID == p.ID)?.Classes.ID,
                Class = _db.ClassStudents.FirstOrDefault(s => s.StudentID == p.ID)?.Classes.ClassName
            });
            return Json(new { data = dataList }, JsonRequestBehavior.AllowGet);
        }

        [Route("students/details/{id?}"), HttpGet]
        public JsonResult StudentDetails(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;

            var data = new List<StudentViewModel>();
            var item = _db.Students.FirstOrDefault(x => x.ID == id);
            var clsId = _db.ClassStudents.FirstOrDefault(x => x.StudentID == item.ID)?.ClassId;
            data.Add(new StudentViewModel
            {
                Student = new StudentsViewModel
                {
                    ID = item.ID,
                    TCNumber = item.TCNumber,
                    StudentNumber = item.StudentNumber,
                    FullName = item.FullName,
                    Birthday = item.Birthday,
                    Image = item.Picture
                },
                ClassId = clsId,
                Class = _db.Classes.FirstOrDefault(x => x.ID == clsId)?.ClassName
            });
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("students/add")]
        [HttpPost]
        public JsonResult StudentAdd(Students std, ClassStudents clsStd)
        {
            if (ModelState.IsValid)
            {
                if (std.ID > 0)
                {
                    _db.Configuration.LazyLoadingEnabled = false;
                    var clsLsn = _db.ClassStudents.Where(x => x.ClassId == clsStd.ClassId && x.StudentID == std.ID).ToList();
                    foreach (var j in clsLsn)
                    {
                        _db.ClassStudents.Remove(j);
                    }
                    var classStdFind =
                        _db.ClassStudents.FirstOrDefault(x => x.StudentID == std.ID && x.ClassId == clsStd.ClassId);
                    if (classStdFind == null)
                    {
                        var classStd = new ClassStudents
                        {
                            ClassId = clsStd.ClassId,
                            StudentID = std.ID,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Today
                        };
                        _db.ClassStudents.Add(classStd);
                    }
                    _db.Entry(std).State = EntityState.Modified;

                    return Json(_db.SaveChanges() > 0 ? "Update" : "Error");
                }
                else
                {
                    var stdControl = _db.Students.FirstOrDefault(x => x.TCNumber == std.TCNumber || x.FullName == std.FullName);
                    if (stdControl != null)
                    {
                        return Json("Saved");
                    }
                    std.Birthday = DateTime.Now;
                    _db.Students.Add(std);

                    var login = new Users
                    {
                        CreatedDate = DateTime.Now,
                        StudentId = std.ID,
                        Password = std.TCNumber,
                        Username = std.TCNumber ?? std.StudentNumber
                    };
                    _db.Users.Add(login);

                    var classStdFind =
                        _db.ClassStudents.FirstOrDefault(x => x.StudentID == std.ID && x.ClassId == clsStd.ClassId);
                    if (classStdFind == null)
                    {
                        var classStd = new ClassStudents
                        {
                            ClassId = clsStd.ClassId,
                            StudentID = std.ID,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Today
                        };
                        _db.ClassStudents.Add(classStd);
                    }
                    else
                    {
                        _db.Entry(clsStd).State = EntityState.Modified;
                    }
                }
            }
            return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
        }

        [Route("students/delete/{id?}")]
        [HttpPost]
        public JsonResult StudentsDelete(int id)
        {
            var std = _db.Students.ToList().Find(x => x.ID == id);
            if (std != null)
            {
                _db.Students.Remove(std);
                return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
            }

            return Json(std, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Teachers & Profession

        [Route("json/teachers"), HttpGet]
        public JsonResult TeacherList()
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var dataList = new List<TeacherViewModel>();
            var data = _db.Teachers.ToList();
            foreach (var item in data)
            {
                var teacherList = new TeacherViewModel
                {
                    FullName = item.FullName,
                    ID = item.ID,
                    TCNumber = item.TCNumber,
                    BirthDay = item.Birthday,
                    Picture = item.Picture,
                    Profession = _db.Professions.FirstOrDefault(x => x.ID == item.Profession)?.Profession
                };
                dataList.Add(teacherList);
            }
            return Json(new { data = dataList }, JsonRequestBehavior.AllowGet);
        }

        [Route("teachers/details/{id?}"), HttpGet]
        public JsonResult TeachersDetails(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var data = _db.Teachers.FirstOrDefault(x => x.ID == id);

            if (data == null) return Json((Teachers)null, JsonRequestBehavior.AllowGet);
            {
                var teacherList = new List<TeacherViewModel>
                {
                    new TeacherViewModel
                    {
                        ID = data.ID,
                        TCNumber = data.TCNumber,
                        FullName = data.FullName,
                        ProfessionId = _db.Professions.FirstOrDefault(x => x.ID == data.Profession)?.ID,
                        Profession = _db.Professions.FirstOrDefault(x => x.ID == data.Profession)?.Profession,
                        Picture = data.Picture,
                        BirthDay = data.Birthday
                    }
                };
                return Json(new { data = teacherList }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("teachers/add")]
        [HttpPost]
        public JsonResult TeacherAdd(Teachers tch)
        {
            if (ModelState.IsValid)
            {
                if (tch.ID > 0)
                {
                    _db.Entry(tch).State = EntityState.Modified;
                    return Json(_db.SaveChanges() > 0 ? "Update" : "Error");
                }
                else
                {
                    var stdControl = _db.Teachers.FirstOrDefault(x => x.TCNumber == tch.TCNumber || x.FullName == tch.FullName);
                    if (stdControl != null)
                    {
                        return Json("Saved");
                    }
                    _db.Teachers.Add(tch);

                    var login = new Users
                    {
                        Username = tch.TCNumber,
                        Password = tch.TCNumber,
                        CreatedDate = DateTime.Now,
                        TeacherId = tch.ID
                    };
                    _db.Users.Add(login);
                }
            }
            return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
        }

        [Route("teachers/delete/{id?}")]
        [HttpPost]
        public JsonResult TeacherDelete(int id)
        {
            var tch = _db.Teachers.ToList().Find(x => x.ID == id);
            if (tch != null)
            {
                _db.Teachers.Remove(tch);
                return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
            }

            return Json(tch, JsonRequestBehavior.AllowGet);
        }


        [Route("json/teachers/profession"), HttpGet]
        public JsonResult ProfessionList()
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var data = _db.Professions.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("teachers/profession/details/{id?}"), HttpGet]
        public JsonResult ProfessionDetails(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var data = _db.Professions.FirstOrDefault(x => x.ID == id);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("teachers/profession/add")]
        [HttpPost]
        public JsonResult ProfessionAdd(Professions pfs)
        {
            if (ModelState.IsValid)
            {
                if (pfs.ID > 0)
                {
                    _db.Entry(pfs).State = EntityState.Modified;
                    return Json(_db.SaveChanges() > 0 ? "Update" : "Error");
                }
                else
                {
                    var stdControl = _db.Professions.FirstOrDefault(x => x.Profession == pfs.Profession);
                    if (stdControl != null)
                    {
                        return Json("Saved");
                    }
                    _db.Professions.Add(pfs);
                }
            }
            return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
        }

        [Route("teachers/profession/delete/{id?}")]
        [HttpPost]
        public JsonResult ProfessionDelete(int id)
        {
            var pfs = _db.Professions.ToList().Find(x => x.ID == id);
            if (pfs != null)
            {
                _db.Professions.Remove(pfs);
                return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
            }

            return Json(pfs, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Lessons

        [Route("json/lessons"), HttpGet]
        public JsonResult LessonsList()
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var data = _db.Lesson.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("lessons/details/{id?}"), HttpGet]
        public JsonResult LessonsDetails(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var data = _db.Lesson.FirstOrDefault(x => x.ID == id);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("lessons/add")]
        [HttpPost]
        public JsonResult LessonsAdd(Lesson lss)
        {
            if (ModelState.IsValid)
            {
                if (lss.ID > 0)
                {
                    _db.Entry(lss).State = EntityState.Modified;
                    return Json(_db.SaveChanges() > 0 ? "Update" : "Error");
                }
                else
                {
                    var stdControl = _db.Lesson.FirstOrDefault(x => x.LessonName == lss.LessonName);
                    if (stdControl != null)
                    {
                        return Json("Saved");
                    }
                    _db.Lesson.Add(lss);
                }
            }
            return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
        }

        [Route("lessons/delete/{id?}")]
        [HttpPost]
        public JsonResult LessonsDelete(int id)
        {
            var lss = _db.Lesson.ToList().Find(x => x.ID == id);
            if (lss != null)
            {
                _db.Lesson.Remove(lss);
                return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
            }

            return Json(lss, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Class

        [Route("json/class/{id?}"), HttpGet]
        public JsonResult ClassList(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var data = new List<ClassViewModel>();

            if (id <= 0 || id == null)
            {
                var dataList = _db.Classes.OrderBy(x => x.ID).ToList();
                foreach (var item in dataList)
                {
                    var lessonFind = _db.ClassLessons.OrderBy(x => x.ID).Where(x => x.ClassID == item.ID).ToList();
                    var stdFind = _db.ClassStudents.OrderBy(x => x.ID).Where(x => x.ClassId == item.ID).ToList();
                    var addClass = new ClassViewModel
                    {
                        ID = item.ID,
                        ClassName = item.ClassName,
                        Teacher = _db.Teachers.FirstOrDefault(x => x.ID == item.TeacherID)?.FullName
                    };

                    var lessonList = new List<LessonsViewModel>();
                    foreach (var i in lessonFind)
                    {
                        lessonList.Add(new LessonsViewModel
                        {
                            ID = i.ID,
                            LessonName = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonID)?.LessonName
                        });
                    }
                    var stdList = new List<StudentsViewModel>();
                    foreach (var j in stdFind)
                    {
                        stdList.Add(new StudentsViewModel
                        {
                            ID = j.ID,
                            TCNumber = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.TCNumber,
                            StudentNumber = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.StudentNumber,
                            FullName = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.FullName,
                            Birthday = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.Birthday,
                            Image = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.Picture
                        });
                    }
                    addClass.Lessons = lessonList;
                    addClass.Students = stdList;
                    data.Add(addClass);
                }
            }
            else
            {
                var tchId = _db.Users.FirstOrDefault(x => x.ID == id)?.TeacherId;
                var dataList = _db.Classes.Where(x => x.TeacherID == tchId).ToList();
                foreach (var item in dataList)
                {
                    var lessonFind = _db.ClassLessons.OrderBy(x => x.ID).Where(x => x.ClassID == item.ID).ToList();
                    var stdFind = _db.ClassStudents.OrderBy(x => x.ID).Where(x => x.ClassId == item.ID).ToList();
                    var addClass = new ClassViewModel
                    {
                        ID = item.ID,
                        ClassName = item.ClassName,
                        Teacher = _db.Teachers.FirstOrDefault(x => x.ID == item.TeacherID)?.FullName
                    };

                    var lessonList = new List<LessonsViewModel>();
                    foreach (var i in lessonFind)
                    {
                        lessonList.Add(new LessonsViewModel
                        {
                            ID = i.ID,
                            LessonName = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonID)?.LessonName
                        });
                    }
                    var stdList = new List<StudentsViewModel>();
                    foreach (var j in stdFind)
                    {
                        stdList.Add(new StudentsViewModel
                        {
                            ID = j.ID,
                            TCNumber = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.TCNumber,
                            StudentNumber = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.StudentNumber,
                            FullName = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.FullName,
                            Birthday = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.Birthday,
                            Image = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.Picture
                        });
                    }
                    addClass.Lessons = lessonList;
                    addClass.Students = stdList;
                    data.Add(addClass);
                }
            }
            
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }


        [Route("class/details/{id?}"), HttpGet]
        public JsonResult ClassDetailList(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var data = new List<ClassViewModel>();
            var dataList = _db.Classes.OrderBy(x => x.ID).Where(x => x.ID == id).ToList();
            foreach (var item in dataList)
            {
                var lessonFind = _db.ClassLessons.OrderBy(x => x.ID).Where(x => x.ClassID == item.ID).ToList();
                var stdFind = _db.ClassStudents.OrderBy(x => x.ID).Where(x => x.ClassId == item.ID).ToList();
                var addClass = new ClassViewModel
                {
                    ID = item.ID,
                    ClassName = item.ClassName,
                    Teacher = _db.Teachers.FirstOrDefault(x => x.ID == item.TeacherID)?.FullName,
                    TeacherId = _db.Teachers.FirstOrDefault(x => x.ID == item.TeacherID)?.ID
                };

                var lessonList = new List<LessonsViewModel>();
                foreach (var i in lessonFind)
                {
                    lessonList.Add(new LessonsViewModel
                    {
                        ID = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonID)?.ID,
                        LessonName = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonID)?.LessonName
                    });
                }
                var stdList = new List<StudentsViewModel>();
                foreach (var j in stdFind)
                {
                    stdList.Add(new StudentsViewModel
                    {
                        ID = j.ID,
                        TCNumber = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.TCNumber,
                        StudentNumber = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.StudentNumber,
                        FullName = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.FullName,
                        Birthday = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.Birthday,
                        Image = _db.Students.FirstOrDefault(x => x.ID == j.StudentID)?.Picture
                    });
                }
                addClass.Lessons = lessonList;
                addClass.Students = stdList;
                data.Add(addClass);
            }
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("class/add")]
        [HttpPost]
        public JsonResult ClassAdd(Classes cls, int[] lessons)
        {
            if (ModelState.IsValid)
            {
                if (cls.ID > 0)
                {
                    _db.Configuration.LazyLoadingEnabled = false;
                    var clsLsn = _db.ClassLessons.Where(x => x.ClassID == cls.ID).ToList();
                    foreach (var j in clsLsn)
                    {
                        _db.ClassLessons.Remove(j);
                    }


                    _db.Entry(cls).State = EntityState.Modified;

                    foreach (var i in lessons)
                    {
                        var lesson = new ClassLessons
                        {
                            ClassID = cls.ID,
                            LessonID = i
                        };

                        _db.ClassLessons.Add(lesson);
                    }




                    return Json(_db.SaveChanges() > 0 ? "Update" : "Error");
                }
                else
                {
                    var stdControl = _db.Classes.FirstOrDefault(x => x.ClassName == cls.ClassName);
                    if (stdControl != null)
                    {
                        return Json("Saved");
                    }
                    _db.Classes.Add(cls);
                    foreach (var i in lessons)
                    {
                        var lesson = new ClassLessons
                        {
                            ClassID = cls.ID,
                            LessonID = i
                        };
                        _db.ClassLessons.Add(lesson);
                    }
                }
            }
            return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
        }

        [Route("class/delete/{id?}")]
        [HttpPost]
        public JsonResult ClassDelete(int id)
        {
            var cls = _db.Classes.ToList().Find(x => x.ID == id);
            if (cls != null)
            {
                _db.Classes.Remove(cls);
                return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
            }

            return Json(cls, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Class Schedule

        /// <summary>
        /// Sınıfın ders programı
        /// </summary>
        /// <param name="id">Kullanıcının ID'si</param>
        /// <returns></returns>
        [Route("json/schedule/{id?}"), HttpGet]
        public JsonResult ScheduleDetailsList(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var classScheduleList = new List<ClassScheduleViewModel>();

            var userId = _db.Users.FirstOrDefault(x => x.ID == id)?.StudentId ??
                         _db.Users.FirstOrDefault(x => x.ID == id)?.TeacherId;

            var userClassFind = _db.ClassStudents.FirstOrDefault(x => x.StudentID == userId)?.ClassId ??
                                _db.Classes.FirstOrDefault(x => x.TeacherID == userId)?.ID;
            var classScheduleFromDb = _db.ClassSchedule.Where(x => x.ClassId == userClassFind).ToList();

            foreach (var i in classScheduleFromDb)
            {
                var lesson = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName;
                classScheduleList.Add(new ClassScheduleViewModel
                {
                    ID = i.ID,
                    ClassId = i.ClassId,
                    Class = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                    Schedule = new ScheduleViewModel
                    {
                        Day = i.Day,
                        LessonId = i.LessonId,
                        Lesson = lesson,
                        Color = i.Color,
                        EndTime = i.EndTime,
                        StartTime = i.StartTime

                    }
                });
            }
            return Json(new
            {
                data = classScheduleList
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("json/schedule/list/{id?}"), HttpGet]
        public JsonResult ScheduleList(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var classScheduleList = new List<ClassScheduleViewModel>();

            var classScheduleFromDb = _db.ClassSchedule.Where(x => x.ClassId == id).ToList();

            foreach (var i in classScheduleFromDb)
            {
                var lesson = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName;
                classScheduleList.Add(new ClassScheduleViewModel
                {
                    ID = i.ID,
                    ClassId = i.ClassId,
                    Class = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                    Schedule = new ScheduleViewModel
                    {
                        Day = i.Day,
                        LessonId = i.LessonId,
                        Lesson = lesson,
                        Color = i.Color,
                        EndTime = i.EndTime,
                        StartTime = i.StartTime

                    }
                });
            }
            return Json(new
            {
                data = classScheduleList
            }, JsonRequestBehavior.AllowGet);
        }
        [Route("json/class/schedule"), HttpGet]
        public JsonResult ScheduleList()
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var classScheduleList = new List<ClassScheduleViewModel>();

            var classScheduleFromDb = _db.ClassSchedule.ToList();

            foreach (var i in classScheduleFromDb.OrderBy(x => x.ClassId))
            {
                var lesson = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName;
                classScheduleList.Add(new ClassScheduleViewModel
                {
                    ID = i.ID,
                    ClassId = i.ClassId,
                    Class = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                    Schedule = new ScheduleViewModel
                    {
                        Day = i.Day,
                        LessonId = i.LessonId,
                        Lesson = lesson,
                        Color = i.Color,
                        EndTime = i.EndTime,
                        StartTime = i.StartTime

                    }
                });
            }
            return Json(new
            {
                data = classScheduleList
            }, JsonRequestBehavior.AllowGet);
        }


        [Route("class/schedule/add")]
        [HttpPost]
        public JsonResult ScheduleAdd(ClassSchedule schedule)
        {
            if (ModelState.IsValid)
            {
                if (schedule.ID > 0)
                {
                    _db.Entry(schedule).State = EntityState.Modified;
                    return Json(_db.SaveChanges() > 0 ? "Update" : "Error");
                }
                else
                {
                    var stdControl = _db.ClassSchedule.Where(x => x.ClassId == schedule.ClassId && x.LessonId == schedule.LessonId && x.Day == schedule.Day).ToList();
                    if (stdControl.Count == 3)
                    {
                        return Json("Saved");
                    }
                    _db.ClassSchedule.Add(schedule);
                }
            }
            return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
        }

        [Route("json/class/schedule/details/{id?}"), HttpGet]
        public JsonResult ScheduleDetailList(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var classScheduleList = new List<ClassScheduleViewModel>();

            var i = _db.ClassSchedule.FirstOrDefault(x => x.ID == id);

            var lesson = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName;
            if (i != null)
                classScheduleList.Add(new ClassScheduleViewModel
                {
                    ID = i.ID,
                    ClassId = i.ClassId,
                    Class = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                    Schedule = new ScheduleViewModel
                    {
                        Day = i.Day,
                        LessonId = i.LessonId,
                        Lesson = lesson,
                        Color = i.Color,
                        EndTime = i.EndTime,
                        StartTime = i.StartTime
                    }
                });
            return Json(new
            {
                data = classScheduleList
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("class/schedule/delete/{id?}")]
        [HttpPost]
        public JsonResult ScheduleDelete(int id)
        {
            var schedule = _db.ClassSchedule.ToList().Find(x => x.ID == id);
            if (schedule != null)
            {
                _db.ClassSchedule.Remove(schedule);
                return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
            }

            return Json((ClassSchedule)null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Home Work

        [Route("homework/add"), HttpPost]
        public JsonResult HomeWorkAdd(HomeWork home)
        {
            if (ModelState.IsValid)
            {

                if (home.ID > 0)
                {
                    var oldData = _db.HomeWork.FirstOrDefault(x => x.ID == home.ID);
                    var data = new HomeWork
                    {
                        ID = home.ID,
                        ClassId = home.ClassId,
                        Description = home.Description,
                        EndTime = home.EndTime,
                        StartTime = home.StartTime,
                        FileName = home.FileName,
                        LessonId = home.LessonId,
                        Title = home.Title,
                        CreatedBy = oldData.CreatedBy,
                        CreatedDate = oldData.CreatedDate
                    };
                    _db.Set<HomeWork>().AddOrUpdate(data);
                    return Json(_db.SaveChanges() > 0 ? "Update" : "NoChanges");
                }
                else
                {
                    var userId = Convert.ToInt32(Session["userId"]);
                    var data = new HomeWork
                    {
                        ClassId = home.ClassId,
                        Description = home.Description,
                        EndTime = home.EndTime,
                        StartTime = home.StartTime,
                        FileName = home.FileName,
                        LessonId = home.LessonId,
                        Title = home.Title,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now
                    };
                    _db.HomeWork.Add(data);
                    return Json(_db.SaveChanges() > 0 ? "Success" : "NoChanges");
                }
            }
            return Json(home, JsonRequestBehavior.AllowGet);
        }

        [Route("json/homework")]
        public JsonResult HomeWorkList()
        {
            var list = _db.HomeWork.ToList();
            var data = new List<HomeWorkViewModel>();
            foreach (var i in list)
            {
                var createdById = _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.TeacherId ??
                                  _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID;
                data.Add(new HomeWorkViewModel
                {
                    ID = i.ID,
                    ClassId = i.ClassId,
                    ClassName = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                    LessonName = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName,
                    CreatedBy = _db.Teachers.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID,
                    CreatedByName = _db.Teachers.FirstOrDefault(x => x.ID == createdById)?.FullName ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.Username,
                    CreatedDate = i.CreatedDate,
                    LessonId = i.LessonId,
                    Description = i.Description,
                    Title = i.Title,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    FileName = i.FileName
                });
            }
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("json/homework/{id?}")]
        public JsonResult HomeWorkList(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;

            var data = new List<HomeWorkViewModel>();

            var list = _db.HomeWork.Where(x => x.CreatedBy == id).ToList();
            if (list.Count <= 0)
            {
                var stdId = _db.Users.FirstOrDefault(x => x.ID == id)?.StudentId;
                var userClass = _db.ClassStudents.FirstOrDefault(x => x.StudentID == stdId)?.ClassId;
                var userWorkList = _db.HomeWork.Where(x => x.ClassId == userClass).ToList();
                foreach (var i in userWorkList)
                {
                    var createdById = _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.TeacherId ??
                                      _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID;
                    data.Add(new HomeWorkViewModel
                    {
                        ID = i.ID,
                        ClassId = i.ClassId,
                        ClassName = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                        LessonName = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName,
                        CreatedBy = id,
                        CreatedByName = _db.Teachers.FirstOrDefault(x => x.ID == createdById)?.FullName ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.Username,
                        CreatedDate = i.CreatedDate,
                        LessonId = i.LessonId,
                        Description = i.Description,
                        Title = i.Title,
                        StartTime = i.StartTime,
                        EndTime = i.EndTime,
                        FileName = i.FileName
                    });
                }
            }
            else
            {
                foreach (var i in list)
                {
                    var createdById = _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.TeacherId ??
                                      _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID;
                    data.Add(new HomeWorkViewModel
                    {
                        ID = i.ID,
                        ClassId = i.ClassId,
                        ClassName = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                        LessonName = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName,
                        CreatedBy = id,
                        CreatedByName = _db.Teachers.FirstOrDefault(x => x.ID == createdById)?.FullName ??
                                        _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.Username,
                        CreatedDate = i.CreatedDate,
                        LessonId = i.LessonId,
                        Description = i.Description,
                        Title = i.Title,
                        StartTime = i.StartTime,
                        EndTime = i.EndTime,
                        FileName = i.FileName
                    });
                }
            }

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("json/homework/details/{id?}")]
        public JsonResult HomeWorkDetails(int? id)
        {
            _db.Configuration.LazyLoadingEnabled = false;
            var i = _db.HomeWork.FirstOrDefault(x => x.ID == id);

            var data = new List<HomeWorkViewModel>();

         
            if (i != null)
            {
                var createdById = _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.TeacherId ??
                                  _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID;
                data.Add(new HomeWorkViewModel
                {
                    ID = i.ID,
                    ClassId = i.ClassId,
                    ClassName = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                    LessonName = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName,
                    CreatedBy = id,
                    CreatedByName = _db.Teachers.FirstOrDefault(x => x.ID == createdById)?.FullName ??
                                    _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.Username,
                    CreatedDate = i.CreatedDate,
                    LessonId = i.LessonId,
                    Description = i.Description,
                    Title = i.Title,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    FileName = i.FileName
                });
            }
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("homework/delete/{id?}")]
        [HttpPost]
        public JsonResult HomeWorkDelete(int id)
        {
            var homework = _db.HomeWork.ToList().Find(x => x.ID == id);
            if (homework != null)
            {
                _db.HomeWork.Remove(homework);
            }
            return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
        }


       
        #endregion

        #region Announcement

        [Route("announcement/add"), HttpPost]
        public JsonResult AnnouncementAdd(AnnouncementViewModel model)
        {
            var date = _db.Announcements.FirstOrDefault(x => x.ID == model.ID)?.Date;
            var createdby = _db.Announcements.FirstOrDefault(x => x.ID == model.ID)?.CreatedBy;
            if (ModelState.IsValid)
            {
                if (model.ID > 0)
                {
                    var data = new Announcements
                    {
                        ID = model.ID,
                        StudentId = model.StudentId,
                        ClassId = model.ClassId,
                        Title = model.Title,
                        AllUser = model.AllUser,
                        LessonId = model.LessonId,
                        Description = model.Description,
                        Date = date,
                        CreatedBy = createdby
                    };
                    //_db.Entry(data).State = EntityState.Modified;
                    _db.Set<Announcements>().AddOrUpdate(data);

                    return Json(_db.SaveChanges() > 0 ? "Update" : "NoChanges");
                }
                else
                {
                    var data = new Announcements
                    {
                        ID = model.ID,
                        StudentId = model.StudentId,
                        ClassId = model.ClassId,
                        Date = DateTime.Now,
                        Title = model.Title,
                        AllUser = model.AllUser,
                        LessonId = model.LessonId,
                        Description = model.Description,
                        CreatedBy = model.CreatedByID
                    };
                    _db.Announcements.Add(data);
                    return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Route("json/announcement/details/{id?}")]
        public JsonResult AnnouncementDetail(int? id)
        {
            var data = new List<AnnouncementListViewModel>();
            var dataList = _db.Announcements.Where(x => x.ID == id).ToList();

            foreach (var i in dataList)
            {
                data.Add(new AnnouncementListViewModel
                {
                    ID = i.ID,
                    StudentId = i.StudentId,
                    ClassId = i.ClassId,
                    AllUser = i.AllUser,
                    Date = i.Date,
                    Description = i.Description,
                    LessonId = i.LessonId,
                    Title = i.Title,
                    Lesson = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName,
                    Class = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                    Student = _db.Students.FirstOrDefault(x => x.ID == i.StudentId)?.FullName,
                    CreatedByID = _db.Teachers.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID,
                    CreatedBy = _db.Teachers.FirstOrDefault(x => x.ID == i.CreatedBy)?.FullName ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.Username,
                });
            }

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Route("announcement/delete/{id?}")]
        [HttpPost]
        public JsonResult AnnouncementDelete(int id)
        {
            var anc = _db.Announcements.ToList().Find(x => x.ID == id);
            if (anc != null)
            {
                _db.Announcements.Remove(anc);
                return Json(_db.SaveChanges() > 0 ? "Success" : "Error");
            }

            return Json(anc, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Duyuruların json çıktısı alınması
        /// </summary>
        /// <param name="id">Sisteme giriş yapan kullanıcının ID'si gönderilir.</param>
        [Route("json/announcement/{id?}"), HttpGet]
        public JsonResult AnnouncementList(int? id)
        {
            var data = new List<AnnouncementListViewModel>();
            //Sisteme giriş yapan kişinin ID'sini öğrenci, öğretmen ve kullanıcı tablosunda arar.
            //Eğer öğrenciyse öğrenci Id'sini öğretmense Öğretmen Id'sini tanımlar


            var tchId = _db.Users.FirstOrDefault(x => x.ID == id)?.TeacherId;
            var stdId = _db.Users.FirstOrDefault(x => x.ID == id)?.StudentId;
            var userId = _db.Users.FirstOrDefault(x => x.ID == id && x.TeacherId == null && x.StudentId == null)?.ID;

            var tch = _db.Teachers.FirstOrDefault(x => x.ID == tchId);
            var std = _db.Students.FirstOrDefault(x => x.ID == stdId);
            var user = _db.Users.FirstOrDefault(x => x.ID == userId);

            //Siteme giriş yapan kullanıcı eğer öğrenciyse sınıfının Id'sini atar.
            var stdClassId = _db.ClassStudents.FirstOrDefault(x => x.StudentID == stdId)?.Classes.ID;
            var tchClassId = _db.Users.FirstOrDefault(x => x.TeacherId == tchId)?.ID;
            if (stdClassId != null)
            {
                var list = _db.Announcements.Where(x => x.AllUser == true || x.StudentId == stdId || x.ClassId == stdClassId).OrderByDescending(x => x.Date).ToList();
                foreach (var i in list)
                {
                    var createdId = _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.TeacherId ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID;
                    data.Add(new AnnouncementListViewModel
                    {
                        ID = i.ID,
                        StudentId = i.StudentId,
                        ClassId = i.ClassId,
                        AllUser = i.AllUser,
                        Date = i.Date,
                        Description = i.Description,
                        LessonId = i.LessonId,
                        Title = i.Title,
                        Lesson = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName,
                        Class = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                        Student = _db.Students.FirstOrDefault(x => x.ID == i.StudentId)?.FullName,
                        CreatedByID = _db.Teachers.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID,
                        CreatedBy = _db.Teachers.FirstOrDefault(x => x.ID == createdId)?.FullName ?? _db.Users.FirstOrDefault(x => x.ID == createdId)?.Username
                    });
                }
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
            //Siteme giriş yapan kullanıcı eğer öğretmense ve hangi sınıfın öğretmeniyse o sınıfın Id'sini atar.
            else if (tchId != null)
            {
                var datalist = _db.Announcements.Where(x => x.CreatedBy == id).ToList();
                foreach (var i in datalist.OrderByDescending(x => x.Date))
                {
                    var teacherId = _db.Users.FirstOrDefault(x => x.TeacherId == i.CreatedBy)?.TeacherId;

                    data.Add(new AnnouncementListViewModel
                    {
                        ID = i.ID,
                        StudentId = i.StudentId,
                        ClassId = i.ClassId,
                        AllUser = i.AllUser,
                        Date = i.Date,
                        Description = i.Description,
                        LessonId = i.LessonId,
                        Title = i.Title,
                        Lesson = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName,
                        Class = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                        Student = _db.Students.FirstOrDefault(x => x.ID == i.StudentId)?.FullName,
                        CreatedByID = _db.Teachers.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID,
                        CreatedBy = _db.Teachers.FirstOrDefault(x => x.ID == tchId)?.FullName ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.Username,
                    });
                }
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var dataList = _db.Announcements.ToList();

                foreach (var i in dataList)
                {
                    var ogrId = _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.TeacherId;
                    data.Add(new AnnouncementListViewModel
                    {
                        ID = i.ID,
                        StudentId = i.StudentId,
                        ClassId = i.ClassId,
                        AllUser = i.AllUser,
                        Date = i.Date,
                        Description = i.Description,
                        LessonId = i.LessonId,
                        Title = i.Title,
                        Lesson = _db.Lesson.FirstOrDefault(x => x.ID == i.LessonId)?.LessonName,
                        Class = _db.Classes.FirstOrDefault(x => x.ID == i.ClassId)?.ClassName,
                        Student = _db.Students.FirstOrDefault(x => x.ID == i.StudentId)?.FullName,
                        CreatedByID = ogrId ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.ID,
                        CreatedBy = _db.Teachers.FirstOrDefault(x => x.ID == ogrId)?.FullName ?? _db.Users.FirstOrDefault(x => x.ID == i.CreatedBy)?.Username
                    });
                }
            }

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Select2 Datas

        [HttpGet]
        [Route("json/select2/students")]
        public JsonResult StudentList(string q)
        {
            var list2 = _db.Students.Select(a => new
            {
                text = a.FullName,
                id = a.ID
            });

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list2 = list2.Where(x => x.text.ToLower().Contains(q.ToLower()));
            }
            return Json(new { items = list2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("json/select2/teachers")]
        public JsonResult TeachersList(string q)
        {
            var list2 = _db.Teachers.Select(a => new
            {
                text = a.FullName,
                id = a.ID
            });

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list2 = list2.Where(x => x.text.ToLower().Contains(q.ToLower()));
            }
            return Json(new { items = list2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("json/select2/teachers/class/{id?}")]
        public JsonResult TeacherClassList(int? id, string q)
        {
            var teacherFind = _db.Users.FirstOrDefault(x => x.ID == id)?.TeacherId;
            var lessonFind = _db.Teachers.OrderBy(x => x.ID).Where(x => x.ID == teacherFind).ToList();

            var list2 = lessonFind.Select(a => new
            {
                text = _db.Classes.FirstOrDefault(x => x.TeacherID == a.ID)?.ClassName,
                id = _db.Classes.FirstOrDefault(x => x.TeacherID == a.ID)?.ID
            });

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list2 = list2.Where(x => x.text.ToLower().Contains(q.ToLower()));
            }
            return Json(new { items = list2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("json/select2/teachers/class/student/{id?}")]
        public JsonResult TeacherStudentList(int? id, string q)
        {
            var teacherFind = _db.Users.FirstOrDefault(x => x.ID == id)?.TeacherId;
            var lessonFind = _db.Teachers.FirstOrDefault(x => x.ID == teacherFind);
            var teacherClass = _db.Classes.FirstOrDefault(x => x.TeacherID == lessonFind.ID);
            var classList = _db.ClassStudents.Where(x => x.ClassId == teacherClass.ID).ToList();
            var list2 = classList.Select(a => new
            {
                text = _db.Students.FirstOrDefault(x => x.ID == a.StudentID)?.FullName,
                id = _db.Students.FirstOrDefault(x => x.ID == a.StudentID)?.ID
            });

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list2 = list2.Where(x => x.text.ToLower().Contains(q.ToLower()));
            }
            return Json(new { items = list2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("json/select2/profession")]
        public JsonResult ProfessionList(string q)
        {
            var list2 = _db.Professions.Select(a => new
            {
                text = a.Profession,
                id = a.ID
            });

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list2 = list2.Where(x => x.text.ToLower().Contains(q.ToLower()));
            }
            return Json(new { items = list2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("json/select2/lessons")]
        public JsonResult LessonsList(string q)
        {
            var list2 = _db.Lesson.Select(a => new
            {
                text = a.LessonName,
                id = a.ID
            });

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list2 = list2.Where(x => x.text.ToLower().Contains(q.ToLower()));
            }
            return Json(new { items = list2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("json/select2/class")]
        public JsonResult ClassList(string q)
        {
            var list2 = _db.Classes.Select(a => new
            {
                text = a.ClassName,
                id = a.ID
            });

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list2 = list2.Where(x => x.text.ToLower().Contains(q.ToLower()));
            }
            return Json(new { items = list2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("json/select2/class/{id?}")]
        public JsonResult ClassLessonList(int? id, string q)
        {

            //var userClassFind = _db.ClassStudents.FirstOrDefault(x => x.StudentID == id);
            var lessonFind = _db.ClassLessons.OrderBy(x => x.ID).Where(x => x.ClassID == id).ToList();

            var list2 = lessonFind.Select(a => new
            {
                text = a.Lesson.LessonName,
                id = _db.Lesson.FirstOrDefault(x => x.LessonName == a.Lesson.LessonName)?.ID
            });

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list2 = list2.Where(x => x.text.ToLower().Contains(q.ToLower()));
            }
            return Json(new { items = list2 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion



    }
}