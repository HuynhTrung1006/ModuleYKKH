﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YKKH_Module.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Module_YKKHEntities : DbContext
    {
        public Module_YKKHEntities()
            : base("name=Module_YKKHEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CAUHOI> CAUHOIs { get; set; }
        public virtual DbSet<CHUDE> CHUDEs { get; set; }
        public virtual DbSet<DAPAN> DAPANs { get; set; }
        public virtual DbSet<DAPANGOPY> DAPANGOPies { get; set; }
        public virtual DbSet<detail_THI> detail_THI { get; set; }
        public virtual DbSet<KHACHHANG> KHACHHANGs { get; set; }
        public virtual DbSet<Quantri> Quantris { get; set; }
        public virtual DbSet<THI> THIs { get; set; }
        public virtual DbSet<TRUNGTHUONG> TRUNGTHUONGs { get; set; }
    }
}