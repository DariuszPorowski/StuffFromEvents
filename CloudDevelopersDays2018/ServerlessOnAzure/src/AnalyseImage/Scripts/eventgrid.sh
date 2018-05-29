#!/bin/bash

# Variables
rgName=<rgName>
storageAccountName=<storageAccountName>
azFuncEndpoint="https://<appName>.azurewebsites.net/runtime/webhooks/EventGridExtensionConfig?functionName=AnalyseImage&code=<code>"

# Create Event Grid event subscription to the Storage Account
storageAccountId=$(az storage account show --name $storageAccountName --resource-group $rgName --query id --output tsv)
az eventgrid event-subscription create --name analyseImageSub --endpoint $azFuncEndpoint --resource-id $storageAccountId --endpoint-type webhook --included-event-types Microsoft.Storage.BlobCreated --subject-begins-with /blobServices/default/containers/images/
