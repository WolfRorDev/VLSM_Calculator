using System;
using System.Collections.Generic;

namespace VLSM_Calculator
{
    /* Author: Domninik Krzywański
     * Website: wolfror.iwhy.me
     * -------------------------
     * Autor: Dominik Krzywański
     * Strona internetowa: wolfror.iwhy.me
     */
    class VLSM_Calc
    {
        static readonly string[] Masks = new string[] {"0.0.0.0", "128.0.0.0", "192.0.0.0", "224.0.0.0", "240.0.0.0", "248.0.0.0", "252.0.0.0", "254.0.0.0",
            "255.0.0.0", "255.128.0.0", "255.192.0.0", "255.224.0.0", "255.240.0.0", "255.248.0.0", "255.252.0.0", "255.254.0.0",
            "255.255.0.0","255.255.128.0", "255.255.192.0", "255.255.224.0", "255.255.240.0", "255.255.248.0", "255.255.252.0",
            "255.255.254.0", "255.255.255.0", "255.255.255.128", "255.255.255.192", "255.255.255.224", "255.255.255.240",
            "255.255.255.248", "255.255.255.252", "255.255.255.254", "255.255.255.255" }; //List of masks

        static bool FirstVLSM;

        static List<VlsmObject> ListOfNetworks = new List<VlsmObject>(); //Calculated VLSM


        static void Main()
        {
            Console.WriteLine("Welcome in \"VLSM Calculator\""); //Greeting user

            Console.WriteLine("Provide ip address: ");
            string IPLine = Console.ReadLine(); //Get ip
            Console.WriteLine("Provide mask: ");
            string MaskLine = Console.ReadLine(); //Get mask
            Console.WriteLine("Provide hosts: ");
            string HostLine = Console.ReadLine(); //Get hosts to calculate
            Console.WriteLine(); //Break line
            int[] HostSorted = HostsSorting(HostLine); //Sorting hosts into an array
            int Mask = MaskCheck(MaskLine); //Mask conversion
            int Overflow = OverflowCheck(HostSorted, Mask); //Checking how many addresses will be without
            string IP = IpCheck(IPLine); //Optional ip address repair
            FirstVLSM = true;

            for (int i = 0; i < Overflow; i++)
            {
                if (i == 0)
                {
                    ListOfNetworks.Add(Calculate_VLSM(IP, Mask, HostSorted[i])); //First network to calculate
                }
                else
                {
                    ListOfNetworks.Add(Calculate_VLSM(ListOfNetworks[i - 1].Broadcast, Mask, HostSorted[i])); //Next nets to calculate
                }
            }

            foreach (VlsmObject item in ListOfNetworks) //Showing calculated data
            {
                Console.WriteLine("-------------------");
                Console.WriteLine("Network: " + item.Network);
                Console.WriteLine("Mask: " + item.Mask);
                Console.WriteLine("First Host: " + item.FirstHost);
                Console.WriteLine("Last Host: " + item.LastHost);
                Console.WriteLine("Broadcast: " + item.Broadcast);
                Console.WriteLine("Wanted host: " + item.WantedHost);
            }
            Console.WriteLine("\nClick enter to close app");
            Console.ReadLine(); //Lock to prevent the application from closing without user interaction
        }

