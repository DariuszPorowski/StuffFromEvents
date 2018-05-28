#!/bin/bash

# Variables
rgName=<rgName>
location=<location>
egTopicName=<egTopicName>
endpointSub=https://<app>.azurewebsites.net/api/updates

# Deploy Azure EventGrid Viewer from:
# https://github.com/dbarkol/azure-event-grid-viewer

# Create a Resource Group
az group create --name $rgName --location $location

# Create an Event Grid topic
az eventgrid topic create --location $location --name $egTopicName --resource-group $rgName

# Get endpoint details
az eventgrid topic show --name $egTopicName --resource-group $rgName --query "endpoint" --output tsv
az eventgrid topic key list --name $egTopicName --resource-group $rgName --query "key1" --output tsv

# Subscribe to events
az eventgrid event-subscription create --resource-group $rgName --topic-name $egTopicName --name custom --endpoint-type webhook --endpoint $endpointSub --included-event-types "All"