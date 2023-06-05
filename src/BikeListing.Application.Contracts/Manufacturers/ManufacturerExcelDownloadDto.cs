using Volo.Abp.Application.Dtos;
using System;

namespace BikeListing.Manufacturers
{
    public class ManufacturerExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? Name { get; set; }

        public ManufacturerExcelDownloadDto()
        {

        }
    }
}