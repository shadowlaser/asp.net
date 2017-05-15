using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.Principal;
using Seagull2.Core.Models;
using Seagull2.Owin;
using Seagull2.Owin.File.Services;
using Seagull2.Owin.Workflow.Extension;
using Seagull2.StraPosit.WebApi.MonitorReport.Models;
using Seagull2.StraPosit.WebApi.MonitorReport.Services;
using Seagull2.StraPosit.WebApi.RegionLocation.StrategyCity.Models;
using SinoOcean.Seagull2.Framework.MasterData;
using SinoOcean.Seagull2.Library.Extension;

namespace Seagull2.StraPosit.WebApi.RegionLocation.StrategyCity.Controllers
{

    [Route("TargetStrategyCity")]
    public class TargetStrategyCityController : ApiController
    {
        public TargetStrategyCityController()
        {

        }

        [AllowAnonymous]//怎么传递凭据?
        [HttpGet]
        public object Get([FromUri]TargetStrategyCityQueryParams queryParams)
        {
            var cities = EnteredStrategyCityAdapter.Instance.GetList(queryParams.BusinessModelCode);

            var models = cities.Select(c => new StrategyCityModel()
            {
                City = new CodeCnNameImple(c.CityCode, c.CityCnName),
                CityRank = new CodeCnNameImple(c.CityRankCode, c.CityRankCnName),
                CityRegion = new CodeCnNameImple(c.CityRegionCode, c.CityRegionCnName),
                SortNo = c.SortNo,
                ValidStatus = c.ValidStatus,
            }).ToArray();

            if (!queryParams.ExportExcel)
            {
                return Ok(models);

            }
            else
            {
                var exportModel = models.Select(m => new StrategyCityExportModel()
                {
                    CityCode = m.City.Code,
                    CityCnName = m.City.CnName,
                    CityRankCode = m.CityRank.Code,
                    CityRankName = m.CityRank.CnName,
                    CityRegionCode = m.CityRegion.Code,
                    CityRegionName = m.CityRegion.CnName
                }).ToArray();
                DataTable table = DataTableExtension.ToDataTable(exportModel);
                byte[] file = DocumentHelper.CreateDocumentAndTable($"{queryParams.BusinessModelName}目标城市{ DateTime.Now.SimulateTime()}", "A1", "table", table.DefaultView, ExcelTableStyles.Dark1);
                return this.Download(new MemoryStream(file), "战略城市.xlsx", "application/vnd.ms-excel");
            }
        }

        private HttpResponseMessage Download(Stream stream, string fileName, string contentType)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = HttpUtility.UrlEncode(fileName);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            response.Content.Headers.ContentLength = stream.Length;
            response.Headers.CacheControl = new CacheControlHeaderValue() { NoStore = true };

            return response;
        }
    }
}