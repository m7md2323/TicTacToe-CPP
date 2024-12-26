﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hotel_Management_System
{
    public enum SystemState
    {
        USER_SELECTION=1,
        MANAGER_LOGIN,
        GUEST_LOGIN,
        GUEST_MENU,
        MANAGER_MENU,
        EXIT
    }
    public enum UserType{
        MANAGER=1,
        GUEST,
        EXIT,
        INVALID_SELECTION

    }
    public enum GuestServiceSelection
    {
        RESERVE_A_ROOM=1,//int const RESERVE_A_ROOM=1;
        REQUEST_A_SERVICE,//int const REQUEST_A_SERVICE=2;
        CHECK_IN,
        CHECK_OUT,
        PAY_FOR_A_RESERVATION,
        PAY_FOR_A_SERVICE,
        LOGOUT
    }
    internal class SystemHandler
    {   
        static Guest loggedInGuest;
        static Manager manager;
        static public SystemState systemState=SystemState.USER_SELECTION;
        public static UserType ChooseUser() {
            systemState = SystemState.USER_SELECTION;
            Console.WriteLine("--------------------[ Hotel Management System ]--------------------");
            Console.WriteLine("Please select the account type : ");
            Console.WriteLine("1. Manager \n2. Guest \n3. To Exit the System.");
            int userChoice=Convert.ToInt32(Console.ReadLine());
            return (UserType)userChoice;
            
        }
        public static bool GuestLogin()
        {
                systemState = SystemState.GUEST_LOGIN;
                Console.Clear();
                Console.WriteLine("--------------------[ Guest Login ]--------------------");
                Console.WriteLine("Please enter your National ID and Password");
                Console.Write("National ID : ");
                int guestID = Convert.ToInt32(Console.ReadLine());
                Console.Write("Password : ");
                int Password= Convert.ToInt32(Console.ReadLine());
                Console.Write("Verifying");LineOfDots();
                Console.Clear();
                if (GuestValidator(guestID, Password))
                {
                    Console.WriteLine("Successful Login!!");
                    Console.WriteLine();
                    loggedInGuest = DatabaseServer.LoadGuestUsingId(guestID);
                    Console.WriteLine($"Hello Mr.{loggedInGuest.Name} and welcome to our hotel system.");
                    Console.WriteLine();
                    EnterGuestSystem();
                }
                else
                {
                    Console.WriteLine("Unsuccessful Login attempt!!");
                    Console.WriteLine("National ID or Password is wrong, please try again!!");
                    Console.WriteLine("Press (1) to try again, (0) to Exit the system.");
                    if (Console.ReadLine() == "1") changeState(SystemState.GUEST_LOGIN);
                    else changeState(SystemState.EXIT);

            }
            return true;
        }
        public static bool GuestValidator(int NationalId,int Password)
        {
            Guest guest = DatabaseServer.LoadGuestUsingId(NationalId);
            if (guest == null) return false;
            if (guest.Password != Password) return false;
            return true;
        }
        public static void EnterGuestSystem()
        {
            systemState = SystemState.GUEST_MENU;
            LoadGuestServicesMenu();
            int guestChoice = Convert.ToInt32(Console.ReadLine());
            switch (guestChoice)
            {
                case (int)GuestServiceSelection.RESERVE_A_ROOM:
                    ReserveRoom();
                    break;
                case (int)GuestServiceSelection.REQUEST_A_SERVICE:
                    RequestService();
                    break;
                case (int)GuestServiceSelection.LOGOUT:
                    loggedInGuest = null;
                    Console.Write("Logging out");LineOfDots();
                    changeState(SystemState.GUEST_LOGIN);
                    break;
                case (int)GuestServiceSelection.CHECK_IN:
                    CheckIn();
                    break;
                case (int)GuestServiceSelection.CHECK_OUT:
                    //CheckOut();
                    break;
                case (int)GuestServiceSelection.PAY_FOR_A_RESERVATION:
                    //PayForReservation();
                    break;
                case (int)GuestServiceSelection.PAY_FOR_A_SERVICE:
                    //PayForServices();
                    break;

            }
        }
        public static void LoadGuestServicesMenu()
        {
            Console.WriteLine("-----------------------[ Guest Hotel Services ]-----------------------");
            //Console.WriteLine("Please select the service you want below: ");
            Console.WriteLine("1. Reserve a room");
            Console.WriteLine("2. Request a service");
            Console.WriteLine("3. Check in");
            Console.WriteLine("4. Check out");
            Console.WriteLine("5. Pay for a reservation");
            Console.WriteLine("6. Pay for a service");
            Console.WriteLine("7. To Logout");
            Console.WriteLine("-----------------------------------------------------------------------");
        }
        //Guest functions///////////////////////////////////////////////////////
        public static void ReserveRoom()
        {
            int roomNumber;
            string checkInDate;
            string checkOutDate;
            string meal = "";
            int mealSelection;
            Console.Clear();
            Console.WriteLine("---------------------------[ Room Reservation ]---------------------------");
            if (ShowAvailableRooms()==false){ 
                Console.WriteLine("We are sorry, there is no available rooms at the moment!!");
                Thread.Sleep(2000);
                changeState(SystemState.GUEST_MENU);
                return;
            }
            Console.WriteLine("Please fill the information below to confirm your reservation.");
            Console.Write("Room Number : ");
            roomNumber = Convert.ToInt32(Console.ReadLine());
            //get room info from database
            Room chosenRoom = DatabaseServer.GetRoom(roomNumber);
            if (chosenRoom == null)
            {
                Console.WriteLine("Wrong room number, please try again");
                Thread.Sleep(2000);
                ReserveRoom();
                return;
            }
            //change room availability to false
            Console.WriteLine("Check In Date and Check Out date in form of (DD/MM/YYYY) : ");
            //remember to handle the case where check-out is before check-in 
            Console.Write("Check In Date : ");
            checkInDate = Console.ReadLine();
            Console.Write("Check Out Date : ");
            checkOutDate = Console.ReadLine();
            //remember to send a payment record of the reservation
            Console.WriteLine("Choose one of the meals type below : ");
            Console.WriteLine("1. Breakfast.");
            Console.WriteLine("2. Breakfast and Lunch.");
            Console.WriteLine("3. Full board.");
            mealSelection = Convert.ToInt32(Console.ReadLine());
            if (mealSelection == 1) meal = "Breakfast";
            if (mealSelection == 2) meal = "Breakfast and Lunch";
            if (mealSelection == 3) meal = "Full Board";
            //calculate the total amount of reservation (total room reservation + the price of the meal)
            Reservation reservation = new Reservation(GenerateId("Reservation"),roomNumber, loggedInGuest.NationalID, checkInDate, checkOutDate, meal);
            Payment resPaymentRecord = new Payment(GenerateId("Payment"), loggedInGuest.NationalID, "Reservation",calculateReservation(reservation,chosenRoom.PricePerDay,meal),"Unpaid");
            Console.WriteLine("Your receipt : ");
            PrintHeaderTable();
            resPaymentRecord.DisplayAllInfo();
            Console.WriteLine("---------------------------------------------------------------------------------");
            DatabaseServer.SaveData("Reservation.txt",reservation);
            DatabaseServer.SaveData("Payment.txt", resPaymentRecord);
            ServiceConfirmedMessage();
        }
        public static void RequestService()
        {
            Payment servPaymentRecord=null;
            Service newServiceRecord = null;
            Console.Clear();
            Console.WriteLine("---------------------------[ Request a service ]---------------------------");
            Console.WriteLine("Please choose the service you want below : ");
            Console.WriteLine("1. Car rental.");
            Console.WriteLine("2. Kids zone.");
            Console.WriteLine("Press (0) to get back.");
            Console.WriteLine("---------------------------------------------------------------------------");
            string guestSelection = Console.ReadLine();
            if (guestSelection == "1")
            {
                Console.Write("Please enter the number of rental days : ");
                int numberOfRentDays= Convert.ToInt32(Console.ReadLine());
                newServiceRecord = new Service(GenerateId("Service"), loggedInGuest.NationalID, "Car rental",CalculateService("Car rental",numberOfRentDays));
                servPaymentRecord = new Payment(GenerateId("Payment"), loggedInGuest.NationalID, "Car rental", CalculateService("Car rental", numberOfRentDays), "Unpaid");
            }
            if (guestSelection == "2")
            {
                Console.Write("Please enter the number of children : ");
                int numberOfChildren = Convert.ToInt32(Console.ReadLine());
                newServiceRecord = new Service(GenerateId("Service"), loggedInGuest.NationalID, "Kids zone", CalculateService("Kids zone", numberOfChildren));
                servPaymentRecord = new Payment(GenerateId("Payment"), loggedInGuest.NationalID, "Kids zone", CalculateService("Kids zone", numberOfChildren), "Unpaid");
            }
            if (guestSelection =="0")
            {
                Console.Clear();
                changeState(SystemState.GUEST_MENU);
                return;
            }
            Console.WriteLine("Your receipt : ");
            PrintHeaderTable();
            servPaymentRecord.DisplayAllInfo();
            Console.WriteLine("---------------------------------------------------------------------------------");
            DatabaseServer.SaveData("Payment.txt", servPaymentRecord);
            DatabaseServer.SaveData("Service.txt", newServiceRecord);
            ServiceConfirmedMessage();
            

        }
        public static void CheckIn()
        {
            Console.Clear();
            Console.WriteLine("--------------------------------------------------[ Check In ]--------------------------------------------------");
            List<Reservation> reservations = DatabaseServer.GetReservations();
            Console.WriteLine("All reservations in your name: ");
           Reservation.PrintHeaderTable();
           for(int i = 0; i < reservations.Count; i++)
           {
                if (reservations[i].NationalId == loggedInGuest.NationalID && reservations[i].ReservationStatus == "Confirmed")
                { 
                    reservations[i].DisplayAllInfo(); 
                }
           }
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Please enter the reservation Id to check in: ");
            int resId = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < reservations.Count; i++)
            {
                if (reservations[i].ID == resId)
                {
                    reservations[i].ReservationStatus = "Checked In";
;               }
            }
            DatabaseServer.SaveUpdatedReservations(reservations);
            ServiceConfirmedMessage();
        }
        public static void PrintHeaderTable()
        {
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.Write($"|  Bill number  ");
            Console.Write($"|  National ID  ");
            Console.Write($"|     Source    ");
            Console.Write($"|     Amount    ");
            Console.WriteLine($"|     Status    |");
        }
        public static bool ShowAvailableRooms()
        {
            List<Room> availableRooms = DatabaseServer.LoadAvailableRooms();
            if (availableRooms.Count == 0) {return false; }
            Console.WriteLine("Currently available rooms : ");
            Room.PrintHeaderTable();
            for (int i = 0; i < availableRooms.Count; i++)
            {
                availableRooms[i].DisplayAllInfo();
            }
            Console.WriteLine("-----------------------------------------------------------------");
            return true;
        }
        public static void changeState(SystemState to)
        {
            Console.Clear();
            if (to == SystemState.GUEST_LOGIN) loggedInGuest = null;
            systemState = to;
        }
        public static double CalculateService(string service,int number) 
        {
            if(service=="Car rental")
            {
                return number * 10;
            }
            return number * 5;
        }
        public static double calculateReservation(Reservation reservation,double roomPrice,string meal)
        {
            var dateFormat = new CultureInfo("en-JO");
            DateTime checkInDate = DateTime.ParseExact(reservation.CheckInDate, "dd/MM/yyyy", dateFormat);
            DateTime checkOutDate = DateTime.ParseExact(reservation.CheckOutDate, "dd/MM/yyyy", dateFormat);
            TimeSpan totalResidenceDays = checkOutDate - checkInDate;
            Console.WriteLine(totalResidenceDays.Days);
            double amount=0;
            if (meal == "Breakfast")
            {
                amount= totalResidenceDays.Days * roomPrice;
            }
            if(meal=="Breakfast and Lunch")
            {
                amount= 1.2 *totalResidenceDays.Days * roomPrice;
            }
            if (meal == "Full Board")
            {
                amount = 1.4*totalResidenceDays.Days * roomPrice;
            }
            return amount;
        }
        //to make sure that all IDs attributes of our classes unique (Reservation ID, Service ID, Payment bill number).
        public static int GenerateId(string objectType)
        {
            return DatabaseServer.LoadLastIdOfObject(objectType)+1;            
        }
        public static void ServiceConfirmedMessage()
        {
            Console.WriteLine("Thank you for using our hotel!!");
            Console.WriteLine("Press (1) to get back to menu, (0) To logout.");
            string userChoice=Console.ReadLine();
            if (userChoice == "1") changeState(SystemState.GUEST_MENU); 
            else { 
                changeState(SystemState.GUEST_LOGIN);
                Console.Write("Logging out"); LineOfDots();
            }
        }
        public static void LineOfDots()
        {
            for (int i = 0; i < 35; i++)
            {
                Console.Write('.');
                Thread.Sleep(12);
            }
            Console.WriteLine();
        }
    }
}
