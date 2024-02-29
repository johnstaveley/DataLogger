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
        const client = new DataLogClient("https://localhost:7277/");
        if (!IsAuthenticated()) {
            // TODO: These are not being passed by reference, fix so the function alters the values
            if (!Authenticate(client, token, tokenExpiry)) {
                return;
            }
        };
        console.log(token, tokenExpiry);

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
    return token != "" && tokenExpiry > new Date();
}

function Authenticate(client, token, tokenExpiry) {
    try {
        addToLog("Authenticating");
        token = "";
        const tokenRequest = new TokenRequest();
        tokenRequest.setUsername("grpcBot");
        tokenRequest.setPassword("grpcBot1!");
        client.authenticate(tokenRequest, null, function (err, response) {
            if (err) {
                console.log(err);
                addToLog(`Error: ${err}`);
                return false;
            } else {
                addToLog(`Success: ${response}`);
                if (response.getSuccess()) {
                    console.log("Authenticated: Token received");
                    token = response.getToken();
                    tokenExpiry = response.getExpiry();
                    addToLog("Authenticated: Token received");
                }
            }
        });
    } catch (exception) {
        addToLog("Exception thrown");
        console.log(exception);
        return false;
    }
    return true;
}
