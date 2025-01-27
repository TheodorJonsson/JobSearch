using Humanizer.DateTimeHumanizeStrategy;
using JobSearch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace JobSearch.Controllers
{
    public class JobController : Controller
    {
        static IList<JobModel> jobList = new List<JobModel>();
       // {
        //    new JobModel { Id = 1, Company = "Umeå Universitet", Position = "Systemutvecklare", Location = "Umeå", Date = DateOnly.FromDateTime(DateTime.Now), Ongoing = true}
        //};
        public static IList<JobModel> deniedList = new List<JobModel>();
        private const string JobListSessionKey = "jobList";
        private const string DeniedListSessionKey = "deniedList";
        private readonly IHttpContextAccessor _contx;

        public JobController(IHttpContextAccessor contx)
        {
            _contx = contx;
            // Retrieve lists from session in the constructor
            string jobListString = _contx.HttpContext.Session.GetString(JobListSessionKey);
            string deniedListString = _contx.HttpContext.Session.GetString(DeniedListSessionKey);

            // Checks if the session string is empty or not,
            // if it is it will create a new list
            // if the session string isnt empty it will create the list using that
            if (string.IsNullOrEmpty(jobListString))
            {
                jobList = new List<JobModel>();
            }
            else
            {
                jobList = JsonConvert.DeserializeObject<List<JobModel>>(jobListString);
            }
            if (string.IsNullOrEmpty(deniedListString))
            {
                deniedList = new List<JobModel>();
            }
            else
            {
                deniedList = JsonConvert.DeserializeObject<List<JobModel>>(deniedListString);
            }


        }
        
        [Route("/Job")]
        public IActionResult Index()
        {
            int nrOfJobs = jobList.Count();
            int nrOfDenials = deniedList.Count();

            ViewBag.nrOfJobs = nrOfJobs;
            ViewBag.nrOfDenials = nrOfDenials;
            ViewBag.jobList = jobList;
            ViewBag.deniedList = deniedList;

            return View();
        }


        // Goes into the addjob page and populates the form  with a default job
        // Sends this job then to the next addjob function
        [Route("/Job/AddJob")]
        [HttpGet]
        public IActionResult AddJob()
        {
            JobModel job = new JobModel() { Id = 2, Company = "Luleå Tekniska Universitet", Position = "Databastekniker",Location = "Luleå", Date = DateOnly.FromDateTime(DateTime.Now)};

            return View(job);
        }


        // Posts the information from the user and creates a jobmodel
        // checks if the job is valid and able to be added to the list
        [Route("/Job/AddJob")]
        [HttpPost]
        public IActionResult AddJob(JobModel job)
        {
            // Checks if the list already contains the job
            if (jobList.Contains(job) || deniedList.Contains(job))
            {
                TempData["idError"] = "ID is already in use";
                return Redirect("/Job/AddJob");
            }
            if (job.Date > DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["dateError"] = "Invalid date";
                return Redirect("/Job/AddJob");
            }
            if (job.Ongoing == true)
            {
                jobList.Add(job);
            }
            else
            {
                deniedList.Add(job);
            }
            UpdateSession();
            return Redirect("/Job");
        }


        // Goes to the specific job and its id to let the user
        // edit the specifics of the job application
        [Route("/Job/EditJob")]
        [Route("/Job/EditJob/{id}")]
        [HttpGet]
        public IActionResult EditJob(int id)
        {
            var job = jobList.Where(j => j.Id == id).FirstOrDefault();
            if (job == null)
            {
                var djob = deniedList.Where(j => j.Id == id).FirstOrDefault();
                if(djob == null)
                {
                    return NotFound();
                }
                return View(djob);
            }
            return View(job);
        }


        /**
         * Gets the job from the form from the first EditJob function
         * First it checks the date of the edited job,
         * if the date is invalid i.e in the future it will tell the user to enter a valid date
         * then it checks if the unedited job is null or not if it isnt 
         * it will remove the old job and add the new to the list.
         * 
         */
        [Route("/Job/EditJob")]
        [Route("/Job/EditJob/{id}")]
        [HttpPost]
        public IActionResult EditJob(JobModel job)
        {

            // Checks if the date is either today or in the past
            // cannot enter a date that is in the future
            if(job.Date > DateOnly.FromDateTime(DateTime.Now))
            {
                String url = "" + job.Id;
                TempData["dateError"] = "Invalid date";
                return Redirect(url);
            }
            // Checks with the job gotten from the ongoing list
            var updatedJob = jobList.Where(j => j.Id == job.Id).FirstOrDefault();
            if (updatedJob != null)
            {
                jobList.Remove(updatedJob);
                if (job.Ongoing == true)
                {
                    jobList.Add(job);
                }
                else
                {
                    deniedList.Add(job);
                }
            }

            // Check with the job item from the denied list
            var deniedUpdatedJob = deniedList.Where(j => j.Id == job.Id).FirstOrDefault();
            if(deniedUpdatedJob != null)
            {
                deniedList.Remove(deniedUpdatedJob);
                if (job.Ongoing == true)
                {
                    jobList.Add(job);
                }
                else
                {
                    deniedList.Add(job);
                }
            }
            UpdateSession();
            return Redirect("/Job");
        }



        // Deletes the selected job from the list
        [Route("/Job/DeleteJob/{id}")]
        public IActionResult DeleteJob(int id)
        {
            var updatedJob = jobList.Where(j => j.Id == id).FirstOrDefault();
            if (updatedJob != null)
            {
                jobList.Remove(updatedJob);
            }

            // Check with the job item from the denied list
            var deniedUpdatedJob = deniedList.Where(j => j.Id == id).FirstOrDefault();
            if (deniedUpdatedJob != null)
            {
                deniedList.Remove(deniedUpdatedJob);
            }
            UpdateSession(); 
            return Redirect("/Job");
        }
     

        // Helper method to update session variables
        private void UpdateSession()
        {
            string jobListString = JsonConvert.SerializeObject(jobList);
            string deniedListString = JsonConvert.SerializeObject(deniedList);

            _contx.HttpContext.Session.SetString(JobListSessionKey, jobListString);
            _contx.HttpContext.Session.SetString(DeniedListSessionKey, deniedListString);
        }
    }
}
