using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Model;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using System.Net;

namespace PatientManagement.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        public readonly IDbConnection dbConnection = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=PatientManagement;Trusted_Connection=True;MultipleActiveResultSets=true");
        
        #region Insert Patient
        [HttpPost]
        public void SavePatientName(IFormCollection keyValuePairs)
        {
            PatientInfo patientInfo = new PatientInfo();
            patientInfo.FirstName = keyValuePairs["FirstName"].ToString();
            patientInfo.LastName = keyValuePairs["LastName"].ToString();
            patientInfo.MailId = keyValuePairs["MailId"].ToString();
            patientInfo.Status = keyValuePairs["Status"].ToString();
            var query = keyValuePairs["queryType"].ToString();
            var patientId = Convert.ToInt32(Convert.ToString(keyValuePairs["PatientId"]));
            var queryType = query;
            patientInfo.Age = Convert.ToInt32(Convert.ToString(keyValuePairs["Age"]));


            //get fileData from IFormCollection
            var fileData =keyValuePairs?.Files[0];
            string fileName = string.Empty;
            byte[] fileDataByte=new byte[0];
            if (fileData?.Length > 0)
            {
                fileName = fileData.FileName;
                //convert to byteArray
                using (var memoryStream = new MemoryStream())
                {
                    fileData.CopyToAsync(memoryStream);
                    fileDataByte = memoryStream.ToArray();
                }
            }

            //get fileData from IFormCollection
            IFormFile fileDataa = keyValuePairs?.Files[0];
            string fileNames = string.Empty;
            byte[] fileDataBytes = new byte[0];
            if (fileDataa?.Length > 0)
            {
                fileName = fileDataa.FileName;
                //convert to byteArray
                using (var memoryStream = new MemoryStream())
                {
                    fileDataa.CopyTo(memoryStream);
                    fileDataByte = memoryStream.ToArray();
                }
            }

            if (queryType == "INSERT")
            {
                using (var connec = dbConnection)
                {
                    connec.Execute("insert into Patient_Info values(@FirstName,@lastName,@Status,@mail,@Bloodgroup,@age,@patientDoc,@docName,0,getdate(),getdate())", new { @FirstName = patientInfo.FirstName, @lastName = patientInfo.LastName, @Status = patientInfo.Status, @mail = patientInfo.MailId, @Bloodgroup = patientInfo.BloodGroup, @age = patientInfo.Age, @patientDoc= fileDataByte, @docName= fileName });
                }
            }
            else if (queryType == "EDIT")
            {
                using (var connec = dbConnection)
                {
                    connec.Execute("update Patient_Info set First_Name=@FirstName,Last_Name=@lastName,Patient_Status=@Status,Mail_Id=@mail,Blood_Group=@Bloodgroup,Age=@age,Patient_Document=@patientDoc,Document_Name=@docName where Patient_Id= @patientId", new { @FirstName = patientInfo.FirstName, @lastName = patientInfo.LastName, @Status = patientInfo.Status, @mail = patientInfo.MailId, @Bloodgroup = patientInfo.BloodGroup, @age = patientInfo.Age, @patientDoc= fileDataByte, @docName = fileName , @patientId = patientId });
                }
            }
            
        }
        #endregion

        #region GetPatient List
        [HttpGet]
        public PatientInfo[] GetPatientInfoList(long? patientId)
        {
            //var patientId = Convert.ToInt32(Convert.ToString(keyValuePairs?["PatientId"]));

            PatientInfo[] patientInfoList = new PatientInfo[0];
             using (var connec = dbConnection)
            {
                if (patientId > 0)
                {
                    patientInfoList = connec.Query<PatientInfo>("select First_Name as FirstName,Last_Name as LastName,Patient_Status as Status,Mail_Id as MailId,Blood_Group as BloodGroup,Age, Patient_Id as PatientId ,Document_Name as PatientDocumentName,Patient_Document as PatientDocument from Patient_Info where Patient_Id=@PatientId", new { @PatientId= patientId }).ToArray();
                }
                else
                {
                    patientInfoList = connec.Query<PatientInfo>("select First_Name as FirstName,Last_Name as LastName,Patient_Status as Status,Mail_Id as MailId,Blood_Group as BloodGroup,Age, Patient_Id as PatientId,Document_Name as PatientDocumentName,Patient_Document as PatientDocument from Patient_Info where Is_Deleted=0").ToArray();
                }
                
            }
            return patientInfoList;
        }
        #endregion

        #region Delete Patient Record
        [HttpGet]
        public bool DeletePatientRecored(long patientId)
        {
            bool isDeleted = true;
            if (patientId > 0)
            {
                using (var connec = dbConnection)
                {
                    connec.Execute("update Patient_Info set Is_Deleted=1 where Patient_Id= @patientId", new { @patientId = patientId });
                }
            }
            return true;
        }
        #endregion

        #region Download Patient Document ByPatientId
        [HttpGet]
        public ActionResult DownloadPatientDocumentByPatientId(int patientId)
        {
            if (patientId > 0)
            {
                byte[] bytes = null;
                string docType = string.Empty;
                using (var connec = dbConnection)
                {
                    var docDetails = connec.Query("Select Document_Name as PatientDocumentName,Patient_Document as PatientDocument from Patient_Info where Patient_Id= @patientId", new { @patientId = patientId } ).FirstOrDefault();
                    if (docDetails != null)
                    {
                        bytes = docDetails.PatientDocument;
                        docType = docDetails.PatientDocumentName;

                        return File(bytes, "application/pdf", docType.Split('.')[0]);
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
