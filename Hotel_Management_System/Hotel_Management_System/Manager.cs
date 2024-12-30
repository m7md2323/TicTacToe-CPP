﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//written by Zaid
namespace Hotel_Management_System
{
    [Serializable]
    internal class Manager
    {
        string Id = "m2024";
        int password = 00;
        
        public string ID
        {
            get { return Id; }
        }
        public int Password
        {
            get { return password;}
        }
      
        public  void viewAllGuests() {
            Console.WriteLine("viewing all guests..");

            List<Guest> GuestsList = new List<Guest>();
            FileStream fs = new FileStream("Guest.txt",FileMode.Open,FileAccess.Read);
            while (fs.Position < fs.Length)
            {
                object requiredGuest = DatabaseServer.bf.Deserialize(fs);
                GuestsList.Add((Guest)requiredGuest);
            
;                
            }
            for (int i=0;i<GuestsList.Count;i++) {
                GuestsList[i].DisplayAllInfo();
            }
            Console.WriteLine("guests successfully, enter [1] to get another manager service or [0] to logOut");
            int choice =Convert.ToInt32(Console.ReadLine());
            if (choice == 1) { SystemHandler.ChooseManagerService(); }
            else
                SystemHandler.ChooseUser();




        }
        
        public void ViewAllReseravtions()
        {
            
            


                Console.WriteLine("viewing all reservations..");
                List<Reservation> ReservationsList = DatabaseServer.GetAllReservations();

                for (int i = 0; i < ReservationsList.Count; i++) { ReservationsList[i].DisplayAllInfo(); }
                Console.WriteLine("\nreservations displayed successfully,type [1] to use another manager service or [0] To exit");
                int choice = Convert.ToInt32(Console.ReadLine());
                if (choice == 1) { SystemHandler.ChooseManagerService(); }
                else SystemHandler.ChooseUser();

            

        }
        public void ViewAllServices()
        {


            Console.WriteLine("viewing all Services..");
            List<Service> ServicessList = DatabaseServer.GetAllServices();

            for (int i = 0; i < ServicessList.Count; i++) { ServicessList[i].DisplayAllInfo(); }
            Console.WriteLine("\nServices displayed successfully,type [1] to use another manager service or [0] To exit");
            int choice = Convert.ToInt32(Console.ReadLine());
            if (choice == 1) { SystemHandler.ChooseManagerService(); }
            else SystemHandler.ChooseUser();

        }
        public void viewAllPayments()
        {
            Console.WriteLine("viewing all payments..");
            List<Payment>AllPaymentsList =DatabaseServer.GetAllPayments();
            for (int i=0;i<AllPaymentsList.Count;i++) {
                AllPaymentsList[i].DisplayAllInfo();
            }
            Console.WriteLine("Payments successfully aquired,type [1] to use another manager service or [0] To exit");
            int choice = Convert.ToInt32(Console.ReadLine());
            if (choice == 1) 
            {
                SystemHandler.ChooseManagerService();   
            }
            else SystemHandler.ChooseUser();

        }
        public void viewAllRooms()
        {
            Console.WriteLine("viewing all rooms..\n");
            List<Room>RoomsList=DatabaseServer.GetAllRooms();
            for (int i = 0; i < RoomsList.Count; i++)
            {
                RoomsList[i].DisplayAllInfo();
            }
                Console.WriteLine("Rooms successfully aquired,type [1] to use another manager service or [0] To exit");
                int choice = Convert.ToInt32(Console.ReadLine());
                if (choice == 1) { SystemHandler.ChooseManagerService(); }
                else SystemHandler.ChooseUser();
            
            


        }
        public void updateRoomInfo()
        {
            Console.WriteLine("update room info..");

        }
        public void generateProfitReport()
        {
            Console.WriteLine("generating profit report..");

        }
    }
}
