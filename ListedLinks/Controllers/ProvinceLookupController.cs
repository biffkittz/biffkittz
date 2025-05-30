using Microsoft.AspNetCore.Mvc;

namespace ListedLinks.Controllers
{
    public class Province
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class ProvinceLookupController : Controller
    {
        [HttpGet]
        public IActionResult GetProvinces(string countryCode)
        {
            var provinces = default(List<Province>);

            if (countryCode == "ca")
            {
                provinces = new List<Province> { new Province { code = "on", name = "ontario" }, new Province { code = "qc", name = "quebec" } };
            }
            else if (countryCode == "us")
            {
                provinces = new List<Province> { new Province { code = "nc", name = "north carolina" }, new Province { code = "pa", name = "pennsylvania" } };
            }
            else
                provinces = new List<Province> { new Province { code = "xy", name = "marfet" }, new Province { code = "zz", name = "yaltern" } };


            return Json(provinces);
        }
    }
}
