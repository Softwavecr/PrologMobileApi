using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using PrologMobileApi.Models;

namespace PrologMobileApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrganizationSummaryController : ControllerBase
    {
        private int callCounter=0;
        private async Task<List<T>> GetData<T>(string uri)
        {
            Console.WriteLine(uri+" . Call# "+ callCounter.ToString() );
            var dataSet = new List<T>();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(uri))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        dataSet = JsonSerializer.Deserialize<List<T>>(apiResponse);              
                    }
                }
                return dataSet;
        }

        [HttpGet]        
        public async Task<IActionResult> Get()
        {                                                
            callCounter=0;
            try
            {
                var organizations = await GetData<Organization>("https://5f0ddbee704cdf0016eaea16.mockapi.io/organizations");

                var response = new List<OrganizationSummary>();
                var responseUsers = new List<UserPhoneGroup>();
                int blacklistTotal=0,totalCount=0;
    
                foreach(var o in organizations)
                {
                    var users = await GetData<User>("https://5f0ddbee704cdf0016eaea16.mockapi.io/organizations/"+o.id+"/users");
                    callCounter++;

                    var organizationUsers = users.Where(x => x.organizationId == o.id);

                    foreach(var ou in organizationUsers){
                    if(callCounter>11)//Work around, issue with AspNetCoreRateLimit 
                    {
                        System.Threading.Thread.Sleep(20000); callCounter=0;
                    }                
                        var phones = await GetData<Phone>("https://5f0ddbee704cdf0016eaea16.mockapi.io/organizations/"+o.id+"/users/"+ou.id+"/phones");
                        callCounter++;               

                        var currentUserPhoneList = phones.Where(x => x.userId == ou.id);
                        var currentUserPhonesBlacklisted = currentUserPhoneList.Where(x => x.Blacklist == true);
                        var currentUserPhone = currentUserPhoneList.FirstOrDefault();

                        if(currentUserPhone != null){
                            responseUsers.Add(new UserPhoneGroup{id=currentUserPhone.userId, email=ou.email, phoneCount=currentUserPhoneList.Count()});
                            totalCount += currentUserPhoneList.Count();
                            blacklistTotal += currentUserPhonesBlacklisted.Count();                        
                        }
                    }                
                    response.Add(new OrganizationSummary{id=o.id, name=o.name, blacklistTotal=blacklistTotal, totalCount=totalCount, users=responseUsers});

                    responseUsers = new List<UserPhoneGroup>();
                    blacklistTotal = totalCount = 0;//System.Threading.Thread.Sleep(2000);
                }
                return Ok(response);
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
                return Conflict();
            }
        }        
        #region Data Generation for offline Testing
        /*
        public IEnumerable<Organization> GetOrganizationsOriginal()
        {
            var organizations = new List<Organization>();
            organizations.Add(new Organization{id="1",createdAt="2020-07-22T05:04:43.836Z",name="King, Ferry and Harvey"});
            organizations.Add(new Organization{id="2",createdAt="2020-07-22T04:41:06.264Z",name="O'Conner - Upton"});
            organizations.Add(new Organization{id="3",createdAt="2020-07-22T19:18:18.352Z",name="Goodwin and Sons"});            
            organizations.Add(new Organization{id="4",createdAt="2020-07-22T16:19:23.182Z",name="Luettgen LLC"});
            organizations.Add(new Organization{id="5",createdAt="2020-07-22T09:53:54.233Z",name="Hickle Group"});
            organizations.Add(new Organization{id="6",createdAt="2020-07-21T21:42:34.264Z",name="Schulist, Reinger and Larson"});
            return organizations;
        }
        public IEnumerable<User> GetUsers()
        {
            var users = new List<User>();
            users.Add(new User{id="1", organizationId="1",createdAt="2020-07-14T01:00:45.801Z",name="Dr. Peter Gleason",email="Crystel.Tillman65@hotmail.com"});
            users.Add(new User{id="7", organizationId="1",createdAt="2020-07-14T07:58:34.987Z",name="Mrs. Ryder Strosin",email="Trenton.Konopelski95@hotmail.com"});
            users.Add(new User{id="13",organizationId="1",createdAt="2020-07-14T02:52:48.990Z",name="Gilberto Schaefer MD",email="Carlo.Simonis@yahoo.com"});
            users.Add(new User{id="19",organizationId="1",createdAt="2020-07-13T20:03:51.431Z",name="Freda Walter",email="Mitchel.Greenholt74@hotmail.com"});            
            users.Add(new User{id="25",organizationId="1",createdAt="2020-07-14T14:36:04.095Z",name="Antoinette Doyle",email="Reanna78@hotmail.com"});
            users.Add(new User{id="31",organizationId="1",createdAt="2020-07-14T11:40:25.023Z",name="Ezequiel Boehm",email="Benton23@gmail.com"});
            users.Add(new User{id="37",organizationId="1",createdAt="2020-07-14T08:17:29.971Z",name="Bartholome Ullrich",email="Ansley_Crist@yahoo.com"});
    
            users.Add(new User{id="2", organizationId="2",createdAt="2020-07-14T01:00:45.801Z",name="Jocelyn Adams IV",email="Una81@yahoo.com"});
            users.Add(new User{id="8", organizationId="2",createdAt="2020-07-14T07:58:34.987Z",name="Krystel Gislason",email="Alexander_Welch31@yahoo.com"});
            users.Add(new User{id="14",organizationId="2",createdAt="2020-07-14T02:52:48.990Z",name="Sabrina Mertz II",email="Vivian_Heidenreich@gmail.com"});
            users.Add(new User{id="20",organizationId="2",createdAt="2020-07-13T20:03:51.431Z",name="Johnny Quigley",email="Baylee_Cormier@hotmail.com"});
            users.Add(new User{id="26",organizationId="2",createdAt="2020-07-14T14:36:04.095Z",name="Karina Aufderhar",email="Demarco25@gmail.com"});
            users.Add(new User{id="32",organizationId="2",createdAt="2020-07-14T11:40:25.023Z",name="Ambrose Langosh",email="Sigmund_Parker65@yahoo.com"});
            users.Add(new User{id="38",organizationId="2",createdAt="2020-07-14T08:17:29.971Z",name="Jadyn Bosco",email="Dereck60@gmail.com"});                        

            users.Add(new User{id="3", organizationId="3",createdAt="2020-07-14T07:58:34.987Z",name="Carli Anderson",email="Reece.Bogan@yahoo.com"});
            users.Add(new User{id="9", organizationId="3",createdAt="2020-07-14T07:58:34.987Z",name="Renee Beahan",email="Loren89@yahoo.com"});
            users.Add(new User{id="15", organizationId="3",createdAt="2020-07-14T07:58:34.987Z",name="Tamara McLaughlin",email="Muriel57@gmail.com"});
            users.Add(new User{id="21", organizationId="3",createdAt="2020-07-14T07:58:34.987Z",name="Dorcas McLaughlin",email="Kenton86@hotmail.com"});
            users.Add(new User{id="27", organizationId="3",createdAt="2020-07-14T07:58:34.987Z",name="Rosella Grimes",email="Estel82@yahoo.com"});
            users.Add(new User{id="33", organizationId="3",createdAt="2020-07-14T07:58:34.987Z",name="Vern Bauch I",email="Alejandra61@hotmail.com"});

            users.Add(new User{id="4", organizationId="4",createdAt="2020-07-14T07:58:34.987Z",name="Angelita Champlin",email="Torrance17@hotmail.com"});
            users.Add(new User{id="10", organizationId="4",createdAt="2020-07-14T07:58:34.987Z",name="Fernando Abbott",email="Armani_Streich@gmail.com"});
            users.Add(new User{id="16", organizationId="4",createdAt="2020-07-14T07:58:34.987Z",name="Keanu Haley",email="Arlo99@yahoo.com"});
            users.Add(new User{id="22", organizationId="4",createdAt="2020-07-14T07:58:34.987Z",name="Kamren Doyle",email="Rodrick55@hotmail.com"});
            users.Add(new User{id="28", organizationId="4",createdAt="2020-07-14T07:58:34.987Z",name="Adriel Labadie",email="Jennyfer_Kovacek@hotmail.com"});
            users.Add(new User{id="34", organizationId="4",createdAt="2020-07-14T07:58:34.987Z",name="Elwyn Wisoky PhD",email="Brenna_Gaylord17@yahoo.com"});
            return users;
        } 
        public IEnumerable<Phone> GetPhones()
        {
            List<Phone> phones = new List<Phone>();
            phones.Add(new Phone{id="1",userId="1",createdAt="2020-07-14T01:00:45.801Z",IMEI=32117,Blacklist=false});
            phones.Add(new Phone{id="39",userId="1",createdAt="2020-07-13T17:53:47.093Z",IMEI=87092,Blacklist=true});
            phones.Add(new Phone{id="77",userId="1",createdAt="2020-07-14T11:31:48.571Z",IMEI=72587,Blacklist=true});

            phones.Add(new Phone{id="2",userId="2",createdAt="2020-07-14T03:19:35.592Z",IMEI=11945,Blacklist=false});
            phones.Add(new Phone{id="40",userId="2",createdAt="2020-07-14T00:01:56.511Z",IMEI=46627,Blacklist=false});
            phones.Add(new Phone{id="78",userId="2",createdAt="2020-07-13T18:22:40.995Z",IMEI=10073,Blacklist=true});

            phones.Add(new Phone{id="3",userId="3",createdAt="2020-07-14T01:00:45.801Z",IMEI=13885,Blacklist=true});
            phones.Add(new Phone{id="41",userId="3",createdAt="2020-07-13T17:53:47.093Z",IMEI=35057,Blacklist=true});
            phones.Add(new Phone{id="79",userId="3",createdAt="2020-07-14T11:31:48.571Z",IMEI=62801,Blacklist=false});

            phones.Add(new Phone{id="4",userId="4",createdAt="2020-07-14T03:19:35.592Z",IMEI=67988,Blacklist=false});
            phones.Add(new Phone{id="42",userId="4",createdAt="2020-07-14T00:01:56.511Z",IMEI=50796,Blacklist=false});
            phones.Add(new Phone{id="80",userId="4",createdAt="2020-07-13T18:22:40.995Z",IMEI=92094,Blacklist=false});
            return phones;
        }*/
        #endregion
    }
}

