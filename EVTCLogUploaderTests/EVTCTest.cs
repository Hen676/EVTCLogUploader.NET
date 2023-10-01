

using EVTCLogUploader.Enums;
using EVTCLogUploader.Services.IO;
using System.Text;

namespace EVTCLogUploaderTests
{
    [TestClass]
    public class EVTCTest
    {
        [TestMethod]
        [DeploymentItem(@"Resources\EVTCFiles\20200719-210434.evtc")]
        public void FileDeploymentCheck()
        {
            var myfile = "20200719-210434.evtc";
            Assert.IsTrue(File.Exists(myfile),"Deployment failed: {0} did not get deployed.",myfile);
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
    }
}