using BundleTransformer.Core.Constants;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YKKH_Module.CustomModel;

namespace YKKH_Module.Controllers
{
    public class AdminController : Controller
    {
        private Models.Module_YKKHEntities _db = new Models.Module_YKKHEntities(); 
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexCD()
        {
            ViewBag.note = null;
            return View(_db.CHUDEs.ToList()) ;
        }

        public ActionResult formThemCD(string id)
        {

            var a = _db.CAUHOIs.Where(x => x.maCD == id).ToList();
            ViewBag.maCD = id;
            ViewBag.dsDA = _db.DAPANs.ToList();
            return View(a);
        }

        public ActionResult formdetailCH(int id)
        {
            var a = _db.DAPANs.Where(x => x.maCH == id).ToList();
            /*ViewBag.CauHoi = _db.CAUHOIs.Find(id).tenCH.ToString();*/
            var cauhoi = _db.CAUHOIs.Find(id);
            Models.DAPAN dapandung= new Models.DAPAN();
            foreach(Models.DAPAN ch in a.ToList())
            {
                if(ch.dapandung==true)
                {
                    dapandung = ch;
                    break;
                }
            }
            var cauA = _db.DAPANs.Where(x => x.maCH == id).First();
            a.Remove(cauA);
            var cauB= a.Where(x=>x.maCH==id).First();
            a.Remove(cauB);
            var cauC = a.Where(x => x.maCH == id).First();
            a.Remove(cauC);
            var cauD = a.Where(x => x.maCH == id).First();
            a.Remove(cauD);

            ViewBag.cauhoi = cauhoi;
            ViewBag.dapandung = dapandung;
            ViewBag.cauA = cauA;
            ViewBag.cauB = cauB;
            ViewBag.cauC = cauC;
            ViewBag.cauD = cauD;
            
            return View();
        }

        
        [HttpPost]
        public ActionResult addCD()
        {
            Models.CHUDE iCD = new Models.CHUDE();
            iCD.maCD = Request["txtmaCD"].ToString();
            iCD.tenCD = Request["txttenCD"].ToString();

            var check = _db.CHUDEs.Find(iCD.maCD);
            if (check != null) return null;

            _db.CHUDEs.Add(iCD);
            _db.SaveChanges();

            return RedirectToAction("IndexCD");

        }

        public ActionResult checkCD(string id)
        {
            var check = _db.CHUDEs.Find(id);
            if (check != null)
                return PartialView();
            return null;
        }

        [HttpPost]
        public ActionResult AddQuestion(FormCollection form, HttpPostedFileBase File)
        {
            string macd = form["macd"];
            string content = form["content"];
            string[] answer = new string[] {
                form["answer_a"],
                form["answer_b"],
                form["answer_c"],
                form["answer_d"]
            };
            //answer = Common.ShuffleArray.Randomize(answer);
            string answer_a = answer[0];
            string answer_b = answer[1];
            string answer_c = answer[2];
            string answer_d = answer[3];
            string correct_answer = form["correct_answer"];
            bool dapandung_a = false;
            bool dapandung_b = false;
            bool dapandung_c = false;
            bool dapandung_d = false;
            if (correct_answer.Contains(answer_a)) dapandung_a = true;
            else if (correct_answer.Contains(answer_b)) dapandung_b = true;
            else if (correct_answer.Contains(answer_c)) dapandung_c = true;
            else if(correct_answer.Contains(answer_d)) dapandung_d = true;
            Models.CAUHOI a = new Models.CAUHOI();
            Models.DAPAN b = new Models.DAPAN();
            a.maCD = macd;
            a.tenCH = content;
            _db.CAUHOIs.Add(a);
            _db.SaveChanges();
            // save answer A
            bool kq_a=addDA(a.maCH, answer_a, dapandung_a);
            //save answer B
            bool kq_b=addDA(a.maCH, answer_b, dapandung_b);
            //save answer c
            bool kq_c=addDA(a.maCH, answer_c, dapandung_c);
            //save answer d
            bool kq_d=addDA(a.maCH, answer_d, dapandung_d);

            if(kq_a==true||kq_b==true||kq_c==true||kq_d==true)
            {
                TempData["status_id"] = true;
                TempData["status"] = "Thêm Thành Công";
            }
            else
            {
                TempData["status_id"] = false;
                TempData["status"] = "Thêm Thất Bại";
            }

            return Redirect("/Admin/formThemCD/" + macd);
        }
        public bool addDA(int mach, string tenda, bool dapandung)
        {
            Models.DAPAN da = new Models.DAPAN();
            da.maCH = mach;
            da.tenDA = tenda;
            da.dapandung = dapandung;

            _db.DAPANs.Add(da);
            _db.SaveChanges();
            return true;
        }

        [HttpPost]
        public ActionResult AddQuestionGY(FormCollection form, HttpPostedFileBase File)
        {
            string macd = form["macd"];
            string content = form["content"];
            Models.CAUHOI a = new Models.CAUHOI();
            a.maCD = macd;
            a.tenCH = content;
            _db.CAUHOIs.Add(a);
            _db.SaveChanges();

            return Redirect("/Admin/formThemCD/"+macd);
        }

        public ActionResult DeleteCH(int id)
        {
            var result = _db.CAUHOIs.Find(id);
            string macd = result.maCD;
            if(result!=null)
            {
                foreach(var check in _db.DAPANs.ToList())
                {
                    if(result.maCH.ToString().Contains(check.maCH.ToString()))
                    {
                        _db.DAPANs.Remove(check);
                        _db.SaveChanges();
                    }
                }
            }

            _db.CAUHOIs.Remove(result);
            _db.SaveChanges();
            return Redirect("/Admin/formThemCD/" + macd);
        }

