using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Newtonsoft.Json;
using System.IO;
// using System;

namespace Backend.Controllers
{
    
    

    [ApiController]
    [Route("api/[controller]")]
    public class CovidController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IUtilService _util;

        private readonly ILogger<CovidController> _logger;

        public CovidController(ILogger<CovidController> logger, IHttpClientFactory clientFactory, IUtilService util)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _util = util;
        }

        [HttpGet("getcovid")]
        public IActionResult GetCovid() {
            HttpClient client = _clientFactory.CreateClient();
            HttpResponseMessage res = client.GetAsync("https://covid19.ddc.moph.go.th/api/Cases/today-cases-by-provinces").Result;
                                                       
            
            if(res.IsSuccessStatusCode) {
                bool dupFlag = false;

                HttpContent content = res.Content;
                string result = content.ReadAsStringAsync().Result;
                List<Provinces> dataProvince = JsonConvert.DeserializeObject<List<Provinces>>(result);
                if (dataProvince.Count == 0) {
                    dupFlag = true;
                    return Ok("No data");
                }
                List<Test> listTest = new List<Test>();
                string conStr = "server=localhost;port=3306;user=root;password=dome;database=database1" ;
                using (var connection = new MySqlConnection(conStr)) {
                    connection.Open();

                    MySqlCommand checkCommand = new MySqlCommand("select * from database1.province order by date desc limit 1", connection);
                    using (MySqlDataReader reader = checkCommand.ExecuteReader()){
                        if (reader.Read()){
                            var checkDate = reader["date"];
                            if (dataProvince[0].date.Equals(checkDate))
                                dupFlag = true;
                        }
                        
                    }
                    if (!dupFlag) {
                        for(int i = 0; i<77 ; i++){
                            Provinces province = dataProvince[i];

                            string textCmd = string.Format("INSERT INTO province VALUES('{0}', '{1}', {2}, {3})", province.date, province.province, province.newCase, province.totalCase ) ;
                            MySqlCommand command = new MySqlCommand(textCmd, connection); 
                            
                            command.ExecuteNonQuery();
                        }
                    }
                }
            } 
            return Ok("Yeah");
        }

        [HttpGet("province")]
        public IActionResult Province(){
            string conStr = "server=localhost;port=3306;user=root;password=dome;database=database1" ;
            List<NewCaseProvince> newProvinceList = new List<NewCaseProvince>() ;
            using (var connection = new MySqlConnection(conStr)) {
                    connection.Open();

                    DateTime date = DateTime.Now ;
                    int day = date.Day ;
                    int month = date.Month ;
                    int year = date.Year ;
                    string dateStr = year.ToString() + "-" + _util.ConvertNumToString(month) + "-" + _util.ConvertNumToString(day) ;
                    string cmd = string.Format("SELECT DATE,NAME, NEWCASE FROM province where DATE = '{0}'", dateStr) ;
                   
                    MySqlCommand checkCommand = new MySqlCommand(cmd, connection);
                    using (MySqlDataReader reader = checkCommand.ExecuteReader()){
                        while(reader.Read()){
                            NewCaseProvince newProvince = new NewCaseProvince() {
                                date = reader["DATE"].ToString(), 
                                province = reader["NAME"].ToString(),
                                newCase = reader["NEWCASE"].ToString()
                            };
                            newProvinceList.Add(newProvince) ;
                        
                        }
                
                    }
                   
                }
            return Ok(newProvinceList);
        }

        // ----------------------------------------------------------------------------------------------------------------------
        [HttpGet("totalCovid")]
        public IActionResult TotalCovid(){
            string conStr = "server=localhost;port=3306;user=root;password=dome;database=database1" ;
            List<NewCaseCovid> newCovidList = new List<NewCaseCovid>() ;
            using (var connection = new MySqlConnection(conStr)) {
                    connection.Open();

                    DateTime date = DateTime.Today ;
                    int day = date.Day ;
                    int month = date.Month ;
                    int year = date.Year ;
                    string dateStr = year.ToString() + "-" + _util.ConvertNumToString(month) + "-" +  _util.ConvertNumToString(day) ;
                    string cmd = string.Format("SELECT * FROM casecovid where DATE = '{0}'", dateStr) ;
                   
                    MySqlCommand checkCommand = new MySqlCommand(cmd, connection);
                    using (MySqlDataReader reader = checkCommand.ExecuteReader()){
                        while(reader.Read()){
                            NewCaseCovid newTotalCovid = new NewCaseCovid() {
                                date = _util.DateTimeToString(reader["DATE"].ToString()), 
                                newCase = reader["NEW"].ToString(),
                                healed = reader["HEALED"].ToString(),
                                totalnew = reader["totalNEW"].ToString(),
                                dead = reader["DEAD"].ToString(),
                                totalheal = reader["totalHEAL"].ToString(),
                                healing = reader["HEALING"].ToString(),
                            };
                            newCovidList.Add(newTotalCovid) ;
                        
                        }
                
                    }
                   
                }
            return Ok(newCovidList);
        }

        // ----------------------------------------------------------------------------------------------------------------------
        [HttpGet("location")]
        public IActionResult Location(){
            string conStr = "server=localhost;port=3306;user=root;password=dome;database=database1" ;
            List<NewLocation> newTotalList = new List<NewLocation>() ; 
            using (var connection = new MySqlConnection(conStr)) {
                    connection.Open();

                    string cmd = string.Format("SELECT * FROM location order by NAME") ;
                   
                    MySqlCommand checkCommand = new MySqlCommand(cmd, connection);
                    using (MySqlDataReader reader = checkCommand.ExecuteReader()){
                        while(reader.Read()){
                            NewLocation newTotalLocation = new NewLocation() {
            
                                name = reader["NAME"].ToString(),
                                latitude = reader["LATITUDE"].ToString(),
                                longitude = reader["LONGITUDE"].ToString(),
                            };
                            newTotalList.Add(newTotalLocation) ;
                        
                        }
                
                    }
                   
                }
            return Ok(newTotalList);
        }

        [HttpGet("totalMonth")]
        public IActionResult WeekCovid(){
            string conStr = "server=localhost;port=3306;user=root;password=dome;database=database1" ;
            List<NewCaseCovid> weekCovidList = new List<NewCaseCovid>() ;
            using (var connection = new MySqlConnection(conStr)) {
                    connection.Open();
                    
                    for(int i = -29 ; i <= 0 ; i++){
                        DateTime date = DateTime.Today.AddDays(i) ;
                        int day = date.Day ;
                        int month = date.Month ;
                        int year = date.Year ;
                        string dateStr = year.ToString() + "-" + _util.ConvertNumToString(month) + "-" +  _util.ConvertNumToString(day) ;

                        string cmd = string.Format("SELECT * FROM casecovid where DATE = '{0}'", dateStr) ;
                    
                        MySqlCommand checkCommand = new MySqlCommand(cmd, connection);
                        using (MySqlDataReader reader = checkCommand.ExecuteReader()){
                            while(reader.Read()){
                                NewCaseCovid newTotalCovid = new NewCaseCovid() {
                                    date = _util.DateTimeToString(reader["DATE"].ToString()), 
                                    newCase = reader["NEW"].ToString(),
                                    healed = reader["HEALED"].ToString(),
                                    totalnew = reader["totalNEW"].ToString(),
                                    dead = reader["DEAD"].ToString(),
                                    totalheal = reader["totalHEAL"].ToString(),
                                    healing = reader["HEALING"].ToString(),
                                };
                                weekCovidList.Add(newTotalCovid) ;
                            
                            }
                    
                        }

                    }
                   
                }
            return Ok(weekCovidList);
        }            

        // ----------------------------------------------------------------------------------------------------------------------
        
    }
}