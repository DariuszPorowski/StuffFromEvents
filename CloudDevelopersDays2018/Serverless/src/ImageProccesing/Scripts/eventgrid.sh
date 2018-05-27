#!/bin/bash

# Variables
rgName=<rgName>
storageAccountName=<storageAccountName>
azFuncEndpoint="https://<appName>.azurewebsites.net/admin/extensions/EventGridExtensionConfig?functionName=ImageProcessing&code=<code>"

# Create Event Grid event subscription to the Storage Account
storageAccountId=$(az storage account show --name $storageAccountName --resource-group $rgName --query id --output tsv)
az eventgrid event-subscription create --name imgProcSub --endpoint $azFuncEndpoint --resource-id $storageAccountId --endpoint-type webhook --subject-begins-with /blobServices/default/containers/images/
