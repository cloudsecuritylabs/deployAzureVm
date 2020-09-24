// Set up using .....
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;


namespace deploy_azure_vm
{    class Program    
    {        static void Main(string[] args)        
        {            
            Console.WriteLine("Starting Deployment");
            // Get Azure credentials
            var credentials = SdkContext.AzureCredentialsFactory                                 
                                .FromFile("./azureauth.properties");

            // Authenticate to Azure
            var azure = Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();         


            //create required variables         
            var groupName = "az204-ResourceGroup2";            
            var vmName = "WinVM";            
            var location = Region.USEast;            
            var vNetName = "az204VNET";            
            var vNetAddress = "10.10.0.0/16";            
            var subnetName = "az204Subnet";           
            var subnetAddress = "10.10.0.0/24";            
            var nicName = "az204NIC";           
            var adminUser = "student";            
            var adminPassword = "P@55w.rd1234";
            
            Console.WriteLine($"It is time to create the resource group {groupName} ...");
            var resourceGroup = azure.ResourceGroups.Define(groupName)
                .WithRegion(location)
                .Create();

            Console.WriteLine($"It is time to create the virtual network {vNetName} ...");
            var network = azure.Networks.Define(vNetName)
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithAddressSpace(vNetAddress)
                .WithSubnet(subnetName, subnetAddress)
                .Create();


            Console.WriteLine($"It is time to creating network interface {nicName} ...");           
            var nic = azure.NetworkInterfaces.Define(nicName)                
                .WithRegion(location)                
                .WithExistingResourceGroup(groupName)                
                .WithExistingPrimaryNetwork(network)                
                .WithSubnet(subnetName)                
                .WithPrimaryPrivateIPAddressDynamic()                
                .Create();

            Console.WriteLine($"Creating virtual machine {vmName} ...");            
            azure.VirtualMachines.Define(vmName)                
                .WithRegion(location)                
                .WithExistingResourceGroup(groupName)                
                .WithExistingPrimaryNetworkInterface(nic)                
                .WithLatestWindowsImage("MicrosoftWindowsServer", "WindowsServer","2012-R2-Datacenter")                .WithAdminUsername(adminUser)                .WithAdminPassword(adminPassword)                .WithComputerName(vmName)                .WithSize(VirtualMachineSizeTypes.StandardDS2V2)                .Create();
            
        }    
            
    }
            
}