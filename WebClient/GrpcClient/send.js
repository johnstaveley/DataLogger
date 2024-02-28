const { IsAliveRequest, IsAliveReply, ReadingRequest, SuccessResponse, DeveloperResponse, DeveloperName, TokenRequest, TokenResponse } = require("./datalog_pb.js");
const { DataLogClient } = require("./datalog_grpc_web_pb.js");
const { Timestamp } = require('google-protobuf/google/protobuf/timestamp_pb.js');
const { Metadata } = require("./node_modules/@grpc/grpc-js/build/src/metadata.js");

const theLog = document.getElementById("theLog");
const theButton = document.getElementById("theButton");
let token = "";
let tokenExpiry = new Date();

function addToLog(msg) {
    const div = document.createElement("div");
    div.innerText = msg;
    theLog.appendChild(div);
}
theButton.addEventListener("click", function () {
    try {
        addToLog("Starting Service Call");
        const client = new DataLogClient("http://localhost:5138/");
        if (!IsAuthenticated()) {
            if (!Authenticate(client)) {
                return;
            }
        };

        const isAliveRequest = new IsAliveRequest();
        isAliveRequest.setName("John");

        addToLog("Calling Service");        
        let metadata = { "authorization": "Bearer " + token };
        client.isAlive(isAliveRequest, metadata, function (err, response) {
            if (err) {
                addToLog(`Error: ${err}`);
            } else {
                addToLog(`Success: ${response}`);
            }
        });

    } catch (exception) {
        addToLog("Exception thrown");
        console.log(exception);
    }
});

function IsAuthenticated() {
    return token != "" || tokenExpiry < new Date();
}

function Authenticate(client) {
    try {
        addToLog("Authenticating");
        const tokenRequest = new TokenRequest();
        tokenRequest.UserName = "grpcBot";
        tokenRequest.Password = "grpcBot1!";
        client.AuthenticateAsync(tokenRequest, function (err, response) {
            console.log(response);
            if (err) {
                addToLog(`Error: ${err}`);
            } else {
                addToLog(`Success: ${response}`);
                if (response.Success) {
                    token = response.Token;
                    tokenExpiry = response.Expiry.ToDateTime();
                    addToLog("Authenticated: Token received");
                    return true;
                }
            }
        });
    } catch (exception) {
        addToLog("Exception thrown");
        console.log(exception);
    }
    return false;
}
