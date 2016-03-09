﻿using System;
using System.Threading.Tasks;
using Energistics.DataAccess;
using Energistics.DataAccess.WITSML141;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.Witsml.Studio.Plugins.WitsmlBrowser.ViewModels;
using PDS.Witsml.Studio.Runtime;
using PDS.Witsml.Studio.ViewModels;

namespace PDS.Witsml.Studio.Plugins.WitsmlBrowser
{
    [TestClass]
    public class WitsmlBrowserViewModelTests
    {
        const string _validWitsmlUri = "http://localhost/Witsml.Web/WitsmlStore.svc";

        private static string _addWellTemplate =
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + Environment.NewLine +
                "<wells version=\"1.4.1.1\" xmlns=\"http://www.witsml.org/schemas/1series\" >" + Environment.NewLine +
                "<well uid=\"{0}\">" + Environment.NewLine +
                "<name>Unit Test Well {1}</name>" + Environment.NewLine +
                "<timeZone>-06:00</timeZone>" + Environment.NewLine +
                "</well>" + Environment.NewLine +
                "</wells>";

        private static string _getWellTemplate =
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + Environment.NewLine +
            "<wells version=\"1.4.1.1\" xmlns=\"http://www.witsml.org/schemas/1series\" >" + Environment.NewLine +
            "<well uid=\"{0}\" />" + Environment.NewLine +
            "</wells>";

        private BootstrapperHarness _bootstrapper;
        private TestRuntimeService _runtime;

        [TestInitialize]
        public void TestSetUp()
        {
            _bootstrapper = new BootstrapperHarness();
            _runtime = new TestRuntimeService(_bootstrapper.Container);
            _runtime.Shell = new ShellViewModel(_runtime);
        }

        [TestMethod]
        public async Task TestSubmitAddToStoreForWell()
        {
            // The expected result
            var expectedUid = Guid.NewGuid().ToString();

            // Create the view model and initialize data to add a well to the store
            var vm = new MainViewModel(_runtime);
            vm.Model.Connection = new Connections.Connection() { Uri = _validWitsmlUri };
            vm.Proxy.Url = vm.Model.Connection.Uri;

            var xmlIn = string.Format(
                _addWellTemplate,
                expectedUid,
                DateTime.Now.ToString("yyyyMMdd-HHmmss"));

            vm.Model.ReturnElementType = OptionsIn.ReturnElements.All;

            // Submit the query
            var result = await vm.SubmitQuery(Functions.AddToStore, xmlIn);
            var suppMsgOut = result.MessageOut;

            // The same uid should be returned as the results.
            Assert.AreEqual(expectedUid, suppMsgOut);
        }

        [TestMethod]
        public async Task TestSubmitGetFromStoreForWell()
        {
            // The expected result
            var expectedUid = Guid.NewGuid().ToString();

            // Create the view model and initialize data to add a well to the store
            var vm = new MainViewModel(_runtime);
            vm.Model.Connection = new Connections.Connection() { Uri = _validWitsmlUri };
            vm.Proxy.Url = vm.Model.Connection.Uri;
            vm.Model.ReturnElementType = OptionsIn.ReturnElements.All;

            // Add a well to the store
            var xmlIn = string.Format(
                _addWellTemplate,
                expectedUid,
                DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            var result = await vm.SubmitQuery(Functions.AddToStore, xmlIn);

            // Retrieve the same well from the store
            xmlIn = string.Format(_getWellTemplate, expectedUid);
            result = await vm.SubmitQuery(Functions.GetFromStore, xmlIn);

            string xmlOut = result.XmlOut;

            // The same uid should be returned as the results.
            Assert.IsNotNull(xmlOut);

            var wellList = EnergisticsConverter.XmlToObject<WellList>(xmlOut);
            Assert.IsNotNull(wellList);
            Assert.AreEqual(1, wellList.Items.Count);
            Assert.AreEqual(expectedUid, (wellList.Items[0] as Well).Uid);
        }

        [TestMethod]
        public async Task TestMainViewModelSubmitQuery()
        {
            var vm = new MainViewModel(_runtime);
            vm.Model.Connection = new Connections.Connection() { Uri = _validWitsmlUri };
            vm.Proxy.Url = vm.Model.Connection.Uri;
            vm.Model.WitsmlVersion = OptionsIn.DataVersion.Version141.Value;

            var result = await vm.SubmitQuery(Functions.GetCap, string.Empty);

            // Test that the xmlOut is a Capserver List
            var capServerList = EnergisticsConverter.XmlToObject<CapServers>(result.XmlOut);
            Assert.IsNotNull(capServerList);

            // Is this the version we're expecting
            Assert.AreEqual(OptionsIn.DataVersion.Version141.Value, capServerList.CapServer.SchemaVersion);
        }
    }
}
