using Microsoft.AspNetCore.Mvc;

namespace APT.Controllers
{
    public class UtilityController : BaseController
    {
        public IActionResult Water()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CalculateWater(int usage)
        {
            decimal total = CalculateWaterPrice(usage);

            ViewBag.Usage = usage;
            ViewBag.Total = total;

            return View("WaterResult");
        }

        private decimal CalculateWaterPrice(int m3)
        {
            decimal total = 0;

            if (m3 <= 10)
            {
                total = m3 * 7000;
            }
            else if (m3 <= 20)
            {
                total =
                    10 * 7000 +
                    (m3 - 10) * 9000;
            }
            else if (m3 <= 30)
            {
                total =
                    10 * 7000 +
                    10 * 9000 +
                    (m3 - 20) * 11000;
            }
            else
            {
                total =
                    10 * 7000 +
                    10 * 9000 +
                    10 * 11000 +
                    (m3 - 30) * 13000;
            }

            return total;
        }

    }
}