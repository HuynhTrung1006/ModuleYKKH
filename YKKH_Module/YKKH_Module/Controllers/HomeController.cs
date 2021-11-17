using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YKKH_Module.CustomModel;
using YKKH_Module.Models;

namespace YKKH_Module.Controllers
{
    
    public class HomeController : Controller
    {
        private Models.Module_YKKHEntities db = new Models.Module_YKKHEntities();
        int so = 0;
        public ActionResult Index()
        {
            
            return View(db.CHUDEs.ToList());
        }
        public ActionResult login()
        {
            return View();
        }

        public ActionResult loginadmin(Models.Quantri model)
        {
            Models.Module_YKKHEntities db = new Models.Module_YKKHEntities();
            if(ModelState.IsValid)
            {
                Models.Quantri result = db.Quantris.Find(model.maQT);
                if (result != null)
                {
                    if (result.password.Contains(model.password) == false)
                    {
                        ModelState.AddModelError("password", "Mật khẩu không hợp lệ");
                        return View("login");
                    }
                }
                else
                {
                    ModelState.AddModelError("password", "Tên đăng nhập hoặc mật khẩu không hợp lệ!");
                    return View("login");
                }
                Session["User"] = result.hoten;
            }
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult logout()
        {
            Session["User"] = null;
            return View("login");
        }

        public ActionResult chonChuDe(string id)
        {
            ViewBag.tenCD = db.CHUDEs.Find(id).tenCD.ToString();
            ViewBag.maCD = db.CHUDEs.Find(id).maCD.ToString();
            var dethi = db.CAUHOIs.Where(x => x.maCD.Contains(id)).ToList();
            var dapan = db.DAPANs.ToList();
            List<cauhoitracnghiem> chtt = new List<cauhoitracnghiem>();
            List<Models.CAUHOI> chgy = new List<Models.CAUHOI>();
            foreach(var check in dethi)
            {
                List<Models.DAPAN> ds = new List<Models.DAPAN>();
                foreach (var checkda in dapan)
                {
                    if(checkda.maCH.ToString().Contains(check.maCH.ToString()))
                    {
                        ds.Add(checkda);
                    }
                }

                cauhoitracnghiem add = new cauhoitracnghiem();
                add.CAUHOI = check;
                add.listDapAn = ds;
                if (add.listDapAn.Count()>0)
                {
                    chtt.Add(add);
                }
                else
                {
                    chgy.Add(check);
                }
                
            }
            ViewBag.chtt = chtt;
            ViewBag.chgy = chgy;
            return View(dethi);

        }

        public ActionResult SubmitTest(FormCollection form)
        {
            //string maKH = form["maKH"].ToString();
            //string hotenKH = form["hotenKH"];
            //string emailKH = form["emailKH"];
            //string sdtKH = form["sdtKH"];
            //int dudoanKH = int.Parse(form["dudoanKH"]);

            List<YKKH_Module.CustomModel.TraLoiTT> a = Session["DATTKH"] as List<TraLoiTT>;
            
            return null;
        }

        [HttpPost]
        public void UpdateDA(FormCollection form)
        {
            int mach = int.Parse(form["ch"]);
            int mada = int.Parse(form["da"]);
            string makh = form["makh"].Trim() ;
            if(makh.Contains(""))
            {
                return;
            }
            detail_THI detail = new detail_THI();
            detail.maCD = mach;
            detail.maDA = mada;
            detail.maKH = makh;
            db.detail_THI.Add(detail);
            db.SaveChanges();
        }

        public ActionResult KiemtraKH(string id)
        {
            ViewBag.maCD = id;
            return View();
        }
        
        [HttpPost]
        public ActionResult checkKH(KHACHHANG kh, FormCollection form)
        {
            if(kh.maKH!=null&&kh.tenKH!=null&&kh.emailKH!=null&&kh.dienthoaiKH!=null)
            {
                string maCD = form["maCD"];
                KHACHHANG check = db.KHACHHANGs.Find(kh.maKH);
                if (check == null)
                {
                    db.KHACHHANGs.Add(kh);
                    db.SaveChanges();
                    Session["KhachHang"] = kh.maKH;
                    return Redirect("/Home/chonChuDe/" + maCD);
                }
                Session["KhachHang"] = kh.maKH;
                return Redirect("/Home/chonChuDe/" + maCD);
            }
            ModelState.AddModelError("dienthoaiKH", "Bạn nhập đầy đủ thông tin");
            return View("KiemtraKH");
        }
        [HttpPost]
        public void addSessionTT(FormCollection form)
        {
            int mada = Convert.ToInt32(form["id"]);
            List<ListDATTKH> a = Session["DATTKH"] as List<ListDATTKH>;
            ListDATTKH b = new ListDATTKH();
            b.maDA = mada;
            a.Add(b);
            Session["DATTKH"] = a;
        }

        [HttpPost]
        public void addSessionGY(FormCollection form)
        {
            int id = Convert.ToInt32(form["id"]);
            string value = form["value"];
            List<ListDAGY> a = Session["DAGYKH"] as List<ListDAGY>;
            ListDAGY b = new ListDAGY();
            b.id = id;
            b.value = value;
            a.Add(b);
            Session["DAGYKH"] = a;
        }

        [HttpPost]
        public ActionResult AddSubmitKH(FormCollection form)
        {
            int dudoansl = int.Parse(form["songuoidudoan"]);
            string tenCD = form["tenCD"];
            
            List<ListDATTKH> listDATT = Session["DATTKH"] as List<ListDATTKH>;
            List<ListDAGY> listDAGY = Session["DAGYKH"] as List<ListDAGY>;

            

            List<DAPAN> listDA = db.DAPANs.ToList();
            int dem = 0;
            foreach (var item in listDATT)
            {
                foreach (var items in listDA)
                {
                    if (item.maDA.ToString().Contains(items.maDA.ToString())&&items.dapandung==true)
                    {
                        dem++;
                    }
                }
            }

            foreach (var item in listDAGY)
            {
                DAPANGOPY gy = new DAPANGOPY();
                gy.maCH = item.id;
                gy.gopyKH = item.value;
                db.DAPANGOPies.Add(gy);
                db.SaveChanges();
            }

            THI thi = new THI();
            thi.maCD = tenCD;
            thi.maKH = Session["KhachHang"].ToString();
            thi.songuoithamgia = dudoansl;
            thi.socaudung = dem;

            db.THIs.Add(thi);
            db.SaveChanges();

            Session["MaCDKH"] = tenCD;
            so = dem;
            return RedirectToAction("Preview");
        }

        public ActionResult Preview()
        {
            ViewBag.makh = Session["KhachHang"].ToString();
            ViewBag.maCDKH = Session["MaCDKH"].ToString();
            ViewBag.KQKH = so;
            

            return View();
        }

        public ActionResult Thoat()
        {
            Session["DATTKH"] = null;
            Session["DAGYKH"] = null;
            Session["KhachHang"] = null;
            Session["MaCDKH"] = null;
            so = 0;
            return RedirectToAction("Index");
        }
    }
}