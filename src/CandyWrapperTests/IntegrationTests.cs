using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace CandyWrapperTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private CandyWrapper _cw;
        private const string URL = "http://demo.sugarcrm.com/sugarcrm/soap.php";
        private const string USER = "will";
        private const string PWD = "will";
        private string _accountId;

        [TestFixtureSetUp]
        public void Setup()
        {
            _cw = new CandyWrapper(URL, USER, PWD);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _cw.Dispose();
        }

        private string GetAccountId()
        {
            if (!string.IsNullOrEmpty(_accountId))
                return _accountId;
            List<SugarEntry> entries = _cw.GetEntryList(SugarModules.Accounts, string.Empty, string.Empty, 0, 1, 0);
            return entries[0].Id;
        }

        [Test]
        public void AccountListWithLargeOffsetShouldReturnNoEnteries()
        {
            List<SugarEntry> entries = _cw.GetEntryList(SugarModules.Accounts, string.Empty, string.Empty, 500, 0, 0);
            Assert.IsNotNull(entries);
            Assert.AreEqual(0, entries.Count);
        }

        [Test]
        public void ListOfLeadsSetShouldNotSkipAnyLeads()
        {
            List<SugarEntry> entries1 = _cw.GetEntryList(SugarModules.Leads, string.Empty, string.Empty, 0, 3, 0);
            List<SugarEntry> entries2 = _cw.GetEntryList(SugarModules.Leads, string.Empty, string.Empty, 2, 2, 0);
            Assert.AreEqual(entries1[2], entries2[0]);
        }

        [Test]
        public void SearchByModuleShouldReturnResults()
        {
            var modules = new List<string>();
            modules.Add(SugarModules.Accounts);
            modules.Add(SugarModules.Leads);
            modules.Add(SugarModules.Opportunities);
            //Seems to return 10 at most
            List<SugarEntry> results = _cw.SearchByModule("a", modules.ToArray(), 0, 20);
            Assert.IsNotNull(results);
            Assert.IsNotEmpty(results);
            Console.WriteLine(results.Count);
            foreach (SugarEntry result in results)
                Console.WriteLine(result.Id);
        }

        [Test]
        public void SessionShouldBeValid()
        {
            Assert.IsNotNull(_cw.Session);
            Assert.AreEqual(26, _cw.Session.Length, "Session string should be 26 characters long.");
        }

        [Test]
        public void ShouldBeAbleToAssignedUserForAccount()
        {
            SugarEntry account = _cw.GetEntry(SugarModules.Accounts, GetAccountId());
            string userId = account["assigned_user_id"];
            Assert.IsNotEmpty(userId);
            SugarEntry user = _cw.GetEntry(SugarModules.Users, userId);
            Assert.IsNotNull(user);
            Console.WriteLine(user);
        }

        [Test]
        public void ShouldBeAbleToGetAccount()
        {
            SugarEntry entry = _cw.GetEntry(SugarModules.Accounts, GetAccountId());
            Assert.IsNotNull(entry);
            Assert.IsNotEmpty(entry);
            Console.WriteLine(entry["name"]);
        }

        [Test]
        public void DemonstarteUsingPattern() {
            //Calls logout when done being used via IDisposable.
            using (var cw = new CandyWrapper(URL, USER, PWD))
            {
                SugarEntry entry = cw.GetEntry(SugarModules.Accounts, GetAccountId());
                Assert.IsNotNull(entry);
                Assert.IsNotEmpty(entry);
                Console.WriteLine(entry["name"]);
            }
        }

        [Test]
        public void ShouldBeAbleToGetAccountList()
        {
            List<SugarEntry> entries = _cw.GetEntryList(SugarModules.Accounts, string.Empty, string.Empty, 0, 10, 0);
            Assert.IsNotNull(entries);
            Assert.IsNotEmpty(entries);
            foreach (SugarEntry entry in entries)
                Console.WriteLine(entry["name"]);
        }

        [Test]
        public void ShouldBeAbleToGetAvailableModules()
        {
            string[] modules = _cw.GetAvailableModules();
            Assert.IsNotNull(modules);
            Assert.IsNotEmpty(modules);
            foreach (string module in modules)
                Console.WriteLine(module);
        }

        [Test]
        public void ShouldBeAbleToUpdateAccount()
        {
            var random = new Random();
            var randonNumber = random.Next(0, 1000); 
            SugarEntry entry = _cw.GetEntry(SugarModules.Accounts, GetAccountId());
            Assert.IsNotNull(entry);
            Assert.IsNotEmpty(entry);
            Console.WriteLine(entry["name"]);
            entry["employees"] = randonNumber.ToString();
            var test = _cw.SetEntry(entry);
            Console.WriteLine(test);
            entry = _cw.GetEntry(SugarModules.Accounts, GetAccountId());
            Assert.AreEqual(randonNumber.ToString(), entry["employees"]);
        }
    }
}