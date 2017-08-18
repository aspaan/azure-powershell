﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.ServiceBus.Models;
using Microsoft.Azure.Management.ServiceBus.Models;
using System.Management.Automation;
using System.Collections.Generic;
using System;

namespace Microsoft.Azure.Commands.ServiceBus.Commands.Namespace
{
    /// <summary>
    /// 'Set-AzureRmServiceBusNamespaceAuthorizationRule' Cmdlet updates specified ServiceBus Namespace AuthorizationRule
    /// </summary>
    [ObsoleteAttribute("'Set-AzureRmServiceBusNamespaceAuthorizationRule' cmdlet is mark as obsolete and will be depricated in upcoming breaking changes build. Please use the New cmdlet 'Set-AzureRmServiceBusAuthorizationRule'", false)]
    [Cmdlet(VerbsCommon.Set, ServiceBusNamespaceAuthorizationRuleVerb, SupportsShouldProcess = true), OutputType(typeof(SharedAccessAuthorizationRuleAttributes))]
    public class SetAzureRmServiceBusAuthorizationRule : AzureServiceBusCmdletBase
    {
        [Parameter(Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "The name of the resource group")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroup { get; set; }

        [Parameter(Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1,
            HelpMessage = "ServiceBus Namespace Name.")]
        [ValidateNotNullOrEmpty]
        [Alias(AliasNamespaceName)]
        public string Namespace { get; set; }

        [Parameter(Mandatory = true,
            Position = 2,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "ServiceBus NameSpace AuthorizationRule Object.")]
        [ValidateNotNullOrEmpty]
        [Alias(AliasAuthRuleObj)]
        public SharedAccessAuthorizationRuleAttributes InputObj { get; set; }

        [Parameter(Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            Position = 3,
            HelpMessage = "AuthorizationRule Name - Required if 'AuthruleObj' not specified.")]
        [ValidateNotNullOrEmpty]
        [Alias(AliasAuthorizationRuleName)]
        public string Name { get; set; }

        [Parameter(Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            Position = 4,
            HelpMessage = "Required if 'AuthruleObj' not specified. Rights - e.g.  @(\"Listen\",\"Send\",\"Manage\")")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("Listen", "Send", "Manage",
            IgnoreCase = true)]
        public string[] Rights { get; set; }

        public override void ExecuteCmdlet()
        {
            SharedAccessAuthorizationRuleAttributes sasRule = new SharedAccessAuthorizationRuleAttributes();

            if (InputObj != null)
            {
                sasRule = InputObj;
            }
            else
            {
                NamespaceAttributes getNameSpace = Client.GetNamespace(ResourceGroup, Namespace);

                IList<Management.ServiceBus.Models.AccessRights?> newListAry = new List<Management.ServiceBus.Models.AccessRights?>();

                foreach (string test in Rights)
                {
                    newListAry.Add(ParseAccessRights(test));
                }

                sasRule.Name = Name;
                sasRule.Rights = newListAry;
                sasRule.Location = getNameSpace.Location;
            }

            // Update a Servicebus Namespace authorizationRule
            
            if (ShouldProcess(target: Name, action: string.Format("Update AuthorizationRule:{0} of NameSpace:{1}", Name, Namespace)))
            {
                WriteObject(Client.CreateOrUpdateNamespaceAuthorizationRules(ResourceGroup, Namespace, sasRule.Name, sasRule));
            }
        }
    }
}
