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

function IsAuthenticated() {
    return token != "" || tokenExpiry < new Date();
}

function Authenticate(client) {
    try {
        addToLog("Authenticating");
        var tokenRequest = new TokenRequest
        {
            UserName = "grpcBot",
                Password = "grpcBot1!"
        };
        var tokenResponse = client.AuthenticateAsync(tokenRequest);
        if (tokenResponse.Success) {
            token = tokenResponse.Token;
            tokenExpiry = tokenResponse.Expiry.ToDateTime();
            addToLog("Authenticated: Token received");
            return true;
        }
    } catch (exception)
    {
        addToLog("Exception thrown");
        console.log(exception);
    }
    return false;
}

theButton.addEventListener("click", function () {
    try {
        addToLog("Starting Service Call");
        const client = new DataLogClient("http://localhost:5138/");
        if (!IsAuthenticated()) {
            Authenticate();
        };

        const isAliveRequest = new IsAliveRequest();
        isAliveRequest.setName("John");

        addToLog("Calling Service");
        var headers = new Metadata();
        headers.Add("Authorization", "Bearer " + token);

        client.isAlive(isAliveRequest, {}, function (err, response) {
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
