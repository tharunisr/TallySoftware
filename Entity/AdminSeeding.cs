using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace TallySoftware.Entity
{
    public static class AdminSeeding
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var Staff = new Staff { StaffId=1,StaffName="admin",StaffType="Admin",Password="snapy@2020"
            ,ConfirmPassword="snapy@2020"};
            modelBuilder.Entity<Staff>().HasData(Staff);
        }

    }
}