        static VlsmObject Calculate_VLSM(string IpAddress, int Mask, int Host)
        {
            string Network, FirstHost, LastHost, Broadcast; //Variables to return
            int Foct, Soct, Toct, Fouroct, rest, VlsmMask, OverFlowTest; //Foct stands for first octet
            string[] Testing = IpAddress.Split('.'); //Split ip address into octets

            #region Overflow calculation
            int tempMask = 0; //Sum of needed hosts
            int HostsInTheTempMask = 2; //The number of hosts on the network needed to accommodate the previously entered number of hosts
            while (tempMask < 31 - Mask)
            {
                HostsInTheTempMask *= 2;
                tempMask++;

            }
            #endregion

            #region Main calculation
            if (Mask >= 8 && Mask <= 30)
            {
                int ManyHosts = 2; //Number of wanted hosts in network
                int MaskAddFromHosts = 0; //Mask for choosen host

                while (ManyHosts - 2 < Host) //Calculating how many hosts is needed to fit the previously entered number of hosts
                {
                    ManyHosts *= 2;
                    MaskAddFromHosts++;

                }

                OverFlowTest = ManyHosts;

                if (31 - MaskAddFromHosts >= Mask && HostsInTheTempMask >= OverFlowTest && Host > 0)
                {
                    VlsmMask = (31 - MaskAddFromHosts);

                    ManyHosts -= 3;

                    Foct = int.Parse(Testing[0]);
                    Soct = int.Parse(Testing[1]);
                    Toct = int.Parse(Testing[2]);
                    Fouroct = int.Parse(Testing[3]);

                    if (Fouroct == 255) //First converting octets
                    {
                        Toct += 1;
                        Fouroct = 0;
                        if (Toct == 256)
                        {
                            Toct = 0;
                            Soct += 1;
                        }
                    }
                    else if (FirstVLSM == false)
                    {
                        Fouroct += 1; //Fourth octet increment for all subnets except the first
                    }

                    Network = (Foct.ToString() + "." + Soct.ToString() + "." + Toct.ToString() + "." + Fouroct.ToString()); //Creating Network variable from octets

                    if (Fouroct == 255) //Second converting octets
                    {
                        Toct += 1;
                        Fouroct = 0;
                        if (Toct == 256)
                        {
                            Toct = 0;
                            Soct += 1;
                        }
                    }
                    else
                    {
                        Fouroct += 1;
                    }


                    FirstHost = (Foct.ToString() + "." + Soct.ToString() + "." + Toct.ToString() + "." + Fouroct.ToString()); //Creating First Host variable from octets
                    ManyHosts += Fouroct;

                    if (ManyHosts >= 256) //Third converting octets
                    {
                        rest = ManyHosts / 256;
                        Toct += rest;
                        if (Toct >= 256)
                        {
                            Soct += Toct / 256;
                            Toct -= ((Toct / 256) * 256);
                        }
                        Fouroct = ManyHosts - (rest * 256);

                    }
                    else
                    {
                        Fouroct = ManyHosts;
                    }

                    LastHost = (Foct.ToString() + "." + Soct.ToString() + "." + Toct.ToString() + "." + Fouroct.ToString()); //Creating Lasy Host variable from octets

                    if (Fouroct == 255) //Fourth converting octets
                    {
                        Fouroct = 0;
                        Toct++;
                        if (Toct == 255)
                        {
                            Toct = 0;
                            Soct++;
                        }
                    }
                    else
                    {
                        Fouroct++;
                    }

                    Broadcast = (Foct.ToString() + "." + Soct.ToString() + "." + Toct.ToString() + "." + Fouroct.ToString()); //Creating Broadcast variable from octets
                }
                else //Returning overflow VlsmObject
                {
                    Network = ("Overflow");
                    VlsmMask = (0);
                    FirstHost = ("Overflow");
                    LastHost = ("Overflow");
                    Broadcast = ("Overflow");

                }


            }
            else //Returning error VlsmObject
            {
                Network = ("Error");
                VlsmMask = (0);
                FirstHost = ("Error");
                LastHost = ("Error");
                Broadcast = ("Error");

            }

            #endregion

            VlsmObject VO = new VlsmObject(Network, VlsmMask, FirstHost, LastHost, Broadcast, Host);

            FirstVLSM = false;

            return VO;

        }

        static int[] HostsSorting(string HostsToSort)
        {
            string[] OriginalHostArray = HostsToSort.Split(',');
            int[] SortedHostArray = new int[OriginalHostArray.Length]; //Creating array to return

            for (int a = 0; a < OriginalHostArray.Length; a++)
            {
                SortedHostArray[a] = int.Parse(OriginalHostArray[a]); //Converting hosts from string to int
            }

            Array.Sort(SortedHostArray); //Sorting array - from the smallest host to the largest
            Array.Reverse(SortedHostArray); //Reversing array - from the largest host to the smallest

            return SortedHostArray;

        }

        static int MaskCheck(string MaskLine)
        {
            int IpMask; //Int to return
            if (MaskLine.Contains(".")) //Checking if the mask looks like this: 255.255.255.0 or 11111111.11111111.11111111.00000000
            {
                string[] Testing = MaskLine.Split('.');
                if (Testing[0].Length == 8)//Checking if the mask looks like this: 11111111.11111111.11111111.00000000
                {
                    int Foct, Soct, Toct, Fouroct;

                    Foct = Convert.ToInt32(Testing[0], 2);
                    Soct = Convert.ToInt32(Testing[1], 2);
                    Toct = Convert.ToInt32(Testing[2], 2);
                    Fouroct = Convert.ToInt32(Testing[3], 2);
                    IpMask = Foct + Soct + Toct + Fouroct;
                }
                else
                {
                    IpMask = Array.IndexOf(Masks, MaskLine); //Searching mask in masks array
                }
            }
            else if (MaskLine.Contains("/")) //Checking if the mask looks like this: /24
            {
                IpMask = int.Parse(MaskLine.Replace("/", string.Empty)); //Deleting "/" from mask
            }
            else //Mask propably looks like this: 24
            {
                IpMask = int.Parse(MaskLine); //Converting from string to int
            }

            return IpMask;
        }

