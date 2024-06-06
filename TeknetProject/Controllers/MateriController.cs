using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TeknetProject.Models;
using System.Threading.Tasks;

namespace TeknetProject.Controllers
{
    public class MateriController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7068/api/Materi");
        private readonly HttpClient _client;

        public MateriController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Materi> materiList = new List<Materi>();
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                materiList = JsonConvert.DeserializeObject<List<Materi>>(data);
            }

            return View(materiList);
        }


        public IActionResult Create()
        {
            return View();
        }

        // POST: Materi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Materi materi)
        {
            try
            {
                string data = JsonConvert.SerializeObject(materi);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response =  _client.PostAsync(_client.BaseAddress, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Materi Created.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int id) 
        {
            try
            {
                Materi materi = new Materi();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    materi = JsonConvert.DeserializeObject<Materi>(data);
                }
                return View(materi);
            }
            catch (Exception ex)
            {
                TempData["error Message"] = ex.Message;
            return View();
            }

        
        }
        [HttpPost]
        public IActionResult Edit(Materi materi)
        {
            try
            {
                string data = JsonConvert.SerializeObject(materi);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Materi Detail Updated";
                    return RedirectToAction("Index");
                }
              
            }
            catch (Exception ex)
            {

                TempData["error Message"] = ex.Message;
                return View();  
            }
            return View();
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                Materi materi = new Materi();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    materi = JsonConvert.DeserializeObject<Materi>(data);
                }
                return View(materi);
            }
            catch (Exception ex)
            {
                TempData["error Message"] = ex.Message;
                return View();
            }


        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Materi Detail Deleted";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {

                TempData["error Message"] = ex.Message;
                return View();
            }
            return View();
        }

    }
}

    