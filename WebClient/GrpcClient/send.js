const { IsAliveRequest, IsAliveReply, ReadingRequest, SuccessResponse, DeveloperResponse, DeveloperName, TokenRequest, TokenResponse } = require("./datalog_pb.js");
const { DataLogPromiseClient } = require("./datalog_grpc_web_pb.js");
const { Timestamp } = require('google-protobuf/google/protobuf/timestamp_pb.js');
const { Metadata } = require("./node_modules/@grpc/grpc-js/build/src/metadata.js");

const myLog = document.getElementById("logDisplay");
const nameInput = document.getElementById("name");
const sendButton = document.getElementById("sendButton");
let tokenResponse = new TokenResponse();

// Write a string to the log div to update the user with progress
function addToLog(msg) {
    const div = document.createElement("div");
    div.innerText = msg;
    myLog.appendChild(div);
}
// Attachs a click event to the button to create a connection to the grpc endpoint
sendButton.addEventListener("click", async function () {
    try {
        const client = new DataLogPromiseClient("https://localhost:8081/");
        if (!IsAuthenticated(tokenResponse)) {
            await Authenticate(client, tokenResponse);
            if (tokenResponse.getSuccess() == false)
            {
                return;
            }
        };
        const isAliveRequest = new IsAliveRequest();
        let name = nameInput.value || "Anonymous";
        isAliveRequest.setName(name);

        addToLog("Calling Service");
        let metadata = { "authorization": "Bearer " + tokenResponse.getToken() };
        const response = await client.isAlive(isAliveRequest, metadata);
        addToLog(`Service call Success: ${response.getMessage()}`);
    } catch (exception) {
        addToLog("Service call Exception thrown");
        console.log(exception);
    }
});

function IsAuthenticated(tokenResponse) {
    return tokenResponse != null && tokenResponse.getToken() != "" && tokenResponse.getExpiry().toDate() > new Date();
}

async function Authenticate(client, tokenResponse) {
    try {
        addToLog("Authenticating");
        const tokenRequest = new TokenRequest();
        tokenRequest.setUsername("grpcBot");
        tokenRequest.setPassword("grpcBot1!");
        const response = await client.authenticate(tokenRequest, {});
        if (response.getSuccess()) {
            console.log("Authenticated: Token received. Expires at " + response.getExpiry().toDate());
            tokenResponse.setSuccess(response.getSuccess());
            tokenResponse.setToken(response.getToken());
            tokenResponse.setExpiry(response.getExpiry());
            addToLog("Authenticated: Token received");
        } else {
            addToLog("Authentication failed");
        };
    } catch (exception) {
        addToLog("Authentication: Exception thrown");
        console.log(exception);
        return false;
    }
    return true;
}