        [HttpPost]
        public ActionResult EditQuestion(FormCollection form, HttpPostedFileBase File)
        {
            string mach = form["mach"];
            string content = form["content"];
            string[] answer = new string[] {
                form["answer_a"],
                form["answer_b"],
                form["answer_c"],
                form["answer_d"]
            };
            //xoa di dap an dax ton tai
            List<Models.DAPAN> list = _db.DAPANs.Where(x => x.maCH.ToString().Equals(mach)).ToList();
            foreach(var x in list)
            {
                _db.DAPANs.Remove(x);
                _db.SaveChanges();
            }
            //xoa cau hoi
            var ch = _db.CAUHOIs.Where(x=>x.maCH.ToString()==mach).FirstOrDefault();
            string macd = ch.maCD;
            if(ch!=null)
            {
                _db.CAUHOIs.Remove(ch);
                _db.SaveChanges();
            }

            //answer = Common.ShuffleArray.Randomize(answer);
            string answer_a = answer[0];
            string answer_b = answer[1];
            string answer_c = answer[2];
            string answer_d = answer[3];
            string correct_answer = form["correct_answer"];
            bool dapandung_a = false;
            bool dapandung_b = false;
            bool dapandung_c = false;
            bool dapandung_d = false;
            if (correct_answer.Contains(answer_a)) dapandung_a = true;
            else if (correct_answer.Contains(answer_b)) dapandung_b = true;
            else if (correct_answer.Contains(answer_c)) dapandung_c = true;
            else if (correct_answer.Contains(answer_d)) dapandung_d = true;
            Models.CAUHOI a = new Models.CAUHOI();
            Models.DAPAN b = new Models.DAPAN();
            a.maCD = macd;
            a.tenCH = content;
            _db.CAUHOIs.Add(a);
            _db.SaveChanges();
            // save answer A
            bool kq_a = addDA(a.maCH, answer_a, dapandung_a);
            //save answer B
            bool kq_b = addDA(a.maCH, answer_b, dapandung_b);
            //save answer c
            bool kq_c = addDA(a.maCH, answer_c, dapandung_c);
            //save answer d
            bool kq_d = addDA(a.maCH, answer_d, dapandung_d);

            if (kq_a == true || kq_b == true || kq_c == true || kq_d == true)
            {
                TempData["status_id"] = true;
                TempData["status"] = "Thêm Thành Công";
            }
            else
            {
                TempData["status_id"] = false;
                TempData["status"] = "Thêm Thất Bại";
            }

            return Redirect("/Admin/formThemCD/" + macd);
        }



        //KHACH HANG THAM GIA

        public ActionResult IndexKH()
        {
        
            return View(_db.THIs.ToList());
        }
        public ActionResult IndexTrungThuong()
        {
            ViewBag.DSTT = _db.TRUNGTHUONGs.ToList();
            return View();
        }

        public ActionResult actionTT(int id)
        {
            var check = _db.THIs.Find(id);
            if (check == null)
            {
                return null;
            }


            

            Models.TRUNGTHUONG tt = new Models.TRUNGTHUONG();
            tt.maCD = check.maCD;
            tt.maKH = check.maKH;
            tt.songuoithamgia = check.songuoithamgia;
            tt.socaudung = check.socaudung;
            tt.maTT = check.maT;
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Views/Admin/MailTT.html"));
            content = content.Replace("{{tenkhachhang}}", check.KHACHHANG.tenKH);
            content = content.Replace("{{makhachhang}}", check.maKH);
            content = content.Replace("{{chude}}", check.CHUDE.tenCD);
            content = content.Replace("{{socaudung}}", check.socaudung.ToString());

            new MailHelper().SendMail(check.KHACHHANG.emailKH, "Thông Báo Trúng Thưởng EVNSPC", content);

            _db.TRUNGTHUONGs.Add(tt);
            _db.SaveChanges()
            ;
            return RedirectToAction("IndexTrungThuong");
        }
        public ActionResult deleteCD(string id)
        {

            var checkCD = _db.CHUDEs.Find(id);
            var checkT = _db.THIs.Where(x => x.maCD.Contains(checkCD.maCD)).ToList();

            if(checkT.Count>0)
            {
                TempData["note"] = "Không Thể Xóa Chủ Đề Do Khách Hàng Đã Tham Gia Chủ Đề";
                return Redirect("/Admin/IndexCD");
            }

            List<Models.CAUHOI> listCH = _db.CAUHOIs.Where(x => x.maCD.Contains(checkCD.maCD)).ToList();
            foreach(var DA in listCH)
            {
                List<Models.DAPAN> checkDA = _db.DAPANs.Where(x => x.maCH.ToString().Contains(DA.maCH.ToString())).ToList();
                if (checkDA != null)
                {
                    foreach (var deDA in checkDA)
                    {
                        _db.DAPANs.Remove(deDA);
                        _db.SaveChanges();
                    }
                }
                _db.CAUHOIs.Remove(DA);
                _db.SaveChanges();
            }
            _db.CHUDEs.Remove(checkCD);
            _db.SaveChanges();
            TempData["note"] = "Thành Công";
            return Redirect("/Admin/IndexCD");
        }
    }
}