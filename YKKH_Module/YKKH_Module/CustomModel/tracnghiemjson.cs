using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YKKH_Module.CustomModel
{
    public class tracnghiemjson
    {
        public List<TracNghiemArray> data { get; set; }
        public string articleID { get; set; }
    }

    public class TracNghiemArray
    {
        public string id { get; set; }
        public string value { get; set; }
        public string parent { get; set; }
        public bool select { get; set; }
        public string group { get; set; }
        public string codgrp { get; set; }
        public string stoques { get; set; }
        public string stoans { get; set; }
        public string contans { get; set; }
        public int stt { get; set; }
    }
}