        static string IpCheck(string IpAddress)
        {
            string[] Testing = IpAddress.Split('.'); //Spliting IP address
            string IpToReturn = IpAddress;

            bool UserError = false;

            for (int i = 0; i < Testing.Length; i++) //The loop fixes the ip address from errors such as too many octets or letters instead of numbers
            {
                try
                {
                    if (!string.IsNullOrEmpty(Testing[i]))
                    {
                        int x = int.Parse(Testing[i]);
                    }
                }
                catch
                {
                    Testing[i] = "0";
                    UserError = true;
                }
            }

            if (UserError == true)
            {
                string[] ErrorArray = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    if (i < Testing.Length)
                    {
                        ErrorArray[i] = Testing[i];
                    }
                    else
                    {
                        ErrorArray[i] = "0";
                    }
                }
                IpToReturn = ErrorArray[0] + "." + ErrorArray[1] + "." + ErrorArray[2] + "." + ErrorArray[3]; //Creating ip address from octets
                Testing = IpToReturn.Split('.');
            }


            if (Testing[0].Length == 8) //Checking if the ip address looks like this: 11000000.00000011.10010101.00000000
            {
                int Foct = Convert.ToInt32(Testing[0], 2); //Convert first octet from binary to decimal
                int Soct = Convert.ToInt32(Testing[1], 2); //Convert second octet from binary to decimal
                int Toct = Convert.ToInt32(Testing[2], 2); //Convert third octet from binary to decimal
                int Fouroct = Convert.ToInt32(Testing[3], 2); //Convert fourth octet from binary to decimal
                IpToReturn = Foct + "." + Soct + "." + Toct + "." + Fouroct; //Creating ip address from octets
            }
            else if (IpAddress.Contains(".")) //Checking if the ip address is full. If the address is incomplete, it will be supplemented.
            {
                if (Testing.Length == 2)
                {
                    if (string.IsNullOrEmpty(Testing[1]))
                    {
                        IpToReturn += "0";
                    }
                    IpToReturn += ".0.0";
                }
                else if (Testing.Length == 3)
                {
                    if (string.IsNullOrEmpty(Testing[2]))
                    {
                        IpToReturn += "0";
                    }
                    IpToReturn += ".0";
                }
                else if (Testing.Length == 4)
                {
                    if (string.IsNullOrEmpty(Testing[3]))
                    {
                        IpToReturn += "0";
                    }
                }
            }
            else
            {
                IpToReturn += ".0.0.0";
            }

            return IpToReturn;
        }


        static int OverflowCheck(int[] ArrayHost, int Mask) //Calculating the number of hosts without overflowing
        {
            int HostsWithoutOverFlow = 0; //Number of hosts without overflowing
            int MainMask = 2; //Number of free hosts in mask
            int HostOver = 0; //Sum of busy network hosts

            for (int i = 0; i <= 30 - Mask; i++)
            {
                MainMask *= 2;
            } //Counts how many hosts are in the mask, including network and broadcast


            foreach (int item in ArrayHost)
            {
                if (MainMask <= HostOver) //Breaking the loop when overflowing occurs
                {
                    break;
                }

                int ManyHosts = 2; //The number of hosts on the network needed to accommodate the previously entered number of hosts
                int MaskAddFromHosts = 0; //Sum of needed hosts

                while (ManyHosts - 2 < item)
                {
                    ManyHosts *= 2;
                    MaskAddFromHosts++;
                }

                HostOver += ManyHosts;
                HostsWithoutOverFlow++;
            }
            return HostsWithoutOverFlow;
        }
    }

    public class VlsmObject //An object containing all the necessary data
    {
        public string Network { get; set; }
        public int Mask { get; set; }
        public string FirstHost { get; set; }
        public string LastHost { get; set; }
        public string Broadcast { get; set; }
        public int WantedHost { get; set; }

        public VlsmObject(string network, int mask, string first_host, string last_host, string broadcast, int wanted_host)
        {
            Network = network;
            Mask = mask;
            FirstHost = first_host;
            LastHost = last_host;
            Broadcast = broadcast;
            WantedHost = wanted_host;
        }
    }
}
