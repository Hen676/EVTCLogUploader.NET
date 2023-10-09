using EVTCLogUploader.Enums;
using EVTCLogUploader.Models;
using EVTCLogUploader.Services;
using EVTCLogUploader.Services.IO;
using System.Text;

namespace EVTCLogUploaderTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        [DeploymentItem(@"Resources\EVTCFiles\20200719-210434.evtc")]
        public void FileDeploymentCheck()
        {
            var myfile = "20200719-210434.evtc";
            Assert.IsTrue(File.Exists(myfile), "Deployment failed: {0} did not get deployed.", myfile);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\EVTCFiles\20200719-210434.evtc")]
        public void FileCanBeRead()
        {
            var myfile = "20200719-210434.evtc";

            BinaryArrayReaderIO reader = new BinaryArrayReaderIO(File.ReadAllBytes(myfile), new UTF8Encoding());

            BinaryReaderHandlerIO handler = new(reader);

            Encounter encounter = handler.GetEncounter(); // Qadim
            string charcterName = handler.GetCharcterName(); // "Cake Panity"
            string userName = handler.GetUserName(); // "Hen.5687"
            Profession charcterClass = handler.GetCharcterProf(); // Necromancer
            Specialization charcterSpec = handler.GetCharcterSpec(); // Scourge

            Assert.IsTrue(encounter == Encounter.Qadim, "Binary Reader Failed: Wrong encounter was given ({0} instead of Quadim)", encounter);
            Assert.IsTrue(charcterName.Equals("Cake Panity"), "Binary Reader Failed: Wrong charcter name was given ({0} instead of Cake Panity)", charcterName);
            Assert.IsTrue(userName.Equals("Hen.5687"), "Binary Reader Failed: Wrong user name was given ({0} instead of Hen.5687)", userName);
            Assert.IsTrue(charcterClass == Profession.Necromancer, "Binary Reader Failed: Wrong profession was given ({0} instead of Necromancer)", charcterClass);
            Assert.IsTrue(charcterSpec == Specialization.Scourge, "Binary Reader Failed: Wrong specialization was given ({0} instead of Scourge)", charcterSpec);
        }

        [TestMethod]
        public void DatabaseCheck()
        {
            string db = ":memory:";

            ILocalDatabaseService localDatabaseService = new LocalDatabaseService(db);

            localDatabaseService.WipeDB();

            localDatabaseService.AddRecords(new List<EVTCFile>
            {
                new EVTCFile("key", "Name defualt", DateTime.Now, "Username defualt", "Charcter name defualt", TimeSpan.Zero, Profession.Unknown, Specialization.None, Encounter.Empty, "Upload url defualt", FileType.None),
                new EVTCFile("Path min", "Name min", DateTime.MinValue, "Username min", "Charcter name min", TimeSpan.MinValue, Profession.Necromancer, Specialization.Reaper, Encounter.Sabir, "Upload url min", FileType.EVTC),
                new EVTCFile("Path max", "Name max", DateTime.MaxValue, "Username max", "Charcter name max", TimeSpan.MaxValue, Profession.Revenant, Specialization.Vindicator, Encounter.HarvestTemple, "Upload url max", FileType.EVTCZIP)
            });

            localDatabaseService.UpdateRecordsURL(new List<EVTCFile>
            {
                new EVTCFile("key", "Name defualt update", DateTime.Now, "Username defualt update", "Charcter name defualt update", TimeSpan.Zero, Profession.Unknown, Specialization.None, Encounter.Empty, "Upload url defualt update", FileType.None)
            });

            List<EVTCFile> rows = localDatabaseService.GetRecords().Result;

            Assert.IsTrue(rows.Count == 3, "Database failed to add and get records: {0} rows created instead of 3.", rows.Count);

            localDatabaseService.WipeDB();
        }
    }
}