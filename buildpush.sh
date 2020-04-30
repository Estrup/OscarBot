#!/bin/bash

docker build -t zappo.azurecr.io/oscarbot .
docker push zappo.azurecr.io/oscarbot
