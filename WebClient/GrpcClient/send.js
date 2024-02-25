const { IsAliveRequest, IsAliveReply, ReadingRequest, SuccessResponse, DeveloperResponse, DeveloperName } = require("./datalog_pb.js");
const { DataLogClient } = require("./datalog_grpc_web_pb.js");
const { Timestamp } = require('google-protobuf/google/protobuf/timestamp_pb.js');

const theLog = document.getElementById("theLog");
const theButton = document.getElementById("theButton");

function addToLog(msg) {
    const div = document.createElement("div");
    div.innerText = msg;
    theLog.appendChild(div);
}

theButton.addEventListener("click", function () {
    try {
        addToLog("Starting Service Call");

        const isAliveRequest = new IsAliveRequest();
        isAliveRequest.setName("John");

        addToLog("Calling Service");
        const client = new DataLogClient("https://localhost:7277/");

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
